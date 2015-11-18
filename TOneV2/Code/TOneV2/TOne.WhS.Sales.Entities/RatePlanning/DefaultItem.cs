using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class DefaultItem
    {
        public bool IsCurrentDefaultRoutingProductEditable { get; set; }

        public int CurrentDefaultRoutingProductId { get; set; }

        public int? NewDefaultRoutingProductId { get; set; }

        public DateTime CurrentBED { get; set; }

        public DateTime? CurrentEED { get; set; }
    }
}
