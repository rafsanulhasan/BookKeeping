using System.Collections.Generic;

namespace BookKeeping.App.Web.Store.Years
{
	public record YearSelectedAction(int Year);
	public record FetchYearsAction(string? EntityTag = null);
	public record YearsFetchingAction();
	public record YearsFetchedInJsonAction(string Json);
	public record YearsFetchedAction(List<int> Years);
	public record YearsFetchingErrorAction(DisplayMessage Message);
}
