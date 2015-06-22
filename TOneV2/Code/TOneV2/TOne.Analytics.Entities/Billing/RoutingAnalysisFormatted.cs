using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class RoutingAnalysisFormatted
    {
        public int SaleZoneID { get; set; }
        public string SaleZone { get; set; }
        public string SupplierID { get; set; }
        public string Supplier { get; set; }
        public decimal Duration { get; set; }
        public string DurationFormatted { get; set; }
        public double CostNet { get; set; }
        public string CostNetFormatted { get; set; }
        public double SaleNet { get; set; }
        public string SaleNetFormatted { get; set; }
        public decimal ASR { get; set; }
        public string ASRFormatted { get; set; }
        public decimal ACD { get; set; }
        public string ACDFormatted { get; set; }
        public string Profit { get; set; }
        public string AVGCost { get; set; }
        public string AVGSale { get; set; }
    }
}
