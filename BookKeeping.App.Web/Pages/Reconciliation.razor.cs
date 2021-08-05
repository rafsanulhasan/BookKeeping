
using BookKeeping.API.DTOs;
using BookKeeping.App.Web.Helpers;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Symbiosis.Json.Specs.Ion;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BookKeeping.App.Web.Pages
{
	public partial class Reconciliation
		: ComponentBase
	{
		private string _error = string.Empty;
		private int _selectedYear;
		private ICollection<int> _years = new List<int>();
		private IncomeExpenseDto _dto = new();
		private bool _invalidSelection;
		private bool _isLoading;

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
						HttpStates.ETags.Add(uri, response.Headers.ETag.Tag);
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
			if (_selectedYear > 0)
			{
				_isLoading = true;
				_invalidSelection = false;
				var uri = $"/api/transactions/{args.Value}";
				_dto = (await Http!.GetFromJsonAsync<IncomeExpenseDto>(uri))!;
			}
			else
			{
				_isLoading = false;
				_invalidSelection = true;
			}
				StateHasChanged();
		}

		protected override async Task OnInitializedAsync()
		{
			await GetYears().ConfigureAwait(true);
		}
	}
}
