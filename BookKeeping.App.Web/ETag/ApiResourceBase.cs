using Microsoft.AspNetCore.Mvc.Abstractions;

using System;

namespace Symbiosis.Json.Specs.Ion
{
	public abstract record ApiResourceBase
		: ETaggable, IEquatable<ApiResourceBase>
	{

		public override int GetHashCode() 
			=> base.GetHashCode();

		public virtual bool Equals(ApiResourceBase? other)
			=> GetEtag()!.Equals(other!.GetEtag());
	}
}
