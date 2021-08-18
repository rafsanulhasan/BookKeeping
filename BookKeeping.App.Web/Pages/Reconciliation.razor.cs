using BookKeeping.App.Web.Store;

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BookKeeping.App.Web.Pages
{
	public partial class Reconciliation
	{
		private bool _disposed;
		private readonly CompositeDisposable _disposables = new();

		protected override void OnInitialized()
		{
			base.OnInitialized();
			ViewModel.GetYears();
			Observable.FromEventPattern<ApplicationState>(
				eh => ViewModel.ApplicationState.StateChanged += eh,
				eh => ViewModel.ApplicationState.StateChanged -= eh
			)
			.Subscribe(_ => StateHasChanged())
			.DisposeWith(_disposables);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!_disposed)
			{
				if (disposing)
					_disposables?.Dispose();
			}
		}
	}
}
