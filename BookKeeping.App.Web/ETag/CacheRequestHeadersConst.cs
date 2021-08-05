using System.ComponentModel;

namespace Microsoft.AspNetCore.Mvc
{
	public static class CacheRequestHeadersConst
	{
		public const string IfNoneMatch = "If-None-Match";
		public const string IfMatch = "If-Match";

		public enum CacheRequestHeaders
		{
			[Description(CacheRequestHeadersConst.IfNoneMatch)]
			IfNoneMatch,

			[Description(CacheRequestHeadersConst.IfMatch)]
			IfMatch
		}
	}
}
