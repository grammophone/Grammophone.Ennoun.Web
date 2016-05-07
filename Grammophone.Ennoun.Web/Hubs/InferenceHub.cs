using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Grammophone.Ennoun.Web.Hubs
{
	/// <summary>
	/// Real-time inference progress hub.
	/// </summary>
	public class InferenceHub : Hub<IInferenceHub>
	{
	}
}