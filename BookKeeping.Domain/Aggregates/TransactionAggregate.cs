using BookKeeping.Data.Abstractions;
using BookKeeping.Data.Entities;
using BookKeeping.Domain.Abstractions;
using BookKeeping.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Linq;
using BookKeeping.Data;

namespace BookKeeping.Domain.Aggregates
{
	public class TransactionAggregate
		: IAggregate<Transaction>, ITransactionAggregate
	{
		private bool _disposedValue;
		private readonly IRepository<TransactionEntity, int> _transactionRepository;
		private readonly BookKeepingDbContext _dbContext;
		private readonly IMapper _mapper;

		public ICollection<Transaction> Incomes { get; set; }
		public ICollection<Transaction> Expenses { get; set; }

		public IDictionary<int, double> IncomeAmounts { get; private set; }
		public IDictionary<int, double> CumuliativeIncomeAmounts { get; private set; }
		public IDictionary<int, double> ExpenseAmounts { get; private set; }
		public IDictionary<int, double> CumuliativeExpenseAmounts { get; private set; }
		public IDictionary<int, double> ResultAmounts { get; private set; }

		public TransactionAggregate(
			IRepository<TransactionEntity, int> transactionRepository,
			BookKeepingDbContext dbContext,
			IMapper mapper
		)
		{
			_transactionRepository = transactionRepository;
			_dbContext = dbContext;
			_mapper = mapper;
			Incomes = new List<Transaction>();
			Expenses = new List<Transaction>();
			IncomeAmounts = new Dictionary<int, double>();
			ExpenseAmounts = new Dictionary<int, double>();
			IncomeAmounts = new Dictionary<int, double>();
			CumuliativeIncomeAmounts = new Dictionary<int, double>();
			CumuliativeExpenseAmounts = new Dictionary<int, double>();
			ResultAmounts = new Dictionary<int, double>();
		}

		public async Task GetTransactionsAsync(int year)
		{
			var transactions = await _transactionRepository
								.Read(t => t.TransactionDate.Year.Equals(year))
								.Join(
									_dbContext.TransactionFlow,
									t => t.TransactionFlowId,
									f => f.Id,
									(t, f) => new TransactionEntity
									{
										Id = t.Id,
										Amount = t.Amount,
										Currency = t.Currency,
										TransactionDate = t.TransactionDate,
										TransactionFlow = f,
										TransactionFlowId = f.Id,
										TransactionType = t.TransactionType,
										TransactionTypeId = t.TransactionTypeId
									}
								)
								.Join(
									_dbContext.TransactionType,
									t => t.TransactionTypeId,
									tt => tt.Id,
									(t, tt) => new TransactionEntity
									{
										Id = t.Id,
										Amount = t.Amount,
										Currency = t.Currency,
										TransactionDate = t.TransactionDate,
										TransactionFlow = t.TransactionFlow,
										TransactionFlowId = t.TransactionFlowId,
										TransactionType = tt,
										TransactionTypeId = tt.Id
									}
								)
								.ToListAsync();

			foreach (var t in transactions)
			{
				var month = t.TransactionDate.Month;
				var transaction = _mapper.Map<Transaction>(t);
				if (t.TransactionFlow.Way.Equals(TransactionFlowConstants.Income))
				{
					Incomes.Add(transaction);
					IncomeAmounts[month] = IncomeAmounts.TryGetValue(month, out var income)
									 ? t.Amount + income
									 : t.Amount;
				}
				if (t.TransactionFlow.Way.Equals(TransactionFlowConstants.Expense))
				{
					Expenses.Add(transaction);
					ExpenseAmounts[month] = ExpenseAmounts.TryGetValue(month, out var expense)
									  ? t.Amount + expense
									  : t.Amount;
				}
			}

			for (var m = 1; m <= 12; m++)
			{
				if (!IncomeAmounts.TryGetValue(m, out var _))
					IncomeAmounts.Add(m, 0);
				if (!ExpenseAmounts.TryGetValue(m, out var _))
					ExpenseAmounts.Add(m, 0);
				if (IncomeAmounts.TryGetValue(m, out var income)
				 && ExpenseAmounts.TryGetValue(m, out var expense)
				)
				{
					CumuliativeIncomeAmounts.Add(
						m,
						m == 1
						 ? income
						 : income + (CumuliativeIncomeAmounts.ContainsKey(m - 1) ? CumuliativeIncomeAmounts[m - 1] : 0)
					);
					CumuliativeExpenseAmounts.Add(
						m,
						m == 1
						 ? expense
						 : expense + (CumuliativeExpenseAmounts.ContainsKey(m - 1) ? CumuliativeExpenseAmounts[m - 1] : 0)
					);
					ResultAmounts[m] = income - expense;
				}
				else if (IncomeAmounts.TryGetValue(m, out var income2))
				{
					CumuliativeIncomeAmounts[m] = m == 1
							 ? income2
							 : income2 + CumuliativeIncomeAmounts[m - 1];
					ResultAmounts[m] = income2;
				}
				else if (ExpenseAmounts.TryGetValue(m, out var expense2))
				{
					CumuliativeIncomeAmounts[m] = m == 1
							 ? 0
							 : (CumuliativeIncomeAmounts.ContainsKey(m - 1) ? CumuliativeIncomeAmounts[m - 1] : 0);
					CumuliativeExpenseAmounts[m] = m == 1
							 ? expense2
							 : expense2 + (CumuliativeExpenseAmounts.ContainsKey(m - 1) ? CumuliativeExpenseAmounts[m - 1] : 0);
					ResultAmounts[m] = 0 - expense2;
				}
			}
		}

		public async Task<ICollection<int>> GetYearsAsync()
		{
			var dates = await _transactionRepository
						.Read()
						.Select(t => t.TransactionDate)
						.ToListAsync();
			return dates
				.Select(d => d.Year)
				.Distinct()
				.ToList();
		}

		#region Disposable Pattern
		private void ClearTransactions()
		{
			IncomeAmounts.Clear();
			ExpenseAmounts.Clear();
			CumuliativeIncomeAmounts.Clear();
			CumuliativeExpenseAmounts.Clear();
			ResultAmounts.Clear();
		}

		public ValueTask DisposeAsync()
		{
			ClearTransactions();
			return _transactionRepository.DisposeAsync();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					ClearTransactions();
					_transactionRepository.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				_disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~TransactionAggregate()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		void IDisposable.Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
