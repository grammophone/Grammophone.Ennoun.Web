using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Grammophone.Ennoun.Web.Controllers.TextProcessorStages
{
	/// <summary>
	/// Adds δασεῖα when missing to 'ρ'.
	/// </summary>
	internal class RhoNormalizationStage : ITextProcessorStage
	{
		public string Process(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			var textBuilder = new StringBuilder(input);

			if (textBuilder[0] == 'Ρ') textBuilder[0] = 'Ῥ';
			if (textBuilder[0] == 'ρ') textBuilder[0] = 'ῥ';

			textBuilder
				.Replace("ῤῥ", "ρρ")
				.Replace(".ρ", ".ῥ")
				.Replace(",ρ", ",ῥ")
				.Replace("·ρ", "·ῥ")
				.Replace(" Ρ", " Ῥ")
				.Replace(" ρ", " ῥ");

			return textBuilder.ToString();
		}
	}
}