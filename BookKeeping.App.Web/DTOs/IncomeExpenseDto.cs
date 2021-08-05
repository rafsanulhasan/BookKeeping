
using Microsoft.AspNetCore.Mvc.Abstractions;

using System.Collections.Generic;

namespace BookKeeping.API.DTOs
{
	public record IncomeExpenseDto
		: ETaggable
	{
		public IDictionary<int, double> Incomes { get; set; } = new Dictionary<int, double>();
		public IDictionary<int, double> CumuliativeIncomes { get; set; } = new Dictionary<int, double>();

		public IDictionary<int, double> Expenses { get; set; } = new Dictionary<int, double>();
		public IDictionary<int, double> CumuliativeExpenses { get; set; } = new Dictionary<int, double>();
		public IDictionary<int, double> Result { get; set; } = new Dictionary<int, double>();
	}
}
