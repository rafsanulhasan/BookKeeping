
using BookKeeping.API.DTOs;
using BookKeeping.App.Web.Store;

using Fluxor;
using Fluxor.Blazor.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Net.Http;

using static BookKeeping.App.Web.Store.DisplayMessage;

namespace BookKeeping.App.Web.Pages
{
	public partial class Reconciliation
		: FluxorComponent
	{
		private string _message = string.Empty;
		private string _error = string.Empty;
		private int _selectedYear;
		private ICollection<int>? _years = null;
		private IncomeExpenseDto? _dto = null;
		private bool _invalidSelection = true;
		private bool _isLoading = true;
		private bool _isLoaded = true;

		[Inject]
		public HttpClient? Http { get; set; }

		[Inject]
		public IState<IncomeExpenseState>? IncomeExpenseState { get; set; }

		[Inject]
		public IState<YearsState>? YearsState { get; set; }

		[Inject]
		public IDispatcher? Dispatcher { get; set; }

		[Inject]
		public ILogger<Reconciliation>? Logger { get; set; }

		private void IncomeExpenseStateChanged(object? sender, IncomeExpenseState e)
		{
			_isLoading = e.IsLoading;
			_isLoaded = e.IsLoaded;
			_invalidSelection = _selectedYear <= 0;
			Logger.LogInformation(_selectedYear.ToString());
			if (e.Data is not null
			 && e.Data.TryGetValue(_selectedYear, out var yearData)
			 && yearData is not null
			)
			{
				_dto = yearData;
			}

			if (e.Message is not null
			 && !string.IsNullOrWhiteSpace(e.Message.Message)
			)
			{
				switch (e.Message.Type)
				{
					case MessageType.Information:
						_message = e.Message.Message;
						break;
					case MessageType.Error:
						_error = e.Message.Message;
						break;
				}
			}
		}

		private void YearsStateChanged(object? sender, YearsState e)
		{
			_isLoading = e.IsLoading;

			if (e.Message is not null)
			{
				switch (e.Message.Type)
				{
					case MessageType.Information:
						_message = e.Message.Message;
						Logger?.LogInformation(_message);
						break;
					case MessageType.Error:
						_error = e.Message.Message;
						Logger?.LogError(_error);
						break;
				};
			}
			if (e.Data is not null)
				_years = e.Data;
			StateHasChanged();
		}

		private void GetYears() 
			=> Dispatcher?.Dispatch(new FetchYearsAction());

		private void OnChange(ChangeEventArgs args)
		{
			if (args.Value is not null
			 && int.TryParse(args.Value.ToString(), out var selectedYear)
			)
			{
				if (selectedYear > 0)
					Dispatcher?.Dispatch(new YearSelectedAction(selectedYear));
				else
					_error = "Please select a valid year";
			}
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			if (YearsState is not null)
				YearsState.StateChanged += YearsStateChanged;
			if (IncomeExpenseState is not null)
				IncomeExpenseState.StateChanged += IncomeExpenseStateChanged;
			GetYears();
			StateHasChanged();
		}

		protected override void Dispose(bool disposing)
		{
			if (YearsState is not null)
				YearsState.StateChanged -= YearsStateChanged;
			if (IncomeExpenseState is not null)
				IncomeExpenseState.StateChanged -= IncomeExpenseStateChanged;
			base.Dispose(disposing);
		}
	}
}
