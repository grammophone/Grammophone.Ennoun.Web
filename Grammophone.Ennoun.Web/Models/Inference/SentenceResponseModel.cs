using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Grammophone.EnnounInference.Sentences;

namespace Grammophone.Ennoun.Web.Models.Inference
{
	public class SentenceResponseModel
	{
		public IList<MessageModel> Messages { get; set; } = new List<MessageModel>();

		public SentenceInferenceModel SentenceInference { get; set; }
	}
}