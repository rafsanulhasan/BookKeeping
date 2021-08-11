
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookKeeping.App.Web.Services
{
	public interface ILocalStorageService
	{
		ValueTask<T> GetItem<T>(string key);
		ValueTask<List<T>?> GetItems<T>(string key);
		ValueTask<T> SetItem<T>(string key, T value);
	}
}