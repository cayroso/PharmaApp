using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public const string SystemRoleName = "System";
        public static ApplicationRoles System = new ApplicationRoles(SystemRoleName.ToLower(), SystemRoleName);

        public const string PediaRoleName = "Pedia";
        public static ApplicationRoles Pedia = new ApplicationRoles(PediaRoleName.ToLower(), PediaRoleName);

        public const string ReceptionistRoleName = "Receptionist";
        public static ApplicationRoles Receptionist = new ApplicationRoles(ReceptionistRoleName.ToLower(), ReceptionistRoleName);

        public const string ParentRoleName = "Parent";
        public static ApplicationRoles Parent = new ApplicationRoles(ParentRoleName.ToLower(), ParentRoleName);

        public static List<ApplicationRoles> Items
        {
            get
            {
                return new List<ApplicationRoles>
                {
                    System,
                    Pedia,
                    Receptionist,
                    Parent
                };
            }
        }
    }
}
