using Fluxor;

using System;

using static BookKeeping.App.Web.Store.DisplayMessage;

namespace BookKeeping.App.Web.Store
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
				IsLoaded = true,
				EntityTag = action.EntityTag
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
				EntityTag = action.EntityTag,
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
