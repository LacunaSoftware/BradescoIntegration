using Lacuna.BradescoIntegration.Models.Contracts;
using Newtonsoft.Json;
using System;

namespace Lacuna.BradescoIntegration.Models.Request {
	public class BuyerAddress : IModel {
		/// <summary>
		/// Cep apenas números
		/// </summary>
		[JsonProperty("cep")]
		public string PostalCode { get; set; }

		/// <summary>
		/// Logradouro do comprador
		/// </summary>
		[JsonProperty("logradouro")]
		public string Street { get; set; }

		/// <summary>
		/// Número do endereço
		/// </summary>
		[JsonProperty("numero")]
		public string Number { get; set; }

		/// <summary>
		/// Campo de complemento para o endereço
		/// </summary>
		[JsonProperty("complemento")]
		public string Complement { get; set; }

		/// <summary>
		/// Bairro do comprador
		/// </summary>
		[JsonProperty("bairro")]
		public string Neighborhood { get; set; }

		/// <summary>
		/// Campo cidade do comprador
		/// </summary>
		[JsonProperty("cidade")]
		public string City { get; set; }

		/// <summary>
		/// Uf do comprador
		/// </summary>
		[JsonProperty("uf")]
		public string UF { get; set; }


		#region Validations

		[JsonIgnore]
		public bool Valid => isValid();

		private bool isValid() {

			if (PostalCode.Length != 8) {
				throw new Exception("Campo cep do comprador deve ter apenas números e ter comprimento de 8 caracteres");
			}
			if (string.IsNullOrEmpty(Street) || Street.Length > 70) {
				throw new Exception("Campo logradouro do comprador não pode estar vazio e deve conter no máximo 70 caracteres");
			}
			if (string.IsNullOrEmpty(Number) || Number.Length > 10) {
				throw new Exception("Campo número do endereço do comprador não pode estar vazio ou conter mais de 10 caracteres");
			}
			if (!string.IsNullOrEmpty(Complement) && Complement.Length > 20) {
				throw new Exception("Campo complemento do endereço do comprador não pode conter mais de 20 caracteres");
			}
			if (string.IsNullOrEmpty(Neighborhood) || Neighborhood.Length > 50) {
				throw new Exception("Campo bairro do comprador não pode estar vazio ou conter mais de 50 caracteres");
			}
			if (string.IsNullOrEmpty(City) || City.Length > 50) {
				throw new Exception("Campo cidade do comprador não pode estar vazio ou conter mais de 50 caracteres");
			}
			if (string.IsNullOrEmpty(UF) || UF.Length != 2) {
				throw new Exception("Campo uf do comprador não pode estar vazio e deve conter 2 caracteres");
			}


			return true;
		}

		#endregion
	}
}
