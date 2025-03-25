using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lacuna.BradescoIntegration.Models.Response {
	public class BradescoResponse {
		[JsonProperty("pedidos")]
		public List<Order> Orders { get; set; }
		[JsonProperty("status")]
		public Status Status { get; set; }
	}
}
