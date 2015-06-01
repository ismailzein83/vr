using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
   public  class BillingStatistic
    {
        public DateTime CallDate { get; set; }
        public string CustomerID { get; set; }
        public string SupplierID { get; set; }
        public int  CostZoneID { get; set; }
        public int SaleZoneID { get; set; }
        public string CostCurrency { get; set; }
        public string SaleCurrency { get; set; }
        public int NumberOfCalls { get; set; }
        public  string FirstCallTime { get; set; }
        public  string LastCallTime { get; set; }
        public decimal MinDuration { get; set; }
        public decimal MaxDuration { get; set; }
        public decimal AvgDuration { get; set; }
        public double CostNets { get; set; }
        public double CostDiscounts { get; set; }
        public double CostCommissions { get; set; }
        public double CostExtraCharges { get; set; }
        public  double SaleNets { get; set; }
        public double SaleDiscounts { get; set; }
        public double SaleCommissions { get; set; }
        public double SaleExtraCharges { get; set; }
        public double SaleRate { get; set; }
        public double CostRate { get; set; }
        public  byte SaleRateType { get; set; }
        public  byte CostRateType { get; set; }
        public decimal SaleDuration { get; set; }
        public decimal CostDuration { get; set; }
     
    }
}
