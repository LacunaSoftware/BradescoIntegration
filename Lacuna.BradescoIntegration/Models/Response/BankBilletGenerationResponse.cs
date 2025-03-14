using Newtonsoft.Json;

namespace Lacuna.BradescoIntegration.Models.Response {
	public class BankBilletGenerationResponse : GenericResponse {
		/// <summary>
		/// Identificador do estabelecimento fornecido pelo Bradesco
		/// </summary>
		[JsonProperty("merchant_id")]
		public string MerchantId { get; set; }

		/// <summary>
		/// código do meio de pagamento
		/// </summary>
		[JsonProperty("meio_pagamento")]
		public string MeioPagamento { get; set; }

		/// <summary>
		/// Dados do boleto retornado pelo webservice
		/// </summary>
		[JsonProperty("boleto")]
		public BradescoBankBillet Boleto { get; set; }

	}
}
