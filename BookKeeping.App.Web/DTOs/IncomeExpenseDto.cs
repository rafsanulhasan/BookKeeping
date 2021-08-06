
using Microsoft.AspNetCore.Mvc.Abstractions;

using Newtonsoft.Json;

using System.Collections.Generic;

namespace BookKeeping.API.DTOs
{
	public record IncomeExpenseDto
		: ETaggable
	{
		[JsonProperty("incomes")]
		public IDictionary<int, double> Incomes { get; set; } = new Dictionary<int, double>();

		[JsonProperty("cumuliativeIncomes")]
		public IDictionary<int, double> CumuliativeIncomes { get; set; } = new Dictionary<int, double>();

		[JsonProperty("expenses")]
		public IDictionary<int, double> Expenses { get; set; } = new Dictionary<int, double>();

		[JsonProperty("cumuliativeExpenses")]
		public IDictionary<int, double> CumuliativeExpenses { get; set; } = new Dictionary<int, double>();

		[JsonProperty("result")]
		public IDictionary<int, double> Result { get; set; } = new Dictionary<int, double>();
	}
}