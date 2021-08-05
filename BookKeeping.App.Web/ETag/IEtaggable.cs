using System.Security.Cryptography;

namespace Microsoft.AspNetCore.Mvc.Abstractions
{
	public interface IEtaggable
	{
		string? GetEtag(HashAlgorithm? algorithm = null);
	}
}