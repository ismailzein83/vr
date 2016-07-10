using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class DefaultRoutingProductToAdd : Vanrise.Entities.IDateEffectiveSettings
    {
        public NewDefaultRoutingProduct NewDefaultRoutingProduct { get; set; }

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
