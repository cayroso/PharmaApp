using App.CQRS.Staffs.Common.Queries.Query;
using Data.App.DbContext;
using Data.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Staffs.Common.Queries.Handler
{
    public sealed class StaffCommonQueryHandler :
        IQueryHandler<GetStaffByIdQuery, GetStaffByIdQuery.Staff>,
        IQueryHandler<SearchStaffQuery, Paged<SearchStaffQuery.Staff>>
    {
        readonly AppDbContext _appDbContext;
        public StaffCommonQueryHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        async Task<GetStaffByIdQuery.Staff> IQueryHandler<GetStaffByIdQuery, GetStaffByIdQuery.Staff>.HandleAsync(GetStaffByIdQuery query)
        {
            var sql = from s in _appDbContext.Users.AsNoTracking()

                      where s.UserId == query.StaffId

                      select new GetStaffByIdQuery.Staff
                      {
                          StaffId = s.UserId,
                          UrlProfilePicture = s.Image == null ? null : s.Image.Url,
                          FirstName = s.FirstName,
                          MiddleName = s.MiddleName,
                          LastName = s.LastName,
                          Email = s.Email,
                          PhoneNumber = s.PhoneNumber,
                          Token = s.ConcurrencyToken,

                          Roles = s.UserRoles.Select(e => new GetStaffByIdQuery.Role
                          {
                              RoleId = e.Role.RoleId,
                              Name = e.Role.Name
                          })

                      };

            var dto = await sql.FirstOrDefaultAsync();

            return dto;
        }

        async Task<Paged<SearchStaffQuery.Staff>> IQueryHandler<SearchStaffQuery, Paged<SearchStaffQuery.Staff>>.HandleAsync(SearchStaffQuery query)
        {
            var sql = from ps in _appDbContext.PharmacyStaffs
                                        .Include(e => e.Staff)
                                            .ThenInclude(e => e.User)
                                                .ThenInclude(e => e.Image)
                                        .AsNoTracking()
                      where ps.PharmacyId == query.PharmacyId

                      select new SearchStaffQuery.Staff
                      {
                          StaffId = ps.Staff.User.UserId,
                          UrlProfilePicture = ps.Staff.User.Image == null ? null : ps.Staff.User.Image.Url,
                          FirstName = ps.Staff.User.FirstName,
                          MiddleName = ps.Staff.User.MiddleName,
                          LastName = ps.Staff.User.LastName,
                          Email = ps.Staff.User.Email,
                          PhoneNumber = ps.Staff.User.PhoneNumber,
                          Roles = ps.Staff.User.UserRoles.Select(e => e.Role.Name)
                      };

            var dto = await sql.ToPagedItemsAsync(query.PageIndex, query.PageSize);

            return dto;
        }
    }
}
