using Newtonsoft.Json;

namespace Lacuna.BradescoIntegration.Models.Response {
	public class GenericResponse {
		/// <summary>
		/// Dados do status da requisição
		/// </summary>
		[JsonProperty("status")]
		public OpStatus Status { get; set; }
	}
}
