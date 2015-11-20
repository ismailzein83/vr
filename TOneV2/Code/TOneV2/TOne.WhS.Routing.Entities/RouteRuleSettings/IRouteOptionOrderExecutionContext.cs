using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteOptionOrderTarget
    {
        Decimal SupplierRate { get; }
    }

    public interface IRouteOptionOrderExecutionContext
    {
        IEnumerable<IRouteOptionOrderTarget> Options { get; set; }
    }
}
