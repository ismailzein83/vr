using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class OwnerInfo
    {
        public string AssignedToSellingProductName { get; set; }

        public string CurrentDefaultRoutingProductName { get; set; }

        public bool IsCurrentDefaultRoutingProductInherited { get; set; }

        public string NewDefaultRoutingProductName { get; set; }

        public string ResetToDefaultRoutingProductName { get; set; }
        public string AssignedToSellingProductCurrencySymbol { get; set; }
    }
}
