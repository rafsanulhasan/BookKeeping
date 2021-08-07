namespace Microsoft.AspNetCore.Mvc.Abstractions
{
	public interface IEtagHandlerFeature
	{
		bool NoneMatch(ETaggable data);
		bool Match(ETaggable data);
		bool ModifiedSince(ETaggable data);
		bool UnmodifiedSince(ETaggable data);
	}
}