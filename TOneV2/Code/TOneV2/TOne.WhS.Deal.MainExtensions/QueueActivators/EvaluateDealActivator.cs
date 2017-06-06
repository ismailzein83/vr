using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Reprocess.Entities;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Queueing.Entities;
using Vanrise.Common;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.GenericData.Pricing;

namespace TOne.WhS.Deal.MainExtensions.QueueActivators
{
    public class EvaluateDealActivator : QueueActivator, IReprocessStageActivator
    {
        public List<string> MainOutputStages { get; set; }
        public List<string> PartialPricedOutputStages { get; set; }
        public List<string> BillingOutputStages { get; set; }

        #region QueueActivator

        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
        }

        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;

            HashSet<DealZoneGroup> saleDealZoneGroups = new HashSet<DealZoneGroup>();
            HashSet<DealZoneGroup> costDealZoneGroups = new HashSet<DealZoneGroup>();

            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);
            List<dynamic> mainCDRs = new List<dynamic>();
            List<dynamic> partialPricedCDRs = new List<dynamic>();

            foreach (var record in batchRecords)
            {
                decimal? recordSaleRateId = record.SaleRateId;
                decimal? recordCostRateId = record.CostRateId;
                if (recordSaleRateId.HasValue && recordCostRateId.HasValue)
                    mainCDRs.Add(record);
                else
                    partialPricedCDRs.Add(record);

                int? saleOrigDealId = record.OrigSaleDealId;
                int? saleOrigZoneGroupNb = record.OrigSaleDealZoneGroupNb;
                if (saleOrigDealId.HasValue && saleOrigZoneGroupNb.HasValue)
                {
                    saleDealZoneGroups.Add(new DealZoneGroup() { DealId = saleOrigDealId.Value, ZoneGroupNb = saleOrigZoneGroupNb.Value });
                }

                int? costOrigDealId = record.OrigCostDealId;
                int? costOrigZoneGroupNb = record.OrigCostDealZoneGroupNb;
                if (costOrigDealId.HasValue && costOrigZoneGroupNb.HasValue)
                {
                    costDealZoneGroups.Add(new DealZoneGroup() { DealId = costOrigDealId.Value, ZoneGroupNb = costOrigZoneGroupNb.Value });
                }
            }

            if (saleDealZoneGroups.Count == 0 && costDealZoneGroups.Count == 0)
                return;

            DealProgressManager dealProgressManager = new DealProgressManager();
            Dictionary<DealZoneGroup, DealProgress> saleDealProgresses = dealProgressManager.GetDealProgresses(saleDealZoneGroups, true);
            if (saleDealProgresses == null)
                saleDealProgresses = new Dictionary<DealZoneGroup, DealProgress>();

            Dictionary<DealZoneGroup, DealProgress> costDealProgresses = dealProgressManager.GetDealProgresses(costDealZoneGroups, false);
            if (costDealProgresses == null)
                costDealProgresses = new Dictionary<DealZoneGroup, DealProgress>();

            Dictionary<PropertyName, string> salePropertyNames = BuildPropertyNames("Sale", "Sale");
            Dictionary<PropertyName, string> costPropertyNames = BuildPropertyNames("Cost", "Supplier");

            List<DealProgress> newSaleDealProgresses = new List<DealProgress>();
            List<DealProgress> newCostDealProgresses = new List<DealProgress>();
            List<DealProgress> existingSaleDealProgresses = saleDealProgresses.Values.ToList();
            List<DealProgress> existingCostDealProgresses = costDealProgresses.Values.ToList();


            foreach (var record in batchRecords)
            {
                int? saleOrigDealId = record.OrigSaleDealId;
                int? saleOrigZoneGroupNb = record.OrigSaleDealZoneGroupNb;
                if (saleOrigDealId.HasValue && saleOrigZoneGroupNb.HasValue)
                {
                    DealZoneGroup saleDealZoneGroup = new DealZoneGroup() { DealId = saleOrigDealId.Value, ZoneGroupNb = saleOrigZoneGroupNb.Value };
                    Func<int, DealZoneGroupTier> getSaleDealZoneGroupTier = GetSaleDealZoneGroupTier(saleDealZoneGroup);
                    UpdateBillingCDRData(saleDealProgresses, saleDealZoneGroup, record, true, salePropertyNames, getSaleDealZoneGroupTier, newSaleDealProgresses);
                }

                int? costOrigDealId = record.OrigCostDealId;
                int? costOrigZoneGroupNb = record.OrigCostDealZoneGroupNb;
                if (costOrigDealId.HasValue && costOrigZoneGroupNb.HasValue)
                {
                    DealZoneGroup costDealZoneGroup = new DealZoneGroup() { DealId = costOrigDealId.Value, ZoneGroupNb = costOrigZoneGroupNb.Value };
                    Func<int, DealZoneGroupTier> getSupplierDealZoneGroupTier = GetSupplierDealZoneGroupTier(costDealZoneGroup);
                    UpdateBillingCDRData(costDealProgresses, costDealZoneGroup, record, false, costPropertyNames, getSupplierDealZoneGroupTier, newCostDealProgresses);
                }
            }

            dealProgressManager.UpdateDealProgresses(existingSaleDealProgresses.Union(existingCostDealProgresses));
            dealProgressManager.InsertDealProgresses(newSaleDealProgresses.Union(newCostDealProgresses));

            DataRecordBatch mainTransformedBatch = DataRecordBatch.CreateBatchFromRecords(mainCDRs, queueItemType.BatchDescription, recordTypeId);
            DataRecordBatch partialPricedTransformedBatch = DataRecordBatch.CreateBatchFromRecords(mainCDRs, queueItemType.BatchDescription, recordTypeId);
            SendOutputData(context, mainTransformedBatch, partialPricedTransformedBatch);
        }

        private void SendOutputData(IQueueActivatorExecutionContext context, DataRecordBatch mainTransformedBatch, DataRecordBatch partialPricedTransformedBatch)
        {
            if (mainTransformedBatch.GetRecordCount() > 0)
            {
                if (this.MainOutputStages != null)
                {
                    foreach (var mainOutputStage in this.MainOutputStages)
                    {
                        context.OutputItems.Add(mainOutputStage, mainTransformedBatch);
                    }
                }
                if (this.BillingOutputStages != null)
                {
                    foreach (var billingOutputStage in this.BillingOutputStages)
                    {
                        context.OutputItems.Add(billingOutputStage, mainTransformedBatch);
                    }
                }
            }

            if (partialPricedTransformedBatch.GetRecordCount() > 0)
            {
                if (this.PartialPricedOutputStages != null)
                {
                    foreach (var partialPricedOutputStage in this.PartialPricedOutputStages)
                    {
                        context.OutputItems.Add(partialPricedOutputStage, partialPricedTransformedBatch);
                    }
                }

                if (this.BillingOutputStages != null)
                {
                    foreach (var billingOutputStage in this.BillingOutputStages)
                    {
                        context.OutputItems.Add(billingOutputStage, partialPricedTransformedBatch);
                    }
                }
            }
        }

        private Func<int, DealZoneGroupTier> GetSaleDealZoneGroupTier(DealZoneGroup saleDealZoneGroup)
        {
            Func<int, DealZoneGroupTier> getSaleDealZoneGroupTier = (targetTierNumber) =>
            {
                return new DealDefinitionManager().GetSaleDealZoneGroupTier(saleDealZoneGroup.DealId, saleDealZoneGroup.ZoneGroupNb, targetTierNumber);
            };
            return getSaleDealZoneGroupTier;
        }

        private Func<int, DealZoneGroupTier> GetSupplierDealZoneGroupTier(DealZoneGroup costDealZoneGroup)
        {
            Func<int, DealZoneGroupTier> getSaleDealZoneGroupTier = (targetTierNumber) =>
            {
                return new DealDefinitionManager().GetSupplierDealZoneGroupTier(costDealZoneGroup.DealId, costDealZoneGroup.ZoneGroupNb, targetTierNumber);
            };
            return getSaleDealZoneGroupTier;
        }

        private Dictionary<PropertyName, string> BuildPropertyNames(string prefixPropName, string secondaryPrefixPropName)
        {
            Dictionary<PropertyName, string> propertyNames = new Dictionary<PropertyName, string>();

            propertyNames.Add(PropertyName.AttemptDateTime, "AttemptDateTime");
            propertyNames.Add(PropertyName.DurationInSeconds, "DurationInSeconds");
            propertyNames.Add(PropertyName.PricedDurationInSeconds, string.Format("{0}DurationInSeconds", prefixPropName));
            propertyNames.Add(PropertyName.DealId, string.Format("{0}DealId", prefixPropName));
            propertyNames.Add(PropertyName.DealZoneGroupNb, string.Format("{0}DealZoneGroupNb", prefixPropName));
            propertyNames.Add(PropertyName.DealTierNb, string.Format("{0}DealTierNb", prefixPropName));
            propertyNames.Add(PropertyName.DealRateTierNb, string.Format("{0}DealRateTierNb", prefixPropName));
            propertyNames.Add(PropertyName.DealDurInSec, string.Format("{0}DealDurInSec", prefixPropName));
            propertyNames.Add(PropertyName.SecondaryDealTierNb, string.Format("Secondary{0}DealTierNb", prefixPropName));
            propertyNames.Add(PropertyName.SecondaryDealRateTierNb, string.Format("Secondary{0}DealRateTierNb", prefixPropName));
            propertyNames.Add(PropertyName.SecondaryDealDurInSec, string.Format("Secondary{0}DealDurInSec", prefixPropName));

            propertyNames.Add(PropertyName.RateId, string.Format("{0}RateId", prefixPropName));
            propertyNames.Add(PropertyName.RateValue, string.Format("{0}RateValue", prefixPropName));
            propertyNames.Add(PropertyName.Net, string.Format("{0}Net", prefixPropName));
            propertyNames.Add(PropertyName.TariffRuleId, string.Format("{0}TariffRuleId", prefixPropName));
            propertyNames.Add(PropertyName.CurrencyId, string.Format("{0}CurrencyId", prefixPropName));
            propertyNames.Add(PropertyName.ExtraChargeRateValue, string.Format("{0}ExtraChargeRateValue", prefixPropName));
            propertyNames.Add(PropertyName.ExtraChargeValue, string.Format("{0}ExtraChargeValue", prefixPropName));

            propertyNames.Add(PropertyName.Zone, string.Format("{0}ZoneId", secondaryPrefixPropName));

            return propertyNames;
        }

        private void UpdateBillingCDRData(Dictionary<DealZoneGroup, DealProgress> dealProgresses, DealZoneGroup dealZoneGroup, dynamic record, bool isSale,
            Dictionary<PropertyName, string> propertyNames, Func<int, DealZoneGroupTier> GetDealZoneGroupTier, List<DealProgress> newDealProgresses)
        {
            decimal durationInSeconds = record.GetFieldValue(propertyNames[PropertyName.DurationInSeconds]);

            DealProgress dealProgress;
            if (dealProgresses.TryGetValue(dealZoneGroup, out dealProgress))
            {
                decimal reachedDuration = dealProgress.ReachedDurationInSeconds.HasValue ? dealProgress.ReachedDurationInSeconds.Value : 0;

                if (!dealProgress.TargetDurationInSeconds.HasValue || (dealProgress.TargetDurationInSeconds - reachedDuration) >= durationInSeconds)
                {
                    dealProgress.ReachedDurationInSeconds = reachedDuration + durationInSeconds;
                    SetPrimaryDealData(dealZoneGroup, record, propertyNames, durationInSeconds, dealProgress.CurrentTierNb);
                    DealZoneGroupTier currentDealZoneGroupTier = GetDealZoneGroupTier(dealProgress.CurrentTierNb);

                    CDRPricingDataInput firstPart = BuildCDRPricingData(record, propertyNames, dealProgress.DealID, durationInSeconds, 100, currentDealZoneGroupTier);
                    SetPricingData(record, propertyNames, firstPart, null);
                }
                else
                {
                    decimal primaryTierDurationInSeconds = dealProgress.TargetDurationInSeconds.Value - reachedDuration;
                    decimal secondaryTierDurationInSeconds = durationInSeconds - primaryTierDurationInSeconds;

                    dealProgress.ReachedDurationInSeconds = dealProgress.TargetDurationInSeconds;
                    DealZoneGroupTier previousDealZoneGroupTier = null;

                    bool isOnMultipleTier = secondaryTierDurationInSeconds != durationInSeconds;
                    if (isOnMultipleTier)
                    {
                        previousDealZoneGroupTier = GetDealZoneGroupTier(dealProgress.CurrentTierNb);
                        SetPrimaryDealData(dealZoneGroup, record, propertyNames, primaryTierDurationInSeconds, dealProgress.CurrentTierNb);
                    }

                    int nextTierNb = dealProgress.CurrentTierNb++;
                    DealZoneGroupTier nextDealZoneGroupTier = GetDealZoneGroupTier(nextTierNb);

                    if (nextDealZoneGroupTier != null)
                    {
                        dealProgress.CurrentTierNb = nextDealZoneGroupTier.TierNumber;
                        dealProgress.TargetDurationInSeconds = nextDealZoneGroupTier.Volume;
                        dealProgress.ReachedDurationInSeconds = secondaryTierDurationInSeconds;

                        if (isOnMultipleTier)
                        {
                            SetSecondaryDealData(record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);

                            CDRPricingDataInput firstPart = BuildCDRPricingData(record, propertyNames, dealProgress.DealID, primaryTierDurationInSeconds, primaryTierDurationInSeconds / durationInSeconds * 100, previousDealZoneGroupTier);
                            CDRPricingDataInput secondPart = BuildCDRPricingData(record, propertyNames, dealProgress.DealID, secondaryTierDurationInSeconds, secondaryTierDurationInSeconds / durationInSeconds * 100, nextDealZoneGroupTier);
                            SetPricingData(record, propertyNames, firstPart, secondPart);
                        }
                        else
                        {
                            SetPrimaryDealData(dealZoneGroup, record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);
                            CDRPricingDataInput firstPart = BuildCDRPricingData(record, propertyNames, dealProgress.DealID, primaryTierDurationInSeconds, 100, nextDealZoneGroupTier);
                            SetPricingData(record, propertyNames, firstPart, null);
                        }
                    }
                    else
                    {
                        if (isOnMultipleTier)
                        {
                            SetSecondaryDealData(record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);

                            CDRPricingDataInput firstPart = BuildCDRPricingData(record, propertyNames, dealProgress.DealID, primaryTierDurationInSeconds, primaryTierDurationInSeconds / durationInSeconds * 100, previousDealZoneGroupTier);
                            CDRPricingDataInput secondPart = BuildCDRPricingData(record, propertyNames, null, secondaryTierDurationInSeconds, secondaryTierDurationInSeconds / durationInSeconds * 100, null);
                            SetPricingData(record, propertyNames, firstPart, secondPart);
                        }
                    }
                }
            }
            else
            {
                DealZoneGroupTier newDealZoneGroupTier = GetDealZoneGroupTier(0);
                if (newDealZoneGroupTier != null)
                {
                    dealProgress = new DealProgress()
                    {
                        CurrentTierNb = newDealZoneGroupTier.TierNumber,
                        DealID = dealZoneGroup.DealId,
                        IsSale = isSale,
                        ReachedDurationInSeconds = durationInSeconds,
                        TargetDurationInSeconds = newDealZoneGroupTier.Volume,
                        ZoneGroupNb = dealZoneGroup.ZoneGroupNb
                    };
                    dealProgresses.Add(dealZoneGroup, dealProgress);
                    newDealProgresses.Add(dealProgress);

                    SetPrimaryDealData(dealZoneGroup, record, propertyNames, durationInSeconds, dealProgress.CurrentTierNb);
                    CDRPricingDataInput firstPart = BuildCDRPricingData(record, propertyNames, dealProgress.DealID, durationInSeconds, 100, newDealZoneGroupTier);
                    SetPricingData(record, propertyNames, firstPart, null);
                }
            }
        }

        private CDRPricingDataInput BuildCDRPricingData(dynamic record, Dictionary<PropertyName, string> propertyNames, int? dealId, decimal durationInSeconds,
            decimal percentage, DealZoneGroupTier dealZoneGroupTier)
        {
            if (dealZoneGroupTier == null)
                return new CDRPricingDataInput() { DurationInSeconds = durationInSeconds };

            long zoneId = record.GetFieldValue(propertyNames[PropertyName.Zone]);
            decimal zoneRate;
            if (dealZoneGroupTier.ExceptionRates == null || !dealZoneGroupTier.ExceptionRates.TryGetValue(zoneId, out zoneRate))
                zoneRate = dealZoneGroupTier.Rate;

            return new CDRPricingDataInput()
            {
                DealId = dealId,
                DurationInSeconds = durationInSeconds,
                Percentage = percentage,
                Rate = zoneRate
            };
        }

        private void SetPricingData(dynamic record, Dictionary<PropertyName, string> propertyNames, CDRPricingDataInput firstPart, CDRPricingDataInput secondPart)
        {
            if (secondPart == null)
            {
                record.SetFieldValue(propertyNames[PropertyName.RateValue], firstPart.Rate);
                record.SetFieldValue(propertyNames[PropertyName.RateId], null);
                record.SetFieldValue(propertyNames[PropertyName.Net], firstPart.Rate * firstPart.DurationInSeconds / 60);
                record.SetFieldValue(propertyNames[PropertyName.ExtraChargeRateValue], null);
                record.SetFieldValue(propertyNames[PropertyName.ExtraChargeValue], null);
                record.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], firstPart.DurationInSeconds);
            }
            else
            {
                decimal secondPartNet;
                decimal secondPartDuration;
                if (secondPart.DealId.HasValue)
                {
                    record.SetFieldValue(propertyNames[PropertyName.RateId], null);
                    record.SetFieldValue(propertyNames[PropertyName.ExtraChargeRateValue], null);
                    record.SetFieldValue(propertyNames[PropertyName.ExtraChargeValue], null);
                    secondPartNet = secondPart.Rate * secondPart.DurationInSeconds / 60;
                    secondPartDuration = secondPart.DurationInSeconds;
                }
                else
                {
                    decimal secondaryRate;
                    ApplyTariffRule(record, propertyNames, secondPart.DurationInSeconds, out secondPartNet, out secondaryRate, out secondPartDuration);
                    secondPart.Rate = secondaryRate;
                }
                decimal rate = firstPart.Rate * firstPart.Percentage + secondPart.Rate * secondPart.Percentage;
                record.SetFieldValue(propertyNames[PropertyName.RateValue], rate);

                decimal totalNet = firstPart.Rate * firstPart.DurationInSeconds / 60 + secondPartNet;
                record.SetFieldValue(propertyNames[PropertyName.Net], totalNet);
                record.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], (firstPart.DurationInSeconds + secondPartDuration));
            }
        }

        private void ApplyTariffRule(dynamic record, Dictionary<PropertyName, string> propertyNames, decimal durationInSeconds, out decimal totalAmount, out decimal effectiveRate, out decimal pricedDuration)
        {
            int tariffRuleId = record.GetFieldValue(propertyNames[PropertyName.TariffRuleId]);
            TariffRuleManager tarrifRuleManager = new TariffRuleManager();
            TariffRule tarrifRule = tarrifRuleManager.GetRule(tariffRuleId);

            decimal? extraChargeRate = record.GetFieldValue(propertyNames[PropertyName.ExtraChargeRateValue]);
            TariffRuleContext context = new TariffRuleContext()
            {
                TargetTime = record.GetFieldValue(PropertyName.AttemptDateTime),
                DestinationCurrencyId = record.GetFieldValue(propertyNames[PropertyName.CurrencyId]),
                Rate = record.GetFieldValue(propertyNames[PropertyName.RateValue]),
                DurationInSeconds = durationInSeconds,
                ExtraChargeRate = extraChargeRate.HasValue ? extraChargeRate.Value : 0,
                SourceCurrencyId = tarrifRule.Settings.CurrencyId
            };
            tarrifRule.Settings.ApplyTariffRule(context);

            record.SetFieldValue(propertyNames[PropertyName.SecondaryDealDurInSec], context.EffectiveDurationInSeconds);
            record.SetFieldValue(propertyNames[PropertyName.ExtraChargeValue], context.ExtraChargeValue);

            totalAmount = context.TotalAmount.HasValue ? context.TotalAmount.Value : default(decimal);
            effectiveRate = context.EffectiveRate;
            pricedDuration = context.EffectiveDurationInSeconds.HasValue ? context.EffectiveDurationInSeconds.Value : default(decimal);
        }

        private void SetSecondaryDealData(dynamic record, Dictionary<PropertyName, string> propertyNames, decimal secondaryTierDurationInSeconds, int? tierNumber)
        {
            if (tierNumber.HasValue)
            {
                record.SetFieldValue(propertyNames[PropertyName.SecondaryDealTierNb], tierNumber);
                record.SetFieldValue(propertyNames[PropertyName.SecondaryDealRateTierNb], tierNumber);
            }
            record.SetFieldValue(propertyNames[PropertyName.SecondaryDealDurInSec], secondaryTierDurationInSeconds);
        }

        private void SetPrimaryDealData(DealZoneGroup dealZoneGroup, dynamic record, Dictionary<PropertyName, string> propertyNames, decimal durationInSeconds, int tierNumber)
        {
            record.SetFieldValue(propertyNames[PropertyName.DealId], dealZoneGroup.DealId);
            record.SetFieldValue(propertyNames[PropertyName.DealZoneGroupNb], dealZoneGroup.ZoneGroupNb);
            record.SetFieldValue(propertyNames[PropertyName.DealTierNb], tierNumber);
            record.SetFieldValue(propertyNames[PropertyName.DealRateTierNb], tierNumber);
            record.SetFieldValue(propertyNames[PropertyName.DealDurInSec], durationInSeconds);
        }

        #endregion

        #region IReprocessStageActivator
        public void CommitChanges(Vanrise.Reprocess.Entities.IReprocessStageActivatorCommitChangesContext context)
        {
            throw new NotImplementedException();
        }

        public void DropStorage(Vanrise.Reprocess.Entities.IReprocessStageActivatorDropStorageContext context)
        {
            throw new NotImplementedException();
        }

        public void ExecuteStage(Vanrise.Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void FinalizeStage(Vanrise.Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {
            throw new NotImplementedException();
        }

        public List<string> GetOutputStages(List<string> stageNames)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Queueing.BaseQueue<Vanrise.Reprocess.Entities.IReprocessBatch> GetQueue()
        {
            throw new NotImplementedException();
        }

        public List<Vanrise.Reprocess.Entities.BatchRecord> GetStageBatchRecords(Vanrise.Reprocess.Entities.IReprocessStageActivatorPreparingContext context)
        {
            throw new NotImplementedException();
        }

        public int? GetStorageRowCount(Vanrise.Reprocess.Entities.IReprocessStageActivatorGetStorageRowCountContext context)
        {
            throw new NotImplementedException();
        }

        public object InitializeStage(Vanrise.Reprocess.Entities.IReprocessStageActivatorInitializingContext context)
        {
            throw new NotImplementedException();
        }
        #endregion

        private enum BatchRecordType { Main, PartialPriced }

        private class CDRPricingDataOutput
        {
            public decimal? DealId { get; set; }
            public decimal DurationInSeconds { get; set; }
            public decimal TotalNet { get; set; }
        }
        private class CDRPricingDataInput
        {
            public decimal? DealId { get; set; }
            public decimal Rate { get; set; }
            public decimal Percentage { get; set; }
            public decimal DurationInSeconds { get; set; }
        }

        private enum PropertyName { DurationInSeconds, PricedDurationInSeconds, DealId, DealZoneGroupNb, DealTierNb, DealRateTierNb, DealDurInSec, SecondaryDealTierNb, SecondaryDealRateTierNb, SecondaryDealDurInSec, RateId, RateValue, Net, TariffRuleId, CurrencyId, ExtraChargeRateValue, ExtraChargeValue, Zone, AttemptDateTime }
    }
}