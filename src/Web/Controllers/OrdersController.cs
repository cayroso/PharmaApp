﻿using App.CQRS.CustomerOrders.Common.Commands.Command.Customer;
using App.CQRS.CustomerOrders.Common.Commands.Command.Pharmacy;
using App.CQRS.Orders.Common.Queries.Query;
using Cayent.Core.Common;
using Cayent.Core.CQRS.Commands;
using Cayent.Core.CQRS.Queries;
using Cayent.Core.Data.Fileuploads;
using Data.App.DbContext;
using Data.App.Models.Orders;
using Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public OrdersController(
            IQueryHandlerDispatcher queryHandlerDispatcher,
            ICommandHandlerDispatcher commandHandlerDispatcher,
            AppDbContext appDbContext)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
            _commandHandlerDispatcher = commandHandlerDispatcher ?? throw new ArgumentNullException(nameof(commandHandlerDispatcher));
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
        {
            var query = new GetCustomerOrderByIdQuery("", TenantId, UserId, id);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetCustomerOrderByIdQuery, GetCustomerOrderByIdQuery.Order>(query, cancellationToken);

            return Ok(dto);
        }

        [HttpGet("search-my-orders")]
        public async Task<IActionResult> GetMyOrders(EnumOrderStatus orderStatus, string c, int p, int s, string sf, int so, CancellationToken cancellationToken = default)
        {
            var query = new SearchOrderQuery("", TenantId, UserId, UserId, null, orderStatus, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchOrderQuery, Paged<SearchOrderQuery.Order>>(query, cancellationToken);

            return Ok(dto);
        }

        [HttpGet("search-pharmacy-orders")]
        public async Task<IActionResult> GetPharmacyOrders(EnumOrderStatus orderStatus, string c, int p, int s, string sf, int so, CancellationToken cancellationToken = default)
        {
            var query = new SearchOrderQuery("", TenantId, UserId, null, PharmacyId, orderStatus, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchOrderQuery, Paged<SearchOrderQuery.Order>>(query, cancellationToken);

            return Ok(dto);
        }

        #region Customer

        [HttpPost("customer/place-order")]
        public async Task<IActionResult> PostCustomerPlaceOrder(CancellationToken cancellationToken = default)
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

                await _commandHandlerDispatcher.HandleAsync(cmd, cancellationToken);

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
                        FileUpload = new FileUpload
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


        [HttpPut("customer/archive")]
        public async Task<IActionResult> PutCustomerArchiveOrder([FromBody] UpdateOrderStatusInfo info, CancellationToken cancellationToken = default)
        {
            var cmd = new CustomerArchiveOrderCommand("", TenantId, UserId, info.OrderId, info.Token);

            await _commandHandlerDispatcher.HandleAsync(cmd, cancellationToken);

            return Ok();
        }

        [HttpPut("customer/cancel")]
        public async Task<IActionResult> PutCustomerCancelledOrder([FromBody] UpdateOrderStatusInfo info, CancellationToken cancellationToken = default)
        {
            var cmd = new CustomerCancelOrderCommand("", TenantId, UserId, info.OrderId, info.Token, info.Notes);

            await _commandHandlerDispatcher.HandleAsync(cmd, cancellationToken);

            return Ok();
        }

        #endregion

        #region Pharmacy


        [HttpPut("pharmacy/accept")]
        public async Task<IActionResult> PutPharmacyAcceptedOrder([FromBody] UpdateOrderStatusInfo info, CancellationToken cancellationToken = default)
        {
            var cmd = new PharmacyAcceptOrderCommand("", TenantId, UserId, info.OrderId, info.Token);

            await _commandHandlerDispatcher.HandleAsync(cmd, cancellationToken);

            return Ok();
        }

        [HttpPut("pharmacy/reject")]
        public async Task<IActionResult> PutPharmacyRejectedOrder([FromBody] UpdateOrderStatusInfo info, CancellationToken cancellationToken = default)
        {
            var cmd = new PharmacyRejectedOrderCommand("", TenantId, UserId, info.OrderId, info.Token, info.Notes);

            await _commandHandlerDispatcher.HandleAsync(cmd, cancellationToken);

            return Ok();
        }

        [HttpPut("pharmacy/ready-for-pickup")]
        public async Task<IActionResult> PutPharmacyReadyForPickupOrder([FromBody] UpdateOrderStatusInfo info, CancellationToken cancellationToken = default)
        {
            var cmd = new PharmacyOrderReadyForPickupCommand("", TenantId, UserId, info.OrderId, info.Token);

            await _commandHandlerDispatcher.HandleAsync(cmd, cancellationToken);

            return Ok();
        }

        [HttpPut("pharmacy/completed")]
        public async Task<IActionResult> PutPharmacyCompletedOrder([FromBody] UpdateOrderStatusInfo info, CancellationToken cancellationToken = default)
        {
            var cmd = new PharmacyCompleteOrderCommand("", TenantId, UserId, info.OrderId, info.Token);

            await _commandHandlerDispatcher.HandleAsync(cmd, cancellationToken);

            return Ok();
        }

        [HttpPut("pharmacy/archived")]
        public async Task<IActionResult> PutPharmacyArchivedOrder([FromBody] UpdateOrderStatusInfo info, CancellationToken cancellationToken = default)
        {
            var cmd = new PharmacyArchiveOrderCommand("", TenantId, UserId, info.OrderId, info.Token);

            await _commandHandlerDispatcher.HandleAsync(cmd, cancellationToken);

            return Ok();
        }

        #endregion
    }


    public class UpdateOrderStatusInfo
    {
        [Required]
        public string OrderId { get; set; }
        [Required]
        public string Token { get; set; }
        public string Notes { get; set; }
    }
}
