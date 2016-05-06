using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grammophone.Ennoun.Web.Models.Inference
{
	/// <summary>
	/// An inferred tag-lemma pair for a word.
	/// </summary>
	public class LemmaInferenceModel
	{
		/// <summary>
		/// The word form.
		/// </summary>
		public string Form { get; set; }

		/// <summary>
		/// The inferred tag of the word.
		/// </summary>
		public TagModel Tag { get; set; }

		/// <summary>
		/// The inferrred lemma of the word. Might be normalized (for example, capitalized or missing accents), 
		/// depending on LanguageProvider.
		/// </summary>
		public string Lemma { get; set; }

		/// <summary>
		/// The minimum distance of the <see cref="Lemma"/> from the lexicon entries.
		/// The value -1.0 signifies 'far away from all'.
		/// </summary>
		public double MinimumLexiconDistance { get; set; } = -1.0;
	}
}