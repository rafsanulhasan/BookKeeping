using BookKeeping.API.DTOs;

using System;
using System.Collections.Generic;

namespace BookKeeping.App.Web.Store.IncomeExpense
{
	public record IncomeExpenseState(
		bool IsLoading,
		bool IsLoaded,
		bool IsFailed,
		DisplayMessage? Message,
		TimeSpan? CacheDuration,
		DateTime? FetchedAt,
		Dictionary<int, IncomeExpenseDto>? Data
	)
		: FetchedStateBase<Dictionary<int, IncomeExpenseDto>>(
			IsLoading,
			IsLoaded,
			IsFailed,
			Message,
			CacheDuration,
			FetchedAt,
			Data
		);
}
