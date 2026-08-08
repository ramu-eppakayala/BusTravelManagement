namespace BusTravelManagement.Utilities.Constants
{
    public static class RoleConstants
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string Operator = "Operator";
        public const string Customer = "Customer";
        public const string Guest = "Guest";

        public static readonly string[] AllRoles = { SuperAdmin, Admin, Operator, Customer };
        public static readonly string[] AdminRoles = { SuperAdmin, Admin };
        public static readonly string[] ElevatedRoles = { SuperAdmin, Admin, Operator };
    }
}
