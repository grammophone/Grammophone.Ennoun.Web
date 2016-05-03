using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grammophone.Ennoun.Web.Controllers.TextProcessorStages
{
	/// <summary>
	/// Complements any character normalization not handled by the language provider.
	/// </summary>
	internal class CharacterNormalizationStage : ITextProcessorStage
	{
		public string Process(string input)
		{
			if (input == null) throw new ArgumentNullException("input");

			return input
				.Replace('˙', '·'); // Correct the type of upper stop.
		}
	}
}