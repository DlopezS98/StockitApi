using System.Linq.Expressions;

namespace StockitApi.Interfaces;

public interface ISpecification<T>
{
    /// <summary>
    /// The criteria to filter entities.
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }
    /// <summary>
    /// The related entities to include in the query.
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }
    IReadOnlyList<string> IncludeStrings { get; }
    /// <summary>
    /// The property to order the entities by in ascending order.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }
    /// <summary>
    /// The property to order the entities by in descending order.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }
    Expression<Func<T, object>>? GroupBy { get; }
    /// <summary>
    /// Number of entities to take (for pagination).
    /// </summary>
    int? Take { get; }
    /// <summary>
    /// Number of entities to skip (for pagination).
    /// </summary>
    int? Skip { get; }
    /// <summary>
    /// Indicates whether pagination is enabled.
    /// </summary>
    bool IsPagingEnabled { get; }
}