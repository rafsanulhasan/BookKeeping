using Fluxor;
using Fluxor.UnsupportedClasses;

using Microsoft.AspNetCore.Components;

using ReactiveUI.Blazor;

using System;
using System.ComponentModel;

namespace BookKeeping.App.Web
{
	/// <summary>
	/// A base component for handling property changes and updating the blazer view appropriately.
	/// </summary>
	/// <typeparam name="T">The type of view model. Must support INotifyPropertyChanged.</typeparam>
	public class StatefulReactiveComponentBase<T> 
          : ReactiveInjectableComponentBase<T>
          where T : class, INotifyPropertyChanged
     {
		[Inject]
		private IActionSubscriber? ActionSubscriber { get; set; }

		private bool _disposed;
		private IDisposable? _stateSubscription;
		private readonly ThrottledInvoker? _stateHasChangedThrottler;

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public StatefulReactiveComponentBase()
			: base()
		{
			_stateHasChangedThrottler = new ThrottledInvoker(() =>
			{
				if (!_disposed)
					InvokeAsync(StateHasChanged);
			});
		}

		/// <summary>
		/// If greater than 0, the feature will not execute state changes
		/// more often than this many times per second. Additional notifications
		/// will be surpressed, and observers will be notified of the latest
		/// state when the time window has elapsed to allow another notification.
		/// </summary>
		protected byte MaximumStateChangedNotificationsPerSecond { get; set; }

		/// <see cref="IActionSubscriber.SubscribeToAction{TAction}(object, Action{TAction})"/>
		public void SubscribeToAction<TAction>(Action<TAction> callback)
		{
			ActionSubscriber?.SubscribeToAction<TAction>(
				this, 
				action =>
				{
					if (!_disposed)
						callback(action);
				}
			);
		}

		/// <summary>
		/// Subscribes to state properties
		/// </summary>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			_stateSubscription = StateSubscriber.Subscribe(this, _ =>
			{
				_stateHasChangedThrottler?.Invoke(
					MaximumStateChangedNotificationsPerSecond
				);
			});
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!_disposed)
			{
				_disposed = true;
				if (disposing)
				{
					if (_stateSubscription == null)
						throw new NullReferenceException();

					_stateSubscription.Dispose();
					ActionSubscriber?.UnsubscribeFromAllActions(this);
				}
			}
		}
	}
}
