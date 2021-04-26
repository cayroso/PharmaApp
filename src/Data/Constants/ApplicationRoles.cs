using System.Collections.Generic;

namespace Data.Constants
{
    public sealed class ApplicationRoles
    {
        private ApplicationRoles(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }

        public const string SystemsRoleName = "Systems";
        public static ApplicationRoles Systems = new ApplicationRoles(SystemsRoleName.ToLower(), SystemsRoleName);

        public const string AdministratorRoleName = "Administrator";
        public static ApplicationRoles Administrator = new ApplicationRoles(AdministratorRoleName.ToLower(), AdministratorRoleName);

        public const string StaffRoleName = "Staff";
        public static ApplicationRoles Staff = new ApplicationRoles(StaffRoleName.ToLower(), StaffRoleName);

        public const string CustomerRoleName = "Customer";
        public static ApplicationRoles Customer = new ApplicationRoles(CustomerRoleName.ToLower(), CustomerRoleName);

        public static List<ApplicationRoles> Items
        {
            get
            {
                return new List<ApplicationRoles>
                {
                    Systems,
                    Administrator,
                    Staff,
                    Customer
                };
            }
        }
    }
}
