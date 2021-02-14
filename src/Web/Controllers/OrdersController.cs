using App.CQRS;
using App.CQRS.CustomerOrders.Common.Commands.Command;
using App.CQRS.Drugs.Common.Queries.Query;
using App.CQRS.Orders.Common.Queries.Query;
using App.Hubs;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Chats;
using Data.Common;
using Data.Enums;
using Data.Identity.DbContext;
using Data.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.BackgroundServices;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        readonly ICommandHandlerDispatcher _commandHandlerDispatcher;
        readonly AppDbContext _appDbContext;
        readonly IHubContext<TripHub, ITripClient> _tripHubContext;

        public OrdersController(
            IQueryHandlerDispatcher queryHandlerDispatcher,
            ICommandHandlerDispatcher commandHandlerDispatcher,
            AppDbContext appDbContext,
            IHubContext<TripHub, ITripClient> tripHubContext)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
            _commandHandlerDispatcher = commandHandlerDispatcher ?? throw new ArgumentNullException(nameof(commandHandlerDispatcher));
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _tripHubContext = tripHubContext ?? throw new ArgumentNullException(nameof(tripHubContext));
        }

        [HttpPost("customer/add-order")]
        public async Task<IActionResult> AddOrder([FromBody] AddOrderInfo info)
        {
            var lines = info.Items.Select(e => new AddCustomerOrderCommand.Line
            {
                DrugId = e.DrugId,
                Quantity = e.DrugQuantity
            });

            var cmd = new AddCustomerOrderCommand("", TenantId, UserId, GuidStr(), info.PharmacyId, lines);

            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok(cmd.OrderId);
        }

        [HttpPut("{id}/change-status/{status}")]
        public async Task<IActionResult> UpdateStatus(string id, EnumOrderStatus status)
        {
            var order = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == id);

            if (order == null)
                return NotFound();

            order.OrderStatus = status;

            switch (status)
            {
                case EnumOrderStatus.Accepted:
                    order.StartPickupDate = DateTime.MaxValue;
                    order.EndPickupDate = DateTime.MaxValue;
                    break;

                case EnumOrderStatus.ReadyForPickup:
                    order.StartPickupDate = DateTime.UtcNow;
                    order.EndPickupDate = DateTime.UtcNow.AddDays(2);
                    break;
            }

            await _appDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var query = new GetCustomerOrderByIdQuery("", TenantId, UserId, id);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetCustomerOrderByIdQuery, GetCustomerOrderByIdQuery.Order>(query);

            return Ok(dto);
        }

        [HttpGet("search-my-orders")]
        public async Task<IActionResult> GetMyOrders(string c, int p, int s, string sf, int so)
        {
            var query = new SearchOrderQuery("", TenantId, UserId, UserId, null, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchOrderQuery, Paged<SearchOrderQuery.Order>>(query);

            return Ok(dto);
        }

        [HttpGet("search-pharmacy-orders")]
        public async Task<IActionResult> GetPharmacyOrders(string c, int p, int s, string sf, int so)
        {
            var query = new SearchOrderQuery("", TenantId, UserId, null, PharmacyId, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchOrderQuery, Paged<SearchOrderQuery.Order>>(query);

            return Ok(dto);
        }
    }
}
