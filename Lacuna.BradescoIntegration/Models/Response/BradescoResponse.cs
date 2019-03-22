using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacuna.BradescoIntegration.Models.Response {
	public class BradescoResponse {
		[JsonProperty("status")]
		public OpStatus Status { get; set; }
		[JsonProperty("token")]
		public TokenInfo Token { get; set; }
		[JsonProperty("pedidos")]
		public List<Order> Orders { get; set; }
	}
}
