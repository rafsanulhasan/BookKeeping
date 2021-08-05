using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

using System;
using System.Security.Cryptography;

namespace Microsoft.AspNetCore.Mvc.Filters
{
	internal class EtagHeaderFilter
		: ActionFilterAttribute, IActionFilter, IAsyncResultFilter, IDisposable
	{
		private readonly HashAlgorithm _hashAlgorithm = SHA512.Create();

		private string? GetEtag(
			ActionExecutingContext context,
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
			ActionExecutingContext context,
			out OkObjectResult? okResult
		)
		{
			var etag = GetEtag(context, out okResult);
			if (!string.IsNullOrWhiteSpace(etag))
			{
				context.HttpContext.Response.Headers.Append("etag", etag);
			}
		}

		private void ProcessETag(
			ActionExecutedContext context,
			out OkObjectResult? okResult
		)
		{
			var etag = GetEtag(context, out okResult);
			if (!string.IsNullOrWhiteSpace(etag))
			{
				context.HttpContext.Response.Headers.Append("etag", etag);
			}
		}

		private void ProcessResult(
			ActionExecutingContext context,
			OkObjectResult okResult,
			IEtagHandlerFeature handler
		)
		{
			if (okResult != null)
			{
				if (okResult.Value is ETaggable taggable)
				{
					if (taggable != null)
					{
						if (handler != null
						 && (handler.Match(taggable) || !handler.NoneMatch(taggable))
						)
						{
							okResult.StatusCode = StatusCodes.Status304NotModified;
							okResult.Value = null;
							context.Result = okResult;
						}
					}
				}
			}
		}

		private void ProcessResult(
			ActionExecutedContext context,
			OkObjectResult okResult,
			IEtagHandlerFeature handler
		)
		{
			if (okResult != null)
			{
				if (okResult.Value is ETaggable taggable)
				{
					if (taggable != null)
					{
						if (handler != null
						 && (handler.Match(taggable) || !handler.NoneMatch(taggable))
						)
						{
							//result.StatusCode = StatusCodes.Status304NotModified;
							okResult.Value = null;
							context.Result = okResult;
						}
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

		//public override void OnActionExecuting(ActionExecutingContext context)
		//{
		//	base.OnActionExecuting(context);

		//	context.HttpContext.Features.Set<IEtagHandlerFeature>(
		//		new EtagHandlerFeature(
		//			_hashAlgorithm,
		//			context.HttpContext.Request.Headers
		//		)
		//	);

		//	var handlerFeature = context.HttpContext.Request.GetEtagHandler();
		//	ProcessETag(context, out var result);
		//	ProcessResult(context, result!, handlerFeature);
		//}

		public override void OnActionExecuted(ActionExecutedContext context)
		{
			base.OnActionExecuted(context);

			context.HttpContext.Features.Set<IEtagHandlerFeature>(
				new EtagHandlerFeature(
					_hashAlgorithm,
					context.HttpContext.Request.Headers
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