using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class CostBySaleZone
    {
        public double? AvgCost { get; set; }
        public string AvgCostFormatted { get; set; }
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
        public CostBySaleZone() { }
        public IEnumerable<CostBySaleZone> GetCostBySaleZoneRDLCSchema()
        {
            return null;
        }
    }
}
