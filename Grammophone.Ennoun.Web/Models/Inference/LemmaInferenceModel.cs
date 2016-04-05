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
	}
}