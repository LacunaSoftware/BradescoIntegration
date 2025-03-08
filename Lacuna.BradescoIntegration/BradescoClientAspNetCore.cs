#if NET6_0_OR_GREATER

using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Lacuna.BradescoIntegration {

	internal class BradescoClientAspNetCore : BradescoClient {

		public BradescoClientAspNetCore(IHttpClientFactory clientFactory, IOptions<BradescoIntegrationOptions> options): base(clientFactory, options.Value) {
		}
	}
}

#endif
