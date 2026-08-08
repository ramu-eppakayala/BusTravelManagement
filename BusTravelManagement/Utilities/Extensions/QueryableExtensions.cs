using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusTravelManagement.Utilities.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> query, int page, int pageSize)
        {
            var totalCount = query.Count();
            var items = query.PageBy(page, pageSize).ToList();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            return ApplyOrder(query, propertyName, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            return ApplyOrder(query, propertyName, "OrderByDescending");
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> query, string propertyName, string methodName)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName);
            if (property == null) return (IOrderedQueryable<T>)query;

            var parameter = Expression.Parameter(type, "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderExpression = Expression.Lambda(propertyAccess, parameter);

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { type, property.PropertyType },
                query.Expression,
                Expression.Quote(orderExpression));

            return (IOrderedQueryable<T>)query.Provider.CreateQuery(resultExpression);
        }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
