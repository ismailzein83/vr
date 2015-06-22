using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class RoutingAnalysis
    {
        public int SaleZoneID { get; set; }
        public string SupplierID { get; set; }
        public decimal Duration { get; set; }
        public double CostNet { get; set; }
        public double SaleNet { get; set; }
        public decimal ASR { get; set; }
        public decimal ACD { get; set; }
    }
}
