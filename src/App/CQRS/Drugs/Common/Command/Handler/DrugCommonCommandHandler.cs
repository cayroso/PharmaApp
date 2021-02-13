using App.CQRS.Drugs.Common.Command.Command;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Drugs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Drugs.Common.Command.Handler
{
    public sealed class DrugCommonCommandHandler :
        ICommandHandler<AddDrugCommand>,
        ICommandHandler<DeleteDrugCommand>,
        ICommandHandler<EditDrugCommand>
    {
        readonly AppDbContext _appDbContext;
        readonly ISequentialGuidGenerator _sequentialGuidGenerator;
        public DrugCommonCommandHandler(AppDbContext appDbContext, ISequentialGuidGenerator sequentialGuidGenerator)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _sequentialGuidGenerator = sequentialGuidGenerator ?? throw new ArgumentNullException(nameof(sequentialGuidGenerator));
        }

        async Task ICommandHandler<AddDrugCommand>.HandleAsync(AddDrugCommand command)
        {
            var drug = new Drug
            {
                DrugId = command.DrugId,
                PharmacyId = command.PharmacyId,
                BrandId = command.BrandId,
                Classification = command.Classification,
                Name = command.Name,
                Stock = command.Stock,
                IsAvailable = command.Stock > 0,
                SafetyStock = command.SafetyStock,
                ReorderLevel = command.ReorderLevel
            };

            drug.Prices.Add(new DrugPrice
            {
                DrugId = command.DrugId,
                Price = command.Price,
            });

            await _appDbContext.AddAsync(drug);

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<DeleteDrugCommand>.HandleAsync(DeleteDrugCommand command)
        {
            var drug = await _appDbContext.Drugs.FirstOrDefaultAsync(e => e.DrugId == command.DrugId);

            drug.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            _appDbContext.Remove(drug);

            await _appDbContext.SaveChangesAsync();
        }

        async Task ICommandHandler<EditDrugCommand>.HandleAsync(EditDrugCommand command)
        {
            var drug = await _appDbContext.Drugs.Include(e => e.Prices).FirstOrDefaultAsync(e => e.DrugId == command.DrugId);

            drug.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            drug.BrandId = command.BrandId;
            drug.Classification = command.Classification;
            drug.Name = command.Name;
            drug.Stock = command.Stock;
            drug.SafetyStock = command.SafetyStock;
            drug.ReorderLevel = command.ReorderLevel;

            var currPrice = drug.Prices.First();

            if (currPrice.Price != command.Price)
            {
                currPrice.Active = false;

                drug.Prices.Add(new DrugPrice
                {
                    DrugId = command.DrugId,
                    Price = command.Price,
                });
            }

            await _appDbContext.SaveChangesAsync();
        }
    }
}
