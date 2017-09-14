using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleRateHistoryQuery
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public int? SellingNumberPlanId { get; set; }

        public string ZoneName { get; set; }

        public int CountryId { get; set; }

        public int CurrencyId { get; set; }

        public bool IsSystemCurrency { get; set; }
    }
}
