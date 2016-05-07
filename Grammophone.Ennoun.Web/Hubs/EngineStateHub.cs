using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Grammophone.Ennoun.Web.Hubs
{
	/// <summary>
	/// Real-time inference engine state hub.
	/// </summary>
	public class EngineStateHub : Hub<IEngineStateHub>
	{
	}
}