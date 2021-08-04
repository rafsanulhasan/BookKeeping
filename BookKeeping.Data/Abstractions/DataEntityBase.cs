using System;

namespace BookKeeping.Data.Abstractions
{
	public abstract record DataEntityBase<TKey>
		: IDataEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		public virtual TKey Id { get; init; } = default!;
	}
}
