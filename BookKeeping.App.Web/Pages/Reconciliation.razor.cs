
using BookKeeping.API.DTOs;
using BookKeeping.App.Web.Store.IncomeExpense;
using BookKeeping.App.Web.Store.Years;
using BookKeeping.App.Web.ViewModels;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using static BookKeeping.App.Web.Store.DisplayMessage;

namespace BookKeeping.App.Web.Pages
{
	public partial class Reconciliation
	{
		private string _message = string.Empty;
		private string _error = string.Empty;
		private int _selectedYear;
		private IncomeExpenseDto? _dto = null;
		private bool _invalidSelection = true;
		private bool _isLoading = true;
		private readonly CompositeDisposable _disposables = new();

		[Inject]
		public new IncomeExpenseViewModel? ViewModel { get; set; }

		[Inject]
		public ILogger<Reconciliation>? Logger { get; set; }

		private void IncomeExpenseStateChanged(object? _, IncomeExpenseState e)
		{
			_isLoading = e.IsLoading;
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
			StateHasChanged();
		}

		private void YearsStateChanged(object? _, YearsState e)
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
			StateHasChanged();
		}

		private void GetYears()
		{
			if (ViewModel is not null)
			{
				if (ViewModel.EntityTagState!.Value.EntityTags.TryGetValue("api/transactions/years", out var eTag))
					ViewModel.Dispatcher?.Dispatch(new FetchYearsAction(eTag));
				else
					ViewModel.Dispatcher?.Dispatch(new FetchYearsAction());
			}
		}

		private void OnChange(ChangeEventArgs args)
		{
			if (args.Value is not null
			 && int.TryParse(args.Value.ToString(), out var selectedYear)
			 && ViewModel is not null
			)
			{
				if (selectedYear > 0)
					ViewModel.Dispatcher?.Dispatch(new YearSelectedAction(selectedYear));
				else
					_error = "Please select a valid year";
			}
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			GetYears();
			StateHasChanged();

			if (ViewModel is not null)
			{
				if (ViewModel.YearsState is not null)
				{
					Observable.FromEventPattern<YearsState>(
						eh => ViewModel.YearsState.StateChanged += eh,
						eh => ViewModel.YearsState.StateChanged -= eh
					)
					.Subscribe(ep => YearsStateChanged(ep.Sender, ep.EventArgs))
					.DisposeWith(_disposables);
				}

				if (ViewModel.IncomeExpenseState is not null)
				{
					Observable.FromEventPattern<IncomeExpenseState>(
						eh => ViewModel.IncomeExpenseState.StateChanged += eh,
						eh => ViewModel.IncomeExpenseState.StateChanged -= eh
					)
					.Subscribe(ep => IncomeExpenseStateChanged(ep.Sender, ep.EventArgs))
					.DisposeWith(_disposables);
				}
			}
		}
	}
}
