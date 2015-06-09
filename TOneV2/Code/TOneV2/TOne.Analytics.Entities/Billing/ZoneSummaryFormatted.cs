using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class ZoneSummaryFormatted
    {
        public string Zone { get; set; }

        public string SupplierID { get; set; }

        public int Calls { get; set; }

        public double? Rate { get; set; }
        public string RateFormatted { get; set; }


        public decimal? DurationNet { get; set; }
        public string DurationNetFormatted { get; set; }
        public byte RateType { get; set; }
        public string RateTypeFormatted { get; set; }
        public decimal DurationInSeconds { get; set; }
        public string DurationInSecondsFormatted { get; set; }
        public double? Net { get; set; }

        public string NetFormatted { get; set; }

        public double? CommissionValue { get; set; }

        public string CommissionValueFormatted { get; set; }

        public double? ExtraChargeValue { get; set; }

    }
}
