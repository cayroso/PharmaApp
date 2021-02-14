using App.CQRS.Pharmacy.Common.Commands.Command;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Pharmacies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Pharmacy.Common.Commands.Handler
{
    public sealed class PharmacyCommonCommandHandler:
        ICommandHandler<EditPharmacyCommand>
    {
        readonly AppDbContext _appDbContext;
        readonly ISequentialGuidGenerator _sequentialGuidGenerator;
        public PharmacyCommonCommandHandler(AppDbContext appDbContext, ISequentialGuidGenerator sequentialGuidGenerator)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _sequentialGuidGenerator = sequentialGuidGenerator ?? throw new ArgumentNullException(nameof(sequentialGuidGenerator));
        }

        async Task ICommandHandler<EditPharmacyCommand>.HandleAsync(EditPharmacyCommand command)
        {
            var pharmacy = await _appDbContext.Pharmacies.FirstOrDefaultAsync(e=> e.PharmacyId == command.PharmacyId);

            pharmacy.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            pharmacy.Name = command.Name;
            pharmacy.PhoneNumber = command.PhoneNumber;
            pharmacy.MobileNumber = command.MobileNumber;
            pharmacy.Email = command.Email;
            pharmacy.PharmacyStatus = command.PharmacyStatus;
            pharmacy.OpeningHours = command.OpeningHours;
            pharmacy.Address = command.Address;
            pharmacy.GeoX = command.GeoX;
            pharmacy.GeoY = command.GeoY;

            await _appDbContext.SaveChangesAsync();
        }
    }
}
