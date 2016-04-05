using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using Grammophone.Ennoun.Web.Models;
using Microsoft.Owin.Security.OAuth;

namespace Grammophone.Ennoun.Web
{
	public static class WebApiConfig
	{
		#region Error handler class

		class EnnounExceptionFilterAttribute : ExceptionFilterAttribute
		{
			public override void OnException(HttpActionExecutedContext actionExecutedContext)
			{
				var exception = actionExecutedContext.Exception;

				/* If we ever introduce a secure domain model, uncomment these lines. */

				//if (exception is AccessDeniedException)
				//{
				//	actionExecutedContext.Response =
				//		actionExecutedContext.Request.CreateErrorResponse(
				//		System.Net.HttpStatusCode.Forbidden,
				//		exception);

				//	return;
				//}

				UserException userException = exception as UserException;

				if (userException != null)
				{
					actionExecutedContext.Response =
						actionExecutedContext.Request.CreateResponse(
						System.Net.HttpStatusCode.InternalServerError,
						new UserErrorModel(userException));

					return;
				}

				var validationException = exception as ValidationException;

				if (validationException != null)
				{
					actionExecutedContext.Response =
						actionExecutedContext.Request.CreateResponse(
						System.Net.HttpStatusCode.BadRequest,
						validationException.ValidationErrors);

					return;
				}

				var telemetry = new Microsoft.ApplicationInsights.TelemetryClient();

				telemetry.TrackException(exception);

				base.OnException(actionExecutedContext);

			}
		}

		#endregion

		public static void Register(HttpConfiguration config)
		{
			// The following two force not using the [Serializable] stream
			// for producing JSON and XML respectively.
			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
				new Newtonsoft.Json.Serialization.DefaultContractResolver();

			config.Formatters.XmlFormatter.UseXmlSerializer = true;

			config.Filters.Add(new EnnounExceptionFilterAttribute());

			config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
				new Newtonsoft.Json.Converters.StringEnumConverter());

			config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
				Newtonsoft.Json.ReferenceLoopHandling.Ignore;

			// Supress redirection for unauthorized Web API calls
			// using the following two lines.
			// Configure Web API to use only bearer token authentication.
			//config.SuppressDefaultHostAuthentication();
			config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
					name: "DefaultApi",
					routeTemplate: "api/{controller}/{id}",
					defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}
