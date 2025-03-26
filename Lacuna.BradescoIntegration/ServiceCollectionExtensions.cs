using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lacuna.BradescoIntegration {

	public static class BradescoIntegrationBradescoIntegration {

		public static IServiceCollection AddBradescoIntegration(this IServiceCollection services, Action<BradescoIntegrationOptions> action = null) {

			if (action != null) {
				services.Configure(action);
			}

			services.AddTransient<BradescoClient, BradescoClientAspNetCore>();
			services.AddSingleton<BradescoIntegrationOptions>();

			return services;
		}
	}
}
