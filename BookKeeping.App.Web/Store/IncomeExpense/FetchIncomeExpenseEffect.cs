using BookKeeping.API.DTOs;
using BookKeeping.App.Web.Store.EntityTag;
using BookKeeping.App.Web.Store.Years;

using Fluxor;

using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
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
		private readonly IState<IncomeExpenseState> _state;
		private readonly IState<EntityTagState> _entityTagState;

		private async Task<HttpResponseMessage?> FetchData(int year)
		{
			if (year == 0)
				return null;

			var uri = $"api/transactions/{year}";

			if (_entityTagState.Value.EntityTags.TryGetValue(uri, out var eTag)
			 && !string.IsNullOrWhiteSpace(eTag)
			)
				_http.DefaultRequestHeaders.Add(HeaderNames.IfMatch, eTag);

			return await _http!
					.GetAsync($"{_http.BaseAddress}{uri}")
					.ConfigureAwait(false);
		}

		private async Task ProcessResponse(
			int year,
			IDispatcher dispatcher,
			HttpResponseMessage? response = null
		)
		{
			var uri = $"api/transactions/{year}";
			var state = _state.Value;
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
			if (response.StatusCode.Equals(HttpStatusCode.NotModified)
			 || (!string.IsNullOrWhiteSpace(response.ReasonPhrase)
				&& response.ReasonPhrase.Equals("Not Modified", StringComparison.OrdinalIgnoreCase)
			    )
			)
			{
				dispatcher.Dispatch(
					new IncomeExpenseFetchingErrorAction(
						new(
							$"Data has not been modified since last fetched at  {_state.Value.FetchedAt} from server",
							MessageType.Information
						)
					)
				);
				return;
			}
			//response = response.EnsureSuccessStatusCode();
			var json = await response
							.Content
							.ReadAsStringAsync()
							.ConfigureAwait(true);
			IncomeExpenseDto? dto = null;
			try
			{
				dto = JsonConvert.DeserializeObject<IncomeExpenseDto>(json)!;
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
			if (dto is null)
				return;

			if (state.Data is null)
			{
				state = state with
				{
					Data = new Dictionary<int, IncomeExpenseDto>() { { year, dto } }
				};
			}
			else if (!state.Data.TryGetValue(year, out var _))
				state.Data.Add(year, dto);

			dispatcher.Dispatch(
				new IncomeExpenseFetchedAction(
					state,
					year,
					response.Headers.ETag?.Tag
				)
			);
			var eTag = response.Headers.ETag?.Tag;
			var entityTags = _entityTagState.Value.EntityTags;
			if (!entityTags.TryGetValue(uri, out var entityTag))
				entityTags.Add(uri, eTag);
			else
				entityTags[uri] = eTag;
			dispatcher.Dispatch(
					new UpdateEntityTagAction(
						entityTags
					)
				);
		}

		public FetchIncomeExpenseEffect(
			HttpClient http,
			IState<IncomeExpenseState> state,
			IState<EntityTagState> entityTagSate
		)
		{
			_http = http;
			_state = state;
			_entityTagState = entityTagSate;
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
			var data = _state.Value.Data;
			if (data is null)
			{
				await FetchAndProcess(action.Year)
					.ConfigureAwait(false);
				return;
			}

			if (!data.ContainsKey(action.Year))
			{
				await FetchAndProcess(action.Year)
						.ConfigureAwait(false);
				return;
			}

			if (DateTime.Now - _state.Value.FetchedAt > _state.Value.CacheDuration)
			{
				await FetchAndProcess(action.Year)
					.ConfigureAwait(false);
				return;
			}
			dispatcher.Dispatch(
				new IncomeExpenseFetchedAction(_state.Value, action.Year)
			);
		}
	}
}
