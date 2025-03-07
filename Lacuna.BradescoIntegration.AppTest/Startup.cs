using Lacuna.BradescoIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lacuna.BradescoIntegration.AppTest {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			//services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			services.AddControllersWithViews();
			// Configuring the Bradesco Integration Options from the appsettings
			services.Configure<BradescoIntegrationOptions>(Configuration.GetSection("Bradesco"));
			// Creating the Bradesco Integration Client as a Singleton and available for the Services and controllers to use it (Asp.Net Core configuration library option)
			services.AddBradescoIntegration();

			// In production, the Angular files will be served from this directory
			services.AddSpaStaticFiles(configuration => {
				configuration.RootPath = "ClientApp/dist";
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			} else {
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseSpaStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller}/{action=Index}/{id?}"
				);
			});

			app.UseSpa(spa => {

				if (env.IsDevelopment()) {
					spa.Options.SourcePath = "ClientApp";
				}
			});
		}
	}
}
