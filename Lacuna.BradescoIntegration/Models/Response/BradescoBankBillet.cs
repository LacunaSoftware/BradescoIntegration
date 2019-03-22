using Newtonsoft.Json;
using System;

namespace Lacuna.BradescoIntegration.Models.Response {
	public class BradescoBankBillet {
		/// <summary>
		/// Valor do título
		/// 1500 se refere ao valor de R$ 15,00
		/// </summary>
		[JsonProperty("valor_titulo")]
		public string Value { get; set; }

		/// <summary>
		///  Data de geração do título
		/// Formato ISO 8601
		/// </summary>
		[JsonProperty("data_geracao")]
		public DateTime CreationDate { get; set; }

		/// <summary>
		/// Data de atualização do título
		/// Nota: Caso o título tenha sido enviado anteriormente e o mesmo esteja apto a ser gerado, o registro será atualizado
		/// Formato ISO 8601
		/// </summary>
		[JsonProperty("data_atualizacao")]
		public DateTime? UpdateDate { get; set; }

		/// <summary>
		/// Linha digitável do boleto
		/// </summary>
		[JsonProperty("linha_digitavel")]
		public string DigitableCode { get; set; }

		/// <summary>
		/// Linha digitável formatada
		/// </summary>
		[JsonProperty("linha_digitavel_formatada")]
		public string DigitableCodeFormated { get; set; }

		/// <summary>
		/// Token identificador do boleto bancário gerado
		/// </summary>
		[JsonProperty("token")]
		public string Token { get; set; }

		/// <summary>
		/// Link de acesso ao boleto bancário
		/// </summary>
		[JsonProperty("url_acesso")]
		public string BankBilletAccessUrl { get; set; }
	}
}
