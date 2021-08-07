using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

using System;
using System.Security.Cryptography;

namespace Microsoft.AspNetCore.Mvc.Filters
{
	internal class EtagHeaderFilter
		: ActionFilterAttribute, IActionFilter, IAsyncResultFilter, IDisposable
	{
		private readonly HashAlgorithm _hashAlgorithm = SHA512.Create();

		private string? GetEtag(
		    ActionExecutedContext context,
		    out OkObjectResult? result
		)
		{
			if (context.Result is OkObjectResult contextResult)
			{
				result = contextResult;
				if (contextResult.Value is ETaggable eTaggable)
				{
					var etag = eTaggable.GetEtag(_hashAlgorithm);
					if (!string.IsNullOrWhiteSpace(etag)
					 && !etag.Contains("\"")
					)
						etag = $"\"{etag}\"";
					return etag;
				}
			}
			result = null;
			return null;
		}

		private void ProcessETag(
			ActionExecutedContext context,
			out OkObjectResult? okResult
		)
		{
			var etag = GetEtag(context, out okResult);
			if (!string.IsNullOrWhiteSpace(etag))
			{
				context.HttpContext.Response.Headers.Append(HeaderNames.ETag, etag);
			}
		}

		private void ProcessResult(
			ActionExecutedContext context,
			OkObjectResult okResult,
			IEtagHandlerFeature handler
		)
		{
			void MutateResult()
			{
				okResult.StatusCode = StatusCodes.Status304NotModified;
				okResult.Value = null;
				context.Result = okResult;
			}
			if (okResult is not null)
			{
				if (okResult.Value is ETaggable taggable)
				{
					if (taggable is not null)
					{
						var match = handler.Match(taggable);
						var noneMatch = handler.NoneMatch(taggable);
						var modifiedSince = handler.ModifiedSince(taggable);
						var unmodifiedSince = handler.UnmodifiedSince(taggable);

						if (match)
							MutateResult();
						else if (!noneMatch)
							MutateResult();
						else if (!modifiedSince)
							MutateResult();
						else if (unmodifiedSince)
							MutateResult();
					}
				}
			}
		}

		public EtagHeaderFilter()
		{
		}

		public EtagHeaderFilter(
			HashAlgorithm hashAlgorithm
		)
			=> _hashAlgorithm = hashAlgorithm;

		public override void OnActionExecuted(ActionExecutedContext context)
		{
			base.OnActionExecuted(context);

			var httpContext = context.HttpContext;

			httpContext.Features.Set<IEtagHandlerFeature>(
				new EtagHandlerFeature(
					_hashAlgorithm,
					httpContext.Request.Headers,
					httpContext.RequestServices.GetRequiredService<ILogger<EtagHandlerFeature>>()
				)
			);

			var handlerFeature = context.HttpContext.Request.GetEtagHandler();
			ProcessETag(context, out var result);
			ProcessResult(context, result!, handlerFeature);
		}

		#region IDisposable Support
		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
#if netstd20 || net50
					_hashAlgorithm?.Dispose();
#endif
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				_disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~EtagHeaderFilter() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}