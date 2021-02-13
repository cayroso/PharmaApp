﻿using App.CQRS.Pharmacy.Common.Queries.Query;
using Data.App.DbContext;
using Data.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Pharmacy.Common.Queries.Handler
{
    public sealed class PharmacyCommonQueryHandler :
        IQueryHandler<GetPharmacyByIdQuery, GetPharmacyByIdQuery.Pharmacy>,
        IQueryHandler<SearchPharmacyQuery, Paged<SearchPharmacyQuery.Pharmacy>>
    {
        readonly AppDbContext _appDbContext;
        public PharmacyCommonQueryHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        async Task<GetPharmacyByIdQuery.Pharmacy> IQueryHandler<GetPharmacyByIdQuery, GetPharmacyByIdQuery.Pharmacy>.HandleAsync(GetPharmacyByIdQuery query)
        {
            var sql = from p in _appDbContext.Pharmacies.AsNoTracking()

                      where p.PharmacyId == query.PharmacyId

                      select new GetPharmacyByIdQuery.Pharmacy
                      {
                          PharmacyId = p.PharmacyId,
                          Address = p.Address,
                          GeoX = p.GeoX,
                          GeoY = p.GeoY,
                          Name = p.Name,
                          Token = p.ConcurrencyToken,
                          OpeningHours = p.OpeningHours,
                          PharmacyStatus = p.PharmacyStatus,
                      };

            var dto = await sql.FirstOrDefaultAsync();

            return dto;
        }

        async Task<Paged<SearchPharmacyQuery.Pharmacy>> IQueryHandler<SearchPharmacyQuery, Paged<SearchPharmacyQuery.Pharmacy>>.HandleAsync(SearchPharmacyQuery query)
        {
            var sql = from p in _appDbContext.Pharmacies.AsNoTracking()

                      select new SearchPharmacyQuery.Pharmacy
                      {
                          PharmacyId = p.PharmacyId,
                          Address = p.Address,
                          GeoX = p.GeoX,
                          GeoY = p.GeoY,
                          Name = p.Name,
                          Token = p.ConcurrencyToken,
                          OpeningHours = p.OpeningHours,
                          PharmacyStatus = p.PharmacyStatus,
                      };

            var dto = await sql.ToPagedItemsAsync(query.PageIndex, query.PageSize);

            return dto;

        }
    }
}
