using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class SaleZoneRoutingProductToAdd
    {
        NewSaleZoneRoutingProduct _newSaleZoneRoutingProduct = new NewSaleZoneRoutingProduct();
        public NewSaleZoneRoutingProduct NewSaleZoneRoutingProduct
        {
            get
            {
                return _newSaleZoneRoutingProduct;
            }
        }

        List<ExistingSaleZoneRoutingProduct> _changedExistingSaleZoneRoutingProducts = new List<ExistingSaleZoneRoutingProduct>();

        public List<ExistingSaleZoneRoutingProduct> ChangedExistingSaleZoneRoutingProducts
        {
            get
            {
                return _changedExistingSaleZoneRoutingProducts;
            }
        }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
