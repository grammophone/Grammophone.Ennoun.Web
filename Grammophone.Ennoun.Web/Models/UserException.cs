using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grammophone.Ennoun.Web.Models
{
	/// <summary>
	/// An exception whose message is intended to be displayed to the user.
	/// </summary>
	[Serializable]
	public class UserException : Exception
	{
		public UserException(string message) : base(message) { }

		public UserException(string message, Exception inner) : base(message, inner) { }

		/// <summary>
		/// Used for serialization.
		/// </summary>
		protected UserException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}