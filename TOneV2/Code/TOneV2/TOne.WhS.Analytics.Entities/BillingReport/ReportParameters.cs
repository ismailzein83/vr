using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class ReportParameters : VRTempPayloadSettings
    {
        public DateTime FromTime { get; set; }

        public DateTime? ToTime { get; set; }

        public string CustomersId { get; set; }

        public string SuppliersId { get; set; }

        public bool GroupByCustomer { get; set; }

        public bool GroupByProfile { get; set; }

        public string CustomerAMUId { get; set; }

        public string SupplierAMUId { get; set; }

        public bool IsCost { get; set; }

        public int CurrencyId { get; set; }

        public string SupplierGroup { get; set; }

        public string CustomerGroup { get; set; }

        public bool GroupBySupplier { get; set; }

        public decimal NormalDuration { get; set; }

        public decimal OffPeakDuration { get; set; }

        public double NormalNet { get; set; }

        public double OffPeakNet { get; set; }

        public double TotalAmount { get; set; }

        public bool IsService { get; set; }

        public bool IsCommission { get; set; }

        public decimal ServicesForCustomer { get; set; }

        public int Margin { get; set; }

        public string ZonesId { get; set; }

        public int Top { get; set; }

        public string AverageSaleNet { get; set; }
        public string AverageCostNet { get; set; }
        public string AverageProfit { get; set; }
        public string AveragePercProfit { get; set; }
        public string MTDSaleNet { get; set; }
        public string MTDCostNet { get; set; }
        public string MTDProfit { get; set; }
        public string ForcastSaleNet { get; set; }
        public string ForcastCostNet { get; set; }
        public string ForcastProfit { get; set; }
        public string InterpolatedDay { get; set; }
        public string InterpolatedSaleNet { get; set; }
        public string InterpolatedCostNet { get; set; }
        public string InterpolatedProfit { get; set; }
        public string InterpolatedPercProfit { get; set; }
        public bool IsExchange { get; set; }

        public bool PageBreak { get; set; }

        public decimal TotalDuration { get; set; }
        public double TotalSale { get; set; }
        public double TotalCost { get; set; }
        public double TotalProfit { get; set; }
        public string CurrencyDescription { get; set; }
    }
}
