using Fluxor;

using System.Collections.Generic;

namespace BookKeeping.App.Web.Store.EntityTag
{
	public class EntityTagFeature
		: Feature<EntityTagState>
	{
		public override string GetName()
			=> "Entity Tag";

		protected override EntityTagState GetInitialState()
			=> new(new Dictionary<string, string?>());
	}
}
