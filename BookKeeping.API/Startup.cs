
using BookKeeping.Domain.Helpers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json;

using System;
using System.IO;
using System.Reflection;

namespace BookKeeping.API
{
	public class Startup
	{
		private readonly IHostEnvironment _hostEnvironment;

		public IConfiguration Configuration { get; }
		public static Assembly? CurrentAssembly { get; private set; }
		public static AssemblyName? CurrentAssemblyName { get; private set; }
		public static Version? AssemblyVersion { get; private set; }
		public static string? ApiName { get; private set; }
		public static string? ApiVersion { get; private set; }

		public Startup(
			IConfiguration configuration,
			IHostEnvironment hostEnvironment
		)
		{
			_hostEnvironment = hostEnvironment;
			Configuration = configuration;
			CurrentAssembly = typeof(Startup).Assembly;
			CurrentAssemblyName = CurrentAssembly.GetName();
			AssemblyVersion = CurrentAssemblyName.Version;
			if (!string.IsNullOrWhiteSpace(CurrentAssemblyName!.Name))
				ApiName = CurrentAssemblyName.Name!;
			else if (!string.IsNullOrWhiteSpace(CurrentAssembly!.FullName))
				ApiName = CurrentAssembly.FullName!;
			ApiVersion = $"v{AssemblyVersion!}";
		}


		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			_ = services.AddOptions();
			//_ = services.AddRazorPages();
			_ = services
				.AddControllersWithViews(opt => opt.Filters.Add<EtagAttribute>(0))
				.AddControllersAsServices()
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
					options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
				})
				//.AddIon<NewtonsoftJsonOutputFormatter>(o=>
				//{
				//	o.AddLinkRewritingFilter = false;
				//	o.AddOutputFormatter = true;
				//	o.RemoveJsonOutputformatter = false;
				//})
				;
			//_ = services.AddApiVersioning(o =>
			//{
			//	o.ReportApiVersions = true;
			//	o.DefaultApiVersion = new ApiVersion(
			//		AssemblyVersion!.Major,
			//		AssemblyVersion.Minor
			//	);
			//});
			_ = services.AddResponseCaching();

			var currentAssemblyName = CurrentAssemblyName!.Name!;

			services = services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc(
					ApiVersion!,
					new OpenApiInfo
					{
						Title = $"{ApiName!} {ApiVersion!}",
						Version = ApiVersion!,
						Contact = new(),
						License = new(),
						TermsOfService = new Uri("https://tos.vivasoft.com"),
						Description = $"{ApiName!} {ApiVersion!}"
					}
				);

				c.IncludeXmlComments(
					 Path.Combine(
					  _hostEnvironment.ContentRootPath,
					  $"{currentAssemblyName}.xml"
					 ),
					 true
				);
			});

			_ = services = services.AddSeed();

			_ = services.AddDomain(
				Configuration,
				"DefaultConnection",
				currentAssemblyName,
				CurrentAssembly!
			);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(
			IApplicationBuilder app,
			IWebHostEnvironment env,
			ISeed seed
		)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseWebAssemblyDebugging();
				seed.Migrate();
				seed.SeedData();
			}

			app.UseSwagger(c => c.SerializeAsV2 = false);

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint(
					$"/swagger/{ApiVersion}/swagger.json",
					$"{ApiName!} {ApiVersion!}"
				);
				//c.RoutePrefix = string.Empty;
			});

			app.UseHttpsRedirection();
			app.UseBlazorFrameworkFiles();
			app.UseStaticFiles();
			app.UseResponseCaching();

			app.UseRouting();

			app.UseAuthorization();
			_ = app.UseEndpoints(endpoints =>
			{
				//_ = endpoints.MapRazorPages();
				_ = endpoints.MapControllers();
				_ = endpoints.MapFallbackToFile("index.html");
				//_ = endpoints.MapFallbackToPage("/_Host");
			});
		}
	}
}
