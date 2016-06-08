using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class DefaultRoutingProductToAdd
    {
        NewDefaultRoutingProduct _newDefaultRoutingProduct = new NewDefaultRoutingProduct();
        public NewDefaultRoutingProduct NewDefaultRoutingProduct
        {
            get
            {
                return _newDefaultRoutingProduct;
            }
        }

        List<ExistingDefaultRoutingProduct> _changedExistingDefaultRoutingProducts = new List<ExistingDefaultRoutingProduct>();

        public List<ExistingDefaultRoutingProduct> ChangedExistingDefaultRoutingProducts
        {
            get
            {
                return _changedExistingDefaultRoutingProducts;
            }
        }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
