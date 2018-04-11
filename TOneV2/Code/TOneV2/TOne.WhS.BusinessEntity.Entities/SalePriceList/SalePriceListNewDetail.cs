using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{

    public class SalePriceListNewDetail
    {
        public SalePriceListNew Entity { get; set; }
        public string OwnerType { get; set; }
        public string CurrencyName { get; set; }
        public string OwnerName { get; set; }
        public string PriceListTypeName { get; set; }
        public string PricelistSourceName { get; set; }
        public string UserName { get; set; }
    }
}
