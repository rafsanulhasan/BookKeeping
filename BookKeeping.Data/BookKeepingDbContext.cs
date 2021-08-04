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
				optionsBuilder.EnableDetailedErrors();
				optionsBuilder.EnableSensitiveDataLogging();
				optionsBuilder.LogTo(
					Console.WriteLine,
					 LogLevel.Information
				);
			}
			optionsBuilder.UseLoggerFactory(_loggerFactory);
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder
				.Entity<TransactionEntity>()
				.HasOne(t => t.TransactionFlow)
				.WithMany(f => f.Transactions)
				.HasForeignKey(t => t.TransactionFlowId)
				.HasPrincipalKey(f => f.Id);
			modelBuilder
				.Entity<TransactionEntity>()
				.HasOne(t => t.TransactionType)
				.WithMany(tt => tt.Transactions)
				.HasForeignKey(t => t.TransactionTypeId)
				.HasPrincipalKey(tt => tt.Id);
			base.OnModelCreating(modelBuilder);
		}
	}
}
