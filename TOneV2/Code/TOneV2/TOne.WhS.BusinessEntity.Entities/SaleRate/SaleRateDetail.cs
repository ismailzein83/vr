using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleRateDetail
    {
        public SaleRate Entity { get; set; }
        public string ZoneName { get; set; }
        public string CurrencyName { get; set; }
        public string RateTypeName { get; set; }
        public decimal ConvertedRate { get; set; }
    }
}
