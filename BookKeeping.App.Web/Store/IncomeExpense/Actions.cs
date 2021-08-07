namespace BookKeeping.App.Web.Store.IncomeExpense
{
	public record IncomeExpenseFetchedAction(IncomeExpenseState State, int Year, string? EntityTag = null);
	public record IncomeExpenseFetchingAction(string Uri);
	public record IncomeExpenseFetchingErrorAction(DisplayMessage Message);
	public record IncomeExpenseProcessedAction(IncomeExpenseState State);
	public record IncomeExpenseProcessingAction(int Year, int Month);
	public record IncomeExpenseProcesingErrorAction(DisplayMessage Message);
}
