using BookKeeping.API.DTOs;
using BookKeeping.App.Web.Store.EntityTag;
using BookKeeping.App.Web.Store.Years;
using BookKeeping.App.Web.ViewModels;

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
		private readonly IncomeExpenseViewModel _viewModel;

		private async Task<HttpResponseMessage?> FetchData(int year)
		{
			if (year == 0)
				return null;

			var uri = $"api/transactions/{year}";
			var eTag = _viewModel.GetETag(uri);

			if (!string.IsNullOrWhiteSpace(eTag))
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
			var state = _viewModel.IncomeExpenseState?.Value;
			var dto = _viewModel.GetIncomeExpense(year);
			var eTag = _viewModel.GetETag(uri);
			if (dto is not null
			 && state is not null
			)
			{
				if (DateTime.Now - state.FetchedAt <= state.CacheDuration)
				{
					dispatcher.Dispatch(
						new IncomeExpenseFetchedAction(
							state,
							year,
							eTag
						)
					);
					return;
				}
			}
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
			//response = response.EnsureSuccessStatusCode();
			var json = await response
							.Content
							.ReadAsStringAsync()
							.ConfigureAwait(true);
			try
			{
				dto = JsonConvert.DeserializeObject<IncomeExpenseDto>(json)!;

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
							year,
							response.Headers.ETag?.Tag
						)
					);
					if (!string.IsNullOrWhiteSpace(eTag))
						_viewModel.EntityTags.Add(new(uri, eTag));
					dispatcher.Dispatch(
							new UpdateEntityTagAction(
								_viewModel.EntityTags
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
			IncomeExpenseViewModel vm,
			IState<IncomeExpenseState> state,
			IState<EntityTagState> entityTagSate
		)
		{
			_http = http;
			_viewModel = vm;
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
			var state = _viewModel.IncomeExpenseState;
			var data = _viewModel.IncomeExpenseByYear;
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
			if (state is not null)
			{
				if (DateTime.Now - state.Value.FetchedAt > state.Value.CacheDuration)
				{
					await FetchAndProcess(action.Year)
						.ConfigureAwait(false);
					return;
				}
				dispatcher.Dispatch(
					new IncomeExpenseFetchedAction(state.Value, action.Year)
				);
			}
		}
	}
}
