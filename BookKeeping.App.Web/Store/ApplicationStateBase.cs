
using System;

namespace BookKeeping.App.Web.Store
{

	public interface IApplicationState { }
	public abstract record ApplicationStateBase(
		bool IsProcessing = false,
		bool IsProcessed = false,
		bool IsFailed = false,
		DisplayMessage? DisplayMessage = null
	) : IApplicationState;

	public abstract record FetchedStateBase<T>(
		TimeSpan? CacheDuration,
		DateTime? FetchedAt,
		T? Data,
		bool IsLoading = false,
		bool IsLoaded = false,
		bool IsFailed = false,
		DisplayMessage? DisplayMessage = null
	)
		: ApplicationStateBase(IsLoading, IsLoaded, IsFailed, DisplayMessage)
		where T : class, new();
}
