using BookKeeping.API.DTOs;

using System;
using System.Collections.Generic;

namespace BookKeeping.App.Web.Store
{
	public record SelectedYearState(int Year) 
		: IApplicationState;

	public record IncomeExpenseState(
		bool IsLoading,
		bool IsLoaded,
		bool IsFailed,
		DisplayMessage? Message,
		TimeSpan? CacheDuration,
		DateTime? FetchedAt,
		Dictionary<int, IncomeExpenseDto>? Data,
		string? EntityTag
	)
		: FetchedStateBase<Dictionary<int, IncomeExpenseDto>>(
			IsLoading,
			IsLoaded,
			IsFailed,
			Message,
			CacheDuration,
			FetchedAt,
			Data,
			EntityTag
		);

	public record YearsState(
		bool IsLoading,
		bool IsLoaded,
		bool IsFailed,
		DisplayMessage? Message,
		TimeSpan? CacheDuration,
		DateTime? FetchedAt,
		List<int>? Data,
		string? EntityTag
	)
		: FetchedStateBase<List<int>>(IsLoading, IsLoaded, IsFailed, Message, CacheDuration, FetchedAt, Data, EntityTag);
}
