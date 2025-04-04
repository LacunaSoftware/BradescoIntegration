using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace Lacuna.BradescoIntegration {

	internal class BradescoClientAspNetCore : BradescoClient {

		public BradescoClientAspNetCore(ILogger<BradescoClient> logger, IHttpClientFactory clientFactory, IOptions<BradescoIntegrationOptions> options) : base(logger, clientFactory, options.Value) {
		}
	}
}
