using System.Collections.Generic;

namespace BookKeeping.App.Web.Store.EntityTag
{
	public record UpdateEntityTagAction(IDictionary<string, string?> EntityTags);
}
