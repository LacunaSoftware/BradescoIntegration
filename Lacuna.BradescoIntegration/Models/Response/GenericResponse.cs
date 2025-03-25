using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lacuna.BradescoIntegration.Models.Response {
	public class GenericResponse {
		/// <summary>
		/// Dados do status da requisição
		/// </summary>
		[JsonProperty("status")]
		public OpStatus Status { get; set; }
		[JsonProperty("token")]
		public TokenInfo TokenInfo { get; set; }
		[JsonProperty("pedidos")]
		public List<Order> Orders { get; set; }
	}
}
