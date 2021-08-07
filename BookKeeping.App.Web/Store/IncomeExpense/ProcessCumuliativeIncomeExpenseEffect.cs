
using BookKeeping.App.Web.Store.IncomeExpense;

using Fluxor;

using System.Threading.Tasks;

namespace BookKeeping.App.Web.Store
{
	public class ProcessCumuliativeIncomeExpenseEffect
		: Effect<IncomeExpenseFetchedAction>
	{
		private readonly IncomeExpenseState _state;

		public ProcessCumuliativeIncomeExpenseEffect(
			IState<IncomeExpenseState> state
		)
			=> _state = state.Value;

		public override Task HandleAsync(
			IncomeExpenseFetchedAction action,
			IDispatcher dispatcher
		)
		{
			if (_state.Data is not null
			 && _state.Data.TryGetValue(action.Year, out var dto)
			)
			{

			}
			return Task.CompletedTask;
		}
	}
}
