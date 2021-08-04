
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BookKeeping.Data.Abstractions;
using BookKeeping.Data.Entities;
using BookKeeping.Data.Repositories;

namespace BookKeeping.Data.Helpers
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddData(
			this IServiceCollection services,
			string connectionString,
			string migrationAssemblyName
		)
		{
			_ = services.AddDbContext<BookKeepingDbContext>(
				dbContextOptions =>
				{
					dbContextOptions.UseSqlServer(
						connectionString,
						sqlServerDbContextBuilder =>
						{
							sqlServerDbContextBuilder
							    .MigrationsAssembly(migrationAssemblyName);
							sqlServerDbContextBuilder.EnableRetryOnFailure(3);
							sqlServerDbContextBuilder.CommandTimeout(60);
							sqlServerDbContextBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
						}
					);
				}
			);

			services.AddScoped<IRepository<TransactionEntity, int>>(sp =>
			{
				return new Repository<TransactionEntity, int>(
					sp.GetRequiredService<BookKeepingDbContext>()
				);
			});

			services.AddScoped<IRepository<TransactionTypeEntity, int>>(sp =>
			{
				return new Repository<TransactionTypeEntity, int>(
					sp.GetRequiredService<BookKeepingDbContext>()
				);
			});

			services.AddScoped<IRepository<TransactionFlowEntity, int>>(sp =>
			{
				return new Repository<TransactionFlowEntity, int>(
					sp.GetRequiredService<BookKeepingDbContext>()
				);
			});

			return services;
		}
	}
}
