using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public enum OrderDirection { Ascending = 0, Descending = 1 }
    public interface IRouteOptionOrderTarget
    {
        Decimal SupplierRate { get; }

        Decimal OptionWeight { get; set; }

        int SupplierId { get; set; }

        long? SaleZoneId { get; }

        long? SupplierZoneId { get; }

        int SupplierServiceWeight { get; }
    }

    public interface IRouteOptionOrderExecutionContext
    {
        OrderDirection OrderDitection { get; set; }
        IEnumerable<IRouteOptionOrderTarget> Options { get; set; }
        RoutingDatabase RoutingDatabase { get; set; }
    }
}
