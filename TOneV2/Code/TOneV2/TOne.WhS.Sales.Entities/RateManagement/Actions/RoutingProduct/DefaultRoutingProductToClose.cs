using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class DefaultRoutingProductToClose
    {
        public DateTime CloseEffectiveDate { get; set; }

        List<ExistingDefaultRoutingProduct> _changedExistingDefaultRoutingProducts = new List<ExistingDefaultRoutingProduct>();

        public List<ExistingDefaultRoutingProduct> ChangedExistingDefaultRoutingProducts
        {
            get
            {
                return _changedExistingDefaultRoutingProducts;
            }
        }
    }
}
