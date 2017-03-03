using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerZoneRateHistoryRecord : Vanrise.Entities.IDateEffectiveSettingsEditable
    {
        public decimal Rate { get; set; }

        public int? SellingProductId { get; set; }

        public int CurrencyId { get; set; }

        public RateChangeType ChangeType { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class CustomerZoneRateHistoryRecordDetail
    {
        public CustomerZoneRateHistoryRecord Entity { get; set; }

        public string SellingProductName { get; set; }

        public string CurrencySymbol { get; set; }
    }
}
