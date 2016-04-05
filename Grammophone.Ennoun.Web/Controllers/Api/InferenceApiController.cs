using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Grammophone.Ennoun.Web.Hubs;
using Grammophone.Ennoun.Web.Models;
using Grammophone.Ennoun.Web.Models.Inference;
using Grammophone.EnnounInference.Sentences;
using Grammophone.GenericContentModel;
using Grammophone.LanguageModel.Grammar;
using Grammophone.Lexica.LexiconModel;
using Microsoft.AspNet.SignalR;

namespace Grammophone.Ennoun.Web.Controllers.Api
{
	/// <summary>
	/// Web API controller for inference operations.
	/// </summary>
	[RoutePrefix("API/Inference")]
	public class InferenceApiController : BaseApiController
	{
		#region Operations

		// GET: api/InferenceApi
		[Route("")]
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET: api/InferenceApi/5
		[Route("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST: api/InferenceApi
		[Route("")]
		public void Post([FromBody]string value)
		{
		}

		// PUT: api/InferenceApi/5
		[Route("{id}")]
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE: api/InferenceApi/5
		[Route("{id}")]
		public void Delete(int id)
		{
		}

		[HttpPost]
		[Route("Infer")]
		public async Task<IList<SentenceResponseModel>> InferAsync([FromBody]TextRequestModel textRequest)
		{
			if (textRequest == null) throw new ArgumentNullException(nameof(textRequest));

			EnsureNoValidationErrors();

			using (var inferenceSession = new InferenceSession(textRequest.ConnectionID))
			{
				return await inferenceSession.InferAsync(textRequest.Text);
			}
		}

		[HttpGet]
		[Route("GetLemmata")]
		public IReadOnlyCollection<LemmaResult> GetLemmata(string form)
		{
			if (form == null) throw new ArgumentNullException(nameof(form));

			using (var inferenceSession = new InferenceSession())
			{
				return inferenceSession.GetLemmata(form);
			}
		}

		[HttpGet]
		[Route("GetBestLemmata")]
		public IReadOnlyCollection<LemmaResult> GetBestLemmata(string form)
		{
			if (form == null) throw new ArgumentNullException(nameof(form));

			using (var inferenceSession = new InferenceSession())
			{
				return inferenceSession.GetBestLemmata(form);
			}
		}

		[HttpGet]
		[Route("GetLemmataByKey")]
		public IReadOnlyCollection<Lemma> GetLemmataByKey(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			using (var inferenceSession = new InferenceSession())
			{
				return inferenceSession.GetLemmataByKey(key);
			}
		}

		#endregion

		#region Private methods

		#endregion
	}
}
