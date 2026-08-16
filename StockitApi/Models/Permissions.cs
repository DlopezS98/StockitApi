using System.ComponentModel;

namespace StockitApi.Models;

public static class Permission
{
    public static class User
    {
        [Description("View users")]
        public const string Read = "users.read";

        [Description("Create/edit users")]
        public const string Write = "users.write";

        [Description("Deactivate users")]
        public const string Delete = "users.delete";
    }

    public static class Role
    {
        [Description("Manage roles/permissions")]
        public const string Manage = "roles.manage";
    }

    public static class Product
    {
        [Description("View products")]
        public const string Read = "products.read";

        [Description("Create/edit products")]
        public const string Write = "products.write";
    }

    public static class Inventory
    {
        [Description("View stock levels")]
        public const string Read = "inventory.read";

        [Description("Adjust stock")]
        public const string Adjust = "inventory.adjust";
    }

    public static class Customer
    {
        [Description("View customers")]
        public const string Read = "customers.read";

        [Description("Create/edit customers")]
        public const string Write = "customers.write";
    }

    public static class Invoice
    {
        [Description("Create/issue invoices")]
        public const string Create = "invoices.create";

        [Description("View invoices")]
        public const string Read = "invoices.read";

        [Description("Update invoices")]
        public const string Update = "invoices.update";

        [Description("Void invoices")]
        public const string Void = "invoices.void";

        [Description("Export invoices")]
        public const string Export = "invoices.export";
    }

    public static class Analytics
    {
        [Description("View analytics")]
        public const string Read = "analytics.read";
    }

    public static class Logs
    {
        [Description("View audit logs")]
        public const string Read = "logs.read";
    }

    public static class Metrics
    {
        [Description("View usage metrics")]
        public const string Read = "metrics.read";
    }
}