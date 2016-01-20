using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class SellingRuleCriteria
    {
        public SaleZoneGroupSettings SaleZoneGroupSettings { get; set; }
        public CustomerGroupSettings CustomerGroupSettings { get; set; }
        public int? SellingProductId { get; set; }
    }
}
