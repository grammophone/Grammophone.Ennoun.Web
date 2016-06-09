using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Grammophone.Ennoun.Web.Models.Inference
{
	/// <summary>
	/// Model for requesting inference on a piece of text.
	/// </summary>
	public class TextRequestModel
	{
		/// <summary>
		/// The input ancient Greek text, allowing a maximum of 5000 characters.
		/// </summary>
		[Required(ErrorMessageResourceType = typeof(TextRequestResources), ErrorMessageResourceName = "TEXT_IS_REQUIRED")]
		[MaxLength(3000, ErrorMessageResourceType = typeof(TextRequestResources), ErrorMessageResourceName = "TEXT_TOO_LONG")]
		public string Text { get; set; }

		/// <summary>
		/// Optional connection ID for reporting real-time progress using SignalR.
		/// </summary>
		public string ConnectionID { get; set; }
	}
}