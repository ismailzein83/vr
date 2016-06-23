﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class CarrierProfitSummaryFormatted
    {
        public string SupplierID { get; set; }
        public string Supplier { get; set; }
        public string CustomerID { get; set; }
        public string Customer { get; set; }
        public decimal? SaleDuration { get; set; }
        public string SaleDurationFormatted { get; set; }
        public decimal? CostDuration { get; set; }
        public string CostDurationFormatted { get; set; }
        public double? CostNet { get; set; }
        public string CostNetFormatted { get; set; }
        public double? SaleNet { get; set; }
        public string SaleNetFormatted { get; set; }
        public double? CostCommissionValue { get; set; }
        public string CostCommissionValueFormatted { get; set; }
        public double? SaleCommissionValue { get; set; }
        public string SaleCommissionValueFormatted { get; set; }
        public double? CostExtraChargeValue { get; set; }
        public string CostExtraChargeValueFormatted { get; set; }
        public double? SaleExtraChargeValue { get; set; }
        public string SaleExtraChargeValueFormatted { get; set; }
        public double? Profit { get; set; }
        public string ProfitFormatted { get; set; }
        public string ProfitPercentageFormatted { get; set; }
        public decimal? AvgMin { get; set; }
        public string AvgMinFormatted { get; set; }



        /// <summary>
        /// DO NOT REMOVE
        /// the purpose of this method is to set the schema in the RDLC file
        /// </summary>
        /// <returns></returns>
        /// 
        public CarrierProfitSummaryFormatted() { }
        public IEnumerable<CarrierProfitSummaryFormatted> GetCarrierProfitSummaryRDLCSchema()
        {
            return null;
        }

    }
}