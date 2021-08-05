using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Microsoft.AspNetCore.Http
{
	public static class HttpRequestExtensions
	{
		public static IEtagHandlerFeature GetEtagHandler(
			this HttpRequest request
		)
			=> request.HttpContext.Features.Get<IEtagHandlerFeature>();
	}
}
