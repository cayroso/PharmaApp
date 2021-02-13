using App.CQRS.Drugs.Common.Queries.Query;
using Data.App.DbContext;
using Data.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Drugs.Common.Queries.Handler
{
    public sealed class DrugCommonQueryHandler :
        IQueryHandler<GetDrugByIdQuery, GetDrugByIdQuery.Drug>,
        IQueryHandler<SearchDrugByPharmacyQuery, Paged<SearchDrugByPharmacyQuery.Drug>>
    {
        readonly AppDbContext _appDbContext;
        public DrugCommonQueryHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        async Task<GetDrugByIdQuery.Drug> IQueryHandler<GetDrugByIdQuery, GetDrugByIdQuery.Drug>.HandleAsync(GetDrugByIdQuery query)
        {
            var sql = from d in _appDbContext.Drugs.AsNoTracking()

                      where d.DrugId == query.DrugId

                      select new GetDrugByIdQuery.Drug
                      {
                          DrugId = d.DrugId,
                          Name = d.Name,
                          Classification = d.Classification,
                          Brand = d.Brand.Name,
                          IsAvailable = d.IsAvailable,
                          Stock = d.Stock,
                          Pharmacy = new GetDrugByIdQuery.Pharmacy
                          {
                              PharmacyId = d.Pharmacy.PharmacyId,
                              Name = d.Pharmacy.Name,
                              Address = d.Pharmacy.Address,
                              OpeningHours = d.Pharmacy.OpeningHours,
                              GeoX = d.Pharmacy.GeoX,
                              GeoY = d.Pharmacy.GeoY,
                          },
                          Price = new GetDrugByIdQuery.DrupPrice
                          {
                              Price = d.Prices.First().Price,
                              SalePrice = d.Prices.First().SalePrice,
                          },
                      };

            var dto = await sql.FirstOrDefaultAsync();

            return dto;
        }

        async Task<Paged<SearchDrugByPharmacyQuery.Drug>> IQueryHandler<SearchDrugByPharmacyQuery, Paged<SearchDrugByPharmacyQuery.Drug>>.HandleAsync(SearchDrugByPharmacyQuery query)
        {
            var sql = from d in _appDbContext.Drugs.AsNoTracking()

                      where string.IsNullOrWhiteSpace(query.PharmacyId) || d.PharmacyId == query.PharmacyId

                      where string.IsNullOrWhiteSpace(query.Criteria)
                        || EF.Functions.Like(d.Name, $"%{query.Criteria}%")
                        || EF.Functions.Like(d.Brand.Name, $"%{query.Criteria}%")
                        || EF.Functions.Like(d.Pharmacy.Name, $"%{query.Criteria}%")

                      select new SearchDrugByPharmacyQuery.Drug
                      {
                          DrugId = d.DrugId,
                          Name = d.Name,
                          Classification = d.Classification,
                          Brand = d.Brand.Name,
                          IsAvailable = d.IsAvailable,

                          Pharmacy = new SearchDrugByPharmacyQuery.Pharmacy
                          {
                              PharmacyId = d.Pharmacy.PharmacyId,
                              Name = d.Pharmacy.Name,
                              Address = d.Pharmacy.Address
                          },
                          Price = new SearchDrugByPharmacyQuery.DrupPrice
                          {
                              Price = d.Prices.First().Price,
                              SalePrice = d.Prices.First().SalePrice
                          },
                      };

            var dto = await sql.ToPagedItemsAsync(query.PageIndex, query.PageSize);

            return dto;
        }
    }
}
