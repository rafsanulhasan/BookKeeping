namespace BookKeeping.App.Web.Store
{
	public record IncomeExpensesUpdatedAction(ApplicationState States);
	public record IncomeExpenseFetchedAction(ApplicationState State);
	public record IncomeExpenseFetchingAction(ApplicationState State);
	public record IncomeExpenseFetchingErrorAction(DisplayMessage Message);
	public record IncomeExpenseProcessedAction(IncomeExpenseState State);
	public record IncomeExpenseProcessingAction(int Year, int Month);
	public record IncomeExpenseProcesingErrorAction(DisplayMessage Message);
}
