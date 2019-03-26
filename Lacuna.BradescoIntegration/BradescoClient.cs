using Lacuna.BradescoIntegration.Models.Request;
using Lacuna.BradescoIntegration.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Lacuna.BradescoIntegration {

	public class BradescoClient {

		private HttpClient _httpPostClient;
		private HttpClient _httpGetClient;

		private readonly BradescoIntegrationOptions options;

		protected HttpClient HttpPostClient {
			get {
				if (_httpPostClient == null) {
					_httpPostClient = new HttpClient {
						BaseAddress = new Uri(options.IsSandbox ? options.SandboxEndpoint : options.Endpoint)
					};
					_httpPostClient.DefaultRequestHeaders.Accept.Clear();

					var authHeader = options.MerchantId + ":" + options.Key;
					var authHeaderBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authHeader));
					_httpPostClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderBase64);
					_httpPostClient.DefaultRequestHeaders.Add("Accept-Charset", Constants.CharSet);
				}
				return _httpPostClient;
			}
		}

		protected HttpClient HttpGetClient {
			get {
				if (_httpGetClient == null) {
					_httpGetClient = new HttpClient {
						BaseAddress = new Uri(options.IsSandbox ? options.SandboxEndpoint : options.Endpoint)
					};
					_httpGetClient.DefaultRequestHeaders.Accept.Clear();

					var authHeader = options.Email + ":" + options.Key;
					var authHeaderBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authHeader));
					_httpGetClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderBase64);
					_httpGetClient.DefaultRequestHeaders.Add("Accept-Charset", Constants.CharSet);
				}
				return _httpGetClient;
			}
		}

		public BradescoClient(BradescoIntegrationOptions options) {
			this.options = options;
		}

		/// <summary>
		/// Creates a bank billet using Bradesco API
		/// </summary>
		public async Task<BankBilletGenerationResponse> CreateBankBilletAsync(BankBilletRequest boleto) {
			try {
				if (!boleto.Valid) {
					throw new Exception("Boleto não é válido");
				}
			} catch (Exception ex) {
				throw new Exception("Boleto não é válido. Consulte as exceções internas", ex);
			}

			var data = JsonConvert.SerializeObject(boleto);
			var requestUri = "/apiboleto/transacao";

			var postResponse = await performHttpRequestAsync(HttpMethod.Post, requestUri,
				() => HttpPostClient.PostAsync(requestUri, new StringContent(data, Encoding.UTF8, Constants.MediaType))
			);

			var stream = await postResponse.Content.ReadAsStreamAsync();

			using (var reader = new StreamReader(stream)) {
				var jsonResp = reader.ReadToEnd();
				var response = JsonConvert.DeserializeObject<BankBilletGenerationResponse>(jsonResp,
					 new IsoDateTimeConverter { DateTimeFormat = Constants.DateRetrievalFormat });

				if (response.Status.Code != "0" && response.Status.Code != "-501") {
					throw new BradescoIntegrationApiException(HttpMethod.Get, new Uri(HttpGetClient.BaseAddress, requestUri), response.Status.Code, response.Status.Message);
				}

				return response;
			}
		}

		/// <summary>
		/// Retrieves a bank billet using Bradesco API
		/// </summary>
		public async Task<BradescoResponse> GetBankBilletAsync(string orderNumber) {

			var authResp = await queryAuthenticationAsync();
			if (authResp.Token == null || authResp.Token.Token == null) {
				throw new Exception("No authentication token");
			}

			var requestUri = $"/SPSConsulta/GetOrderById/{options.MerchantId}?token={authResp.Token.Token}&orderId={orderNumber}";
			var resp = await performHttpRequestAsync(HttpMethod.Get, requestUri,
				() => HttpGetClient.GetAsync(requestUri)
			);

			var stream = await resp.Content.ReadAsStreamAsync();

			using (var reader = new StreamReader(stream)) {
				var jsonretorno = reader.ReadToEnd();
				var response = JsonConvert.DeserializeObject<BradescoResponse>(jsonretorno,
					 new IsoDateTimeConverter { DateTimeFormat = Constants.DateRetrievalFormat });

				if (response.Status.Code != "0" && response.Status.Code != "-501") {
					throw new BradescoIntegrationApiException(HttpMethod.Get, new Uri(HttpGetClient.BaseAddress, requestUri), response.Status.Code, response.Status.Message);
				}

				return response;
			}
		}

		/// <summary>
		/// Retrieves a list of bank billets using Bradesco API according to the parameters received. Difference of start and end dates cannot be greater than 6 days (Bradesco API constraint)
		/// </summary>
		public async Task<BradescoResponsePaginated> ListBankBilletAsync(DateTime startDate, DateTime endDate, int status = 0, int offset = 1, int limit = 100) {

			if (endDate < startDate) {
				throw new Exception("End date mt be greater than the star date.");
			}

			if (endDate - startDate > TimeSpan.FromDays(6)) {
				throw new Exception("Difference of start and end dates cannot be greater than 6 days.");
			}

			var authResp = await queryAuthenticationAsync();
			if (authResp.Token == null || authResp.Token.Token == null) {
				throw new Exception("No authentication token");
			}

			var requestUri = $"/SPSConsulta/GetOrderListPayment/{options.MerchantId}/boleto?token={authResp.Token.Token}&dataInicial={startDate.ToString(Constants.DateUriFormat)}&dataFinal={endDate.ToString(Constants.DateUriFormat)}&status={status}&offset={offset}&limit={limit}";

			var resp = await performHttpRequestAsync(HttpMethod.Get, requestUri,
				() => HttpGetClient.GetAsync(requestUri)
			);

			var stream = await HttpGetClient.GetStreamAsync(requestUri);

			using (var reader = new StreamReader(stream)) {
				var jsonretorno = reader.ReadToEnd();
				var response = JsonConvert.DeserializeObject<BradescoResponsePaginated>(jsonretorno,
					 new IsoDateTimeConverter { DateTimeFormat = Constants.DateRetrievalFormat });

				if (response.Status.Code != "0" && response.Status.Code != "-501") {
					throw new BradescoIntegrationApiException(HttpMethod.Get, new Uri(HttpGetClient.BaseAddress, requestUri), response.Status.Code, response.Status.Message);
				}

				return response;
			}
		}

		/// <summary>
		/// Retrieves an authentication token for the Bradesco API GET operations
		/// </summary>
		private async Task<BradescoResponse> queryAuthenticationAsync() {
			var requestUri = $"/SPSConsulta/Authentication/{options.MerchantId}";
			var resp = await HttpGetClient.GetAsync(requestUri);

			if (!resp.IsSuccessStatusCode) {
				throw new BradescoIntegrationHttpException(HttpMethod.Get, new Uri(HttpGetClient.BaseAddress, requestUri), resp.StatusCode, resp.ReasonPhrase);
			}

			var stream = await HttpGetClient.GetStreamAsync(requestUri);

			using (var reader = new StreamReader(stream)) {
				var jsonretorno = reader.ReadToEnd();
				var response = JsonConvert.DeserializeObject<BradescoResponse>(jsonretorno,
					 new IsoDateTimeConverter { DateTimeFormat = Constants.DateRetrievalFormat });
				return response;
			}
		}

		private async Task<HttpResponseMessage> performHttpRequestAsync(HttpMethod verb, string requestUri, Func<Task<HttpResponseMessage>> asyncFunc) {
			var uri = new Uri(HttpPostClient.BaseAddress, requestUri);
			HttpResponseMessage httpResponse;
			try {
				httpResponse = await asyncFunc();
			} catch (Exception ex) {
				throw new BradescoIntegrationUnreachableException(verb, uri, ex);
			}
			if (!httpResponse.IsSuccessStatusCode) {
				throw new BradescoIntegrationHttpException(verb, uri, httpResponse.StatusCode, httpResponse.ReasonPhrase);
			}
			return httpResponse;
		}


	}
}
