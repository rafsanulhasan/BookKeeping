using BookKeeping.Data;
using BookKeeping.Data.Abstractions;
using BookKeeping.Data.Entities;
using BookKeeping.Data.Helpers;
using BookKeeping.Domain.Aggregates;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Linq;
using System.Reflection;

namespace BookKeeping.Domain.Helpers
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection RegisterMappiongProfiles(
			this IServiceCollection services,
			params Assembly[] externalAssemblies
		)
		{
			var currentAssembly = typeof(ServiceCollectionExtensions).Assembly;
			var newExternalAssemblies = externalAssemblies is null || externalAssemblies.Length == 0
				? (new Assembly[1] { currentAssembly })
				: externalAssemblies.Concat(new[] { currentAssembly })
								.ToArray();
			return services.AddAutoMapper(newExternalAssemblies);
		}

		public static IServiceCollection AddDomain(
			this IServiceCollection services,
			IConfiguration configuration,
			string connectionStringName,
			string migrationAssemblyName,
			params Assembly[]? externalMappingAssemblies
		)
		{
			if (externalMappingAssemblies is not null
			 && externalMappingAssemblies.Length > 0
			)
			{
				_ = services.RegisterMappiongProfiles(externalMappingAssemblies);
			}

			_ = services.AddScoped<ITransactionAggregate, TransactionAggregate>();

			return services.AddData(
				configuration.GetConnectionString(connectionStringName),
				migrationAssemblyName
			);
		}

		public static IServiceCollection AddSeed(
			this IServiceCollection services
		)
			=> services.AddScoped<ISeed>(sp
				=> new Seed(
					sp.GetRequiredService<IRepository<TransactionEntity, int>>(),
					sp.GetRequiredService<IRepository<TransactionTypeEntity, int>>(),
					sp.GetRequiredService<IRepository<TransactionFlowEntity, int>>(),
					sp.GetRequiredService<BookKeepingDbContext>()
				)
			);
	}
}
