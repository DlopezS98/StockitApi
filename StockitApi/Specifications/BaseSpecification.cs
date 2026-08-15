using System.Linq.Expressions;
using StockitApi.Interfaces;

namespace StockitApi.Specifications;

public class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; protected set; }
    public List<Expression<Func<T, object>>> Includes { get; protected set; } = [];
    public Expression<Func<T, object>>? OrderBy { get; protected set; }
    public Expression<Func<T, object>>? OrderByDescending { get; protected set; }
    public int? Take { get; protected set; }
    public int? Skip { get; protected set; }
    public bool IsPagingEnabled { get; protected set; }

    public Expression<Func<T, object>>? GroupBy { get; protected set; }

    public IReadOnlyList<string> IncludeStrings => _includeStrings.AsReadOnly();
    private List<string> _includeStrings = [];

    public void ApplyCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);
    public void AddInclude(string includeString) => _includeStrings.Add(includeString);

    public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) => OrderBy = orderByExpression;

    public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression) => OrderByDescending = orderByDescendingExpression;
    public void ApplyGroupBy(Expression<Func<T, object>> groupByExpression) => GroupBy = groupByExpression;

    public void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}