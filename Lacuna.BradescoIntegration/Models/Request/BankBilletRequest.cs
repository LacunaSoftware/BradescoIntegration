using Lacuna.BradescoIntegration.Models.Contracts;
using Lacuna.BradescoIntegration.Request;
using Newtonsoft.Json;
using System;

namespace Lacuna.BradescoIntegration.Models.Request {
	/// <summary>
	/// Objeto para requisição junto ao webservice do Bradesco
	/// </summary>
	public class BankBilletRequest : IModelo {
		/// <summary>
		/// Token enviado pela loja para ser utiliado como parametro adicional da url de confirmação do pedido.
		/// A url de confirmação do pedido é configurada no gerenciador do lojista.
		/// Verifique a documentação de integração para mais detalhes.
		/// </summary>
		[JsonProperty("token_request_confirmacao_pagamento")]
		public string PaymentConfirmationTokenRequest { get; set; }

		/// <summary>
		/// Identificador do estabelecimento fornecido pelo Bradesco
		/// Exemplo:
		/// 18022016Pedido_100_54878
		/// </summary>
		[JsonProperty("merchant_id")]
		public string MerchantId { get; set; }

		/// <summary>
		/// Valor fixo: 300  conforme especificado na documentação do Bradesco
		/// </summary>
		[JsonProperty("meio_pagamento")]
		public string PaymentType { get; set; } = "300";

		/// <summary>
		/// Dados do pedido
		/// </summary>
		[JsonProperty("pedido")]
		public OrderRequestData Order { get; set; }

		/// <summary>
		/// Dados do pedido para gerar o boleto
		/// </summary>
		[JsonProperty("boleto")]
		public BankBilletRequestConfig BankBilletRequestConfig { get; set; }

		/// <summary>
		/// Dados que identificam o comprador | dados do sacado
		/// </summary>
		[JsonProperty("comprador")]
		public BankBilletBuyerData BuyerInfo { get; set; }

		#region Validações

		[JsonIgnore]
		public bool Valid => isValid();

		private bool isValid() {
			if (string.IsNullOrEmpty(MerchantId) || MerchantId.Length != 9) {
				throw new Exception("Merchant Id can't be null and must have 9 digits");
			}

			return Order.Valid && BuyerInfo.Valid && BankBilletRequestConfig.Valid;
		}

		#endregion
	}
}
