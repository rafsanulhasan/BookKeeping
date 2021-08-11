using System;
using System.Collections.Generic;

namespace BookKeeping.App.Web.Store
{
	public record YearsState(
		bool IsLoading,
		bool IsLoaded,
		bool IsFailed,
		TimeSpan? CacheDuration,
		DateTime? FetchedAt,
		List<int>? Data,
		DisplayMessage? DisplayMessage
	)
		: FetchedStateBase<List<int>>(
			CacheDuration,
			FetchedAt,
			Data,
			IsLoading,
			IsLoaded,
			IsFailed,
			DisplayMessage
		  );
}
