using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Grammophone.Ennoun.Web.Models;

namespace Grammophone.Ennoun.Web.Controllers.Api
{
	/// <summary>
	/// Base class for controllers, offering validation helpers.
	/// </summary>
	public abstract class BaseApiController : ApiController
	{
		#region Protected methods

		/// <summary>
		/// If there are validation errors, returns a <see cref="ValidationErrorsModel"/>
		/// encapsulating them, else returns null.
		/// </summary>
		protected ValidationErrorsModel GetValidationErrorsModel()
		{
			if (ModelState.IsValid) return null;

			return new ValidationErrorsModel { ModelStates = ModelState };
		}

		/// <summary>
		/// If there are validation errors, throws a <see cref="ValidationException"/>
		/// which will result in a "Bad Request" HTTP response with a
		/// <see cref="ValidationErrorsModel"/> payload, else does nothing.
		/// </summary>
		/// <exception cref="ValidationException">Thrown when there are validation errors.</exception>
		protected void EnsureNoValidationErrors()
		{
			if (ModelState.IsValid) return;

			throw new ValidationException(GetValidationErrorsModel());
		}

		#endregion
	}
}