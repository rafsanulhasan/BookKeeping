using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace BookKeeping.Data.Abstractions
{
	public class RepositoryBase<TEntity, TKey>
		: IRepository<TEntity, TKey>
		where TKey : IEquatable<TKey>
		where TEntity : class, IDataEntity<TKey>, new()
	{
		private Task<EntityEntry<TEntity>>? _updateTask;
		private Task<EntityEntry<TEntity>>? _deleteByKeyTask;
		private Task<EntityEntry<TEntity>>? _deleteTask;
		private bool _disposedValue;

		protected BookKeepingDbContext Context { get; }
		protected DbSet<TEntity> Set { get; }

		private async Task<EntityEntry<TEntity>> UpdateAsyncTask(
			TKey id,
			TEntity entity
		)
		{
			if (id is null)
				throw new ArgumentNullException(nameof(id));
			if (entity is null)
				throw new ArgumentNullException(nameof(entity));

			var existingEntity = await ReadSingleOrDefaultAsync(id);

			return existingEntity is null
			 ? throw new InvalidOperationException($"Entity can't be updated. no data found with the correspinding id")
			 : Set.Update(entity);
		}
		private async Task<EntityEntry<TEntity>> DeleteAsyncTask(
		    TKey id
		)
		{
			if (id is null)
				throw new ArgumentNullException(nameof(id));

			var existingEntity = await ReadSingleOrDefaultAsync(id);

			return existingEntity is null
			 ? throw new InvalidOperationException($"Entity can't be updated. no data found with the correspinding id")
			 : Set.Remove(existingEntity);
		}
		private async Task<EntityEntry<TEntity>> DeleteAsyncTask(
		    TEntity entity
		)
		{
			if (entity is null)
				throw new ArgumentNullException(nameof(entity));

			var existingEntity = await ReadSingleOrDefaultAsync(e
								=> e.Id.Equals(entity.Id)
							 );

			return existingEntity is null
			 ? throw new InvalidOperationException($"Entity can't be updated. no data found with the correspinding id")
			 : Set.Remove(existingEntity);
		}

		protected RepositoryBase(BookKeepingDbContext context)
		{
			Context = context;
			Set = context.Set<TEntity>();
		}

		public EntityEntry<TEntity> Create(TEntity entity)
			=> Context.Add(entity);

		public IQueryable<TEntity> Read()
			=> Set.AsQueryable();

		public IQueryable<TEntity> Read(TKey id)
			=> Set.Where(e => e.Id.Equals(id));

		public IQueryable<TEntity> Read(
			Expression<Func<TEntity, bool>> predicate
		)
			=> Set.Where(predicate);

		public Task<TEntity> ReadSingleAsync(
			TKey id,
			CancellationToken cancellationToken = default
		)
			=> Set.SingleAsync(
				e => e.Id.Equals(id),
				cancellationToken
			);

		public Task<TEntity> ReadSingleAsync(
			Expression<Func<TEntity, bool>> predicate,
			CancellationToken cancellationToken = default
		)
			=> Set.SingleAsync(predicate, cancellationToken);

		public Task<TEntity> ReadSingleOrDefaultAsync(
			TKey id,
			 CancellationToken cancellationToken = default
		)
			=> Set.SingleOrDefaultAsync(
				e => e.Id.Equals(id),
				cancellationToken
			);

		public Task<TEntity> ReadSingleOrDefaultAsync(
			Expression<Func<TEntity, bool>> predicate,
			CancellationToken cancellationToken = default
		)
			=> Set.SingleOrDefaultAsync(predicate, cancellationToken);

		public Task<EntityEntry<TEntity>> UpdateAsync(
			TKey id,
			TEntity entity
		)
		{
			_updateTask ??= UpdateAsyncTask(id, entity);
			return _updateTask;
		}

		public Task<EntityEntry<TEntity>> DeleteAsync(
			TKey id
		)
		{
			_deleteByKeyTask ??= DeleteAsyncTask(id);
			return _deleteByKeyTask;
		}

		public Task<EntityEntry<TEntity>> DeleteAsync(
			TEntity entity
		)
		{
			_deleteTask ??= DeleteAsyncTask(entity);
			return _deleteTask;
		}

		public Task<int> SaveChangesAsync(
			CancellationToken cancellationToken = default
		)
			=> Context.SaveChangesAsync(cancellationToken);

		public Task<int> SaveChangesAsync(
			bool acceptAllChangesBeforeSuccess,
			CancellationToken cancellationToken = default
		)
			=> Context.SaveChangesAsync(
				acceptAllChangesBeforeSuccess,
				cancellationToken
			);

		#region Disposable Pattern

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					Context?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				_disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~RepositoryBase()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public ValueTask DisposeAsync()
			=> Context.DisposeAsync();

		#endregion
	}
}
