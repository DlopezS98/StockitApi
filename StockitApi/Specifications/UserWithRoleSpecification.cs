using StockitApi.Entities;

namespace StockitApi.Specifications;

public class UserWithRoleSpecification : BaseSpecification<User>
{
    public UserWithRoleSpecification()
    {
        AddInclude(u => u.Role!);
    }

    public UserWithRoleSpecification(int id)
    {
        ApplyCriteria(u => u.Id == id);
        AddInclude(u => u.Role!);
    }
}
