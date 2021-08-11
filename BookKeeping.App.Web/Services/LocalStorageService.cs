using Microsoft.JSInterop;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookKeeping.App.Web.Services
{
	public class LocalStorageService 
		: ILocalStorageService
	{
		public IJSRuntime Js { get; }
		public LocalStorageService(
			IJSRuntime js
		)
		{
			Js = js;
		}

		public ValueTask<T> GetItem<T>(string key)
		{
			return Js.InvokeAsync<T>("getItem", key);
		}

		public ValueTask<List<T>?> GetItems<T>(string key)
		{
			return Js.InvokeAsync<List<T>?>("getItems", key);
		}

		public ValueTask<T> SetItem<T>(string key, T value)
		{
			return Js.InvokeAsync<T>("setItem", key, value);
		}
	}
}
