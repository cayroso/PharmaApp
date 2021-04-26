using App.CQRS.Pharmacy.Common.Queries.Query;
using Cayent.Core.Common;
using Cayent.Core.CQRS.Queries;
using Data.App.DbContext;
using Data.Constants;
using Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Controllers;
using Cayent.Core.Common.Extensions;

namespace Web.Areas.Systems.Controllers
{
    [Authorize(Policy = ApplicationRoles.SystemsRoleName)]
    [ApiController]
    [Route("api/systems/[controller]")]
    [Produces("application/json")]
    public class DefaultController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        public DefaultController(IQueryHandlerDispatcher queryHandlerDispatcher)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromServices] AppDbContext appDbContext, int year, int month)
        {
            var now = DateTime.UtcNow;

            var startMonth = new DateTime(now.Year, now.Month, 1);
            var endMonth = startMonth.AddMonths(1);

            var startWeek = now.AddDays(-(int)now.DayOfWeek);
            var endWeek = startWeek.AddDays(7);

            var startLastWeek = startWeek.AddDays(-7);
            var endLastWeek = startWeek;

            var startToday = now.Date;
            var endToday = startToday.AddDays(1);

            var startYesterday = now.Date.AddDays(-1);
            var endYesterday = now.Date;

            var orderHistory = await appDbContext.OrderTimelines
                .Include(e => e.Order)
                .Where(e => e.Order.PharmacyId == PharmacyId
                    && e.DateTimeline >= startMonth && e.DateTimeline <= endMonth)
                .AsNoTracking()
                .ToListAsync();

            var numberOfPlaced = orderHistory.Count(e => e.Status == EnumOrderStatus.Placed);
            var numberOfCancelled = orderHistory.Count(e => e.Status == EnumOrderStatus.Cancelled);
            var numberOfRejected = orderHistory.Count(e => e.Status == EnumOrderStatus.Rejected);
            var numberOfCompleted = orderHistory.Count(e => e.Status == EnumOrderStatus.Completed);

            var orders = await appDbContext.Orders
                .Include(e => e.Customer)
                    .ThenInclude(e => e.User)
                .Include(e => e.LineItems)
                    .ThenInclude(e => e.Drug)
                .AsNoTracking()
                .Where(e =>
                        e.PharmacyId == PharmacyId
                        && e.OrderDate >= startMonth && e.OrderDate <= endMonth
                        && (e.OrderStatus == EnumOrderStatus.Completed))
                .ToListAsync();

            var orderLines = orders.SelectMany(e => e.LineItems);

            var weekOrders = orders.Where(e => e.OrderDate >= startWeek && e.OrderDate <= endWeek).ToList();
            var lastWeekOrders = orders.Where(e => e.OrderDate >= startLastWeek && e.OrderDate <= endLastWeek).ToList();
            var todayOrders = orders.Where(e => e.OrderDate >= startToday && e.OrderDate <= endToday).ToList();
            var yesterdayOrders = orders.Where(e => e.OrderDate >= startYesterday && e.OrderDate <= endYesterday).ToList();

            var monthSales = orders.Sum(e => e.GrossPrice);
            var weekSales = weekOrders.Sum(e => e.GrossPrice);
            var lastWeekSales = lastWeekOrders.Sum(e => e.GrossPrice);
            var todaySales = todayOrders.Sum(e => e.GrossPrice);
            var yesterdaySales = yesterdayOrders.Sum(e => e.GrossPrice);

            var monthSold = orders.Count();
            var weekSold = weekOrders.Count();
            var lastWeekSold = lastWeekOrders.Count();
            var todaySold = todayOrders.Count();
            var yesterdaySold = yesterdayOrders.Count();

            // top medicines
            var topMedicines = orderLines
                            .GroupBy(e => e.Drug.Name)
                            .Select(e => new
                            {
                                Name = e.Key,
                                TotalPrice = e.Sum(p => p.ExtendedPrice),
                                TotalCount = e.Sum(p => p.Quantity),
                            }).OrderByDescending(e => e.TotalPrice).ToList();

            // top customers
            var topCustomers = orders
                            .GroupBy(e => e.Customer.User.FirstLastName)
                            .Select(e => new
                            {
                                Name = e.Key,
                                TotalPrice = e.Sum(p => p.GrossPrice),
                                TotalOrders = e.Count(),
                            }).OrderByDescending(e => e.TotalPrice).ToList();

            var dto = new
            {
                numberOfPlaced,
                numberOfCancelled,
                numberOfRejected,
                numberOfCompleted,

                monthSales,
                weekSales,
                lastWeekSales,
                todaySales,
                yesterdaySales,

                monthSold,
                weekSold,
                lastWeekSold,
                todaySold,
                yesterdaySold,

                topMedicines,
                topCustomers,
            };

            return Ok(dto);
        }


        [HttpGet("pharmacies")]
        public async Task<IActionResult> GetPharmaciesAsync(string c, int p, int s, string sf, int so, CancellationToken cancellationToken = default)
        {
            var query = new SearchPharmacyQuery("", TenantId, UserId, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchPharmacyQuery, Paged<SearchPharmacyQuery.Pharmacy>>(query, cancellationToken);

            return Ok(dto);
        }

        [HttpGet("pharmacies/{id}")]
        public async Task<IActionResult> GetPharmaciesAsync(string id, CancellationToken cancellationToken = default)
        {
            var query = new GetPharmacyByIdQuery("", TenantId, UserId, id);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetPharmacyByIdQuery, GetPharmacyByIdQuery.Pharmacy>(query, cancellationToken);

            return Ok(dto);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsersAsync(string c, int p, int s, string sf, int so, [FromServices] AppDbContext appDbContext, CancellationToken cancellationToken = default)
        {
            //var query = new SearchPharmacyQuery("", TenantId, UserId, c, p, s, sf, so);

            //var dto = await _queryHandlerDispatcher.HandleAsync<SearchPharmacyQuery, Paged<SearchPharmacyQuery.Pharmacy>>(query, cancellationToken);

            //return Ok(dto);
            var sql = await appDbContext.Users
                        .AsNoTracking()
                        .ToPagedItemsAsync(p, s, cancellationToken);

            return Ok(sql);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUsersAsync(string id, [FromServices] AppDbContext appDbContext, CancellationToken cancellationToken = default)
        {
            //var query = new SearchPharmacyQuery("", TenantId, UserId, c, p, s, sf, so);

            //var dto = await _queryHandlerDispatcher.HandleAsync<SearchPharmacyQuery, Paged<SearchPharmacyQuery.Pharmacy>>(query, cancellationToken);

            //return Ok(dto);
            var sql = await appDbContext.Users
                        .AsNoTracking()
                        .Where(e => e.UserId == id)
                        .FirstOrDefaultAsync();

            return Ok(sql);
        }
    }
}
