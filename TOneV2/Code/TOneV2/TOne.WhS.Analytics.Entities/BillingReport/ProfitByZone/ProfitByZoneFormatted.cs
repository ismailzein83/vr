﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class ProfitByZoneFormatted
    {
        public string CostZone { get; set; }
        public string SaleZone { get; set; }
        public string SupplierID { get; set; }
        public string CustomerID { get; set; }
        public int Calls { get; set; }
        public decimal? DurationNet { get; set; }
        public string DurationNetFormated { get; set; }
        public decimal? SaleDuration { get; set; }
        public string SaleDurationFormated { get; set; }
        public decimal? CostDuration { get; set; }
        public string CostDurationFormated { get; set; }
        public double? CostNet { get; set; }
        public string CostNetFormated { get; set; }
        public double? SaleNet { get; set; }
        public string SaleNetFormated { get; set; }
        public string Profit { get; set; }
        public double? ProfitSum { get; set; }
        public string ProfitPercentage { get; set; }

                
        /// <summary>
        /// DO NOT REMOVE
        /// the purpose of this method is to set the schema in the RDLC file
        /// </summary>
        /// <returns></returns>
        /// 
        public ProfitByZoneFormatted() { }
        public IEnumerable<ProfitByZoneFormatted> GetProfitByZoneFormattedRDLCSchema()
        {
            return null;
        }
    }
}
