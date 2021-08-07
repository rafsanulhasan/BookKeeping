using System.Collections.Generic;

namespace BookKeeping.App.Web.Store
{
	public record FetchYearsAction();
	public record YearsFetchingAction();
	public record YearsFetchedInJsonAction(string Json);
	public record YearsFetchedAction(List<int> Years, string? EntityTag);
	public record YearsFetchingErrorAction(DisplayMessage Message);
	public record YearSelectedAction(int Year);
	public record IncomeExpenseFetchedAction(IncomeExpenseState State, int Year, string? EntityTag = null);
	public record IncomeExpenseFetchingAction(string Uri);
	public record IncomeExpenseFetchingErrorAction(DisplayMessage Message);
	public record IncomeExpenseProcessedAction(IncomeExpenseState State);
	public record IncomeExpenseProcessingAction(int Year, int Month);
	public record IncomeExpenseProcesingErrorAction(DisplayMessage Message);
}
