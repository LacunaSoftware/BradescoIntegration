using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacuna.BradescoIntegration {

	public class BradescoIntegrationOptions {

		public bool IsSandbox { get; set; }

		public string Endpoint { get; set; }

		public string SandboxEndpoint { get; set; }

		public string MerchantId { get; set; }

		public string Key { get; set; }

		public string Email { get; set; }
	}
}
