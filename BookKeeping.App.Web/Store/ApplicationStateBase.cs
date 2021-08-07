
using System;

namespace BookKeeping.App.Web.Store
{

	public interface IApplicationState { }
	public abstract record ApplicationStateBase(
		bool IsProcessing,
		bool IsProcessed,
		bool IsFailed,
		DisplayMessage? Message
	) : IApplicationState;

	public abstract record FetchedStateBase<T>(
		bool IsLoading,
		bool IsLoaded,
		bool IsFailed,
		DisplayMessage? Message,
		TimeSpan? CacheDuration,
		DateTime? FetchedAt,
		T? Data,
		string? EntityTag
	) 
		: ApplicationStateBase(IsLoading, IsLoaded, IsFailed, Message)
		where T: class, new();
}
