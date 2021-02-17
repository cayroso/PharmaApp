using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services
{
    public class Response
    {
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public double TotalPrice { get; set; }

        public string PharmacyId { get; set; }
        public string PharmacyName { get; set; }

        public string CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string Notes { get; set; }
    }

    public interface IOrderClient
    {
        Task CustomerPlacedOrder(Response response);
        Task CustomerCancelledOrder(Response response);
        Task CustomerSetOrderToArchived(Response response);

        Task PharmacyAcceptedOrder(Response response);
        Task PharmacySetOrderReadyForPickup(Response response);
        Task PharmacySetOrderToCompleted(Response response);
        Task PharmacySetOrderToArchived(Response response);

    }

    [Authorize]
    public class OrderHub : Hub<IOrderClient>
    {

    }
}
