
using BookKeeping.API.DTOs;
using BookKeeping.App.Web.Helpers;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookKeeping.App.Web.Pages
{
	public partial class Reconciliation
		: ComponentBase
	{
		private string _error = string.Empty;
		private int _selectedYear;
		private ICollection<int>? _years = null;
		private IncomeExpenseDto? _dto = null;
		private bool _invalidSelection = true;
		private bool _isLoading = true;

		[Inject]
		public HttpClient? Http { get; set; }

		[Inject]
		public ILogger<Reconciliation>? Logger { get; set; }

		private async Task GetYears()
		{

			try
			{
				var uri = "api/transactions/years";
				if (HttpStates.ETags.TryGetValue(uri, out string? eTag))
				{
					Http!.DefaultRequestHeaders.Add(CacheRequestHeadersConst.IfNoneMatch, eTag);
				}
				//var resource = await Http!.GetFromJsonAsync<ApiResource<YearsList>>(uri);
				//Logger.LogInformation(resource!.GetEtag());
				var response = await Http!.GetAsync($"{Http.BaseAddress}{uri}").ConfigureAwait(true);
				if (response!.StatusCode.Equals(HttpStatusCode.OK))
				{
					if (response.Headers.ETag is not null)
					{
						HttpStates.ETags.TryAdd(uri, response.Headers.ETag.Tag);
					}
					var resourceJson = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
					Logger.LogInformation(resourceJson);
					var resource = JsonConvert.DeserializeObject<YearsList>(resourceJson);
					_years = resource!.Years;
				}
			}
			catch (Exception ex)
			{
				_error = ex.Message;
				Logger.LogError(ex.Message);
			}
		}

		private async void OnChange(ChangeEventArgs args)
		{
			var val = args.Value;
			Logger.LogInformation($"Selected {val}");
			if (_selectedYear > 0)
			{
				var uri = $"api/transactions/{args.Value}";
				try
				{
					_isLoading = true;
					_invalidSelection = false;
					if (HttpStates.ETags.TryGetValue(uri, out var eTag))
					{
						Http!.DefaultRequestHeaders.Add(CacheRequestHeadersConst.IfNoneMatch, eTag);
					}
					var response = await Http!.GetAsync($"{Http.BaseAddress}{uri}").ConfigureAwait(false);

					Logger.LogInformation(JsonConvert.SerializeObject(response));
					if (response is not null)
					{
						if (response.Headers.ETag is not null)
						{
							HttpStates.ETags.TryAdd(uri, response.Headers.ETag.Tag);
						}
						_isLoading = false;
						Logger.LogInformation("response succeeded");
						Debug.WriteLine("rs");
						var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						Logger.LogInformation(json);
						var dto = JsonConvert.DeserializeObject<IncomeExpenseDto>(json);
						if (dto is not null
						 && !string.IsNullOrWhiteSpace(response.ReasonPhrase) 
						 && response.ReasonPhrase.Equals("OK")
						)
						{
							_isLoading = false;
							_invalidSelection = false;
							_dto = dto;
						}
						if (response.StatusCode.Equals(StatusCodes.Status304NotModified)
						 || (!string.IsNullOrWhiteSpace(response.ReasonPhrase) && response.ReasonPhrase.Equals("Not Modified"))
						)
						{
							_error = "Response got from cache";
						}
					}
				}
				catch (Exception ex)
				{
					_isLoading = false;
					_invalidSelection = false;
					_error = ex.Message;
				}
			}
			else
			{
				_isLoading = false;
				_invalidSelection = true;
				_error = "Please select valid option";
			}
			StateHasChanged();
		}

		protected override async Task OnInitializedAsync()
		{
			await GetYears().ConfigureAwait(true);
		}
	}
}
