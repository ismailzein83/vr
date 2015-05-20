using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.Analytics.Entities
{
    public class CarrierRateView : IFlaggedServiceEntity
    {
        public int ZoneID { get; set; }
        public string CodeGroup { get; set; }
        public string ZoneName { get; set; }
        public string Code { get; set; }
        public int RateID { get; set; }
        public decimal? Rate { get; set; }
        public string CurrencyID { get; set; }
        public decimal? OffPeakRate { get; set; }
        public decimal? WeekendRate { get; set; }
        public int ChangeID { get; set; }
        public string Change { get; set; }
        public DateTime? RateBeginEffectiveDate { get; set; }
        public DateTime? RateEndEffectiveDate { get; set; }
        public DateTime? CodeBeginEffectiveDate { get; set; }
        public DateTime? CodeEndEffectiveDate { get; set; }
        public int PricelistID { get; set; }
        public DateTime? PricelistBeginEffectiveDate { get; set; }
        public string UserName { get; set; }


        public short FlaggedServiceID { get; set; }
        public string FlaggedServiceSymbol { get; set; }
        public string FlaggedServiceColor { get; set; }
    }
}
