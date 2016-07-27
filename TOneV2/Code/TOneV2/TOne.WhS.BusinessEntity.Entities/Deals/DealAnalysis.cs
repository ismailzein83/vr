using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class DealAnalysis
    {
        public string Name { get; set; }
    }

    public enum DealAnalysisType {  ByCostRate = 1, BySaleRate = 2}
    public class DealAnalysisSettings
    {
        public int CarrierAccountId { get; set; }

        public DealAnalysisType AnalysisType { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public List<DealAnalysisOutbound> Outbounds { get; set; }

        public List<DealAnalysisInbound> Inbounds { get; set; }


    }

    public class DealAnalysisOutbound
    {
        public string Name { get; set; }

        public List<long> SaleZoneIds { get; set; }

        public int CommitedVolume { get; set; }

        /// <summary>
        /// Calculated
        /// </summary>
        public int DailyVolume { get; set; }

        public Decimal Rate { get; set; }

        public Decimal CurrentCost { get; set; }

        /// <summary>
        /// Calculated
        /// </summary>
        public Decimal CostSavingFromRate { get; set; }

        /// <summary>
        /// Calculated
        /// </summary>
        public Decimal TotalDealSaving { get; set; }

        /// <summary>
        /// Calculated
        /// </summary>
        public Decimal RevenuePerDeal { get; set; }
    }

    public class DealAnalysisInbound
    {
        public string Name { get; set; }

        public List<long> SupplierZoneIds { get; set; }

        public int CommitedVolume { get; set; }

        /// <summary>
        /// Calculated
        /// </summary>
        public int DailyVolume { get; set; }

        public Decimal Rate { get; set; }

        public Decimal CurrentCost { get; set; }

        /// <summary>
        /// Calculated
        /// </summary>
        public Decimal ProfitFromRate { get; set; }

        /// <summary>
        /// Calculated
        /// </summary>
        public Decimal DealProfit { get; set; }

        /// <summary>
        /// Calculated
        /// </summary>
        public Decimal Revenue { get; set; }
    }
}
