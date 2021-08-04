using BookKeeping.Data.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;

namespace BookKeeping.Data
{
	public class BookKeepingDbContext
		: DbContext
	{
		private readonly IHostEnvironment _hostEnvironment;
		private readonly ILoggerFactory _loggerFactory;

		public DbSet<TransactionEntity> Transactions { get; set; }
		public DbSet<TransactionFlowEntity> TransactionFlow { get; set; }
		public DbSet<TransactionTypeEntity> TransactionType { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public BookKeepingDbContext(
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
			DbContextOptions<BookKeepingDbContext> options,
			IHostEnvironment hostEnvironment,
			ILoggerFactory loggerFactory
		)
			: base(options)
		{
			_hostEnvironment = hostEnvironment;
			_loggerFactory = loggerFactory;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (_hostEnvironment.IsDevelopment())
			{
				_ = optionsBuilder.EnableDetailedErrors();
				_ = optionsBuilder.EnableSensitiveDataLogging();
				_ = optionsBuilder.LogTo(
					Console.WriteLine,
					LogLevel.Information
				);
			}
			_ = optionsBuilder.UseLoggerFactory(_loggerFactory);
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			var entityBuilder = modelBuilder.Entity<TransactionEntity>();

			_ = entityBuilder.ToTable("Transactions");
			_ = entityBuilder
				.HasOne(t => t.TransactionFlow)
				.WithMany(f => f.Transactions)
				.HasForeignKey(t => t.TransactionFlowId)
				.HasPrincipalKey(f => f.Id);
			_ = entityBuilder
				.HasOne(t => t.TransactionType)
				.WithMany(tt => tt.Transactions)
				.HasForeignKey(t => t.TransactionTypeId)
				.HasPrincipalKey(tt => tt.Id);

			_ = modelBuilder
				.Entity<TransactionFlowEntity>()
				.ToTable("TransactionFlows");

			_ = modelBuilder
				.Entity<TransactionTypeEntity>()
				.ToTable("TransactionTypes");

			base.OnModelCreating(modelBuilder);
		}
	}
}
