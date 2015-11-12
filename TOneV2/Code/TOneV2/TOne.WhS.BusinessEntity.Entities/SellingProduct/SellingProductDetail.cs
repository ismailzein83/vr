using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SellingProductDetail
    {

        public SellingProduct Entity { get; set; }
        public string SellingNumberPlanName { get; set; }
        public string DefaultRoutingProductName { get; set; }
    }
}
