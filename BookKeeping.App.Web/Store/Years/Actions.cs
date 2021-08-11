namespace BookKeeping.App.Web.Store
{
	public record YearSelectedAction(ApplicationState State);
	public record FetchYearsAction(string? EntityTag = null);
	public record YearsFetchingAction();
	public record YearsFetchedAction(ApplicationState State);
	public record YearsFetchingErrorAction(DisplayMessage Message);
}
