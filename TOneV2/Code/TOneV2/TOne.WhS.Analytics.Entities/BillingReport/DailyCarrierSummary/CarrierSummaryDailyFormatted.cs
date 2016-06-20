using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class CarrierSummaryDailyFormatted
    {
        public string Day { get; set; }
        public string Date { get; set; }
        public string CarrierID { get; set; }
        public string Carrier { get; set; }
        public int? Attempts { get; set; }
        public string AttemptsFormatted { get; set; }
        public decimal? DurationNet { get; set; }
        public string DurationNetFormatted { get; set; }
        public decimal? Duration { get; set; }
        public string DurationFormatted { get; set; }
        public double? Net { get; set; }
        public string NetFormatted { get; set; }

        /// <summary>
        /// DO NOT REMOVE
        /// the purpose of this method is to set the schema in the RDLC file
        /// </summary>
        /// <returns></returns>
        /// 
        public CarrierSummaryDailyFormatted() { }
        public IEnumerable<CarrierSummaryDailyFormatted> GetCarrierSummaryDailyRDLCSchema()
        {
            return null;
        }

    }
}
