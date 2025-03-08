#if NET6_0_OR_GREATER

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace Lacuna.BradescoIntegration {

	public static class BradescoIntegrationBradescoIntegration {

		public static IServiceCollection AddBradescoIntegration(this IServiceCollection services, Action<BradescoIntegrationOptions> action = null) {

			if (action != null) {
				services.Configure(action);
			}
			services.AddTransient<BradescoClient, BradescoClientAspNetCore>();
			services.AddSingleton<BradescoIntegrationOptions>();
			services.AddHttpClient<BradescoClientAspNetCore>();
			return services;
		}
	}
}

#endif
