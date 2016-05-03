using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grammophone.Ennoun.Web.Controllers.TextProcessorStages
{
	/// <summary>
	/// Define a simple interface for a stage of text processing.
	/// </summary>
	internal interface ITextProcessorStage
	{
		string Process(string input);
	}
}