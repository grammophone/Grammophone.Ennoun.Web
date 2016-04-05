using System.Web;
using System.Web.Mvc;
using Microsoft.ApplicationInsights;

namespace Grammophone.Ennoun.Web
{
	public class FilterConfig
	{
		#region Auxilliary classes

		internal class LoggingErrorAttribute : HandleErrorAttribute
		{
			private TelemetryClient telemetryClient;

			public LoggingErrorAttribute()
			{
				this.telemetryClient = new TelemetryClient();
			}

			public override void OnException(ExceptionContext filterContext)
			{
				if (filterContext != null
					&& filterContext.HttpContext != null
					&& filterContext.Exception != null)
				{
					if (filterContext.HttpContext.IsCustomErrorEnabled)
					{
						this.telemetryClient.TrackException(filterContext.Exception);
					}
				}

				base.OnException(filterContext);
			}
		}

		#endregion

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			//filters.Add(new HandleErrorAttribute());
			filters.Add(new LoggingErrorAttribute());
		}
	}
}
