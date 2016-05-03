using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grammophone.Ennoun.Web.Hubs;
using Grammophone.Ennoun.Web.Models;
using Grammophone.Ennoun.Web.Models.Inference;
using Grammophone.EnnounInference;
using Grammophone.EnnounInference.Configuration;
using Grammophone.EnnounInference.Sentences;
using Grammophone.GenericContentModel;
using Grammophone.LanguageModel.Grammar;
using Grammophone.LanguageModel.Provision;
using Grammophone.Lexica;
using Grammophone.Lexica.Configuration;
using Grammophone.Lexica.LexiconModel;
using Microsoft.AspNet.SignalR;

namespace Grammophone.Ennoun.Web.Controllers
{
	/// <summary>
	/// Session for performing inference tasks. 
	/// Instances are intended to be created during requests.
	/// </summary>
	public class InferenceSession : IDisposable
	{
		#region Constants

		private const string LanguageProviderKey = "grc";

		private const string DefaultLexiconName = "Liddell-Scott";

		private const double MaximumEditDistance = 1.0;

		private const double LowProbabilityThreshold = 0.05;

		#endregion

		#region Private fields

		private static Lazy<LanguageProvider> lazyLanguageProvider;

		private Lazy<IHubContext<IInferenceHub>> lazyInferenceHubContext;

		/// <summary>
		/// This backing field really holds an <see cref="InferenceEngineState"/>, but it is typed as an integer
		/// to enable atomic operations offered by <see cref="Interlocked"/>.
		/// </summary>
		private static int engineState;

		private static InferenceResource inferenceResource;

		private static IDictionary<string, Lexicon> lexicaByName;

		private static Lexicon defaultLexicon;

		private static readonly IReadOnlyList<TextProcessorStages.ITextProcessorStage> textProcessorStages;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="connectionID">The optional SignalR connection ID of the client in order to report live progress.</param>
		public InferenceSession(string connectionID = null)
		{
			this.ConnectionID = connectionID;

			this.lazyInferenceHubContext = 
				new Lazy<IHubContext<IInferenceHub>>(() => GlobalHost.ConnectionManager.GetHubContext<InferenceHub, IInferenceHub>(),
					LazyThreadSafetyMode.None);

		}

		/// <summary>
		/// Static contstructor.
		/// </summary>
		static InferenceSession()
		{
			lazyLanguageProvider =
				new Lazy<LanguageProvider>(() => EnnounInference.Configuration.InferenceEnvironment.Setup.LanguageProviders[LanguageProviderKey],
					LazyThreadSafetyMode.ExecutionAndPublication);

			textProcessorStages = new TextProcessorStages.ITextProcessorStage[] 
			{
				new TextProcessorStages.HyphenationTextProcessorStage(),
				new TextProcessorStages.CharacterNormalizationStage()
			};
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The current state of he inference engine.
		/// </summary>
		public static InferenceEngineState EngineState
		{
			get
			{
				return (InferenceEngineState)engineState;
			}
		}

		/// <summary>
		/// The language provider configured for the ancient Greek language.
		/// </summary>
		public static LanguageProvider GreekLanguageProvider
		{
			get
			{
				return lazyLanguageProvider.Value;
			}
		}

		/// <summary>
		/// SignalR hub context for inference progress notifications.
		/// </summary>
		public IHubContext<IInferenceHub> InferenceHubContext
		{
			get
			{
				return lazyInferenceHubContext.Value;
			}
		}

		/// <summary>
		/// The optional SignalR connection ID of the client in order to report live progress.
		/// </summary>
		public string ConnectionID { get; private set; }

		#endregion

		#region Public methods

		/// <summary>
		/// Start the inference engine.
		/// </summary>
		/// <returns>Returns a task completing the operation.</returns>
		/// <remarks>
		/// The state of the engine is reported in property <see cref="EngineState"/>.
		/// </remarks>
		public static async Task StartEngineAsync()
		{
			var currentEngineState = (InferenceEngineState)
				Interlocked.CompareExchange(ref engineState, (int)InferenceEngineState.Starting, (int)InferenceEngineState.NotStarted);

			if (currentEngineState == InferenceEngineState.NotStarted)
			{
				engineState = (int)InferenceEngineState.Starting;

				try
				{
					await LoadInferenceResourceAsync();

					await LoadLexiconAsync();

					engineState = (int)InferenceEngineState.Started;
				}
				catch
				{
					engineState = (int)InferenceEngineState.Error;
					throw;
				}
			}
		}

		/// <summary>
		/// Infer part-of-speech tagging and lemmatization of a piece of text.
		/// The text must contain complete ancient Greek sentences, including final punctuation, if possible.
		/// </summary>
		/// <param name="text">The text to analyze.</param>
		/// <returns>
		/// Returns a task whose result holds a list of <see cref="SentenceResponseModel"/> elements
		/// describing the results of each of the sentences found in the <paramref name="text"/>.
		/// </returns>
		/// <remarks>
		/// Currently a mock return value is given.
		/// </remarks>
		public async Task<IList<SentenceResponseModel>> InferAsync(string text)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			EnsureEngineStateStarted();

			text = ProcessText(text);

			var languageProvider = inferenceResource.LanguageProvider;

			var sentenceBreaker = languageProvider.SentenceBreaker;

			using (var textReader = new System.IO.StringReader(text))
			{
				string[] sentences = sentenceBreaker.SeparateSentences(textReader).ToArray();

				var sentenceResponses = new List<SentenceResponseModel>(sentences.Length);

				double sentencesLength = sentences.Length;

				for (int i = 0; i < sentences.Length; i++)
				{
					string sentence = sentences[i];

					await Task.Run(() => 
					{
						var sentenceInference = inferenceResource.SentenceClassifier.InferSentence(sentence);

						var sentenceResponse = new SentenceResponseModel
						{
							SentenceInference = TransferSentenceInferenceModel(sentenceInference),
							Messages = GetMessagesForSentence(sentence, sentenceInference)
						};

						sentenceResponses.Add(sentenceResponse);
					});

					double progress = 100.0 * (i + 1) / sentencesLength;

					ReportProgress(progress);
				}

				return sentenceResponses;
			}
		}

		/// <summary>
		/// Find lexicon lemmata approximately equal to a given <paramref name="form"/>.
		/// </summary>
		/// <param name="form">The form to search for.</param>
		/// <returns>
		/// Returns the lemmata found within a configured edit distance.
		/// </returns>
		public IReadOnlyList<LemmaResult> GetLemmata(string form)
		{
			if (form == null) throw new ArgumentNullException(nameof(form));

			EnsureEngineStateStarted();

			return defaultLexicon.GetLemmata(form, MaximumEditDistance);
		}

		/// <summary>
		/// Find lexicon lemmata being closest to a given <paramref name="form"/>.
		/// </summary>
		/// <param name="form">The form to search for.</param>
		/// <returns>
		/// Returns the closest lemmata found within a configured edit distance.
		/// </returns>
		public IReadOnlyList<LemmaResult> GetBestLemmata(string form)
		{
			if (form == null) throw new ArgumentNullException(nameof(form));

			return defaultLexicon.GetBestLemmata(form, MaximumEditDistance);
		}

		/// <summary>
		/// Find lexicon lemmata having a given key.
		/// </summary>
		/// <param name="key">The key to match.</param>
		/// <returns>Returns the collection of lemmata found.</returns>
		public IReadOnlyCollection<Lemma> GetLemmataByKey(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			return defaultLexicon.GetLemmata(key);
		}

		/// <summary>
		/// Free any resources held by the session.
		/// </summary>
		/// <remarks>
		/// Currently this method does nothing.
		/// </remarks>
		public void Dispose()
		{
		}

		#endregion

		#region Private methods

		private static IList<MessageModel> GetMessagesForSentence(string sentence, SentenceInference sentenceInference)
		{
			if (sentence == null) throw new ArgumentNullException(nameof(sentence));
			if (sentenceInference == null) throw new ArgumentNullException(nameof(sentenceInference));

			var messages = new List<MessageModel>();

			var sentenceBreaker = inferenceResource.LanguageProvider.SentenceBreaker;

			if (sentence.Length > 0)
			{
				char lastCharacter = sentence[sentence.Length - 1];

				if (!sentenceBreaker.IsSentenceDelimiter(lastCharacter))
				{
					messages.Add(new MessageModel
					{
						Level = MessageLevel.Warning,
						Text = InferenceMessages.SENTENCE_PUNCTUATION_MISSING
					});
				}

				if (sentenceInference.Probability == 0.0)
				{
					messages.Add(new MessageModel
					{
						Level = MessageLevel.Error,
						Text = InferenceMessages.SENTENCE_IMPOSSIBLE
					});
				}
				else if (sentenceInference.Probability <= LowProbabilityThreshold)
				{
					messages.Add(new MessageModel
					{
						Level = MessageLevel.Warning,
						Text = InferenceMessages.SENTENCE_PROBABILITY_LOW
					});
				}
			}

			return messages;
		}

		private static async Task LoadLexiconAsync()
		{
			var languageProvider = GreekLanguageProvider;

			IReadOnlyCollection<Lexicon> lexica;

			if (!LexicaEnvironment.Lexica.ContainsKey(languageProvider))
			{
				lexica = await LexicaEnvironment.ImportLexicaAsync(languageProvider);
			}
			else
			{
				lexica = LexicaEnvironment.Lexica[languageProvider];
			}

			lexicaByName = lexica.ToDictionary(l => l.Name);

			// For some strange reason (possibly a compiler bug), the lexicaByName.TryGetValue method
			// is treated as returning always false, despite returning true.
			// Let's work around it using two steps.

			if (!lexicaByName.ContainsKey(DefaultLexiconName))
				throw new ApplicationException($"The lexicon with name {DefaultLexiconName} was not found in the loaded lexica.");

			defaultLexicon = lexicaByName[DefaultLexiconName];

		}

		private static Task LoadInferenceResourceAsync()
		{

			return Task.Factory.StartNew(() =>
			{
				var languageProvider = GreekLanguageProvider;

				if (!InferenceEnvironment.InferenceResources.ContainsKey(languageProvider))
				{
					Exception exception = null;

					System.Diagnostics.Trace.WriteLine($"Environment.Is64BitProcess = {Environment.Is64BitProcess}");

					// Use low-level thread in order to specify stack size, because the serialized model has deep recursion.
					var loaderThread = new Thread(() =>
					{
						try
						{
							inferenceResource = InferenceEnvironment.LoadInferenceResource(languageProvider);

							if (inferenceResource == null)
								throw new ApplicationException("No inference resource provider is available for the ancient Greek language.");
						}
						catch (Exception ex)
						{
							exception = ex;
						}
					},
					0x1000000 /* 16MB stack */);

					loaderThread.Start();

					loaderThread.Join();

					if (exception != null) throw exception;
				}
				else
				{
					inferenceResource = InferenceEnvironment.InferenceResources[languageProvider];
				}
			}, TaskCreationOptions.LongRunning);

		}

		/// <summary>
		/// Report progress percentage.
		/// If there is no <see cref="ConnectionID"/> specified, this method does nothing.
		/// </summary>
		/// <param name="percentage">Value from 0.0 to 100.0.</param>
		private void ReportProgress(double percentage)
		{
			if (this.ConnectionID != null)
			{
				this.InferenceHubContext.Clients.Client(this.ConnectionID).SetProgress(percentage);
			}
		}

		private static TagModel TransferTagModel(Tag tag)
		{
			if (tag == null) return null;

			return new TagModel
			{
				Type = tag.Type.Name,
				Inflections = tag.Inflections.Select(i => new InflectionModel { Name = i.Name, Type = i.Type.Name })
			};
		}

		private static LemmaInferenceModel TransferLemmaInferenceModel(LemmaInference lemmaInference)
		{
			if (lemmaInference == null) throw new ArgumentNullException(nameof(lemmaInference));

			return new LemmaInferenceModel
			{
				Form = lemmaInference.Form,
				Tag = TransferTagModel(lemmaInference?.Tag),
				Lemma = lemmaInference?.Lemma
			};
		}

		private static SentenceInferenceModel TransferImpossibleSentenceInferenceModel(IEnumerable<string> words)
		{
			if (words == null) throw new ArgumentNullException(nameof(words));

			return new SentenceInferenceModel
			{
				Probability = 0.0,
				LemmaInferences = words.Select(w => new LemmaInferenceModel { Form = w })
			};
		}

		private static SentenceInferenceModel TransferSentenceInferenceModel(SentenceInference sentenceInference)
		{
			if (sentenceInference == null) throw new ArgumentNullException(nameof(sentenceInference));

			return new SentenceInferenceModel
			{
				Probability = sentenceInference.Probability,
				LemmaInferences = sentenceInference.LemmaInferences.Select(li => TransferLemmaInferenceModel(li))
			};
		}

		private static void EnsureEngineStateStarted()
		{
			switch (EngineState)
			{
				case InferenceEngineState.NotStarted:
				case InferenceEngineState.Starting:
					throw new UserException("The inference engine has not yet started. Please try after 10 minutes.");

				case InferenceEngineState.Error:
					throw new UserException("There has been an error while starting the inference engine.");
			}
		}

		private static string ProcessText(string text)
		{
			if (text == null) return null;

			foreach (var stage in textProcessorStages)
			{
				text = stage.Process(text);
			}

			return text;
		}

		#region Mocking methods

		private IList<SentenceResponseModel> CreateMockSentenceResponse()
		{
			return new List<SentenceResponseModel>
			{
				new SentenceResponseModel
				{
					Messages = new List<MessageModel>
					{
						new MessageModel { Text = "Looks like a Greek sentence.", Level = MessageLevel.Information },
						new MessageModel { Text = "Should include final punctuation.", Level = MessageLevel.Warning }
					},
					SentenceInference = TransferSentenceInferenceModel( 
						CreateMockSentenceInference()
					)
				}
			};
		}

		private SentenceInference CreateMockSentenceInference()
		{
			return new SentenceInference(CreateMockLemmaInferences(), 0.7);
		}

		private static IReadOnlySequence<LemmaInference> CreateMockLemmaInferences()
		{
			var languageProvider = GreekLanguageProvider;

			var grammarModel = languageProvider.GrammarModel;

			var lemmaInferences = new LemmaInference[]
			{
				new LemmaInference(
					"ἐν",
					grammarModel.GetTag(grammarModel.TagTypes["prep"], null , "ἐν"),
					"ἐν"),
				new LemmaInference(
					"ἀρχῇ",
					grammarModel.GetTag(grammarModel.TagTypes["noun"], new Inflection[]
					{
						grammarModel.InflectionTypes["case"].Inflections["dat sg"],
						grammarModel.InflectionTypes["gender"].Inflections["fem"]
					}),
					"ἀρχη"),
				new LemmaInference(
					"ἦν",
					grammarModel.GetTag(grammarModel.TagTypes["verb"], new Inflection[]
					{
						grammarModel.InflectionTypes["person"].Inflections["3rd sg"],
						grammarModel.InflectionTypes["mood"].Inflections["ind"],
						grammarModel.InflectionTypes["tense"].Inflections["imperf"],
						grammarModel.InflectionTypes["voice"].Inflections["act"]
					}),
					"εἰμι"),
				new LemmaInference(
					"ὁ",
					grammarModel.GetTag(grammarModel.TagTypes["article"], new Inflection[]
					{
						grammarModel.InflectionTypes["case"].Inflections["nom sg"],
						grammarModel.InflectionTypes["gender"].Inflections["masc"]
					}),
					"ὁ"),
				new LemmaInference(
					"Λόγος",
					grammarModel.GetTag(grammarModel.TagTypes["noun"], new Inflection[]
					{
						grammarModel.InflectionTypes["case"].Inflections["nom sg"],
						grammarModel.InflectionTypes["gender"].Inflections["masc"]
					}),
					"λογος")
			};

			return new ReadOnlySequence<LemmaInference>(lemmaInferences);
		}

		#endregion

		#endregion
	}
}