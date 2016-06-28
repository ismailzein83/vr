using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class CostBySaleZoneSupplier
    {
        public string SupplierID { get; set; }

        public double? HighestRate { get; set; }
        public string HighestRateFormatted { get; set; }

        public int salezoneID { get; set; }
        public string salezoneIDFormatted { get; set; }

        public decimal? AvgDuration { get; set; }
        public string AvgDurationFormatted { get; set; }

                
        /// <summary>
        /// DO NOT REMOVE
        /// the purpose of this method is to set the schema in the RDLC file
        /// </summary>
        /// <returns></returns>
        /// 
        public CostBySaleZoneSupplier() { }
        public IEnumerable<CostBySaleZoneSupplier> GetCostBySaleZoneSupplierRDLCSchema()
        {
            return null;
        }
    }
}
