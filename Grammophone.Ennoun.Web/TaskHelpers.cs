using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Grammophone.Ennoun.Web
{
	/// <summary>
	/// Extension methods for <see cref="Task"/>.
	/// </summary>
	public static class TaskHelpers
	{
		/// <summary>
		/// Handle any exceptions occurred in a <see cref="Task"/>
		/// by logging them to Application Insights.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <returns>
		/// Returns the task which handles the exceptions. It only runs if exceptions exist.
		/// </returns>
		public static Task LogExceptions(this Task task)
		{
			if (task == null) throw new ArgumentNullException(nameof(task));

			return task.ContinueWith(t => 
			{
				var telemetry = new Microsoft.ApplicationInsights.TelemetryClient();

				telemetry.TrackException(t.Exception);
			}, 
			TaskContinuationOptions.OnlyOnFaulted);

		}
	}
}