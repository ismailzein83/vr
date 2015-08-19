using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierSummaryStatsQuery
    {
        public string CarrierType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string CustomerID { get; set; }
        public string SupplierID { get; set; }
        public int TopRecord { get; set; }
        public string GroupByProfile { get; set; }
        public int? CustomerAmuID { get; set; }
        public int? SupplierAmuID { get; set; }
        public string Currency { get; set; }
        public string ShowInactive { get; set; }
    }
}
