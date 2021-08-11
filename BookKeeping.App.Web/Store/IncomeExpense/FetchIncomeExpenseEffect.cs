using BookKeeping.API.DTOs;

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
		private IDispatcher _dispatcher;
		private readonly IState<ApplicationState> _appState;
		private HttpResponseMessage? _response;
		private readonly Dictionary<string, string?> _entityTags;
		private IncomeExpenseState? _selectedState;

		private async Task FetchData(int year)
		{
			if (year <= 0)
			{
				_dispatcher.Dispatch(
					new IncomeExpenseFetchingErrorAction(
						new(
							"Select a valid year",
							MessageType.Error
						)
					)
				);
				return;
			}
			var uri = $"api/transactions/{year}";
			var states = _appState.Value.IncomeExpenseStatsByYear;
			if (states is not null
			 && states.TryGetValue(year, out var selectedState)
			 && selectedState is not null
			)
			{
				var message = "Data has not been modified since last fetched";
				var cacheDuration = selectedState.CacheDuration ?? TimeSpan.FromMinutes(1);
				var lastFetchedAt = selectedState.FetchedAt;
				var diff = DateTime.UtcNow - lastFetchedAt;
				if (diff > TimeSpan.Zero
				 && diff < cacheDuration
				)
				{
					message = $"{message} at {selectedState.FetchedAt} from server";
					_selectedState = selectedState with
					{
						IsLoading = false,
						IsLoaded = true,
						IsFailed = false,
						DisplayMessage = new(message, MessageType.Information)
					};
					return;
				}
			}
			if (_entityTags is not null)
			{
				if (_entityTags.TryGetValue(uri, out var eTag))
				{
					if (!string.IsNullOrWhiteSpace(eTag))
						_http.DefaultRequestHeaders.Add(HeaderNames.IfMatch, eTag);
				}
			}
			_response = await _http!
						.GetAsync($"{_http.BaseAddress}{uri}")
						.ConfigureAwait(true);

		}

		private async Task ProcessResponse(int year)
		{
			var appState = _appState.Value with
			{
				SelectedYear = year
			};
			if (_selectedState is not null
			 && _selectedState.Data is not null
			 && (_response is null
				    || _response.StatusCode == HttpStatusCode.NotModified
				    || (!string.IsNullOrWhiteSpace(_response.ReasonPhrase)
					   && _response.ReasonPhrase.Equals("Not Modified")
					  )
				    )
			)
			{
				appState.IncomeExpenseStatsByYear?.TryAdd(year, _selectedState);
				appState = appState with
				{
					SelectedIncomeExpense = _selectedState
				};
				var lastFetchedAt = _selectedState.FetchedAt;
				var cacheDuration = _selectedState.CacheDuration ?? TimeSpan.FromMinutes(1);
				var diff = DateTime.Now - lastFetchedAt;
				if (diff > TimeSpan.Zero
				 && diff <= cacheDuration
				)
				{
					_dispatcher.Dispatch(new IncomeExpenseFetchedAction(appState));
					return;
				}
			}

			if (_response is not null)
			{
				var eTagState = _appState.Value.EntityTags ?? new(new());
				var uri = $"api/transactions/{year}";

				if (!eTagState.EntityTags.TryGetValue(uri, out var eTag))
				{
					eTag = _response.Headers.ETag?.Tag;
				}

				//response = response.EnsureSuccessStatusCode();
				var json = await _response
								.Content
								.ReadAsStringAsync()
								.ConfigureAwait(true);
				try
				{
					var dto = JsonConvert.DeserializeObject<IncomeExpenseDto>(json)!;
					if (dto is not null)
					{
						_selectedState = new(
							false,
							true,
							false,
							new("Fetched Data Successfully", MessageType.Information),
							TimeSpan.FromMinutes(1),
							DateTime.Now,
							dto
						);
						appState.IncomeExpenseStatsByYear?.TryAdd(
							year,
							_selectedState
						);

						appState.EntityTags?.EntityTags.TryAdd(
							uri,
							eTag
						);

						appState = appState with
						{
							SelectedYear = year,
							SelectedIncomeExpense = _selectedState
						};

						_dispatcher.Dispatch(
							new IncomeExpenseFetchedAction(appState)
						);
					}
				}
				catch (Exception ex)
				{
					_dispatcher.Dispatch(
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

		}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public FetchIncomeExpenseEffect(
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
			HttpClient http,
			IState<ApplicationState> appState
		)
		{
			_http = http;
			_appState = appState;
			_entityTags = appState.Value.EntityTags?.EntityTags ?? new();
		}

		public override async Task HandleAsync(
			YearSelectedAction action,
			IDispatcher dispatcher
		)
		{
			_dispatcher = dispatcher;
			if (action.State.SelectedYear.HasValue)
			{
				if (action.State.SelectedYear > 0)
				{
					if (_appState.Value.IncomeExpenseStatsByYear is not null
					 && _appState.Value.IncomeExpenseStatsByYear.TryGetValue(action.State.SelectedYear.Value, out var selectedState)
					 && selectedState is not null
					)
					{
						var cacheDuration = selectedState.CacheDuration;
						var lastFetchedAt = selectedState.FetchedAt;
						var diff = DateTime.Now - lastFetchedAt;
						if (diff > TimeSpan.Zero && diff < cacheDuration)
						{
							var appState = _appState.Value with
							{
								IncomeExpenseStatsByYear = _appState.Value.IncomeExpenseStatsByYear,
								SelectedIncomeExpense = selectedState,
								SelectedYear = action.State.SelectedYear
							};
							dispatcher.Dispatch(
								new IncomeExpenseFetchedAction(appState)
							);
							return;
						}
					}
					else
					{
						await FetchData(action.State.SelectedYear.Value)
									.ConfigureAwait(true);
						await ProcessResponse(action.State.SelectedYear.Value)
								.ConfigureAwait(true);
						return;
					}
				}
				dispatcher.Dispatch(
					new IncomeExpenseFetchingErrorAction(
						new(
							$"Invalid year selected: {action.State.SelectedYear.Value}",
							MessageType.Error
						)
					)
				);
				return;
			}
			dispatcher.Dispatch(
				new IncomeExpenseFetchingErrorAction(
					new(
						$"Please select a valid year",
						MessageType.Error
					)
				)
			);
		}
	}
}
