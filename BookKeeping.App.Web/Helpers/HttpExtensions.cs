using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookKeeping.App.Web.Helpers
{
	public static class HttpExtensions
	{
		public static async Task<T> GetAsync<T>(
			this HttpClient client,
			string uri,
			Action<T> onSuccess,
			Action<string> onFailed
		)
			where T : new()
		{
			T? dto = new();
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
					if (response.Headers.ETag is not null)
					{
						HttpStates.ETags.TryAdd(uri, response.Headers.ETag.Tag);
					}
					var json = await response.Content.ReadAsStringAsync();
					onFailed(json);
					dto = JsonConvert.DeserializeObject<T>(json);
					if (dto is not null)
					{
						onSuccess(dto);
						return dto;
					}
					else
					{
						onFailed("null returned");
						throw new Exception();
					}
				}
			}
			catch (Exception ex)
			{
				onFailed(ex.Message);
			}
			if (dto is null)
			{
				onFailed("Null returned");
			}
			return dto!;
		}
	}
}
