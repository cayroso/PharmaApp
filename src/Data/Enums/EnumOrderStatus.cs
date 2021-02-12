using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Enums
{
    public enum EnumOrderStatus
    {
        Unknown = 0,
        Placed,
        Accepted,        
        ReadyForPickup,
        Completed,
        Cancelled,
        Archived
    }
}
