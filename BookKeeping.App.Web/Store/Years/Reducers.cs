using Fluxor;

using static BookKeeping.App.Web.Store.DisplayMessage;

namespace BookKeeping.App.Web.Store.Years
{
	public static class Reducers
	{
		[ReducerMethod]
		public static YearsState UpdateYearsState(
			YearsState state,
			YearsFetchedAction action
		)
			=> state with
			{
				Data = action.Years,
				IsLoading = false,
				IsLoaded = true
			};

		[ReducerMethod]
		public static YearsState FetchJsonYearsState(
			YearsState state,
			YearsFetchedInJsonAction action
		)
			=> state with
			{
				Message = new($"Fetched years with {action.Json}", MessageType.Information),
				IsLoading = false,
				IsLoaded = true
			};
	}
}
