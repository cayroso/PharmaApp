﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.CQRS.Orders.Common.Queries.Query;
using Data.App.DbContext;
using Data.Common;
using Data.Constants;
using Data.Enums;
using Data.Identity.DbContext;
using Cayent.Core.CQRS.Queries;
using Cayent.Core.Common;
using Cayent.Core.Common.Extensions;
using System.Threading;

namespace App.CQRS.Orders.Common.Queries.Handler
{
    public sealed class CustomerOrderQueryHandler :
        IQueryHandler<GetCustomerOrderByIdQuery, GetCustomerOrderByIdQuery.Order>,
        IQueryHandler<SearchOrderQuery, Paged<SearchOrderQuery.Order>>
    {
        readonly AppDbContext _dbContext;
        public CustomerOrderQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        async Task<GetCustomerOrderByIdQuery.Order> IQueryHandler<GetCustomerOrderByIdQuery, GetCustomerOrderByIdQuery.Order>.HandleAsync(GetCustomerOrderByIdQuery query, CancellationToken cancellationToken)
        {
            var sql = from o in _dbContext.Orders

                                //.Include(e => e.LineItems)
                                //    .ThenInclude(e => e.Drug)
                                //.Include(e => e.LineItems)
                                //    .ThenInclude(e => e.DrugPrice)
                                .AsNoTracking()

                      where o.OrderId == query.OrderId

                      select new GetCustomerOrderByIdQuery.Order
                      {
                          OrderId = o.OrderId,
                          Number = o.Number,
                          GrossPrice = o.GrossPrice,
                          Status = o.OrderStatus,
                          DateOrdered = o.OrderDate,
                          DateStartPickup = o.StartPickupDate,
                          DateEndPickup = o.EndPickupDate,
                          Token = o.ConcurrencyToken,
                          Pharmacy = new GetCustomerOrderByIdQuery.Pharmacy
                          {
                              PharmacyId = o.Pharmacy.PharmacyId,
                              Name = o.Pharmacy.Name,
                              PhoneNumber = o.Pharmacy.PhoneNumber,
                              MobileNumber = o.Pharmacy.MobileNumber,
                              Email = o.Pharmacy.Email,
                              OpeningHours = o.Pharmacy.OpeningHours,
                              Address = o.Pharmacy.Address
                          },
                          Customer = new GetCustomerOrderByIdQuery.Customer
                          {
                              CustomerId = o.Customer.CustomerId,
                              UrlProfilePicture = o.Customer.User.Image.Url,
                              Name = o.Customer.User.FirstLastName,
                              Email = o.Customer.User.Email,
                              PhoneNumber = o.Customer.User.PhoneNumber
                          },
                          Lines = o.LineItems.OrderBy(e => e.LineNumber).Select(e => new GetCustomerOrderByIdQuery.Orderline
                          {
                              LineNumber = e.LineNumber,
                              DrugName = e.Drug.Name,
                              Classification = e.Drug.Classification,
                              DrugPrice = e.Drug.Prices.First().Price,
                              Quantity = e.Quantity,
                              ExtendedPrice = e.ExtendedPrice
                          }),
                          FileUploadUrls = o.FileUploads.Select(e => e.FileUpload.Url),
                          Timelines = o.Timelines.OrderByDescending(e => e.DateTimeline).Select(e => new GetCustomerOrderByIdQuery.OrderTimeline
                          {
                              DateTimeline = e.DateTimeline,
                              Notes = e.Notes,
                              Status = e.Status,
                              User = e.User.FirstLastName
                          })
                      };

            var dto = await sql.FirstOrDefaultAsync();

            return dto;
        }

        async Task<Paged<SearchOrderQuery.Order>> IQueryHandler<SearchOrderQuery, Paged<SearchOrderQuery.Order>>.HandleAsync(SearchOrderQuery query, CancellationToken cancellationToken)
        {
            var sql = from o in _dbContext.Orders

                                //.Include(e => e.LineItems)
                                //    .ThenInclude(e => e.Drug)
                                //.Include(e => e.LineItems)
                                //    .ThenInclude(e => e.DrugPrice)
                                .AsNoTracking()

                      where string.IsNullOrWhiteSpace(query.CustomerId) || string.IsNullOrWhiteSpace(query.PharmacyId)
                             || o.CustomerId == query.CustomerId || o.PharmacyId == query.PharmacyId
                      where query.OrderStatus == EnumOrderStatus.Unknown || o.OrderStatus == query.OrderStatus
                      where string.IsNullOrWhiteSpace(query.Criteria)
                        || EF.Functions.Like(o.Number, $"%{query.Criteria}%")

                      select new SearchOrderQuery.Order
                      {
                          OrderId = o.OrderId,
                          Number = o.Number,
                          GrossPrice = o.GrossPrice,
                          Status = o.OrderStatus,
                          DateOrdered = o.OrderDate,
                          DateStartPickup = o.StartPickupDate,
                          DateEndPickup = o.EndPickupDate,
                          NumberOfItems = o.LineItems.Count(),

                          Pharmacy = new SearchOrderQuery.Pharmacy
                          {
                              PharmacyId = o.Pharmacy.PharmacyId,
                              Name = o.Pharmacy.Name,
                              PhoneNumber = o.Pharmacy.PhoneNumber,
                              MobileNumber = o.Pharmacy.MobileNumber,
                              Email = o.Pharmacy.Email,
                              OpeningHours = o.Pharmacy.OpeningHours,
                              Address = o.Pharmacy.Address
                          },
                          Customer = new SearchOrderQuery.Customer
                          {
                              CustomerId = o.Customer.CustomerId,
                              UrlProfilePicture = o.Customer.User.Image.Url,
                              Name = o.Customer.User.FirstLastName,
                              Email = o.Customer.User.Email,
                              PhoneNumber = o.Customer.User.PhoneNumber
                          }
                      };

            var dto = await sql.ToPagedItemsAsync(query.PageIndex, query.PageSize, cancellationToken);

            return dto;
        }

        //async Task<GetCustomerOrderByIdQuery.CustomerOrder> IQueryHandler<GetCustomerOrderByIdQuery, GetCustomerOrderByIdQuery.CustomerOrder>.HandleAsync(GetCustomerOrderByIdQuery query)
        //{
        //    //var includeAllTasks = query.RoleId == ApplicationRoles.Manager.Id || query.RoleId == ApplicationRoles.Assistant.Id;

        //    var sql = from o in _dbContext.Orders
        //                        .IgnoreQueryFilters()
        //                        .AsNoTracking()

        //              where o.OrderId == query.OrderId

        //              select new GetCustomerOrderByIdQuery.CustomerOrder
        //              {
        //                  CustomerOrderId = o.OrderId,
        //                  Number = o.Number,

        //                  Customer = o.DeliveryAddress.RecipientName,
        //                  PhoneNumber = o.DeliveryAddress.PhoneNumber,
        //                  Address = o.DeliveryAddress.Address,


        //                  OrderDateTime = o.OrderDateTime,
        //                  DeliveryDateTime = o.DeliveryDateTime,
        //                  ExpectedMinDeliveryDateTime = o.ExpectedMinDeliveryDateTime,
        //                  ExpectedMaxDeliveryDateTime = o.ExpectedMaxDeliveryDateTime,
        //                  OrderStatus = o.OrderStatus,
        //                  PaymentMethod = o.PaymentMethod,
        //                  ShippingSetting = new GetCustomerOrderByIdQuery.ShippingSetting
        //                  {
        //                      Name = o.ShippingSetting.Name,
        //                      MinDelayInHours = o.ShippingSetting.MinDelayInHours,
        //                      MaxDelayInHours = o.ShippingSetting.MaxDelayInHours,
        //                      FlatRate = o.ShippingSetting.FlatRate,
        //                      PricePercentage = o.ShippingSetting.PricePercentage,
        //                      MinimumOrderValue = o.ShippingSetting.MinimumOrderValue,
        //                      ShipmentType = o.ShippingSetting.ShipmentType
        //                  },

        //                  GrossPrice = o.GrossPrice,
        //                  DeliveryFee = o.DeliveryFee,
        //                  NetPrice = o.NetPrice,
        //                  AmountPaid = o.AmountPaid,
        //                  //DeliveryOption = new GetOrderByIdQuery.DeliveryOption
        //                  //{
        //                  //    Name = o.DeliveryOption.Name,
        //                  //    Amount = o.DeliveryOption.Amount,
        //                  //    MinDelayInHours = o.DeliveryOption.MinDelayInHours,
        //                  //    MaxDelayInHours = o.DeliveryOption.MaxDelayInHours,
        //                  //    Notes = o.DeliveryOption.Notes
        //                  //},
        //                  Lines = o.LineItems.OrderBy(e => e.LineNumber).Select(e => new GetCustomerOrderByIdQuery.Orderline
        //                  {
        //                      ExtendedPrice = e.ExtendedPrice,
        //                      LineNumber = e.LineNumber,
        //                      ProductName = e.Product.Name,
        //                      ProductPrice = e.Product.Prices.First().Price,
        //                      Quantity = e.QuantityOrdered,
        //                      Bottles = e.Bottles.Select(b => new GetCustomerOrderByIdQuery.Bottle
        //                      {
        //                          BottleId = b.BottleId,
        //                          Name = b.Bottle.Name,
        //                          TrackingNumber = b.Bottle.TrackingNumber,
        //                          IsReturned = b.IsReturned
        //                      }).ToList()
        //                  }).ToList(),
        //                  OrderNotes = o.OrderNotes.OrderBy(e => e.DateCreated).Select(e => new GetCustomerOrderByIdQuery.OrderNote
        //                  {
        //                      Note = e.Note,
        //                      DateCreated = e.DateCreated,
        //                      SystemGenerated = e.SystemGenerated,
        //                      User = e.User.FirstLastName
        //                  }).ToList(),
        //                  OrderPayments = o.OrderPayments.OrderBy(e => e.DateCreated).Select(e => new GetCustomerOrderByIdQuery.OrderPayment
        //                  {
        //                      DateCreated = e.DateCreated,
        //                      AmountDue = e.AmountDue,
        //                      AmountPaid = e.AmountPaid,
        //                      Note = e.Note,
        //                      User = e.User.FirstLastName
        //                  }).ToList(),
        //                  Histories = o.StatusHistories.OrderBy(e => e.HistoryDateTime).Select(e => new GetCustomerOrderByIdQuery.History
        //                  {
        //                      HistoryDateTime = e.HistoryDateTime,
        //                      OrderStatus = e.OrderStatus.ToString(),
        //                      User = e.User.FirstLastName,
        //                      Note = e.Note
        //                  }).ToList(),
        //                  //UserTasks = o.UserTasks.Where(e => e.RoleId == query.RoleId || includeAllTasks)
        //                  //                      .OrderBy(e => e.Title).Select(e => new GetOrderByIdQuery.UserTask
        //                  //                      {
        //                  //                          UserTaskId = e.UserTaskId,
        //                  //                          RoleId = e.RoleId,
        //                  //                          UserId = e.UserId,
        //                  //                          UserFirstLastName = string.IsNullOrWhiteSpace(e.UserId) ? string.Empty : e.User.FirstLastName,
        //                  //                          Title = e.Title,
        //                  //                          Status = e.Status,
        //                  //                          DateAssigned = e.DateAssigned,
        //                  //                          DateCompleted = e.DateCompleted,
        //                  //                          Token = e.ConcurrencyToken,
        //                  //                      }).ToList(),
        //                  Token = o.ConcurrencyToken
        //              };

        //    var dto = await sql.FirstOrDefaultAsync();

        //    return dto;
        //}


        //async Task<Paged<SearchOrderQuery.Order>> IQueryHandler<SearchOrderQuery, Paged<SearchOrderQuery.Order>>.HandleAsync(SearchOrderQuery query)
        //{
        //    var sql = from o in _dbContext.Orders.AsNoTracking()

        //              where query.Status == EnumOrderStatus.Unknown || o.OrderStatus == query.Status
        //              where o.OrderDateTime >= query.DateStart && o.OrderDateTime <= query.DateEnd

        //              where string.IsNullOrWhiteSpace(query.Criteria)
        //                    || EF.Functions.Like(o.Number, $"%{query.Criteria}%")
        //              //|| EF.Functions.Like(o.FirstName, $"%{query.Criteria}%")
        //              //|| EF.Functions.Like(o.LastName, $"%{query.Criteria}%")
        //              //|| EF.Functions.Like(o.PhoneNumber, $"%{query.Criteria}%")
        //              //|| EF.Functions.Like(o.Address, $"%{query.Criteria}%")


        //              orderby o.OrderDateTime

        //              select new SearchOrderQuery.Order
        //              {
        //                  CustomerOrderId = o.OrderId,
        //                  Number = o.Number,
        //                  ShipmentType = o.ShippingSetting.ShipmentType,
        //                  NetPrice = o.NetPrice,
        //                  AmountPaid = o.AmountPaid,
        //                  OrderStatus = o.OrderStatus,
        //                  PaymentMethod = o.PaymentMethod,
        //                  OrderDateTime = o.OrderDateTime,
        //                  ExpectedMinDeliveryDateTime = o.ExpectedMinDeliveryDateTime,
        //                  ExpectedMaxDeliveryDateTime = o.ExpectedMaxDeliveryDateTime,

        //                  RecipientName = o.DeliveryAddress.RecipientName,
        //                  Phone = o.DeliveryAddress.PhoneNumber,
        //                  Address = o.DeliveryAddress.Address
        //              };

        //    var pagedItems = await sql.ToPagedItemsAsync(query.PageIndex, query.PageSize);

        //    return pagedItems;
        //}

    }
}
