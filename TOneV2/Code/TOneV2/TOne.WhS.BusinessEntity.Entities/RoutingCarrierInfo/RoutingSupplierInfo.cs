using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RoutingSupplierInfo : RoutingCarrierInfo
    {
        public int SupplierId { get; set; }

        public override int CarrierInfoId { get { return SupplierId; } }

        public override string Title { get { return "Supplier"; } }
    }
}