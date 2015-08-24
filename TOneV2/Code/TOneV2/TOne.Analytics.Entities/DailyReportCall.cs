using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class DailyReportCall
    {
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public int Attempts { get; set; }
        public int SuccessfulAttempts { get; set; }
        public Decimal DurationInMinutes { get; set; }
        public Decimal ASR { get; set; }
        public Decimal ACD { get; set; }
        public Decimal PDD { get; set; }
        public double CostRate { get; set; }
        public string CostRateDescription { get; set; }
        public double SaleRate { get; set; }
        public string SaleRateDescription { get; set; }
    }
}
