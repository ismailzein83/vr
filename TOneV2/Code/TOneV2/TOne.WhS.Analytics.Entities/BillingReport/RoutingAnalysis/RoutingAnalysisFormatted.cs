using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport.RoutingAnalysis
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
        public double Profit { get; set; }
        public string ProfitFormatted { get; set; }
        public double AVGCost { get; set; }
        public string AVGCostFormatted { get; set; }
        public double AVGSale { get; set; }
        public string AVGSaleFormatted { get; set; }

         /// <summary>
        /// DO NOT REMOVE
        /// the purpose of this method is to set the schema in the RDLC file
        /// </summary>
        /// <returns></returns>
        /// 
        public RoutingAnalysisFormatted() { }
        public IEnumerable<RoutingAnalysisFormatted> GetRoutingAnalysisFormattedRDLCSchema()
        {
            return null;
        }

    }
}
