using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class SaleZoneRoutingProductToClose
    {
        public DateTime CloseEffectiveDate { get; set; }

        List<ExistingSaleZoneRoutingProduct> _changedExistingSaleZoneRoutingProducts = new List<ExistingSaleZoneRoutingProduct>();

        public List<ExistingSaleZoneRoutingProduct> ChangedExistingSaleZoneRoutingProducts
        {
            get
            {
                return _changedExistingSaleZoneRoutingProducts;
            }
        }
    }
}
