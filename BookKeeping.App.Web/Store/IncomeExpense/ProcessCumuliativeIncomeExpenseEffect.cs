
using Fluxor;

using System.Threading.Tasks;

namespace BookKeeping.App.Web.Store
{
	public class ProcessCumuliativeIncomeExpenseEffect
		: Effect<IncomeExpenseFetchedAction>
	{
		private readonly ApplicationState _state;

		public ProcessCumuliativeIncomeExpenseEffect(
			IState<ApplicationState> state
		)
			=> _state = state.Value;

		public override Task HandleAsync(
			IncomeExpenseFetchedAction action,
			IDispatcher dispatcher
		)
		{
			if (action.State.SelectedYear.HasValue)
			{
				if (_state.IncomeExpenseStatsByYear is not null
			      && _state.IncomeExpenseStatsByYear.TryGetValue(action.State.SelectedYear.Value, out var dto)
				)
				{

				}
			}
			return Task.CompletedTask;
		}
	}
}
