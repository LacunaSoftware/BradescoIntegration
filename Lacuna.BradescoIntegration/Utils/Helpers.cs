﻿using System.Linq;
using System.Text.RegularExpressions;

namespace Lacuna.BradescoIntegration.Utils {
	public static class Helpers {
		/// <summary>
		/// Verifica se um CPF é válido
		/// </summary>
		/// <param name="cpf"></param>
		/// <returns></returns>
		public static bool IsValidCpf(string cpf) {
			cpf = RemoveNotAlphanumeric(cpf);
			if (cpf.Length > 11) { return false; }
			while (cpf.Length != 11) {
				cpf = '0' + cpf;
			}

			var igual = true;
			for (var i = 1; i < 11 && igual; i++) {
				if (cpf[i] != cpf[0]) {
					igual = false;
				}
			}

			if (igual || cpf == "12345678909") {
				return false;
			}

			var numeros = new int[11];
			for (var i = 0; i < 11; i++) {
				numeros[i] = int.Parse(cpf[i].ToString());
			}

			var soma = 0;
			for (var i = 0; i < 9; i++) {
				soma += (10 - i) * numeros[i];
			}

			var resultado = soma % 11;
			if (resultado == 1 || resultado == 0) {
				if (numeros[9] != 0) {
					return false;
				}
			} else if (numeros[9] != 11 - resultado) {
				return false;
			}

			soma = 0;
			for (var i = 0; i < 10; i++) {
				soma += (11 - i) * numeros[i];
			}

			resultado = soma % 11;

			if (resultado == 1 || resultado == 0) {
				if (numeros[10] != 0) {
					return false;
				}
			} else {
				if (numeros[10] != 11 - resultado) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Verificar se um CNPJ é válido
		/// </summary>
		/// <param name="cnpj"></param>
		/// <returns></returns>
		public static bool IsValidCnpj(string cnpj) {
			cnpj = RemoveNotAlphanumeric(cnpj);
			string CNPJ;
			CNPJ = cnpj.Replace(".", "");
			CNPJ = CNPJ.Replace("/", "");
			CNPJ = CNPJ.Replace("-", "");

			int[] digitos, soma, resultado;
			int nrDig;
			string ftmt;
			bool[] CNPJOk;

			ftmt = "6543298765432";
			digitos = new int[14];
			soma = new int[2];
			soma[0] = 0;
			soma[1] = 0;
			resultado = new int[2];
			resultado[0] = 0;
			resultado[1] = 0;
			CNPJOk = new bool[2];
			CNPJOk[0] = false;
			CNPJOk[1] = false;

			try {
				for (nrDig = 0; nrDig < 14; nrDig++) {
					digitos[nrDig] = int.Parse(
					 CNPJ.Substring(nrDig, 1));
					if (nrDig <= 11) {
						soma[0] += (digitos[nrDig] *
								int.Parse(ftmt.Substring(
								  nrDig + 1, 1)));
					}

					if (nrDig <= 12) {
						soma[1] += (digitos[nrDig] *
								int.Parse(ftmt.Substring(
								  nrDig, 1)));
					}
				}

				for (nrDig = 0; nrDig < 2; nrDig++) {
					resultado[nrDig] = (soma[nrDig] % 11);
					if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1)) {
						CNPJOk[nrDig] = (
								digitos[12 + nrDig] == 0);
					} else {
						CNPJOk[nrDig] = (
								digitos[12 + nrDig] == (
								11 - resultado[nrDig]));
					}
				}

				return (CNPJOk[0] && CNPJOk[1]);

			} catch {
				return false;
			}

		}

		/// <summary>
		/// Remove caracteres não númericos de uma string
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string RemoveNotAlphanumeric(string text) {
			if (string.IsNullOrEmpty(text)) {
				return string.Empty;
			}

			var reg = new Regex(@"[^0-9]");
			return reg.Replace(text, string.Empty);
		}

		/// <summary>
		/// Verifica se um IPV4 é válido
		/// </summary>
		/// <param name="ip"></param>
		/// <returns></returns>
		public static bool IsValidIPv4(string ip) {
			var reg = new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
			return reg.IsMatch(ip);
		}

		/// <summary>
		/// Valida o número do pedido
		/// </summary>
		/// <param name="numeroPedido"></param>
		/// <returns></returns>
		public static bool IsValidOrderNumber(string numeroPedido) {
			var reg = new Regex("(^[A-Za-z0-9\\._]*\\d+[A-Zaz0-9\\._-]*$)");
			return reg.IsMatch(numeroPedido);
		}

		/// <summary>
		/// Cria o número do pedido
		/// </summary>
		/// <param name="pedidoId">Id do pedido </param>
		/// <param name="boletoNumero">número da versão do boleto para o pedido</param>
		/// <param name="letra1">Primeira letra</param>
		/// <param name="letra2">Segunda letra</param>
		/// <returns>Exemplo de retorno: P8976_A98</returns>
		public static string OrderNumberGeneration(long pedidoId, long boletoNumero, string letra1 = "P", string letra2 = "O") {
			var pedidoStr = new string(pedidoId.ToString("0000").Take(4).ToArray());
			var boletoNumeroStr = new string(boletoNumero.ToString("00").Take(2).ToArray());

			return $"{letra1}{pedidoStr}_{letra2}{boletoNumeroStr}";
		}
	}
}
