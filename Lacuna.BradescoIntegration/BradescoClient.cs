using Lacuna.BradescoIntegration.Models.Request;
using Lacuna.BradescoIntegration.Models.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Lacuna.BradescoIntegration {

	public class BradescoClient {

		private readonly ILogger<BradescoClient> logger;
		private readonly BradescoIntegrationOptions options;
		private readonly HttpClient client;

		public BradescoClient(ILogger<BradescoClient> logger, IHttpClientFactory clientFactory, BradescoIntegrationOptions bradescoOptions) {
			this.logger = logger;
			this.options = bradescoOptions;

			client = clientFactory.CreateClient();
			client.BaseAddress = new Uri(
				options.IsSandbox ?
				options.SandboxEndpoint : options.Endpoint);
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Add("Accept-Charset", Constants.CharSet);
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

			var data = new StringContent(JsonConvert.SerializeObject(boleto), Encoding.UTF8, Constants.MediaType);
			var requestUri = "/apiboleto/transacao";

			return await sendHttpRequestAsync<BankBilletGenerationResponse>(HttpMethod.Post, requestUri, data);
		}

		/// <summary>
		/// Retrieves a bank billet using Bradesco API
		/// </summary>
		public async Task<BradescoResponse> GetBankBilletAsync(string orderNumber) {
			var authResp = await queryAuthenticationAsync();
			var requestUri = $"/SPSConsulta/GetOrderById/{options.MerchantId}?token={authResp.TokenInfo.Token}&orderId={orderNumber}";

			return await sendHttpRequestAsync<BradescoResponse>(HttpMethod.Get, requestUri);
		}

		/// <summary>
		/// Retrieves a list of bank billets using Bradesco API according to the parameters received. Difference of start and end dates cannot be greater than 6 days (Bradesco API constraint)
		/// </summary>
		public async Task<BradescoResponsePaginated> ListBankBilletAsync(DateTime startDate, DateTime endDate, int status = 0, int offset = 1, int limit = 100) {
			if (!verifyDate(startDate, endDate)) {
				return new BradescoResponsePaginated();
			}

			var authResp = await queryAuthenticationAsync();
			if (authResp == null) {
				return new BradescoResponsePaginated();
			}
			var requestUri = $"/SPSConsulta/GetOrderListPayment/{options.MerchantId}/boleto?token={authResp.TokenInfo.Token}&dataInicial={startDate.ToString(Constants.DateUriFormat)}&dataFinal={endDate.ToString(Constants.DateUriFormat)}&status={status}&offset={offset}&limit={limit}";

			return await sendHttpRequestAsync<BradescoResponsePaginated>(HttpMethod.Get, requestUri);
		}

		/// <summary>
		/// Retrieves an authentication token for the Bradesco API GET operations
		/// </summary>
		private async Task<BradescoResponse> queryAuthenticationAsync() {
			var requestUri = $"/SPSConsulta/Authentication/{options.MerchantId}";

			var response = await sendHttpRequestAsync<BradescoResponse>(HttpMethod.Get, requestUri);
			if (response.TokenInfo == null || response.TokenInfo.Token == null) {
				logger.LogDebug($"Token retrieving failed. {JsonConvert.SerializeObject(response)}");
				return null;
			}
			return response;
		}

		private async Task<T> sendHttpRequestAsync<T>(HttpMethod method, string endpoint, HttpContent content = null) {
			setAuthorizationHeader(method);

			HttpResponseMessage httpResponse;

			if (method == HttpMethod.Get) {
				httpResponse = await client.GetAsync(endpoint);
			} else if (method == HttpMethod.Post) {
				httpResponse = await client.PostAsync(endpoint, content);
			} else {
				throw new NotSupportedException($"HTTP method {method} not supported.");
			}

			await handleErrorResponseAsync(httpResponse, endpoint);

			var responseContent = await httpResponse.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(responseContent, new IsoDateTimeConverter { DateTimeFormat = Constants.DateRetrievalFormat });
		}

		private void setAuthorizationHeader(HttpMethod method) {

			if (method == HttpMethod.Get) {
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
					"Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(options.Email + ":" + options.Key)));

			} else if (method == HttpMethod.Post) {
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
					"Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(options.MerchantId + ":" + options.Key)));
			} else {
				throw new NotSupportedException($"HTTP method {method} not supported.");
			}

		}

		private async Task handleErrorResponseAsync(HttpResponseMessage response, string requestUri) {
			var responseContent = await response.Content.ReadAsStringAsync();
			var content = JsonConvert.DeserializeObject<GenericResponse>(responseContent);

			if (!response.IsSuccessStatusCode) {
				throw new BradescoIntegrationHttpException(HttpMethod.Get, new Uri(client.BaseAddress, requestUri), response.StatusCode, content.Status.Message);
			}
			if (response.RequestMessage.Method == HttpMethod.Get && content.Status.Code != "0" && content.Status.Code != "-501") {
				throw new BradescoIntegrationApiException(HttpMethod.Get, new Uri(client.BaseAddress, requestUri), content.Status.Code, content.Status.Message);
			} else if (response.RequestMessage.Method == HttpMethod.Post && content.Status.Code != "0" && content.Status.Code != "-501" && content.Status.Code != "93005999") {

				// Considerando como sucesso respostas com código 93005999, conforme manual:
				// "93005999 - ² Para este retorno, não é necessária nenhuma ação pois o Registro será processado internamente"
				throw new BradescoIntegrationApiException(HttpMethod.Post, new Uri(client.BaseAddress, requestUri), content.Status.Code, content.Status.Message);
			}
		}

		private bool verifyDate(DateTime startDate, DateTime endDate) {
			if (endDate < startDate) {
				logger.LogInformation("End date should be greater than the start date.");
				return false;
			}

			if (endDate - startDate > TimeSpan.FromDays(6)) {
				logger.LogInformation("Difference of start and end dates cannot be greater than 6 days.");
				return false;
			}
			return true;
		}
	}
}
