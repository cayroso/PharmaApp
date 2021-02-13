using App.CQRS;
using App.CQRS.Drugs.Common.Command.Command;
using App.CQRS.Drugs.Common.Queries.Query;
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
    public class DrugsController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        readonly ICommandHandlerDispatcher _commandHandlerDispatcher;
        readonly AppDbContext _appDbContext;
        readonly IHubContext<TripHub, ITripClient> _tripHubContext;

        public DrugsController(
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
        public async Task<IActionResult> Get(string c, int p, int s, string sf, int so)
        {
            var query = new SearchDrugByPharmacyQuery("", TenantId, UserId, PharmacyId, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchDrugByPharmacyQuery, Paged<SearchDrugByPharmacyQuery.Drug>>(query);

            return Ok(dto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Get(string pharmacyId, string c, int p, int s, string sf, int so)
        {
            var query = new SearchDrugByPharmacyQuery("", TenantId, UserId, pharmacyId, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchDrugByPharmacyQuery, Paged<SearchDrugByPharmacyQuery.Drug>>(query);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var query = new GetDrugByIdQuery("", TenantId, UserId, id);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetDrugByIdQuery, GetDrugByIdQuery.Drug>(query);

            return Ok(dto);
        }

        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] AddDrugInfo info)
        {
            var cmd = new AddDrugCommand("", TenantId, UserId, PharmacyId, GuidStr(), info.BrandId, info.Classification, info.Name, info.Price,
                info.Stock, info.SafetyStock, info.ReorderLevel);

            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok(cmd.DrugId);
        }

        [HttpPut()]
        public async Task<IActionResult> Put([FromBody] EditDrugInfo info)
        {
            var cmd = new EditDrugCommand("", TenantId, UserId, info.DrugId, info.Token, info.BrandId, info.Classification, info.Name, info.Price,
                info.Stock, info.SafetyStock, info.ReorderLevel);

            await _commandHandlerDispatcher.HandleAsync(cmd);

            return Ok(cmd.DrugId);
        }


    }
}
