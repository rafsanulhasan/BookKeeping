using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

using System;
using System.Security.Cryptography;

namespace Microsoft.AspNetCore.Mvc.Filters
{
	internal class EtagHandlerFeature
		: IEtagHandlerFeature, IDisposable
	{
		private readonly HashAlgorithm _hashAlgorithm;
		private readonly IHeaderDictionary _headers;

		public ILogger<EtagHandlerFeature>? Logger { get; set; }

		public EtagHandlerFeature(
			HashAlgorithm hashAlgorithm,
			IHeaderDictionary headers,
			ILogger<EtagHandlerFeature> logger
		)
		{
			_hashAlgorithm = hashAlgorithm;
			_headers = headers;
			Logger = logger;
		}

		private bool CheckRequestHeader(
			IEtaggable data, 
			CacheRequestHeaders header
		)
		{
			var headerName = header switch
			{
				CacheRequestHeaders.IfMatch => HeaderNames.IfMatch,
				CacheRequestHeaders.IfNoneMatch => HeaderNames.IfNoneMatch,
				CacheRequestHeaders.IfModifiedSince => HeaderNames.IfModifiedSince,
				CacheRequestHeaders.IfUnmodifiedSince => HeaderNames.IfUnmodifiedSince,
				_ => string.Empty,
			};

			var headerHasValue = _headers.TryGetValue(
				headerName, 
				out var headerEntityTag
			);
			headerEntityTag = $"\"{headerEntityTag}\"";

			var entityTag = data.GetEtag(_hashAlgorithm);
			entityTag = $"\"{entityTag}\"";

			switch (header)
			{
				case CacheRequestHeaders.IfMatch:
					if (!headerHasValue)
						return false;

					return !entityTag.Equals(headerEntityTag);

				case CacheRequestHeaders.IfNoneMatch:
					if (!headerHasValue)
						return true;

					return entityTag.Equals(headerEntityTag);

				case CacheRequestHeaders.IfModifiedSince:
					if (!headerHasValue)
						return true;

					return entityTag.Equals(headerEntityTag);

				case CacheRequestHeaders.IfUnmodifiedSince:
					if (!headerHasValue)
						return false;

					return !entityTag.Equals(headerEntityTag);

				default:
					return false;
			}
		}

		public bool Match(ETaggable data)
			=> CheckRequestHeader(data, CacheRequestHeaders.IfMatch);

		public bool NoneMatch(ETaggable data)
			=> CheckRequestHeader(data, CacheRequestHeaders.IfNoneMatch);

		public bool ModifiedSince(ETaggable data)
			=> CheckRequestHeader(data, CacheRequestHeaders.IfModifiedSince);

		public bool UnmodifiedSince(ETaggable data)
			=> CheckRequestHeader(data, CacheRequestHeaders.IfUnmodifiedSince);

		#region IDisposable Support
		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					_hashAlgorithm.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				_disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~EtagHandlerFeature() {
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