
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BookKeeping.Data.Helpers
{
	public static class QueryableExtensions
	{
		public static IQueryable<TEntity> ApplyPaging<TEntity>(
			this IQueryable<TEntity> query,
			int pageNumber = 0,
			int pageSize = 0
		)
			where TEntity : class, new()
		{
			if (pageNumber <= 0)
				pageNumber = 0;
			if (pageSize <= 0)
				pageSize = 0;
			var offset = pageSize * pageNumber;
			return query.Skip(offset).Take(pageSize);
		}
		public static IQueryable<TEntity> ApplySorting<TEntity, TSortKey>(
			this IQueryable<TEntity> query,
			Expression<Func<TEntity, TSortKey>> sortKeySelectorPredicate,
			SortOrder sortOrder
		)
		    where TEntity : class, new()
			=> sortOrder == SortOrder.Ascending
				? query.OrderBy(sortKeySelectorPredicate)
				: query.OrderByDescending(sortKeySelectorPredicate);
	}
}
