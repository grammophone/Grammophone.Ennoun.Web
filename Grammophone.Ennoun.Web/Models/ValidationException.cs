using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grammophone.Ennoun.Web.Models
{
	/// <summary>
	/// Thrown from <see cref="Controllers.Api.BaseApiController"/> descendants
	/// when there exist validation errors. 
	/// </summary>
	[Serializable]
	public class ValidationException : Exception
	{
		#region Construction

		public ValidationException(ValidationErrorsModel validationErrors)
			: base("Validation error")
		{
			if (validationErrors == null) throw new ArgumentNullException(nameof(validationErrors));

			this.ValidationErrors = validationErrors;
		}

		/// <summary>
		/// USed for serialization.
		/// </summary>
		protected ValidationException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

		#endregion

		#region Public properties

		public ValidationErrorsModel ValidationErrors { get; private set; }

		#endregion
	}
}