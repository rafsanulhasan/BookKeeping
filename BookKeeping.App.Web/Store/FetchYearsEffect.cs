
using BookKeeping.API.DTOs;

using Fluxor;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Net.Http;
using System.Threading.Tasks;

namespace BookKeeping.App.Web.Store
{
	public class FetchYearsEffect
		: Effect<FetchYearsAction>
	{
		private readonly HttpClient _http;
		private readonly IState<YearsState> _state;

		public FetchYearsEffect(
			HttpClient http,
			IState<YearsState> state
		)
		{
			_http = http;
			_state = state;
		}

		public override async Task HandleAsync(
			FetchYearsAction action,
			IDispatcher dispatcher
		)
		{
			if (!string.IsNullOrWhiteSpace(_state.Value.EntityTag))
			{
				_http.DefaultRequestHeaders.Add(
					CacheRequestHeadersConst.IfNoneMatch,
					_state.Value.EntityTag
				);
			}
			var uri = "api/transactions/years";
			var response = await _http!.GetAsync(
				$"{_http.BaseAddress}{uri}"
			).ConfigureAwait(true);
			var resourceJson = await response
								.Content
								.ReadAsStringAsync()
								.ConfigureAwait(true);
			dispatcher.Dispatch(new YearsFetchedInJsonAction(resourceJson));
			var resource = JsonConvert.DeserializeObject<YearsList>(resourceJson);
			if (resource is not null)
			{
				dispatcher.Dispatch(
					new YearsFetchedAction(resource.Years, response.Headers.ETag?.Tag)
				);
			}
		}
	}
}
