using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grammophone.Ennoun.Web.Controllers.TextProcessorStages
{
	/// <summary>
	/// Removes CR, LF, CRLF and substitutes them with space.
	/// </summary>
	internal class LineFeedRemovalStage : ITextProcessorStage
	{
		public string Process(string input)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			return input
				.Replace("\r\n", " ")
				.Replace('\r', ' ')
				.Replace('\n', ' ');
		}
	}
}