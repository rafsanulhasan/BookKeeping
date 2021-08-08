using BookKeeping.API.DTOs;
using BookKeeping.App.Web.Pages;
using BookKeeping.App.Web.Store.EntityTag;
using BookKeeping.App.Web.Store.IncomeExpense;
using BookKeeping.App.Web.Store.Years;

using Fluxor;

using Microsoft.Extensions.Logging;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BookKeeping.App.Web.ViewModels
{
	public class IncomeExpenseViewModel
		: ReactiveObject
	{
		private readonly CompositeDisposable _disposables = new();
		public IState<IncomeExpenseState>? IncomeExpenseState { get; }

		public IState<YearsState>? YearsState { get; }

		public IState<EntityTagState>? EntityTagState { get; }

		public IDispatcher? Dispatcher { get; }

		public ILogger<Reconciliation>? Logger { get; }

		public IDictionary<int, IncomeExpenseDto> IncomeExpenseByYear { get; set; }
			= new Dictionary<int, IncomeExpenseDto>();

		public IDictionary<string, string?> EntityTags { get; set; }
			= new Dictionary<string, string?>();

		public IncomeExpenseViewModel(
			IState<IncomeExpenseState> incomes,
			IState<YearsState> years,
			IState<EntityTagState> entityTags,
			IDispatcher dispatcher,
			ILogger<Reconciliation> logger
		)
		{
			IncomeExpenseState = incomes;
			YearsState = years;
			EntityTagState = entityTags;
			Dispatcher = dispatcher;
			Logger = logger;

			this
				.WhenAnyValue(vm => vm.EntityTagState)
				.Where(s => s is not null)
				.Select(s => s.Value)
				.Where(s => s is not null)
				.Select(s => s.EntityTags)
				.Where(tags => tags is not null)
				.Where(tags => tags.Count > 0)
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(tags => EntityTags = tags)
				.DisposeWith(_disposables);

			this
				.WhenAnyValue(vm => vm.IncomeExpenseState)
				.Where(s => s is not null)
				.Where(s => s.Value is not null)
				.Select(s => s.Value)
				.Where(s => s.Data is not null)
				.Select(s => s.Data)
				.Where(dto => dto is not null)
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(dto => IncomeExpenseByYear = dto!)
				.DisposeWith(_disposables);
		}

		public string? GetETag(string url)
			=> EntityTags.TryGetValue(url, out var eTag)
			 ? eTag
			 : null;

		public IncomeExpenseDto? GetIncomeExpense(int year) 
			=> IncomeExpenseByYear.TryGetValue(year, out var dto)
			 ? dto
			 : null;
	}
}
