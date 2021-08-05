namespace Symbiosis.Json.Specs.Ion
{
	public record ApiResource<T>
		: ApiResourceBase
	where T : class, new()
	{
		public T? Value { get; set; }
	}
}
