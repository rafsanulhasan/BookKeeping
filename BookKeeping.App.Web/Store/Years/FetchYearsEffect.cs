
using BookKeeping.API.DTOs;

using Fluxor;

using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using static BookKeeping.App.Web.Store.DisplayMessage;

namespace BookKeeping.App.Web.Store
{
	public class FetchYearsEffect
		: Effect<FetchYearsAction>
	{
		private readonly HttpClient _http;
		private readonly IState<ApplicationState> _appState;

		public FetchYearsEffect(
			HttpClient http,
			IState<ApplicationState> appState
		)
		{
			_http = http;
			_appState = appState;
		}

		public override async Task HandleAsync(
			FetchYearsAction action,
			IDispatcher dispatcher
		)
		{
			dispatcher.Dispatch(new YearsFetchingAction());
			var uri = "api/transactions/years";

			if (_appState.Value.YearsState is not null)
			{
				var cacheDuration = _appState.Value.YearsState.CacheDuration
							  ?? TimeSpan.FromMinutes(1);
				var lastFetchedAt = _appState.Value.YearsState.FetchedAt
							  ?? DateTime.UtcNow;
				var diff = DateTime.UtcNow - lastFetchedAt;
				if (diff > TimeSpan.Zero
				 && diff < cacheDuration
				)
				{
					if (_appState.Value.EntityTags!.EntityTags.ContainsKey(uri))
					{
						dispatcher.Dispatch(new YearsFetchedAction(_appState.Value));
						return;
					}
				}
			}

			string? eTag = null;
			if (!_appState.Value.EntityTags?.EntityTags.TryGetValue(uri, out eTag) ?? true)
			{
				if (_appState.Value.EntityTags?.EntityTags.ContainsValue(action.EntityTag) ?? false)
					eTag = action.EntityTag;
			}

			if (!string.IsNullOrWhiteSpace(eTag))
				_http.DefaultRequestHeaders.Add(
					HeaderNames.IfMatch,
					eTag
				);

			var response = await _http!
							.GetAsync($"{_http.BaseAddress}{uri}")
							.ConfigureAwait(true);

			if (response is not null)
			{
				if (_appState.Value.YearsState is not null)
				{
					var cacheDuration = _appState.Value.YearsState.CacheDuration
								  ?? TimeSpan.FromMinutes(1);
					var lastFetchedAt = _appState.Value.YearsState.FetchedAt
								  ?? DateTime.UtcNow;
					var diff = DateTime.UtcNow - lastFetchedAt;
					if (diff > TimeSpan.Zero
					 && diff < cacheDuration
					)
					{
						if (response.StatusCode == HttpStatusCode.NotModified
						 || (!string.IsNullOrWhiteSpace(response.ReasonPhrase) && response.ReasonPhrase.Equals("Not Modified"))
						)
						{
							if (_appState.Value.EntityTags!.EntityTags.ContainsKey(uri))
							{
								dispatcher.Dispatch(new YearsFetchedAction(_appState.Value));
							}
						}
					}
					return;
				}
				var responseCode = response.StatusCode;
				var responsePhase = response.ReasonPhrase;
				if (responseCode == HttpStatusCode.NotModified
				 || (!string.IsNullOrWhiteSpace(responsePhase) && responsePhase.Equals("Not Modified"))
				)
				{
					if (_appState.Value.YearsState is YearsState state
					 && state is not null
					 && state.Data is not null
					)
					{
						var cacheDuration = state.CacheDuration ?? TimeSpan.FromMinutes(1);
						var lastFetchedAt = state.FetchedAt;
						var diff = DateTime.Now - lastFetchedAt;
						if (diff > TimeSpan.Zero
						 && diff < cacheDuration
						)
						{

							return;
						}
					}
				}
				var resourceJson = await response
									.Content
									.ReadAsStringAsync()
									.ConfigureAwait(true);
				//dispatcher.Dispatch(new YearsFetchedInJsonAction(resourceJson));
				var resource = JsonConvert.DeserializeObject<YearsList>(resourceJson)!;
				var appState = _appState.Value with
				{
					YearsState = new(
						 false,
						 true,
						 false,
						 TimeSpan.FromMinutes(1),
						 DateTime.UtcNow,
						 resource?.Years,
						 new("Loading completed", MessageType.Information)
					 )
				};
				dispatcher.Dispatch(
					new YearsFetchedAction(appState)
				);
				eTag = response.Headers.ETag?.Tag;
				if (appState.EntityTags is null)
					appState = appState with
					{
						EntityTags = new EntityTagState(new() { { uri, eTag } })
					};
				else
					appState.EntityTags.EntityTags.TryAdd(uri, eTag);

				dispatcher.Dispatch(
					new UpdateEntityTagAction(appState)
				);
			}
		}
	}
}

