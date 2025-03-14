using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lacuna.BradescoIntegration.Models.Response {
	public class BradescoResponse : GenericResponse {
		[JsonProperty("token")]
		public TokenInfo TokenInfo { get; set; }
		[JsonProperty("pedidos")]
		public List<Order> Orders { get; set; }
	}
}
