using BookKeeping.API.DTOs;

using System;

namespace BookKeeping.App.Web.Store
{
	public record IncomeExpenseState(
		bool IsLoading,
		bool IsLoaded,
		bool IsFailed,
		DisplayMessage? DisplayMessage,
		TimeSpan? CacheDuration,
		DateTime? FetchedAt,
		IncomeExpenseDto? Data = null
	)
		: FetchedStateBase<IncomeExpenseDto>(
			CacheDuration,
			FetchedAt,
			Data,
			IsLoading,
			IsLoaded,
			IsFailed,
			DisplayMessage
		);
}
