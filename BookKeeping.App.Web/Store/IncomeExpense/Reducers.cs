using Fluxor;

namespace BookKeeping.App.Web.Store
{
	public static partial class Reducers
	{

		[ReducerMethod]
		public static ApplicationState UpdateIncomeExpensesStateReducer(
			ApplicationState state,
			IncomeExpenseFetchedAction action
		)
			=> state with
			{
				IncomeExpenseStatsByYear = action.State.IncomeExpenseStatsByYear,
				SelectedYear = action.State.SelectedYear,
				SelectedIncomeExpense = action.State.SelectedIncomeExpense,
				IsLoading = action.State.SelectedIncomeExpense?.IsLoading ?? action.State.IsLoading,
				IsLoaded = action.State.SelectedIncomeExpense?.IsLoaded ?? action.State.IsLoaded,
				IsFailed = action.State.SelectedIncomeExpense?.IsFailed ?? action.State.IsFailed,
				DisplayMessage = action.State.SelectedIncomeExpense?.DisplayMessage ?? action.State.DisplayMessage
			};

		[ReducerMethod]
		public static ApplicationState UpdateIncomeExpenseStateReducer(
			ApplicationState state,
			IncomeExpenseFetchingAction action
		)
			=> state with
			{
				IncomeExpenseStatsByYear = action.State.IncomeExpenseStatsByYear
									?? new()
									{
										{
											action.State.SelectedYear ?? 0,
											action.State.SelectedIncomeExpense
										}
									},
				SelectedYear = action.State.SelectedYear,
				SelectedIncomeExpense = action.State.SelectedIncomeExpense ?? state.SelectedIncomeExpense,
				IsLoading = true,
				IsLoaded = false,
				IsFailed = false,
				DisplayMessage = action.State.SelectedIncomeExpense?.DisplayMessage
			};

		[ReducerMethod]
		public static ApplicationState UpdateIncomeExpenseErrorStateReducer(
			ApplicationState state,
			IncomeExpenseFetchingErrorAction action
		)
			=> state with
			{
				IsFailed = true,
				DisplayMessage = action.Message
			};
	}
}
