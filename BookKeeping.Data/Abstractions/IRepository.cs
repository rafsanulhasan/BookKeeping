
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace BookKeeping.Data.Abstractions
{
	public interface IRepository<TEntity, in TKey>
		: IDisposable, IAsyncDisposable
		where TKey : IEquatable<TKey>
		where TEntity : class, IDataEntity<TKey>, new()
	{
		EntityEntry<TEntity> Create(TEntity entity);
		IQueryable<TEntity> Read();
		IQueryable<TEntity> Read(TKey id);
		IQueryable<TEntity> Read(
			Expression<Func<TEntity, bool>> predicate
		);

		Task<TEntity> ReadSingleAsync(
			TKey id,
			CancellationToken cancellationToken = default
		);
		Task<TEntity> ReadSingleAsync(
			Expression<Func<TEntity, bool>> predicate,
			CancellationToken cancellationToken = default
		);

		Task<TEntity> ReadSingleOrDefaultAsync(
			TKey id,
			CancellationToken cancellationToken = default
		);
		Task<TEntity> ReadSingleOrDefaultAsync(
			Expression<Func<TEntity, bool>> predicate,
			CancellationToken cancellationToken = default
		);

		Task<EntityEntry<TEntity>> UpdateAsync(
			TKey id,
			TEntity entity
		);

		Task<int> SaveChangesAsync(
			CancellationToken cancellationToken = default
		);
		Task<int> SaveChangesAsync(
			 bool acceptAllChangesBeforeSuccess,
			 CancellationToken cancellationToken = default
		 );
	}
}
