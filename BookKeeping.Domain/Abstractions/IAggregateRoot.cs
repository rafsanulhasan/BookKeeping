using System;

namespace BookKeeping.Domain.Abstractions
{
	public interface IAggregateRoot<TDomainEntity>
		: IDisposable
		where TDomainEntity : class, IDomainEntity, new()
	{
	}
}
