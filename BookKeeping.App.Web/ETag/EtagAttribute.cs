using Microsoft.AspNetCore.Mvc.Filters;

using System;
using System.Security.Cryptography;

namespace Microsoft.AspNetCore.Mvc
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class EtagAttribute
		: Attribute, IFilterFactory, IDisposable
	{
		private HashAlgorithm _hashAlgorithm = SHA512.Create();
		public bool IsReusable => true;
		public HashAlgorithms HashAlgorithm { get; set; } = HashAlgorithms.SHA512;

		public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
		{
			_hashAlgorithm = HashAlgorithm switch
			{
				HashAlgorithms.SHA1 => SHA1.Create(),
				HashAlgorithms.SHA256 => SHA256.Create(),
				HashAlgorithms.SHA384 => SHA384.Create(),
				_ => SHA512.Create(),
			};

			return new EtagHeaderFilter(_hashAlgorithm);
		}

		#region IDisposable Support
		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_hashAlgorithm?.Dispose();
				}

				_disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
