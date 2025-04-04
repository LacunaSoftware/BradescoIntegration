using Lacuna.BradescoIntegration.AppTest.Classes;
using Lacuna.BradescoIntegration.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Lacuna.BradescoIntegration.AppTest.Controllers {

	[Route("api/[controller]")]
	public class BankBilletController : Controller {

		private readonly BradescoClient bradescoClient;
		private readonly IOptions<BradescoIntegrationOptions> bradescoOptions;

		public BankBilletController(BradescoClient bradescoClient, IOptions<BradescoIntegrationOptions> bradescoOptions) {
			this.bradescoClient = bradescoClient;
			this.bradescoOptions = bradescoOptions;
		}

		[HttpPost]
		public async Task<IActionResult> BoletoTestAsync() {
			var rng = TestHelpers.GenerateBankBilletRequisition(bradescoOptions.Value.MerchantId);

			var retorno = await bradescoClient.CreateBankBilletAsync(rng);
			return Ok(retorno);
		}

		[HttpPost("v2")]
		public async Task<IActionResult> BoletoTestAsync([FromBody] BankBilletRequest data) {
			var retorno = await bradescoClient.CreateBankBilletAsync(data);
			return Ok(retorno);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetBoletoTestAsync(string id) {
			var retorno = await bradescoClient.GetBankBilletAsync(id);
			return Ok(retorno);
		}

		[HttpGet]
		public async Task<IActionResult> GetBoletoAsync() {
			var now = DateTime.Now;
			var retorno = await bradescoClient.ListBankBilletAsync(now.AddDays(-1), now);
			return Ok(retorno);
		}
	}
}
