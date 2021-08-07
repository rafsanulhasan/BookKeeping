using Fluxor;

using System;

using static BookKeeping.App.Web.Store.DisplayMessage;

namespace BookKeeping.App.Web.Store.IncomeExpense
{
	public static class Reducers
	{

		[ReducerMethod]
		public static IncomeExpenseState UpdateIncomeExpenseStateReducer(
			IncomeExpenseState state,
			IncomeExpenseFetchedAction action
		)
			=> state with
			{
				IsLoading = false,
				IsLoaded = true,
				IsFailed = false,
				FetchedAt = DateTime.UtcNow,
				Data = action.State.Data,
				Message = new DisplayMessage("Fetched data successfully", MessageType.Information)
			};

		[ReducerMethod]
		public static IncomeExpenseState UpdateIncomeExpenseStateReducer(
			IncomeExpenseState state,
			IncomeExpenseFetchingAction action
		)
			=> state with
			{
				IsLoading = true,
				IsLoaded = false,
				Message = new DisplayMessage(
					$"Fetchting Income and Expenses from API endpint: {action.Uri}",
					MessageType.Information
				),
				Data = state.Data
			};

		[ReducerMethod]
		public static IncomeExpenseState UpdateIncomeExpenseStateReducer(
			IncomeExpenseState state,
			IncomeExpenseFetchingErrorAction action
		)
			=> state with
			{
				IsLoading = false,
				IsLoaded = false,
				IsFailed = true,
				Message = action.Message
			};
	}
}
