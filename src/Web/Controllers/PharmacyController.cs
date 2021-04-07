using App.CQRS.Pharmacy.Common.Commands.Command;
using App.CQRS.Pharmacy.Common.Queries.Query;
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
    public class PharmacyController : BaseController
    {
        readonly IQueryHandlerDispatcher _queryHandlerDispatcher;
        readonly ICommandHandlerDispatcher _commandHandlerDispatcher;
        readonly AppDbContext _appDbContext;

        public PharmacyController(
            IQueryHandlerDispatcher queryHandlerDispatcher,
            ICommandHandlerDispatcher commandHandlerDispatcher,
            AppDbContext appDbContext)
        {
            _queryHandlerDispatcher = queryHandlerDispatcher ?? throw new ArgumentNullException(nameof(queryHandlerDispatcher));
            _commandHandlerDispatcher = commandHandlerDispatcher ?? throw new ArgumentNullException(nameof(commandHandlerDispatcher));
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var query = new GetPharmacyByIdQuery("", TenantId, UserId, PharmacyId);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetPharmacyByIdQuery, GetPharmacyByIdQuery.Pharmacy>(query, cancellationToken);

            return Ok(dto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Get(string c, int p, int s, string sf, int so, CancellationToken cancellationToken = default)
        {
            var query = new SearchPharmacyQuery("", TenantId, UserId, c, p, s, sf, so);

            var dto = await _queryHandlerDispatcher.HandleAsync<SearchPharmacyQuery, Paged<SearchPharmacyQuery.Pharmacy>>(query, cancellationToken);

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
        {
            var query = new GetPharmacyByIdQuery("", TenantId, UserId, id);

            var dto = await _queryHandlerDispatcher.HandleAsync<GetPharmacyByIdQuery, GetPharmacyByIdQuery.Pharmacy>(query, cancellationToken);

            return Ok(dto);
        }

        [HttpPut()]
        public async Task<IActionResult> Put([FromBody] EditPharmacyInfo info, CancellationToken cancellationToken = default)
        {
            var cmd = new EditPharmacyCommand("", TenantId, UserId, info.PharmacyId, info.Token, info.Name, info.PhoneNumber, info.MobileNumber, info.Email,
                info.PharmacyStatus, info.OpeningHours, info.Address, info.GeoX, info.GeoY);

            await _commandHandlerDispatcher.HandleAsync(cmd, cancellationToken);

            return Ok();
        }
    }
}
