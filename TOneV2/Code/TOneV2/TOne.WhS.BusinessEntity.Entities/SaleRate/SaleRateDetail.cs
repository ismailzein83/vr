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
        public decimal ConvertedRate { get; set; }
        public List<SaleOtherRateDetail> OtherRates { set; get; }
        public bool IsRateInherited { get; set; }
    }

    public class SaleOtherRateDetail
    {
        public SaleRate Entity { get; set; }
        public string ZoneName { get; set; }
        public string CurrencyName { get; set; }
        public string RateTypeName { get; set; }
        public decimal ConvertedRate { get; set; }
        public bool IsRateInherited { get; set; }
    }
}
