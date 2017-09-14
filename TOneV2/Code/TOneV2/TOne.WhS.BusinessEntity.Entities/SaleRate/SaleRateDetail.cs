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

        public int CountryId { get; set; }

        public string RateTypeName { get; set; }

        public string DisplayedCurrency { get; set; }

        public decimal DisplayedRate { get; set; }

        public bool IsRateInherited { get; set; }

        public long? PriceListFileId { get; set; }
    }
}
