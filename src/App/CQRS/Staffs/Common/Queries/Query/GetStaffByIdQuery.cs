using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Staffs.Common.Queries.Query
{
    public sealed class GetStaffByIdQuery : AbstractQuery<GetStaffByIdQuery.Staff>
    {

        public string StaffId { get; }


        public GetStaffByIdQuery(string correlationId, string tenantId, string userId, string staffId)
               : base(correlationId, tenantId, userId)
        {
            StaffId = staffId;
        }


        public class Staff
        {
            public string StaffId { get; set; }

            public string UrlProfilePicture { get; set; }

            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Token { get; set; }

            public IEnumerable<Role> Roles { get; set; } = new List<Role>();
        }

        public class Role
        {
            public string RoleId { get; set; }
            public string Name { get; set; }
        }
    }
}
