using BookKeeping.API.DTOs;
using BookKeeping.App.Web.Store.EntityTag;
using BookKeeping.App.Web.Store.Years;

using Fluxor;

using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using static BookKeeping.App.Web.Store.DisplayMessage;

namespace BookKeeping.App.Web.Store.IncomeExpense
{
	public class FetchIncomeExpenseEffect
		: Effect<YearSelectedAction>
	{
		private readonly HttpClient _http;
		private readonly IState<EntityTagState> _entityTagState;
		private readonly IState<IncomeExpenseState> _incomeExpenseState;

		private async Task<HttpResponseMessage?> FetchData(int year)
		{
			if (year == 0)
				return null;

			var uri = $"api/transactions/{year}";
			if (_entityTagState.Value.EntityTags.TryGetValue(uri, out var eTag))
			{
				if (!string.IsNullOrWhiteSpace(eTag))
					_http.DefaultRequestHeaders.Add(HeaderNames.IfMatch, eTag);
			}
			return await _http!
					.GetAsync($"{_http.BaseAddress}{uri}")
					.ConfigureAwait(true);
		}

		private async Task ProcessResponse(
			int year,
			IDispatcher dispatcher,
			HttpResponseMessage? response = null
		)
		{
			if (response is null)
			{
				dispatcher.Dispatch(
					new IncomeExpenseFetchingErrorAction(
						new(
							"Unknown error occured while fetching incomes and expenses",
							MessageType.Error
						)
					)
				);
				return;
			}

			var state = _incomeExpenseState.Value;

			if (response.StatusCode.Equals(HttpStatusCode.NotModified)
			 || (!string.IsNullOrWhiteSpace(response.ReasonPhrase)
				&& response.ReasonPhrase.Equals("Not Modified", StringComparison.OrdinalIgnoreCase)
			    )
			)
			{
				var message = "Data has not been modified since last fetched";
				if (state is not null)
					message = $"{message} at {state.FetchedAt} from server";

				dispatcher.Dispatch(
						new IncomeExpenseFetchingErrorAction(
							new(
								message,
								MessageType.Information
							)
						)
					);
				return;
			}

			var eTagState = _entityTagState.Value;
			var uri = $"api/transactions/{year}";

			if (!eTagState.EntityTags.TryGetValue(uri, out var eTag))
			{
				eTag = response.Headers.ETag?.Tag;
			}

			if (state.Data is not null)
			{
				if (DateTime.Now - state.FetchedAt <= state.CacheDuration)
				{
					dispatcher.Dispatch(
						new IncomeExpenseFetchedAction(
							state,
							year
						)
					);
					return;
				}
			}
			//response = response.EnsureSuccessStatusCode();
			var json = await response
							.Content
							.ReadAsStringAsync()
							.ConfigureAwait(true);
			try
			{
				var dto = JsonConvert.DeserializeObject<IncomeExpenseDto>(json)!;

				if (dto is not null)
				{
					state = new(
						false,
						true,
						false,
						new(),
						TimeSpan.FromMinutes(1),
						DateTime.Now,
						new() { { year, dto } }
					);

					dispatcher.Dispatch(
						new IncomeExpenseFetchedAction(
							state,
							year
						)
					);
					_entityTagState.Value.EntityTags.Add(new(uri, eTag));
					dispatcher.Dispatch(
							new UpdateEntityTagAction(
								_entityTagState
									.Value
									.EntityTags
							)
						);
				}
			}
			catch (Exception ex)
			{
				dispatcher.Dispatch(
					new IncomeExpenseProcesingErrorAction(
						new()
						{
							Message = ex.Message,
							Type = MessageType.Error
						}
					)
				);
			}

		}

		public FetchIncomeExpenseEffect(
			HttpClient http,
			IState<IncomeExpenseState> state,
			IState<EntityTagState> entityTagSate
		)
		{
			_http = http;
			_entityTagState = entityTagSate;
			_incomeExpenseState = state;
		}

		public override async Task HandleAsync(
			YearSelectedAction action,
			IDispatcher dispatcher
		)
		{
			async Task FetchAndProcess(int year)
			{
				var response = await FetchData(action.Year)
							.ConfigureAwait(true);
				await ProcessResponse(action.Year, dispatcher, response)
						.ConfigureAwait(true);
			}
			var state = _incomeExpenseState.Value;
			var data = state.Data;
			if (data is null)
			{
				await FetchAndProcess(action.Year)
					.ConfigureAwait(true);
				return;
			}

			if (!data.ContainsKey(action.Year))
			{
				await FetchAndProcess(action.Year)
						.ConfigureAwait(true);
				return;
			}
			if (state is not null)
			{
				if (DateTime.Now - state.FetchedAt > state.CacheDuration)
				{
					await FetchAndProcess(action.Year)
						.ConfigureAwait(true);
					return;
				}
				dispatcher.Dispatch(
					new IncomeExpenseFetchedAction(state, action.Year)
				);
			}
		}
	}
}
