using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePriceListCustomerChange
    {
        public long BatchId { get; set; }
        public int PriceListId { get; set; }
        public int CountryId { get; set; }
    }
}
