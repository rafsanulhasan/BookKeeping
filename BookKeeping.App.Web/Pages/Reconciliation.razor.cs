
using BookKeeping.API.DTOs;
using BookKeeping.App.Web.Store.EntityTag;
using BookKeeping.App.Web.Store.IncomeExpense;
using BookKeeping.App.Web.Store.Years;
using BookKeeping.App.Web.ViewModels;

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

		[Inject]
		public new IncomeExpenseViewModel? ViewModel { get; set; }

		[Inject]
		public HttpClient? Http { get; set; }

		[Inject]
		public IState<IncomeExpenseState>? IncomeExpenseState { get; set; }

		[Inject]
		public IState<YearsState>? YearsState { get; set; }

		[Inject]
		public IState<EntityTagState>? EntityTagState { get; set; }

		[Inject]
		public IDispatcher? Dispatcher { get; set; }

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
		}

		private void GetYears()
		{
			if (EntityTagState!.Value.EntityTags.TryGetValue("api/transactions/years", out var eTag))
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

			GetYears();

			Observable.FromEventPattern<IncomeExpenseState>(
				e => ViewModel!.IncomeExpenseState!.StateChanged += e,
				e => ViewModel!.IncomeExpenseState!.StateChanged -= e
			)
			.Subscribe(e => IncomeExpenseStateChanged(e.Sender, e.EventArgs))
			.DisposeWith(_disposables);
		}
	}
}
