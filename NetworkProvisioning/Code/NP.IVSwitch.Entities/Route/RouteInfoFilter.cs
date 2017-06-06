using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class RouteInfoFilter
    {
        public int? AssignableToCarrierAccountId { get; set; }
        public List<int> SupplierIds { get; set; }
    }
}
