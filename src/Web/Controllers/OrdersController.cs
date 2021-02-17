using App.CQRS;
using App.CQRS.CustomerOrders.Common.Commands.Command;
using App.CQRS.Drugs.Common.Queries.Query;
using App.CQRS.Orders.Common.Queries.Query;
using App.Hubs;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Chats;
using Data.App.Models.Orders;
using Data.Common;
using Data.Enums;
using Data.Identity.DbContext;
using Data.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
        public async Task<IActionResult> AddOrder()
        {
            var infoFile = Request.Form.Files.FirstOrDefault(e => e.Name == "payload");
            var imagefiles = Request.Form.Files.Where(e => e.Name == "files");

            if (infoFile != null && infoFile.Length > 0)
            {
                var streamReader = new StreamReader(infoFile.OpenReadStream());

                var infoJson = streamReader.ReadToEnd();

                var info = JsonConvert.DeserializeObject<AddOrderInfo>(infoJson);

                var lines = info.Items.Select(e => new CustomerPlaceOrderCommand.Line
                {
                    DrugId = e.DrugId,
                    Quantity = e.DrugQuantity
                });

                var cmd = new CustomerPlaceOrderCommand("", TenantId, UserId, GuidStr(), info.PharmacyId, lines);

                await _commandHandlerDispatcher.HandleAsync(cmd);

                //  handle file uploads
                var order = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == cmd.OrderId);

                foreach (var file in imagefiles)
                {                    
                    var bytes = new byte[file.Length];

                    using (var stream = file.OpenReadStream())
                    {
                        stream.Read(bytes);
                    }

                    var fileUploadId = GuidStr();

                    var orderFileUpload = new OrderFileUpload
                    {
                        OrderId = order.OrderId,
                        FileUpload = new Data.App.Models.FileUploads.FileUpload
                        {
                            FileUploadId = fileUploadId,
                            FileName = file.FileName,
                            ContentDisposition = file.ContentDisposition,
                            ContentType = file.ContentType,
                            Content = bytes,
                            Length = file.Length,
                            DateCreated = DateTime.UtcNow,
                            //Url = $"api/files/{TenantId}/{fileUploadId}",
                            Url = $"api/files/{fileUploadId}",
                        }
                    };

                    order.FileUploads.Add(orderFileUpload);
                }

                await _appDbContext.SaveChangesAsync();

                return Ok(cmd.OrderId);
            }
            //var inf = [FromBody] AddOrderInfo info;

            return BadRequest();
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
