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
        public string Cost_Currency { get; set; }
        public string Sale_Currency { get; set; }
        public int NumberOfCalls { get; set; }
        public  TimeSpan FirstCallTime { get; set; }
        public  TimeSpan LastCallTime { get; set; }
        public  decimal MinDuration { get; set; }
        public  decimal MaxDuration { get; set; }
        public  decimal AvgDuration { get; set; }
        public  decimal Cost_Nets { get; set; }
        public  decimal Cost_Discounts { get; set; }
        public  decimal Cost_Commissions { get; set; }
        public  decimal Cost_ExtraCharges { get; set; }
        public  decimal Sale_Nets { get; set; }
        public  decimal Sale_Discounts { get; set; }
        public  decimal Sale_Commissions { get; set; }
        public  decimal Sale_ExtraCharges { get; set; }
        public  decimal Sale_Rate { get; set; }
        public  decimal Cost_Rate { get; set; }
        public  byte Sale_RateType { get; set; }
        public  byte Cost_RateType { get; set; }
        public  decimal SaleDuration { get; set; }
        public  decimal CostDuration { get; set; }
     
    }
}
