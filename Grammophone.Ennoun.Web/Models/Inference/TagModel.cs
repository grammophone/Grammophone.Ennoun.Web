using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grammophone.Ennoun.Web.Models.Inference
{
	public class TagModel
	{
		public string Type { get; set; }

		public IEnumerable<InflectionModel> Inflections { get; set; }
	}
}