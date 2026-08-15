using Microsoft.EntityFrameworkCore;
using StockitApi.Interfaces;

namespace StockitApi.Data.Repositories;

public class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        // Apply criteria
        var query = inputQuery;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }

        if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.GroupBy != null)
        {
            query = query.GroupBy(specification.GroupBy).SelectMany(g => g);
        }

        if (specification.IsPagingEnabled)
        {
            if (specification.Skip.HasValue) query = query.Skip(specification.Skip.Value);
            if (specification.Take.HasValue) query = query.Take(specification.Take.Value);
        }

        // Apply includes
        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
        query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        return query;
    }
}
