using BookKeeping.Domain.Abstractions;
using BookKeeping.Domain.Entities;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookKeeping.Domain.Aggregates
{
	public interface ITransactionAggregate
		: IAggregate<Transaction>
	{
		IDictionary<int, double> IncomeAmounts { get; }
		IDictionary<int, double> ExpenseAmounts { get; }

		IDictionary<int, double> CumuliativeIncomeAmounts { get; }
		IDictionary<int, double> CumuliativeExpenseAmounts { get; }
		IDictionary<int, double> ResultAmounts { get; }
		Task GetTransactionsAsync(int year);
		Task<ICollection<int>> GetYearsAsync();
	}
}
