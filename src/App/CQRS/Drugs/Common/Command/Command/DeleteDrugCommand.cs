using Cayent.Core.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Drugs.Common.Command.Command
{
    public sealed class DeleteDrugCommand : AbstractCommand
    {
        public string DrugId { get; }
        public string Token { get; }

        public DeleteDrugCommand(string correlationId, string tenantId, string userId,
            string drugId, string token)
            : base(correlationId, tenantId, userId)
        {
            DrugId = drugId;
            Token = token;
        }
    }
}
