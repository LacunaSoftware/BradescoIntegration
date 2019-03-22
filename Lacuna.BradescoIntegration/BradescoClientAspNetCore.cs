#if NETSTANDARD2_0

using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lacuna.BradescoIntegration {

	internal class BradescoClientAspNetCore : BradescoClient {

		public BradescoClientAspNetCore(IOptions<BradescoIntegrationOptions> options): base(options.Value) {
		}
	}
}

#endif
