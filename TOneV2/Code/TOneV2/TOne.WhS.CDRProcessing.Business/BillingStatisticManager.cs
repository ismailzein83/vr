using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Data;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Entities;

namespace TOne.WhS.CDRProcessing.Business
{
    public class BillingStatisticManager : Vanrise.Common.Business.StatisticManagement.StatisticBatchManager<BillingMainCDR, BillingStatistic, BillingStatisticBatch>
    {

        protected override BillingStatistic CreateStatisticItemFromRawItem(BillingMainCDR rawItem)
        {
            BillingStatistic statisticItem = new BillingStatistic();
            statisticItem.CustomerId = rawItem.BillingCDR.CustomerId;
            statisticItem.SupplierId = rawItem.BillingCDR.SupplierId;
            statisticItem.SaleZoneId = rawItem.BillingCDR.SaleZoneID;
            statisticItem.SupplierZoneId = rawItem.BillingCDR.SupplierZoneID;
            statisticItem.CostCurrency = rawItem.Cost.CurrencyId;
            statisticItem.SaleCurrency = rawItem.Sale.CurrencyId;
            statisticItem.CallDate = rawItem.BillingCDR.Attempt.Date;

            return statisticItem;
        }

        protected override void GetRawItemBatchTimeRange(BillingMainCDR rawItem, out DateTime batchStart, out DateTime batchEnd)
        {
            batchStart = rawItem.BillingCDR.Attempt.Date;
            batchEnd = batchStart.AddHours(24);
        }

        protected override string GetStatisticItemKey(BillingStatistic statisticItem)
        {
            return BillingStatistic.GetStatisticItemKey(statisticItem.CustomerId, statisticItem.SupplierId, statisticItem.SaleZoneId, statisticItem.SupplierZoneId, statisticItem.CostCurrency, statisticItem.SaleCurrency,statisticItem.CallDate,statisticItem.SaleRateType,statisticItem.CostRateType);
        }

        protected override string GetStatisticItemKey(BillingMainCDR rawItem)
        {
            return BillingStatistic.GetStatisticItemKey(rawItem.BillingCDR.CustomerId, rawItem.BillingCDR.SupplierId, rawItem.BillingCDR.SaleZoneID, rawItem.BillingCDR.SupplierZoneID, rawItem.Cost.CurrencyId, rawItem.Sale.CurrencyId,rawItem.BillingCDR.Attempt.Date,rawItem.Sale.RateType,rawItem.Cost.RateType);
        }

        protected override Dictionary<string, long> GetStatisticItemsIdsByKeyFromDB(BillingStatisticBatch batch)
        {
            IBillingStatisticDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<IBillingStatisticDataManager>();
            return dataManager.GetBillingStatisticItemsIdsByKeyFromDB(batch);
        }

        protected override void InsertStatisticItemsToDB(IEnumerable<BillingStatistic> items)
        {
            IBillingStatisticDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<IBillingStatisticDataManager>();
            dataManager.InsertBillingStatisticItemsToDB(items);
        }

        protected override void UpdateStatisticItemFromRawItem(BillingStatistic statisticItem, BillingMainCDR item)
        {
            statisticItem.NumberOfCalls++;

            if (statisticItem.FirstCallTime==null || statisticItem.FirstCallTime.GreaterThan(item.BillingCDR.Attempt)) statisticItem.FirstCallTime = new Time(item.BillingCDR.Attempt);
            if (statisticItem.LastCallTime == null || statisticItem.LastCallTime.LessThan(item.BillingCDR.Attempt)) statisticItem.LastCallTime = new Time(item.BillingCDR.Attempt);
            if (item.BillingCDR.DurationInSeconds < statisticItem.MinDuration)   statisticItem.MinDuration   = item.BillingCDR.DurationInSeconds;
            if (item.BillingCDR.DurationInSeconds > statisticItem.MaxDuration)   statisticItem.MaxDuration   = item.BillingCDR.DurationInSeconds;
            statisticItem.AvgDuration = (statisticItem.AvgDuration * (statisticItem.NumberOfCalls - 1) + (item.BillingCDR.DurationInSeconds)) / (decimal)statisticItem.NumberOfCalls;
            statisticItem.SaleDuration += (int)item.Sale.EffectiveDurationInSeconds;
            statisticItem.CostDuration += (int)item.Cost.EffectiveDurationInSeconds;

            statisticItem.SaleExtraCharges += (decimal)item.Sale.ExtraChargeValue;
            statisticItem.CostExtraCharges += (decimal)item.Cost.ExtraChargeValue;

            statisticItem.SaleNets += (decimal)item.Sale.TotalNet;
            statisticItem.CostNets += (decimal)item.Cost.TotalNet;

            statisticItem.SaleRate = (statisticItem.SaleRate * (statisticItem.NumberOfCalls - 1) + (decimal)item.Sale.RateValue) / (decimal)statisticItem.NumberOfCalls;
            statisticItem.CostRate = (statisticItem.CostRate * (statisticItem.NumberOfCalls - 1) + (decimal)item.Cost.RateValue) / (decimal)statisticItem.NumberOfCalls;
        }

        protected override void UpdateStatisticItemsInDB(IEnumerable<BillingStatistic> items)
        {
            IBillingStatisticDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<IBillingStatisticDataManager>();
            dataManager.UpdateBillingStatisticItemsInDB(items);
        }
    }
}
