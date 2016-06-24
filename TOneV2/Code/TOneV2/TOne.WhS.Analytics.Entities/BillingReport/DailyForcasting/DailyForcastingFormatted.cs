using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class DailyForcastingFormatted
    {
        public string Day { get; set; }
        public double? SaleNet { get; set; }
        public string SaleNetFormatted { get; set; }
        public double? CostNet { get; set; }
        public string CostNetFormatted { get; set; }
        public string ProfitFormatted { get; set; }
        public string ProfitPercentageFormatted { get; set; }

                /// <summary>
        /// DO NOT REMOVE
        /// the purpose of this method is to set the schema in the RDLC file
        /// </summary>
        /// <returns></returns>
        /// 
        public DailyForcastingFormatted() { }
        public IEnumerable<DailyForcastingFormatted> GetDailyForcastingFormattedRDLCSchema()
        {
            return null;
        }
    }
}
