using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Grammophone.Ennoun.Web.Hubs
{
	public class InferenceHub : Hub<IInferenceHub>
	{
		public void Test()
		{
		}
	}
}