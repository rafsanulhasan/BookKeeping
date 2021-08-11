
using Fluxor;

namespace BookKeeping.App.Web.Store
{
	public static partial class EntityTagReducer
	{
		[ReducerMethod]
		public static ApplicationState UpdateEntityTag(
			ApplicationState state,
			UpdateEntityTagAction action
		)
			=> state with
			{
				EntityTags = action.State.EntityTags
			};
	}
}
