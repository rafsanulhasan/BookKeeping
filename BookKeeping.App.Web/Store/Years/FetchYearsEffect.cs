
using BookKeeping.API.DTOs;
using BookKeeping.App.Web.Store.EntityTag;

using Fluxor;

using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;

using System.Net.Http;
using System.Threading.Tasks;

namespace BookKeeping.App.Web.Store.Years
{
	public class FetchYearsEffect
		: Effect<FetchYearsAction>
	{
		private readonly HttpClient _http;
		private readonly IState<EntityTagState> _entityTagState;

		public FetchYearsEffect(
			HttpClient http,
			IState<EntityTagState> entityTagState
		)
		{
			_http = http;
			_entityTagState = entityTagState;
		}

		public override async Task HandleAsync(
			FetchYearsAction action,
			IDispatcher dispatcher
		)
		{
			if (!string.IsNullOrWhiteSpace(action.EntityTag))
			{
				_http.DefaultRequestHeaders.Add(
					HeaderNames.IfMatch,
					action.EntityTag
				);
			}
			var uri = "api/transactions/years";
			var response = await _http!.GetAsync(
				$"{_http.BaseAddress}{uri}"
			).ConfigureAwait(false);
			var resourceJson = await response
								.Content
								.ReadAsStringAsync()
								.ConfigureAwait(false);
			//dispatcher.Dispatch(new YearsFetchedInJsonAction(resourceJson));
			var resource = JsonConvert.DeserializeObject<YearsList>(resourceJson);
			if (resource is not null)
			{
				dispatcher.Dispatch(
					new YearsFetchedAction(resource.Years)
				);

				var tags = _entityTagState.Value.EntityTags;

				if (!tags.ContainsKey(uri) && response.Headers.ETag is not null)
				{
					tags.Add(uri, response.Headers.ETag.Tag);
					dispatcher.Dispatch(
						new UpdateEntityTagAction(tags)
					);
				}
			}
		}
	}
}
