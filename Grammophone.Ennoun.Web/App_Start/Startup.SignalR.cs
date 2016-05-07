using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Owin;

namespace Grammophone.Ennoun.Web
{
	public partial class Startup
	{
		public void ConfigureSignalR(IAppBuilder app)
		{
			if (app == null) throw new ArgumentNullException(nameof(app));

			// Make enums serialized as strings.

			var serializer = new JsonSerializer();
			serializer.Converters.Add(new StringEnumConverter());

			GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

			app.MapSignalR();
		}
	}
}