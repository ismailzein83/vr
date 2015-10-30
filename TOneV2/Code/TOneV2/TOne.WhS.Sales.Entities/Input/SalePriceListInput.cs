using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities.Queries
{
    public class SalePriceListInput
    {
        public int CustomerId { get; set; }

        public List<SaleRate> NewSaleRates { get; set; }
    }
}
