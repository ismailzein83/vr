using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class SummaryByZone
    {
        public string Zone { get; set; }

        public string SupplierID { get; set; }

        public int Calls { get; set; }

        public decimal? Rate { get; set; }
        public string RateFormatted { get; set; }
        public decimal? DurationNet { get; set; }
        public string DurationNetFormatted { get; set; }
        public int? RateType { get; set; }
        public string RateTypeFormatted { get; set; }
        public decimal DurationInSeconds { get; set; }
        public string DurationInSecondsFormatted { get; set; }
        public double? Net { get; set; }
        public string NetFormatted { get; set; }

        public decimal? CommissionValue { get; set; }

        public string CommissionValueFormatted { get; set; }

        public decimal? ExtraChargeValue { get; set; }

                
        /// <summary>
        /// DO NOT REMOVE
        /// the purpose of this method is to set the schema in the RDLC file
        /// </summary>
        /// <returns></returns>
        /// 
        public SummaryByZone() { }
        public IEnumerable<SummaryByZone> GetSummaryByZoneRDLCSchema()
        {
            return null;
        }
    }
}
