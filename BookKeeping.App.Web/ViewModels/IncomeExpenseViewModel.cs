using BookKeeping.App.Web.Store;

using Fluxor;

using Microsoft.AspNetCore.Components;

using ReactiveUI;

using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using static BookKeeping.App.Web.Store.DisplayMessage;

namespace BookKeeping.App.Web.ViewModels
{
	public class IncomeExpenseViewModel
		: ReactiveObject
	{
		private readonly CompositeDisposable _disposables;
		public IDispatcher Dispatcher { get; }
		public IState<ApplicationState> ApplicationState { get; }

		private string _errorMessage;
		public string ErrorMessage
		{
			get => _errorMessage;
			set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
		}

		private string _informationalMessage;
		public string InformationalMessage
		{
			get => _informationalMessage;
			set => this.RaiseAndSetIfChanged(ref _informationalMessage, value);
		}

		private bool _isInvalidSelection;
		public bool IsInvalidSelection
		{
			get => _isInvalidSelection;
			set => this.RaiseAndSetIfChanged(ref _isInvalidSelection, value);
		}

		private bool _isError;
		public bool IsError
		{
			get => _isError;
			set => this.RaiseAndSetIfChanged(ref _isError, value);
		}

		private bool _isErrorShown;
		public bool IsErrorShown
		{
			get => _isError;
			set => this.RaiseAndSetIfChanged(ref _isErrorShown, value);
		}

		private int _selectedYear;
		public int SelectedYear
		{
			get => _selectedYear;
			set => this.RaiseAndSetIfChanged(ref _selectedYear, value);
		}

		private IncomeExpenseState _selectedState;
		public IncomeExpenseState SelectedState
		{
			get => _selectedState;
			set => this.RaiseAndSetIfChanged(ref _selectedState, value);
		}

		public IncomeExpenseViewModel(
			IDispatcher dispatcher,
			IState<ApplicationState> applicationState
		)
		{
			_disposables = new();
			_errorMessage = string.Empty;
			_informationalMessage = string.Empty;
			_selectedState = applicationState.Value.SelectedIncomeExpense
						?? new(true, false, false, new(), TimeSpan.FromMinutes(1), null);
			Dispatcher = dispatcher;
			ApplicationState = applicationState;

			Observable.FromEventPattern<ApplicationState>(
				eh => ApplicationState.StateChanged += eh,
				eh => ApplicationState.StateChanged -= eh
			)
			.Subscribe(ep =>
			{
				var state = ep.EventArgs;
				var dm = state.DisplayMessage
					 ?? new("Locading...", MessageType.Information);
				if (dm is not null)
				{
					switch (dm.Type)
					{
						case MessageType.Information:
							InformationalMessage = dm.Message;
							break;

						case MessageType.Error:
							ErrorMessage = dm.Message;
							break;
					}
				}
				dm = state.YearsState?.DisplayMessage;
				if (dm is not null)
				{
					switch (dm.Type)
					{
						case MessageType.Information:
							InformationalMessage = dm.Message;
							break;

						case MessageType.Error:
							ErrorMessage = dm.Message;
							break;
					}
				}
				SelectedState = state.SelectedIncomeExpense ?? _selectedState;
			})
			.DisposeWith(_disposables);

			this
				.WhenAnyValue(vm => vm.SelectedYear)
				.Where(y => y > 0)
				.Subscribe(y =>
				{
					Dispatcher.Dispatch(
						new YearSelectedAction(
							applicationState.Value with
							{
								SelectedYear = y
							}
						)
					);
				})
				.DisposeWith(_disposables);

			this
				.WhenAnyValue(vm => vm.SelectedYear)
				.Select(y => y <= 0)
				.BindTo(this, vm => vm.IsInvalidSelection)
				.DisposeWith(_disposables);

			//this
			//	.WhenAnyValue(vm => vm.SelectedYear)
			//	.StartWith(0)
			//	.Where(y => y <= 0)
			//	.Subscribe(_ => ErrorMessage = "Invalid selection")
			//	.DisposeWith(_disposables);

			//this
			//	.WhenAnyValue(vm => vm.SelectedYear)
			//	.Where(y => y > 0)
			//	.Subscribe(invalid => ErrorMessage = string.Empty)
			//	.DisposeWith(_disposables);
		}

		public void GetYears()
		{
			if (ApplicationState.Value.EntityTags is not null
			 && ApplicationState.Value.EntityTags.EntityTags.TryGetValue("api/transactions/years", out var eTag)
			)
				Dispatcher?.Dispatch(new FetchYearsAction(eTag));
			else
				Dispatcher?.Dispatch(new FetchYearsAction());
		}

		public void OnChange(ChangeEventArgs args)
			=> SelectedYear = args.Value is not null && int.TryParse(args.Value.ToString(), out var selectedYear)
			 ? selectedYear
			 : 0;
	}
}
