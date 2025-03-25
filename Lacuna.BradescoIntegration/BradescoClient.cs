using Lacuna.BradescoIntegration.Models.Request;
using Lacuna.BradescoIntegration.Models.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lacuna.BradescoIntegration {

	public class BradescoClient {

		private readonly ILogger<BradescoClient> logger;
		private readonly BradescoIntegrationOptions options;
		private readonly HttpClient getClient;
		private readonly HttpClient postClient;

		public BradescoClient(ILogger<BradescoClient> logger, IHttpClientFactory clientFactory, BradescoIntegrationOptions bradescoOptions) {
			this.logger = logger;
			this.options = bradescoOptions;

			getClient = clientFactory.CreateClient("GetClient");
			postClient = clientFactory.CreateClient("PostClient");
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

			var response = await sendHttpRequestAsync(HttpMethod.Post, requestUri, data);
			return JsonConvert.DeserializeObject<BankBilletGenerationResponse>(await response.Content.ReadAsStringAsync(), new IsoDateTimeConverter { DateTimeFormat = Constants.DateRetrievalFormat });
		}

		/// <summary>
		/// Retrieves a bank billet using Bradesco API
		/// </summary>
		public async Task<BradescoResponse> GetBankBilletAsync(string orderNumber) {
			var authResp = await queryAuthenticationAsync();
			if (authResp == null || authResp.TokenInfo == null || authResp.TokenInfo.Token == null) {
				throw new Exception("No authentication token");
			}
			var requestUri = $"/SPSConsulta/GetOrderById/{options.MerchantId}?token={authResp.TokenInfo.Token}&orderId={orderNumber}";

			var response = await sendHttpRequestAsync(HttpMethod.Get, requestUri);
			return JsonConvert.DeserializeObject<BradescoResponse>(await response.Content.ReadAsStringAsync(), new IsoDateTimeConverter { DateTimeFormat = Constants.DateRetrievalFormat });
		}

		/// <summary>
		/// Retrieves a list of bank billets using Bradesco API according to the parameters received. Difference of start and end dates cannot be greater than 6 days (Bradesco API constraint)
		/// </summary>
		public async Task<BradescoResponsePaginated> ListBankBilletAsync(DateTime startDate, DateTime endDate, int status = 0, int offset = 1, int limit = 100) {
			verifyDate(startDate, endDate);

			var authResp = await queryAuthenticationAsync();
			if (authResp == null || authResp.TokenInfo == null || authResp.TokenInfo.Token == null) {
				throw new Exception("No authentication token");
			}
			var requestUri = $"/SPSConsulta/GetOrderListPayment/{options.MerchantId}/boleto?token={authResp.TokenInfo.Token}&dataInicial={startDate.ToString(Constants.DateUriFormat)}&dataFinal={endDate.ToString(Constants.DateUriFormat)}&status={status}&offset={offset}&limit={limit}";

			var response = await sendHttpRequestAsync(HttpMethod.Get, requestUri);
			return JsonConvert.DeserializeObject<BradescoResponsePaginated>(await response.Content.ReadAsStringAsync(), new IsoDateTimeConverter { DateTimeFormat = Constants.DateRetrievalFormat });
		}

		/// <summary>
		/// Retrieves an authentication token for the Bradesco API GET operations
		/// </summary>
		private async Task<GenericResponse> queryAuthenticationAsync() {
			var requestUri = $"/SPSConsulta/Authentication/{options.MerchantId}";

			var httpResponse = await sendHttpRequestAsync(HttpMethod.Get, requestUri);
			var responseContent = await httpResponse.Content.ReadAsStringAsync();

			if (!httpResponse.IsSuccessStatusCode) {
				logger.LogDebug($"Token retrieving failed. {responseContent}");
				return null;
			}

			var response = JsonConvert.DeserializeObject<GenericResponse>(responseContent, new IsoDateTimeConverter { DateTimeFormat = Constants.DateRetrievalFormat });
			return response;
		}

		private async Task<HttpResponseMessage> sendHttpRequestAsync(HttpMethod method, string endpoint, HttpContent content = null) {

			HttpResponseMessage httpResponse;

			if (method == HttpMethod.Get) {
				httpResponse = await getClient.GetAsync(endpoint);
			} else if (method == HttpMethod.Post) {
				httpResponse = await postClient.PostAsync(endpoint, content);
			} else {
				throw new NotSupportedException($"HTTP method {method} not supported.");
			}

			var responseContent = await httpResponse.Content.ReadAsStringAsync();
			handleErrorResponseAsync(httpResponse, responseContent, endpoint);

			return httpResponse;
		}

		private void handleErrorResponseAsync(HttpResponseMessage response, string responseContent, string requestUri) {
			var content = JsonConvert.DeserializeObject<GenericResponse>(responseContent, new IsoDateTimeConverter { DateTimeFormat = Constants.DateRetrievalFormat });

			if (!response.IsSuccessStatusCode) {
				throw new BradescoIntegrationHttpException(HttpMethod.Get, new Uri(getClient.BaseAddress, requestUri), response.StatusCode, content.Status.Message);
			}
			if (response.RequestMessage.Method == HttpMethod.Get && content.Status.Code != "0" && content.Status.Code != "-501") {
				throw new BradescoIntegrationApiException(HttpMethod.Get, new Uri(getClient.BaseAddress, requestUri), content.Status.Code, content.Status.Message);
			} else if (response.RequestMessage.Method == HttpMethod.Post && content.Status.Code != "0" && content.Status.Code != "-501" && content.Status.Code != "93005999") {

				// Considerando como sucesso respostas com código 93005999, conforme manual:
				// "93005999 - ² Para este retorno, não é necessária nenhuma ação pois o Registro será processado internamente"
				throw new BradescoIntegrationApiException(HttpMethod.Post, new Uri(postClient.BaseAddress, requestUri), content.Status.Code, content.Status.Message);
			}
			logger.LogDebug($"handleErrorResponseAsync: {responseContent}");
		}

		private void verifyDate(DateTime startDate, DateTime endDate) {
			if (endDate < startDate) {
				throw new Exception("End date should be greater than the start date.");
			}

			if (endDate - startDate > TimeSpan.FromDays(6)) {
				throw new Exception("Difference of start and end dates cannot be greater than 6 days.");
			}
		}
	}
}
