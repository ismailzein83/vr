using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class BillingStatistic : Vanrise.Entities.StatisticManagement.IStatisticItem, Vanrise.Entities.StatisticManagement.IRawItem
    {
        static BillingStatistic()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(Time), "Hour", "Minute", "Second", "MilliSecond");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingStatistic), "StatisticItemId","CallDate", "CustomerId", "SupplierId", "SaleZoneId", "SupplierZoneId",
                "CostCurrency", "SaleCurrency", "NumberOfCalls", "FirstCallTime", "LastCallTime", "MinDuration", "MaxDuration", "NumberOfCalls", "AvgDuration", "CostNets",
                "CostExtraCharges", "SaleNets", "SaleExtraCharges", "SaleRate", "CostRate", "SaleDuration", "CostDuration","SaleRateType","CostRateType");
        }

        public static string GetStatisticItemKey(int customerId, int supplierId, long saleZoneId, long supplierZoneId, int costCurrency, int saleCurrency, DateTime callDate, int saleRateType, int costRateType)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}", customerId, supplierId, saleZoneId, supplierZoneId, costCurrency, saleCurrency, callDate, saleRateType, costRateType);
        }

        public long StatisticItemId {get; set; }

        public DateTime CallDate { get; set; }
        public int CustomerId { get; set; }
        public int SupplierId { get; set; }
        public long SaleZoneId { get; set; }
        public long SupplierZoneId { get; set; }
        public int CostCurrency { get; set; }
        public int SaleCurrency { get; set; }
        public int NumberOfCalls { get; set; }
        public Time FirstCallTime { get; set; }
        public Time LastCallTime { get; set; }
        public int MinDuration { get; set; }
        public int MaxDuration { get; set; }
        public decimal AvgDuration { get; set; }
        public decimal CostNets { get; set; }
        public decimal CostExtraCharges { get; set; }
        public decimal SaleNets { get; set; }
        public decimal SaleExtraCharges { get; set; }
        public decimal SaleRate { get; set; }
        public decimal CostRate { get; set; }
        public int SaleDuration { get; set; }
        public int CostDuration { get; set; }
        public int SaleRateType { get; set; }
        public int CostRateType { get; set; }
    }
}
