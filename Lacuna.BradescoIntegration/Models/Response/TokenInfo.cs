using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lacuna.BradescoIntegration.Models.Response {
	public class TokenInfo {
		[JsonProperty("token")]
		public string Token { get; set; }
		[JsonProperty("dataCriacao")]
		public DateTime CreationDate { get; set; }
	}
}
