using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class CustomerSettings
    {
        public int ConfigId { get; set; }

        public SupplierPriceListConnectorBase PriceListConnector { get; set; }
    }
}
