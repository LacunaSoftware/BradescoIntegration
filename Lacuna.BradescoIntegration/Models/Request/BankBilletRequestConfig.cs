﻿using Lacuna.BradescoIntegration.Models.Contracts;
using Lacuna.BradescoIntegration.Models.Request;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Lacuna.BradescoIntegration.Request {
	/// <summary>
	/// Informações do cedente
	/// </summary>
	public class BankBilletRequestConfig : IModel {
		/// <summary>
		/// Nome do beneficiário / cedente
		/// </summary>
		[JsonProperty("beneficiario")]
		public string Beneficiary { get; set; } //150 chars

		/// <summary>
		/// Codigo da carteira (dois digitos)
		/// Exemplo: 26
		/// </summary>
		[JsonProperty("carteira")]
		public string BankWallet { get; set; } // 2 chars

		/// <summary>
		/// Nosso número (identificador do boleto).
		/// O dígito será calculado pela plataforma do Bradesco deve ter 11 dígitos
		/// </summary>
		[JsonProperty("nosso_numero")]
		public string BankBilletNumber { get; set; } // 11 chars

		/// <summary>
		/// Data de emissão do boleto
		/// </summary>
		[JsonProperty("data_emissao")]
		[JsonConverter(typeof(CustomDateTimeConverter))]
		public DateTime EmissionDate { get; set; } //2016-03-01

		/// <summary>
		/// Data de vencimento do boleto
		/// </summary>
		[JsonProperty("data_vencimento")]
		[JsonConverter(typeof(CustomDateTimeConverter))]
		public DateTime ValidUntil { get; set; } //2016-03-01

		/// <summary>
		/// Valor do boleto para pagamento. Exemplo: 1500 Refere-se ao valor de R$ 15,00
		/// </summary>
		[JsonProperty("valor_titulo")]
		public string Value { get; set; }

		/// <summary>
		/// Url do logotio que será exibido no topo do boleto.
		/// Exemplo: https://onlinesites.com.br/downloads/lo1.png
		/// </summary>
		[JsonProperty("url_logotipo")]
		public string UrlLogo { get; set; } //url da logo do cliente limite maximo de 255 chars nao é obrigatorio

		/// <summary>
		/// Mensagem de cabeçalho exibida no topo do boleto
		/// </summary>
		[JsonProperty("mensagem_cabecalho")]
		public string HeaderMessage { get; set; } // 255 chars no máximo

		/// <summary>
		/// Tipo de renderização.
		/// Caso não seja enviado um valor será utilizado o valor configurado no Gerenciador do Lojista.
		/// </summary>
		[JsonProperty("tipo_renderizacao")]
		public byte RenderType { get; set; }

		/// <summary>
		/// Informações de registro do boleto.
		/// </summary>
		[JsonProperty("registro")]
		public BankBilletRegistry RegistryInfo { get; set; }

		/// <summary>
		/// Informações de instrução no boleto.
		/// </summary>
		[JsonProperty("instrucoes")]
		public IDictionary<string, string> Instructions { get; protected set; }

		public int AddInstruction(string instruction, int pos = 0) {
			if (string.IsNullOrEmpty(instruction)) {
				return 0;
			}

			if (Instructions == null) {
				Instructions = new Dictionary<string, string>();
			}

			if (pos > 0 && pos <= 12) {
				Instructions[$"instrucao_linha_{pos}"] = instruction.Trim().Substring(0, 60);
				return 0;
			} else if (pos == 0) {
				var instructionsCount = Instructions.Count >= 11 ? 11 : Instructions.Count;
				var newPos = instructionsCount + 1;
				var entryName = $"instrucao_linha_{newPos}";

				Instructions[entryName] = instruction.Trim().Substring(0, 60);
				return 0;
			} else {
				return -1;
			}
		}

		public int AddInstructions(List<string> instructions) {
			if (Instructions == null) {
				Instructions = new Dictionary<string, string>();
			}

			for (int i = 0; i < 12; i++) {
				var trimmedString = instructions[i].Trim();
				Instructions[$"instrucao_linha_{i+1}"] = trimmedString.Length > 60 ? trimmedString.Substring(0, 60) : trimmedString;
			}

			return 0;
		}

		public void ClearInstructions() {
			Instructions = new Dictionary<string, string>();
		}

		#region Validations

		[JsonIgnore]
		public bool Valid => isValid();

		private bool isValid() {
			if (string.IsNullOrEmpty(Beneficiary)) {
				throw new Exception("Nome do beneficiário não pode estar vazio");
			}
			if (Beneficiary.Length > 150) {
				throw new Exception("Nome do beneficiário maior que 150 characteres");
			}

			if (BankWallet.Length != 2) {
				throw new Exception("Carteira deve ter dois digitos");
			}

			if (BankBilletNumber.Length != 11) {
				throw new Exception("Nosso número deve ter 11 dígitos");
			}

			if (EmissionDate == null) {
				throw new Exception("É necessário informar uma data de emissão do boleto");
			}

			if (ValidUntil == null) {
				throw new Exception("É necessário informar uma data de vencimento para o boleto");
			}

			if (!int.TryParse(Value, out var valorBoleto) || valorBoleto <= 0) {
				throw new Exception("Valor do boleto não pode estar vazio ou ter um valor menor ou igual a zero");
			}

			if (!string.IsNullOrEmpty(UrlLogo) && UrlLogo.Length > 255) {
				throw new Exception("Url da logo não pode conter mais do que 255 caracteres");
			}

			if (!string.IsNullOrEmpty(HeaderMessage) && HeaderMessage.Length > 255) {
				throw new Exception("Mensagem do cabeçalho não pode conter mais do que 255 caracteres");
			}

			if (RenderType > 2) {
				throw new Exception("Valor do tipo de renderização é inválido");
			}

			if(Instructions != null && Instructions.Count != 0 && (Instructions.Count > 12 || Instructions.Any(p => p.Value.Length > 60))) {
				throw new Exception("Não podem haver mais de 12 instruções e elas não podem ter mais de 60 caracteres (cada uma)");
			}


			return true;

		}

		#endregion
	}
}
