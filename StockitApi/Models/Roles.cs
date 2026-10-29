namespace StockitApi.Models;

// Defines the baseline roles and their associated permissions.
public static class Role
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string SalesRep = "SalesRep";
    public const string WarehouseStaff = "WarehouseStaff";
    public const string Auditor = "Auditor";

    public static readonly string[] BuiltInRoles = [Admin, Manager, SalesRep, WarehouseStaff, Auditor];

    public static readonly string[] AdminPermissions = Permission.GetAllPermissions();
    public static readonly string[] ManagerPermissions =
    [
        Permission.User.Read, Permission.Product.Read, Permission.Product.Write,
        Permission.Inventory.Read, Permission.Inventory.Adjust, Permission.Customer.Read,
        Permission.Customer.Write, Permission.Invoice.Create, Permission.Invoice.Read,
        Permission.Invoice.Void, Permission.Invoice.Export, Permission.Analytics.Read,
        Permission.Metrics.Read,
    ];

    public static readonly string[] SalesRepPermissions =
    [
        Permission.Product.Read, Permission.Inventory.Read, Permission.Customer.Read,
        Permission.Customer.Write, Permission.Invoice.Create, Permission.Invoice.Read,
    ];

    public static readonly string[] WarehouseStaffPermissions =
    [
        Permission.Product.Read, Permission.Inventory.Read, Permission.Inventory.Adjust,
        Permission.Invoice.Read,
    ];

    public static readonly string[] AuditorPermissions =
    [
        Permission.User.Read, Permission.Product.Read, Permission.Inventory.Read,
        Permission.Customer.Read, Permission.Invoice.Read, Permission.Invoice.Export,
        Permission.Analytics.Read, Permission.Logs.Read, Permission.Metrics.Read,
    ];
}