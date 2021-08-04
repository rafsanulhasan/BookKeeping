using BookKeeping.Data.Abstractions;

using System;

namespace BookKeeping.Data.Repositories
{
	internal class Repository<TEntity, TKey>
		: RepositoryBase<TEntity, TKey>
		where TKey : IEquatable<TKey>
		where TEntity : class, IDataEntity<TKey>, new()
	{
		public Repository(BookKeepingDbContext context) 
			: base(context)
		{
		}
	}
}
