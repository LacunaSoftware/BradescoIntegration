using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lacuna.BradescoIntegration.Models {
	public class Order {
		[JsonProperty("numero")]
		public string Number { get; set; }
		[JsonProperty("valor")]
		public string Value { get; set; }
		[JsonProperty("data")]
		public DateTime Date { get; set; }
		[JsonProperty("valorPago")]
		public string PayedValue { get; set; }
		[JsonProperty("dataPagamento")]
		public DateTime? PaymentDate { get; set; }
		[JsonProperty("dataCredito")]
		public DateTime? CreditDate { get; set; }
		[JsonProperty("linhaDigitavel")]
		public string DigitableCode { get; set; }
		[JsonProperty("status")]
		public string Status { get; set; }
		[JsonProperty("erro")]
		public string Error { get; set; }
	}
}
