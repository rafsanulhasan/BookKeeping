using System;

namespace BookKeeping.Domain.Abstractions
{
	public interface IAggregate<TDomainEntity>
		: IDisposable, IAsyncDisposable
		where TDomainEntity : class, IDomainEntity, new()
	{
	}
}
