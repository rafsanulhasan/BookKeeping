using System.Collections.Generic;

namespace BookKeeping.App.Web.Store
{
	public record EntityTagState(Dictionary<string, string?> EntityTags);
}
