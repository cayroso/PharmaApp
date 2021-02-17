using App.CQRS.Staffs.Common.Commands.Command;
using App.Services;
using Data.App.DbContext;
using Data.Identity.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Staffs.Common.Commands.Handler
{
    public sealed class StaffCommonCommandHandler:
        ICommandHandler<AddStaffCommand>
    {
        readonly AppDbContext _appDbContext;
        readonly IdentityWebContext _identityWebContext;
        readonly ISequentialGuidGenerator _sequentialGuidGenerator;
        public StaffCommonCommandHandler(
            AppDbContext appDbContext,
            IdentityWebContext identityWebContext,
            ISequentialGuidGenerator sequentialGuidGenerator)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _identityWebContext = identityWebContext ?? throw new ArgumentNullException(nameof(identityWebContext));
            _sequentialGuidGenerator = sequentialGuidGenerator ?? throw new ArgumentNullException(nameof(sequentialGuidGenerator));
        }

        async Task ICommandHandler<AddStaffCommand>.HandleAsync(AddStaffCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
