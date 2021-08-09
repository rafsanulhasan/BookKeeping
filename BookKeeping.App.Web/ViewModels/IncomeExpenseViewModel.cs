using BookKeeping.App.Web.Store.EntityTag;
using BookKeeping.App.Web.Store.Years;

using Fluxor;

using Microsoft.AspNetCore.Components;

using ReactiveUI;

namespace BookKeeping.App.Web.ViewModels
{
	public class IncomeExpenseViewModel
		: ReactiveObject
	{
		public IDispatcher Dispatcher { get; }
		public IState<EntityTagState> EntityTagState { get; }

		private string _errorMessage;
		public string ErrorMessage
		{
			get => _errorMessage;
			set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
		}

		public IncomeExpenseViewModel(
			IDispatcher dispatcher,
			IState<EntityTagState> entityTagState
		)
		{
			Dispatcher = dispatcher;
			EntityTagState = entityTagState;
		}


		public void GetYears()
		{
			if (EntityTagState is not null
			 && EntityTagState.Value.EntityTags.TryGetValue("api/transactions/years", out var eTag)
			)
				Dispatcher?.Dispatch(new FetchYearsAction(eTag));
			else
				Dispatcher?.Dispatch(new FetchYearsAction());
		}

		public void OnChange(ChangeEventArgs args)
		{
			if (args.Value is not null
			 && int.TryParse(args.Value.ToString(), out var selectedYear)
			)
			{
				if (selectedYear > 0)
					Dispatcher?.Dispatch(new YearSelectedAction(selectedYear));
				else
					ErrorMessage = "Please select a valid year";
			}
		}
	}
}
