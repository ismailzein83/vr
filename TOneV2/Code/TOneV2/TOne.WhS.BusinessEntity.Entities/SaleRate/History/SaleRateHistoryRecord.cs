using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleRateHistoryRecord : Vanrise.Entities.IDateEffectiveSettingsEditable
    {
        public decimal Rate { get; set; }

        public decimal ConvertedRate { get; set; }

        public RateChangeType ChangeType { get; set; }

        public int CurrencyId { get; set; }

        public int? SellingProductId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class SaleRateHistoryRecordDetail
    {
        public SaleRateHistoryRecord Entity { get; set; }

        public string ConvertedToCurrencySymbol { get; set; }

        public string SellingProductName { get; set; }
    }
}
