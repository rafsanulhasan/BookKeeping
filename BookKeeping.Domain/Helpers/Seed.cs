using BookKeeping.Data;
using BookKeeping.Data.Abstractions;
using BookKeeping.Data.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using System;
using System.Linq;

using static BookKeeping.Domain.Entities.TransactionFlowConstants;

namespace BookKeeping.Domain.Helpers
{
	internal class Seed : ISeed
	{
		private readonly IRepository<TransactionEntity, int> _transactionRepository;
		private readonly IRepository<TransactionTypeEntity, int> _transactionTypeRepository;
		private readonly IRepository<TransactionFlowEntity, int> _transactionFlowRepository;
		private readonly DatabaseFacade _dbFacade;

		private void CreateTransactionFlows()
		{
			if (_transactionFlowRepository.Read().ToList().Any())
				return;
			_transactionFlowRepository.Create(new()
			{
				Way = Income
			});
			_transactionFlowRepository.Create(new()
			{
				Way = Expense
			});
			_transactionFlowRepository.SaveChangesAsync().Wait();
		}
		private void CreateTransactionTypes()
		{
			if (_transactionTypeRepository.Read().ToList().Any())
				return;
			_transactionTypeRepository.Create(new()
			{
				Type = "Type 1"
			});
			_transactionTypeRepository.Create(new()
			{
				Type = "Type 2"
			});
			_transactionTypeRepository.Create(new()
			{
				Type = "Type 3"
			});
			_transactionTypeRepository.SaveChangesAsync().Wait();
		}

		private void CreateTransaction(
			double amount,
			int month,
			int year,
			TransactionFlowEntity flow,
			TransactionTypeEntity type
		)
		{
			var date = new Random().Next(1, 28);
			_transactionRepository.Create(new()
			{
				TransactionDate = DateTime.Parse($"{date}/{month}/{year}"),
				Amount = amount,
				Currency = "$",
				TransactionFlowId = flow.Id,
				TransactionFlow = flow,
				TransactionTypeId = type.Id,
				TransactionType = type
			});
		}

		private void CreateTransactionsFor2020()
		{
			var flowEntities = _transactionFlowRepository.Read().ToList();
			var incomeFlow = flowEntities.Where(t => t.Way.Equals(Income)).Single();
			var expenseFlow = flowEntities.Where(t => t.Way.Equals(Expense)).Single();

			var transactionTypeEntities = _transactionTypeRepository.Read().ToList();
			var transactionType1 = transactionTypeEntities.Where(t => t.Type.Contains(1.ToString())).Single();
			var transactionType2 = transactionTypeEntities.Where(t => t.Type.Contains(2.ToString())).Single();
			var transactionType3 = transactionTypeEntities.Where(t => t.Type.Contains(3.ToString())).Single();

			#region January
			CreateTransaction(
				50,
				1,
				2020,
				incomeFlow,
				transactionType1
			);
			CreateTransaction(
				50,
				1,
				2020,
				incomeFlow,
				transactionType2
			);
			CreateTransaction(
				100,
				1,
				2020,
				expenseFlow,
				transactionType3
			);
			CreateTransaction(
				100,
				1,
				2020,
				expenseFlow,
				transactionType2
			);
			#endregion
			#region February
			CreateTransaction(
				25,
				2,
				2020,
				incomeFlow,
				transactionType2
			);
			CreateTransaction(
				25,
				2,
				2020,
				incomeFlow,
				transactionType3
			);
			CreateTransaction(
				70,
				2,
				2020,
				expenseFlow,
				transactionType1
			);
			#endregion
			#region March
			CreateTransaction(
				150,
				3,
				2020,
				incomeFlow,
				transactionType2
			);
			CreateTransaction(
				60,
				3,
				2020,
				expenseFlow,
				transactionType3
			);
			CreateTransaction(
				60,
				3,
				2020,
				expenseFlow,
				transactionType1
			);
			#endregion
			#region April
			CreateTransaction(
				50,
				4,
				2020,
				expenseFlow,
				transactionType1
			);
			CreateTransaction(
				50,
				4,
				2020,
				expenseFlow,
				transactionType3
			);
			CreateTransaction(
				50,
				4,
				2020,
				expenseFlow,
				transactionType2
			);
			#endregion
			#region May
			CreateTransaction(
				400,
				5,
				2020,
				incomeFlow,
				transactionType3
			);
			CreateTransaction(
				400,
				5,
				2020,
				incomeFlow,
				transactionType1
			);
			CreateTransaction(
				300,
				5,
				2020,
				expenseFlow,
				transactionType2
			);
			#endregion
			#region June
			CreateTransaction(
				50,
				6,
				2020,
				incomeFlow,
				transactionType2
			);
			CreateTransaction(
				50,
				6,
				2020,
				expenseFlow,
				transactionType3
			);
			#endregion
			#region July
			CreateTransaction(
				50,
				7,
				2020,
				incomeFlow,
				transactionType2
			);
			CreateTransaction(
				50,
				7,
				2020,
				incomeFlow,
				transactionType1
			);
			CreateTransaction(
				50,
				7,
				2020,
				expenseFlow,
				transactionType3
			);
			#endregion
		}

		private void CreateTransactionsFor2021()
		{
			var flowEntities = _transactionFlowRepository.Read().ToList();
			var incomeFlow = flowEntities.Where(t => t.Way.Equals(Income)).Single();
			var expenseFlow = flowEntities.Where(t => t.Way.Equals(Expense)).Single();

			var transactionTypeEntities = _transactionTypeRepository.Read().ToList();
			var transactionType1 = transactionTypeEntities.Where(t => t.Type.Contains(1.ToString())).Single();
			var transactionType2 = transactionTypeEntities.Where(t => t.Type.Contains(2.ToString())).Single();
			var transactionType3 = transactionTypeEntities.Where(t => t.Type.Contains(3.ToString())).Single();

			#region January
			CreateTransaction(
				100,
				1,
				2021,
				incomeFlow,
				transactionType1
			);
			CreateTransaction(
				200,
				1,
				2021,
				expenseFlow,
				transactionType3
			);
			#endregion
			#region February
			CreateTransaction(
				50,
				2,
				2021,
				incomeFlow,
				transactionType2
			);
			CreateTransaction(
				70,
				2,
				2021,
				expenseFlow,
				transactionType1
			);
			#endregion
			#region March
			CreateTransaction(
				150,
				3,
				2021,
				incomeFlow,
				transactionType2
			);
			CreateTransaction(
				120,
				3,
				2021,
				expenseFlow,
				transactionType3
			);
			#endregion
			#region April
			CreateTransaction(
				200,
				4,
				2021,
				expenseFlow,
				transactionType3
			);
			#endregion
			#region May
			CreateTransaction(
				400,
				5,
				2021,
				incomeFlow,
				transactionType3
			);
			CreateTransaction(
				400,
				5,
				2021,
				incomeFlow,
				transactionType1
			);
			CreateTransaction(
				300,
				5,
				2021,
				expenseFlow,
				transactionType2
			);
			#endregion
			#region June
			CreateTransaction(
				50,
				6,
				2021,
				incomeFlow,
				transactionType2
			);
			CreateTransaction(
				50,
				6,
				2021,
				expenseFlow,
				transactionType3
			);
			#endregion
			#region July
			CreateTransaction(
				50,
				7,
				2021,
				incomeFlow,
				transactionType2
			);
			CreateTransaction(
				50,
				7,
				2021,
				incomeFlow,
				transactionType1
			);
			CreateTransaction(
				50,
				7,
				2021,
				expenseFlow,
				transactionType3
			);
			#endregion
		}

		private void CreateTransactions()
		{
			if (_transactionRepository.Read().ToList().Any())
				return;

			CreateTransactionsFor2020();
			CreateTransactionsFor2021();

			_transactionRepository.SaveChangesAsync().Wait();
		}

		public Seed(
			IRepository<TransactionEntity, int> transactionRepository,
			IRepository<TransactionTypeEntity, int> transactionTypeRepository,
			IRepository<TransactionFlowEntity, int> transactionFlowRepository,
			BookKeepingDbContext dbContext
		)
		{
			_transactionRepository = transactionRepository;
			_transactionTypeRepository = transactionTypeRepository;
			_transactionFlowRepository = transactionFlowRepository;
			_dbFacade = dbContext.Database;
		}

		public void Migrate()
		{
			_dbFacade.EnsureCreated();
			_dbFacade.Migrate();
		}

		public void SeedData()
		{
			CreateTransactionFlows();
			CreateTransactionTypes();
			CreateTransactions();
		}
	}
}
