using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class ZoneSummaryDetailed
    {
        public string Zone { get; set; }

        public int? ZoneId { get; set; }

        public string SupplierID { get; set; }

        public int Calls { get; set; }

        public decimal? DurationNet { get; set; }

        public decimal? DurationInSeconds { get; set; }

        public double? Rate { get; set; }

        public double? Net { get; set; }

        public decimal? OffPeakDurationInSeconds { get; set; }

        public double? OffPeakRate { get; set; }

        public double? OffPeakNet { get; set; }

        public decimal? WeekEndDurationInSeconds { get; set; }

        public double? WeekEndRate { get; set; }

        public double? WeekEndNet { get; set; }


        public double? Discount { get; set; }

        public double? CommissionValue { get; set; }

        public double? ExtraChargeValue { get; set; }

        public double? Services { get; set; }


    }
}
