using BookKeeping.API.DTOs;

using Microsoft.AspNetCore.Components;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BookKeeping.App.Web.Pages
{
	public partial class Reconciliation
		: ComponentBase
	{
		private int _selectedYear;
		private ICollection<int> _years;
		private IncomeExpenseDto _dto;
		private bool _invalidSelection;
		private bool _isLoading;

		[Inject]
		public HttpClient Http { get; set; }

		private async void OnChange(ChangeEventArgs args)
		{
			if (_selectedYear > 0)
			{
				_isLoading = true;
				_invalidSelection = false;
				_dto = await Http.GetFromJsonAsync<IncomeExpenseDto>($"/api/Transactions/{args.Value}");
				StateHasChanged();
			}
			else
			{
				_isLoading = false;
				_invalidSelection = true;
			}
		}

		protected override async Task OnInitializedAsync()
		{
			_years = await Http.GetFromJsonAsync<int[]>("/api/Transactions/years");
		}
	}
}
