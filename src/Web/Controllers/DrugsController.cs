using App.CQRS.Drugs.Common.Command.Command;
using App.CQRS.Drugs.Common.Queries.Query;
using Cayent.Core.Common;
using Cayent.Core.CQRS.Commands;
using Cayent.Core.CQRS.Queries;
using Data.App.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
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

        public DrugsController(
            IQueryHandlerDispatcher queryHandlerDispatcher,
            ICommandHandlerDispatcher commandHandlerDispatcher,
            AppDbContext appDbContext)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
            _commandHandlerDispatcher = commandHandlerDispatcher ?? throw new ArgumentNullException(nameof(commandHandlerDispatcher));
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        [HttpGet()]
        public async Task<IActionResult> Get(string c, int p, int s, string sf, int so, CancellationToken cancellationToken = default)
        {
            var query = new SearchDrugByPharmacyQuery("", TenantId, UserId, PharmacyId, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchDrugByPharmacyQuery, Paged<SearchDrugByPharmacyQuery.Drug>>(query, cancellationToken);

            return Ok(dto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Get(string pharmacyId, string c, int p, int s, string sf, int so, CancellationToken cancellationToken = default)
        {
            var query = new SearchDrugByPharmacyQuery("", TenantId, UserId, pharmacyId, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchDrugByPharmacyQuery, Paged<SearchDrugByPharmacyQuery.Drug>>(query, cancellationToken);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
        {
            var query = new GetDrugByIdQuery("", TenantId, UserId, id);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetDrugByIdQuery, GetDrugByIdQuery.Drug>(query, cancellationToken);

            return Ok(dto);
        }

        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] AddDrugInfo info, CancellationToken cancellationToken = default)
        {
            var cmd = new AddDrugCommand("", TenantId, UserId, PharmacyId, GuidStr(), info.BrandId, info.Classification, info.Name, info.Price,
                info.Stock, info.SafetyStock, info.ReorderLevel);

            await _commandHandlerDispatcher.HandleAsync(cmd, cancellationToken);

            return Ok(cmd.DrugId);
        }

        [HttpPut()]
        public async Task<IActionResult> Put([FromBody] EditDrugInfo info, CancellationToken cancellationToken = default)
        {
            var cmd = new EditDrugCommand("", TenantId, UserId, info.DrugId, info.Token, info.BrandId, info.Classification, info.Name, info.Price,
                info.Stock, info.SafetyStock, info.ReorderLevel);

            await _commandHandlerDispatcher.HandleAsync(cmd, cancellationToken);

            return Ok(cmd.DrugId);
        }


    }
}
