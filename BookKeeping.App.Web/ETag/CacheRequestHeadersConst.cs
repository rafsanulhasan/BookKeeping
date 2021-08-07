using Microsoft.Net.Http.Headers;

using System.ComponentModel;

namespace Microsoft.AspNetCore.Mvc
{
	public enum CacheRequestHeaders
	{
		[Description(HeaderNames.IfNoneMatch)]
		IfNoneMatch,

		[Description(HeaderNames.IfMatch)]
		IfMatch,

		[Description(HeaderNames.IfModifiedSince)]
		IfModifiedSince,

		[Description(HeaderNames.IfUnmodifiedSince)]
		IfUnmodifiedSince
	}
}
