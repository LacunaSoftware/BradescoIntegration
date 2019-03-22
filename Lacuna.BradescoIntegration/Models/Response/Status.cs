using Newtonsoft.Json;

namespace Lacuna.BradescoIntegration.Models.Response {
	public class Status {
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

		/// <summary>
		/// Apresentado quando houver um erro associado com a geração do boleto, com a finalidade de apresentar maiores
		/// informações a respeito do problema.
		/// </summary>
		[JsonProperty("detalhes")]
		public string Details { get; set; } = string.Empty;
	}
}
