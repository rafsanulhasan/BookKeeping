
using Fluxor;

using System.Collections.Generic;

using static BookKeeping.App.Web.Store.DisplayMessage;

namespace BookKeeping.App.Web.Store
{
	public record ApplicationState(
		Dictionary<int, IncomeExpenseState?>? IncomeExpenseStatsByYear = null,
		IncomeExpenseState? SelectedIncomeExpense = null,
		int? SelectedYear = null,
		YearsState? YearsState = null,
		EntityTagState? EntityTags = null,
		bool IsProcessing = false,
		bool IsProcessed = false,
		bool IsLoading = true,
		bool IsLoaded = false,
		bool IsFailed = false,
		DisplayMessage? DisplayMessage = null
	) : ApplicationStateBase(IsProcessing, IsProcessed, IsFailed, DisplayMessage);

	public class ApplicationStateFeature
		: Feature<ApplicationState>
	{
		public override string GetName()
			=> "BookKeeping App";

		protected override ApplicationState GetInitialState()
			=> new(
				IsLoading: true, 
				IsLoaded: false, 
				IsFailed: false,
				IncomeExpenseStatsByYear: new(),
				EntityTags: new(new()),
				DisplayMessage: new("Loading Years...", MessageType.Information)
			);
	}
}