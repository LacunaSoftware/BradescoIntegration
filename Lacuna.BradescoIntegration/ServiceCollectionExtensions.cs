using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace Lacuna.BradescoIntegration {

	public static class BradescoIntegrationBradescoIntegration {

		private static BradescoIntegrationOptions options;

		public static IServiceCollection AddBradescoIntegration(this IServiceCollection services, Action<BradescoIntegrationOptions> action = null) {

			if (action != null) {
				services.Configure(action);
			}

			services.AddHttpClient("BradescoIntegration:GetClient")
				.ConfigureHttpClient((serviceProvider, c) => {
					var options = serviceProvider.GetRequiredService<IOptions<BradescoIntegrationOptions>>().Value;

					c.BaseAddress = new Uri(
						options.IsSandbox ? options.SandboxEndpoint : options.Endpoint
					);

					c.DefaultRequestHeaders.Accept.Clear();
					c.DefaultRequestHeaders.Add("Accept-Charset", Constants.CharSet);
					c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
						"Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(options.Email + ":" + options.Key))
					);
				});

			services.AddHttpClient("BradescoIntegration:PostClient")
				.ConfigureHttpClient((serviceProvider, c) => {
					var options = serviceProvider.GetRequiredService<IOptions<BradescoIntegrationOptions>>().Value;

					c.BaseAddress = new Uri(
						options.IsSandbox ? options.SandboxEndpoint : options.Endpoint
					);

					c.DefaultRequestHeaders.Accept.Clear();
					c.DefaultRequestHeaders.Add("Accept-Charset", Constants.CharSet);
					c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
						"Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(options.MerchantId + ":" + options.Key))
					);
			});

			services.AddSingleton<BradescoClient, BradescoClientAspNetCore>();

			return services;
		}
	}
}
