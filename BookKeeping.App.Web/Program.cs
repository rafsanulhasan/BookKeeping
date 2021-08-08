using BookKeeping.App.Web.ViewModels;

using Fluxor;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookKeeping.App.Web
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder
							.CreateDefault(args);

			builder.RootComponents.Add<App>("#app");

			var services = builder.Services;
			var hostEnvironment = builder.HostEnvironment;

			services.AddScoped(sp =>
			{
				var http = new HttpClient
				{
					BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
				};
				http!.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
				http.DefaultRequestHeaders.Add("api-version", "1.0");
				http.DefaultRequestHeaders.Add(HeaderNames.CacheControl, "public,max-age=60");
				return http;
			});

			services.AddFluxor(c =>
			{
				c.ScanAssemblies(typeof(Program).Assembly);
				if (hostEnvironment.IsDevelopment())
					c.UseReduxDevTools();
			});

			services.AddScoped<IncomeExpenseViewModel>();

			services.AddLogging();

			await builder.Build().RunAsync();
		}
	}
}
