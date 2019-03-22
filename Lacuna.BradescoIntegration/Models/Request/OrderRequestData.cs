using Lacuna.BradescoIntegration.Models.Contracts;
using Newtonsoft.Json;
using Lacuna.BradescoIntegration.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacuna.BradescoIntegration.Models.Request {
	public class OrderRequestData : IModelo {
		/// <summary>
		/// Identificador do pedido na loja.
		/// Formato Alfanúmerico
		/// Ex.:  P8976_A98
		/// </summary>
		[JsonProperty("numero")]
		public string Number { get; set; }

		/// <summary>
		/// Valor do pedido
		/// Exemplo: 1500 Refere-se ao valor de R$ 15,00
		/// </summary>
		[JsonProperty("valor")]
		public string Value { get; set; }

		[JsonProperty("descricao")]
		public string Description { get; set; }


		//(^[A-Za-z0-9\\._]*\\d+[A-Zaz0-9\\._-]*$)

		#region Validations

		[JsonIgnore]
		public bool Valid => isValid();

		private bool isValid() {

			if (string.IsNullOrEmpty(Number)) {
				throw new Exception("Order number can't be null");
			}

			if (!Helpers.IsValidOrderNumber(Number)) {
				throw new Exception("Order number not valid. See the documentation for further info");
			}

			if (string.IsNullOrEmpty(Value)) {
				throw new Exception("Order value can't be empty or null");
			}

			int valorInt;

			if (!int.TryParse(Value, out valorInt)) {
				throw new Exception("Order value must be a number");
			}

			if (string.IsNullOrEmpty(Description) || Description.Length > 255) {
				throw new Exception("Order description can't be empty and can't be larger than 255 characters");
			}


			return true;

		}

		#endregion
	}
}
