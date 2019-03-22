using Lacuna.BradescoIntegration.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lacuna.BradescoIntegration {
	public abstract class BradescoIntegrationException : Exception {

		public HttpMethod Verb { get; set; }

		public Uri Uri { get; set; }

		public BradescoIntegrationException(string message, HttpMethod verb, Uri uri, Exception innerException = null) : base(message, innerException) {
			Verb = verb;
			Uri = uri;
		}
	}

	public class BradescoIntegrationUnreachableException : BradescoIntegrationException {

		public BradescoIntegrationUnreachableException(HttpMethod verb, Uri uri, Exception innerException = null) : base($"Bradesco API {verb} {uri} is unreachable", verb, uri, innerException) {
		}
	}

	public class BradescoIntegrationHttpException : BradescoIntegrationException {

		public HttpStatusCode StatusCode { get; private set; }

		public string ErrorMessage { get; private set; }

		public BradescoIntegrationHttpException(HttpMethod verb, Uri uri, HttpStatusCode statusCode, string errorMessage = null, Exception innerException = null) : base(formatExceptionMessage(verb, uri, statusCode, errorMessage), verb, uri, innerException) {
			StatusCode = statusCode;
			ErrorMessage = errorMessage;
		}

		private static string formatExceptionMessage(HttpMethod verb, Uri uri, HttpStatusCode statusCode, string errorMessage) {
			var sb = new StringBuilder();
			sb.AppendFormat("Bradesco API {0} {1} returned HTTP error {2}", verb.Method, uri.AbsoluteUri, (int)statusCode);
			if (Enum.IsDefined(typeof(HttpStatusCode), statusCode)) {
				sb.AppendFormat(" ({0})", statusCode);
			}
			if (!string.IsNullOrWhiteSpace(errorMessage)) {
				sb.AppendFormat(": {0}", errorMessage);
			}
			return sb.ToString();
		}
	}

	public class BradescoIntegrationApiException : BradescoIntegrationException {

		public string Code { get; set; }

		public string BradescoMessage { get; set; }

		public string Details { get; set; } = string.Empty;

		public BradescoIntegrationApiException(HttpMethod verb, Uri uri, string code, string message, string details = null)
			: base(formatErrorMessage(verb, uri, code, message, details), verb, uri) {
			Code = code;
			BradescoMessage = message;
			Details = details;
		}

		private static string formatErrorMessage(HttpMethod verb, Uri uri, string code, string message, string details) {
			if (string.IsNullOrEmpty(details)) {
				return string.Format("Bradesco API {0} {1} returned an error. Bradesco error code: {2}. Message: {3}", verb, uri, code, message);
			} else {
				return string.Format("Bradesco API {0} {1} returned an error. Bradesco error code: {2}. Message: {3}. Details: {4}", verb, uri, code, message, details);
			}
		}
	}
}
