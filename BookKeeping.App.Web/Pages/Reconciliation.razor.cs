
using BookKeeping.API.DTOs;
using BookKeeping.App.Web.Store.EntityTag;
using BookKeeping.App.Web.Store.IncomeExpense;
using BookKeeping.App.Web.Store.Years;

using Fluxor;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

using System;
using System.Net.Http;
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
			if (EntityTagState is not null
			 && EntityTagState.Value.EntityTags.TryGetValue("api/transactions/years", out var eTag)
			)
				Dispatcher?.Dispatch(new FetchYearsAction(eTag));
			else
				Dispatcher?.Dispatch(new FetchYearsAction());
		}

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
			ViewModel.GetYears();
			StateHasChanged();

			if (YearsState is not null)
			{
				Observable.FromEventPattern<YearsState>(
					eh => YearsState.Value.StateChanged += eh,
					eh => YearsState.Value.StateChanged -= eh
				)
				.Subscribe(ep => YearsStateChanged(ep.Sender, ep.EventArgs))
				.DisposeWith(_disposables);
			}

			if (IncomeExpenseState is not null)
			{
				Observable.FromEventPattern<IncomeExpenseState>(
					eh => IncomeExpenseState.Value.StateChanged += eh,
					eh => IncomeExpenseState.Value.StateChanged -= eh
				)
				.Subscribe(ep => IncomeExpenseStateChanged(ep.Sender, ep.EventArgs))
				.DisposeWith(_disposables);
			}
		}
	}
}
