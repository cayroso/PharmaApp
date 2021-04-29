using App.CQRS.Users.Common.Queries.Query;
using Cayent.Core.Common;
using Cayent.Core.CQRS.Queries;
using Data.App.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cayent.Core.Common.Extensions;

namespace App.CQRS.Users.Common.Queries.Handler
{
    public sealed class UserCommonQueryHandler :
        IQueryHandler<GetUserByIdQuery, GetUserByIdQuery.User>,
        IQueryHandler<SearchUserQuery, Paged<SearchUserQuery.User>>
    {
        readonly AppDbContext _dbContext;
        public UserCommonQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        async Task<GetUserByIdQuery.User> IQueryHandler<GetUserByIdQuery, GetUserByIdQuery.User>.HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var sql = from u in _dbContext.Users

                      join ps in _dbContext.PharmacyStaffs on u.UserId equals ps.StaffId into pss
                      from ps in pss.DefaultIfEmpty()

                      join p in _dbContext.Pharmacies on ps.PharmacyId equals p.PharmacyId into phs
                      from p in phs.DefaultIfEmpty()

                      where u.UserId == query.UserIdToGet

                      select new GetUserByIdQuery.User
                      {
                          UserId = u.UserId,
                          FirstName = u.FirstName,
                          MiddleName = u.MiddleName,
                          LastName = u.LastName,
                          Email = u.Email,
                          PhoneNumber = u.PhoneNumber,
                          
                          PharmacyId = p != null ? p.PharmacyId : "",
                          PharmacyName = p != null ? p.Name : "",

                          Roles = u.UserRoles.Select(e => new GetUserByIdQuery.Role
                          {
                              RoleId = e.Role.RoleId,
                              Name = e.Role.Name
                          })
                      };

            var dto = await sql.FirstOrDefaultAsync();

            return dto;
        }

        async Task<Paged<SearchUserQuery.User>> IQueryHandler<SearchUserQuery, Paged<SearchUserQuery.User>>.HandleAsync(SearchUserQuery query, CancellationToken cancellationToken)
        {
            var sql = from u in _dbContext.Users

                      join ps in _dbContext.PharmacyStaffs on u.UserId equals ps.StaffId into pss
                      from ps in pss.DefaultIfEmpty()

                      join p in _dbContext.Pharmacies on ps.PharmacyId equals p.PharmacyId into phs
                      from p in phs.DefaultIfEmpty()

                      select new SearchUserQuery.User
                      {
                          UserId = u.UserId,
                          UrlProfilePicture = u.Image == null ? null : u.Image.Url,
                          FirstName = u.FirstName,
                          MiddleName = u.MiddleName,
                          LastName = u.LastName,
                          Email = u.Email,
                          PhoneNumber = u.PhoneNumber,
                          Roles = u.UserRoles.Select(e => e.Role.Name),

                          PharmacyId = p != null ? p.PharmacyId : "",
                          PharmacyName = p != null ? p.Name : ""

                      };

            var dto = await sql.ToPagedItemsAsync(query.PageIndex, query.PageSize, cancellationToken);

            return dto;
        }
    }
}
