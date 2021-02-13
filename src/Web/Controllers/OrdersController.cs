using App.CQRS;
using App.CQRS.CustomerOrders.Common.Commands.Command;
using App.CQRS.Drugs.Common.Queries.Query;
using App.CQRS.Orders.Common.Queries.Query;
using App.Hubs;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Chats;
using Data.Common;
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

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var query = new GetCustomerOrderByIdQuery("", TenantId, UserId, id);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetCustomerOrderByIdQuery, GetCustomerOrderByIdQuery.Order>(query);

            return Ok(dto);
        }
    }
}
