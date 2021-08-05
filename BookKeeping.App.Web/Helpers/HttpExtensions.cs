using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BookKeeping.App.Web.Helpers
{
	public static class HttpExtensions
	{
		public static async Task<T> Get<T>(
			this HttpClient client,
			string uri,
			Action<T> onSuccess,
			Action<string> onFailed
		)
			where T : new()
		{
			var dto = default(T);
			if (string.IsNullOrWhiteSpace(uri))
				throw new ArgumentNullException(nameof(uri));
			try
			{
				if (HttpStates.ETags.TryGetValue(uri, out var eTag))
				{
					client.DefaultRequestHeaders.Add(CacheRequestHeadersConst.IfNoneMatch, eTag);
				}
				var response = await client.GetAsync($"{client.BaseAddress}{uri}");
				if (response.StatusCode.Equals(StatusCodes.Status200OK))
				{
					if (response.Headers.ETag is EntityTagHeaderValue eTagHeaderValue)
					{
						HttpStates.ETags.Add(uri, eTagHeaderValue.Tag);
					}
					var json = await response.Content.ReadAsStringAsync();
					dto = JsonConvert.DeserializeObject<T>(json);
					if (dto is not null)
						onSuccess(dto);
					else
						onFailed("Null value returned");
				}
			}
			catch (Exception ex)
			{
				onFailed(ex.Message);
			}
			return dto!;
		}
	}
}
