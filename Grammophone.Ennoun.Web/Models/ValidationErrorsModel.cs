using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Grammophone.Ennoun.Web.Models
{
	/// <summary>
	/// Encapsulates validation errors caught in Web API operations.
	/// </summary>
	public class ValidationErrorsModel
	{
		public ICollection<KeyValuePair<string, ModelState>> ModelStates { get; set; }
	}
}
