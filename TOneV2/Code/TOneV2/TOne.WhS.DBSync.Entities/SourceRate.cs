using System;
using Vanrise.Entities.EntitySynchronization;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceRate : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public int? PriceListId { get; set; }
        public int? ZoneId { get; set; }
        public decimal? Rate { get; set; }
        public decimal? OffPeakRate { get; set; }
        public decimal? WeekendRate { get; set; }
        public DateTime? BeginEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public Int16? Change { get; set; }
        public Int16? ServicesFlag { get; set; }
        public string CurrencyId { get; set; }
        public RateTypeEnum? RateType { get; set; }
    }
}
