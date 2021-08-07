using System.Collections.Generic;

namespace BookKeeping.App.Web.Store.EntityTag
{
	public record EntityTagState(IDictionary<string, string?> EntityTags);
}
