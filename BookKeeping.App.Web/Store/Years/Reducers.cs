using Fluxor;

namespace BookKeeping.App.Web.Store
{
	public static partial class Reducers
	{
		[ReducerMethod]
		public static ApplicationState UpdateYearsState(
			ApplicationState state,
			YearsFetchedAction action
		)
			=> state with
			{
				YearsState = action.State.YearsState ?? state.YearsState,
				EntityTags = action.State.EntityTags ?? state.EntityTags,
				IncomeExpenseStatsByYear = state.IncomeExpenseStatsByYear,
				SelectedIncomeExpense = state.SelectedIncomeExpense,
				IsLoading = action.State.YearsState?.IsLoading ?? state.YearsState?.IsLoading ?? false,
				IsLoaded = action.State.YearsState?.IsLoaded ?? state.YearsState?.IsLoaded ?? true,
				IsFailed = action.State.YearsState?.IsFailed ?? state.YearsState?.IsFailed ?? false,
				IsProcessing = state.IsProcessing,
				IsProcessed = state.IsProcessed,
				DisplayMessage = action.State.YearsState?.DisplayMessage
			};

		[ReducerMethod]
		public static ApplicationState UpdateYearsState(
			ApplicationState state,
			YearSelectedAction action
		)
			=> state with
			{
				SelectedYear = action.State.SelectedYear,
				SelectedIncomeExpense = action.State.SelectedIncomeExpense,
				IsLoading = action.State.YearsState?.IsLoading ?? false,
				IsLoaded = action.State.YearsState?.IsLoaded ?? true,
				IsFailed = action.State.YearsState?.IsFailed ?? false,
				DisplayMessage = action.State.YearsState?.DisplayMessage
			};
	}
}
