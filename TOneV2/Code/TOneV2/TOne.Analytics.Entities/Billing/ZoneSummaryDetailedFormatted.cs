using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class ZoneSummaryDetailedFormatted
    {
        public string Zone { get; set; }

        public int? ZoneId { get; set; }

        public string SupplierID { get; set; }

        public int Calls { get; set; }

        public decimal? DurationNet { get; set; }
        public string DurationNetFormatted { get; set; }
        public decimal? DurationInSeconds { get; set; }
        public string DurationInSecondsFormatted { get; set; }

        public double? Rate { get; set; }
        public string RateFormatted { get; set; }
        public double? Net { get; set; }
        public string NetFormatted { get; set; }

        public decimal? OffPeakDurationInSeconds { get; set; }
        public string OffPeakDurationInSecondsFormatted { get; set; }
        public double? OffPeakRate { get; set; }
        public string OffPeakRateFormatted { get; set; }
        public double? OffPeakNet { get; set; }
        public string OffPeakNetFormatted { get; set; }

        public decimal? WeekEndDurationInSeconds { get; set; }
        public string WeekEndDurationInSecondsFormatted { get; set; }
        public double? WeekEndRate { get; set; }
        public string WeekEndRateFormatted { get; set; }
        public double? WeekEndNet { get; set; }
        public string WeekEndNetFormatted { get; set; }

        public double? Discount { get; set; }

        public double? CommissionValue { get; set; }
        public string CommissionValueFormatted { get; set; }

        public string TotalDurationFormatted { get; set; }
        public string TotalAmountFormatted { get; set; }

        public double? ExtraChargeValue { get; set; }

        public string ExtraChargeValueFormatted { get; set; }
    }
}
