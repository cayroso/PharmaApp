using App.CQRS;
using App.CQRS.Drugs.Common.Queries.Query;
using App.CQRS.Pharmacy.Common.Commands.Command;
using App.CQRS.Pharmacy.Common.Queries.Query;
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
    public class PharmacyController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        readonly ICommandHandlerDispatcher _commandHandlerDispatcher;
        readonly AppDbContext _appDbContext;
        readonly IHubContext<TripHub, ITripClient> _tripHubContext;

        public PharmacyController(
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

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            var query = new GetPharmacyByIdQuery("", TenantId, UserId, PharmacyId);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetPharmacyByIdQuery, GetPharmacyByIdQuery.Pharmacy>(query);

            return Ok(dto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Get(string c, int p, int s, string sf, int so)
        {
            var query = new SearchPharmacyQuery("", TenantId, UserId, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchPharmacyQuery, Paged<SearchPharmacyQuery.Pharmacy>>(query);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var query = new GetPharmacyByIdQuery("", TenantId, UserId, id);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetPharmacyByIdQuery, GetPharmacyByIdQuery.Pharmacy>(query);

            return Ok(dto);
        }

        [HttpPut()]
        public async Task<IActionResult> Put([FromBody] EditPharmacyInfo info)
        {
            var cmd = new EditPharmacyCommand("", TenantId, UserId, info.PharmacyId, info.Token, info.Name, info.PharmacyStatus, info.OpeningHours, info.Address, info.GeoX, info.GeoY);

            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok();
        }
    }
}
