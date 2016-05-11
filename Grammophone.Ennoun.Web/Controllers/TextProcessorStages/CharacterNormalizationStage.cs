using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

			var stringBuilder = new StringBuilder(input.Length);

			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];

				switch (c)
				{
					case '˙': // Correct the type of upper stop.
					case '\x00B7':
						c = '·';
						break;

					case '\'': // Correct the type of apostrophe.
					case '\x2019':
						c = '\x1FBF';
						break;
				}

				stringBuilder.Append(c);
			}

			return stringBuilder.ToString();
		}
	}
}