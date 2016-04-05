using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grammophone.Ennoun.Web.Models.Inference
{
	/// <summary>
	/// The result of inference upon a sentence, including probability.
	/// </summary>
	public class SentenceInferenceModel
	{
		/// <summary>
		/// The inferred components of the sentence
		/// or null if the sequence of the given words is estimated as impossible.
		/// </summary>
		public IEnumerable<LemmaInferenceModel> LemmaInferences { get; set; }

		/// <summary>
		/// The probability of this inference, according to the sentence model.
		/// </summary>
		public double Probability { get; set; }
	}
}