using Newtonsoft.Json;

using System.Security.Cryptography;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.Abstractions
{
	public abstract record ETaggable
		: IEtaggable
	{
		public virtual string? GetEtag(HashAlgorithm? algorithm = null)
		{
			try
			{
				var json = JsonConvert.SerializeObject(this);

				if (string.IsNullOrWhiteSpace(json))
					return string.Empty;

				var jsonBytes = Encoding.UTF8.GetBytes(json);

				if (jsonBytes == null || jsonBytes.Length == 0)
					return string.Empty;

				var builder = new StringBuilder();
				for (int i = 0; i < jsonBytes.Length; i++)
				{
					builder.Append(jsonBytes[i].ToString("x2"));
				}
				return builder.ToString();

				//return string.Join(
				//	"", 
				//	jsonBytes.Select(b => b.ToString()).ToArray()
				//);

				//if (algorithm == null)
				//	algorithm = SHA512.Create();

				//algorithm.Initialize();
				//var hash = algorithm.ComputeHash(jsonBytes);

				//return hash == null || hash.Length == 0
				//	? string.Empty
				//	: Encoding.ASCII.GetString(hash);
			}
			catch
			{

			}
			return string.Empty;
		}
	}
}
