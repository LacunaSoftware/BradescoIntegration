using Lacuna.BradescoIntegration.Models.Contracts;
using Newtonsoft.Json;
using Lacuna.BradescoIntegration.Utils;
using System;

namespace Lacuna.BradescoIntegration.Models.Request {
	public class BankBilletBuyerData : IModel {

		/// <summary>
		/// Nome do pagador/sacado
		/// </summary>
		[JsonProperty("nome")]
		public string Name { get; set; }

		/// <summary>
		/// CPF ou CNPJ Informar somente números
		/// </summary>
		[JsonProperty("documento")]
		public string Document { get; set; }

		/// <summary>
		/// Endereço IP do Comprador
		/// </summary>
		[JsonProperty("ip")]
		public string Ip { get; set; }

		/// <summary>
		/// User agent do comprador
		/// </summary>
		[JsonProperty("user_agent")]
		public string UserAgent { get; set; }

		/// <summary>
		/// Dados de endereço do comprador
		/// </summary>
		[JsonProperty("endereco")]
		public BuyerAddress Address { get; set; }



		#region Validations

		[JsonIgnore]
		public bool Valid => isValid();

		private bool isValid() {
			if (string.IsNullOrEmpty(Name) || Name.Length > 40) {
				throw new Exception("Campo nome do comprador não pode estar vazio ou conter mais de 40 caracteres");
			}
			if (string.IsNullOrEmpty(Document)) {
				throw new Exception("Campo Documento do comprador não pode estar vazio");
			}
			//int documentoInt;
			//if (!int.TryParse(Documento, out documentoInt))
			//{
			//    throw new Exception("Campo Documento do comprador deve conter apenas números");
			//}

			if (!Helpers.IsValidCnpj(Document) && !Helpers.IsValidCpf(Document)) {
				throw new Exception("Campo documento do comprador deve ser um cpf ou cnpj válido");
			}

			if (!string.IsNullOrEmpty(Ip) && (Ip.Length < 16 || Ip.Length > 50) && !Helpers.IsValidIPv4(Ip)) {
				throw new Exception("Campo ip precisa de um IPv4 válido caso preenchido e conter entre 16 e 50 caracteres");
			}

			if (!string.IsNullOrEmpty(UserAgent) && UserAgent.Length > 255) {
				throw new Exception("Campo UserAgent não pode conter mais de 255 caracteres");
			}

			return Address.Valid;
		}

		#endregion
	}
}
