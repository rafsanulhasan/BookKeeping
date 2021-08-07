using Microsoft.Net.Http.Headers;

using System.ComponentModel;

namespace Microsoft.AspNetCore.Mvc
{
	public enum CacheRequestHeaders
	{
		[Description(HeaderNames.IfNoneMatch)]
		IfNoneMatch,

		[Description(HeaderNames.IfMatch)]
		IfMatch
	}
}
