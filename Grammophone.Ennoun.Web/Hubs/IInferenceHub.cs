using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grammophone.Ennoun.Web.Hubs
{
	/// <summary>
	/// Methods exposed by the inference clients.
	/// </summary>
	public interface IInferenceHub
	{
		/// <summary>
		/// Update the progress of the inference.
		/// </summary>
		/// <param name="percentage">The percentage of the progress, scaled to 100.</param>
		void SetProgress(double percentage);
	}
}
