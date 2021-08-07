using System;
using System.Collections.Generic;

namespace BookKeeping.App.Web.Store.Years
{
	public record YearsState(
		bool IsLoading,
		bool IsLoaded,
		bool IsFailed,
		DisplayMessage? Message,
		TimeSpan? CacheDuration,
		DateTime? FetchedAt,
		List<int>? Data
	)
		: FetchedStateBase<List<int>>(
			IsLoading,
			IsLoaded,
			IsFailed,
			Message,
			CacheDuration,
			FetchedAt,
			Data
		  );
}
