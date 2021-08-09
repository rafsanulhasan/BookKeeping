using BookKeeping.App.Web.Store.EntityTag;
using BookKeeping.App.Web.Store.Years;

using Fluxor;

using ReactiveUI;

namespace BookKeeping.App.Web.ViewModels
{
	public class IncomeExpenseViewModel
		: ReactiveObject
	{
		public IDispatcher Dispatcher { get; }
		public IState<EntityTagState> EntityTagState { get; }

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
	}
}
