using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grammophone.Ennoun.Web.Hubs
{
	/// <summary>
	/// Methods exposed by the engine state clients.
	/// </summary>
	public interface IEngineStateHub
	{
		/// <summary>
		/// Update the indicator of the state of the inference engine.
		/// </summary>
		/// <param name="engineState">The inference engine state.</param>
		void SetEngineState(Controllers.InferenceEngineState engineState);
	}
}
