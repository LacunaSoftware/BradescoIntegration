using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lacuna.BradescoIntegration.Models.Response {
	public class OpStatus {
		/// <summary>
		/// Código da mensagem de retorno - Ver tabela de códigos de retorno na documentação do Bradesco.
		/// </summary>
		[JsonProperty("codigo")]
		public string Code { get; set; }

		/// <summary>
		/// Descritivo da mensagem de retorno
		/// </summary>
		[JsonProperty("mensagem")]
		public string Message { get; set; }
	}
}
