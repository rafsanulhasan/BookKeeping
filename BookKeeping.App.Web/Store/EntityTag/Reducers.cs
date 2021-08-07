using Fluxor;

namespace BookKeeping.App.Web.Store.EntityTag
{
	public static class EntityTagReducer
	{
		[ReducerMethod]
		public static EntityTagState UpdateEntityTag(
			EntityTagState state,
			UpdateEntityTagAction action
		)
			=> state with
			{
				EntityTags =action.EntityTags
			};
	}
}
