using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Grammophone.Ennoun.Web.Models.Inference;

namespace Grammophone.Ennoun.Web.Controllers
{
	public class InferenceController : Controller
	{
		// GET: Inference
		public ActionResult Index()
		{
			return View("Index");
		}

		[HttpPost]
		public ActionResult Infer(TextRequestModel textRequest)
		{
			if (textRequest == null) throw new ArgumentNullException(nameof(textRequest));

			if (!ModelState.IsValid) return View("Index", textRequest);

			return View("Infer", textRequest);
		}

		[HttpGet]
		public ActionResult Tools()
		{
			return View();
		}
	}
}