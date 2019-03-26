using Lacuna.BradescoIntegration.Models.Request;
using Lacuna.BradescoIntegration.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lacuna.BradescoIntegration.AppTest.Classes {
	public class TestHelpers {
		public static BankBilletRequest GenerateBankBilletRequisition(string merchantId) {
			var ret = new BankBilletRequest {
				MerchantId = merchantId,
				//TokenRequestConfirmacaoPagamento = "abc123456789",
				BankBilletRequestConfig = GenerateRequestConfig(),
				BuyerInfo = GenerateBuyerInfo(),
				Order = GenerateOrder()
			};

			return ret;
		}

		public static BankBilletRequestConfig GenerateRequestConfig() {
			var ret = new BankBilletRequestConfig {
				//Registro = GeraInformacoesRegistro(),
				Beneficiary = "Nome Beneficiario",
				BankBilletNumber = $"01234567891",
				EmissionDate = DateTime.Now,
				ValidUntil = DateTime.Now.AddDays(5),
				HeaderMessage = "Teste mensagem cabeçalho",
				Value = "1501",
				BankWallet = "26"
			};

			// The bank billet can have no more than 12 instructions, each one with max length of 60 characters
			ret.AddInstructions(new List<string>() { "Linha 01", "Linha 02", "Linha 03", "Linha 04", "Linha 05", "Linha 06", "Linha 07", "Linha 08", "Linha 09", "Linha 10", "Linha 11", "Linha 12" });

			return ret;
		}

		public static BankBilletRegistry GenerateRegistryInfo() {
			var ret = new BankBilletRegistry();

			return ret;
		}


		public static OrderRequestData GenerateOrder() {
			var ret = new OrderRequestData {
				Value = "1500",
				Number = $"P8976_A98",
				Description = "Pedido na loja Boleto Bradesco"
			};

			return ret;
		}

		public static BankBilletBuyerData GenerateBuyerInfo() {
			var ret = new BankBilletBuyerData {
				//UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36",
				Address = GenerateBuyerAddress(),
				Name = "Integration Test",
				//Ip = "127.0.0.1",
				Document = "55500000756"
			};

			return ret;
		}

		public static BuyerAddress GenerateBuyerAddress() {
			var ret = new BuyerAddress {
				Number = "123",
				City = "Divinopolis",
				Street = "Avenida Coronel Julio Ribeiro Gontijo",
				Neighborhood = "Esplanada",
				PostalCode = "35501000",
				UF = "MG"
			};
			return ret;
		}
	}
}
