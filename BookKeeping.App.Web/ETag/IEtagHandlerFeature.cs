namespace Microsoft.AspNetCore.Mvc.Abstractions
{
	public interface IEtagHandlerFeature
	{
		bool NoneMatch(ETaggable data);
		bool Match(ETaggable data);
	}
}