using BookKeeping.App.Web.Pages;
using BookKeeping.App.Web.Store.EntityTag;
using BookKeeping.App.Web.Store.IncomeExpense;
using BookKeeping.App.Web.Store.Years;

using Fluxor;

using Microsoft.Extensions.Logging;

using ReactiveUI;

namespace BookKeeping.App.Web.ViewModels
{
	public class IncomeExpenseViewModel
		: ReactiveObject
	{
		public IState<IncomeExpenseState>? IncomeExpenseState { get; }

		public IState<YearsState>? YearsState { get; }

		public IState<EntityTagState>? EntityTagState { get; }

		public IDispatcher? Dispatcher { get; }

		public ILogger<Reconciliation>? Logger { get; }

		public IncomeExpenseViewModel(
			IState<IncomeExpenseState> incomes,
			IState<YearsState> years,
			IState<EntityTagState> entityTags,
			IDispatcher dispatcher,
			ILogger<Reconciliation> logger
		)
		{
			IncomeExpenseState = incomes;
			YearsState = years;
			EntityTagState = entityTags;
			Dispatcher = dispatcher;
			Logger = logger;
		}
	}
}
