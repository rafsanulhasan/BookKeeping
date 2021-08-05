using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;

using System;
using System.Security.Cryptography;

using static Microsoft.AspNetCore.Mvc.CacheRequestHeadersConst;

namespace Microsoft.AspNetCore.Mvc.Filters
{
	internal class EtagHandlerFeature
		: IEtagHandlerFeature, IDisposable
	{
		private readonly HashAlgorithm _hashAlgorithm;
		private readonly IHeaderDictionary _headers;

		public EtagHandlerFeature(
			HashAlgorithm hashAlgorithm,
			IHeaderDictionary headers
		)
		{
			_hashAlgorithm = hashAlgorithm;
			_headers = headers;
		}

		private bool CheckRequestHeader(
			IEtaggable data, 
			CacheRequestHeaders header
		)
		{
			var headerName = header == CacheRequestHeaders.IfMatch 
				          ? IfMatch 
						: IfNoneMatch;

			var headerHasValue = _headers.TryGetValue(
				headerName, 
				out var eTag
			);
			eTag = $"\"{eTag}\"";

			var entityTag = data.GetEtag(_hashAlgorithm);
			entityTag = $"\"{entityTag}\"";

			switch (header)
			{
				case CacheRequestHeaders.IfMatch:
					if (!headerHasValue)
						return false;

					return entityTag.Equals(eTag);
				default:
					if (!headerHasValue)
						return true;

					return !entityTag.Equals(eTag);
			}
		}

		public bool Match(ETaggable data)
			=> CheckRequestHeader(data, CacheRequestHeaders.IfMatch);

		public bool NoneMatch(ETaggable data)
			=> CheckRequestHeader(data, CacheRequestHeaders.IfNoneMatch);

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