using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Grammophone.Ennoun.Web.Startup))]
namespace Grammophone.Ennoun.Web
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);

			app.MapSignalR();

			// Start the inference engine asynchronously.
			Controllers.InferenceSession.StartEngineAsync().LogExceptions();
		}
	}
}
