using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Reprocess.Entities;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Queueing.Entities;
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
            BatchRecordType batchRecordType;

            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();

            HashSet<DealZoneGroup> saleDealZoneGroups = new HashSet<DealZoneGroup>();
            HashSet<DealZoneGroup> costDealZoneGroups = new HashSet<DealZoneGroup>();
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            dynamic firstRecord = batchRecords.FirstOrDefault();
            if (firstRecord == null)
                return;
            decimal? firstRecordSaleRateId = firstRecord.SaleRateId;
            decimal? firstRecordCostRateId = firstRecord.CostRateId;
            if (firstRecordSaleRateId.HasValue && firstRecordCostRateId.HasValue)
                batchRecordType = BatchRecordType.Main;
            else
                batchRecordType = BatchRecordType.PartialPriced;

            ManipulateRecords(batchRecords,
                (saleDealZoneGroup, record) => { saleDealZoneGroups.Add(saleDealZoneGroup); },
                (costDealZoneGroup, record) => { costDealZoneGroups.Add(costDealZoneGroup); });

            if (saleDealZoneGroups.Count == 0 && costDealZoneGroups.Count == 0)
                return;

            DealProgressManager dealProgressManager = new DealProgressManager();
            Dictionary<DealZoneGroup, DealProgress> saleDealProgresses = dealProgressManager.GetDealProgresses(saleDealZoneGroups, true);
            Dictionary<DealZoneGroup, DealProgress> costDealProgresses = dealProgressManager.GetDealProgresses(costDealZoneGroups, false);

            Dictionary<string, string> salePropertyNames = BuildPropertyNames("Sale");
            Dictionary<string, string> costPropertyNames = BuildPropertyNames("Cost");


            List<DealProgress> newSaleDealProgresses = new List<DealProgress>();
            List<DealProgress> newCostDealProgresses = new List<DealProgress>();
            List<DealProgress> existingSaleDealProgresses = saleDealProgresses != null ? saleDealProgresses.Values.ToList() : new List<DealProgress>();
            List<DealProgress> existingCostDealProgresses = costDealProgresses != null ? costDealProgresses.Values.ToList() : new List<DealProgress>();

            if (saleDealProgresses == null)
                saleDealProgresses = new Dictionary<DealZoneGroup, DealProgress>();

            if (costDealProgresses == null)
                costDealProgresses = new Dictionary<DealZoneGroup, DealProgress>();

            ManipulateRecords(batchRecords,
                (saleDealZoneGroup, record) =>
                {
                    DealSaleZoneGroup dealSaleZoneGroup = dealDefinitionManager.GetDealSaleZoneGroup(saleDealZoneGroup.DealId, saleDealZoneGroup.ZoneGroupNb);
                    Func<int?, DealZoneGroupTier> getSaleDealZoneGroupTier = (previousTierNumber) =>
                    {
                        DealSaleZoneGroupTier dealZoneGroupTier = dealSaleZoneGroup.Tiers.Where(itm => previousTierNumber.HasValue ? itm.TierNumber > previousTierNumber.Value : true).FirstOrDefault();
                        if (dealZoneGroupTier == null)
                            return null;
                        return new DealZoneGroupTier() { TierNumber = dealZoneGroupTier.TierNumber, Volume = dealZoneGroupTier.Volume };
                    };

                    UpdateBillingCDRData(saleDealProgresses, saleDealZoneGroup, record, true, salePropertyNames, getSaleDealZoneGroupTier, newSaleDealProgresses);
                },
                (costDealZoneGroup, record) =>
                {
                    DealSupplierZoneGroup dealSupplierZoneGroup = dealDefinitionManager.GetDealSupplierZoneGroup(costDealZoneGroup.DealId, costDealZoneGroup.ZoneGroupNb);
                    Func<int?, DealZoneGroupTier> getSupplierDealZoneGroupTier = (previousTierNumber) =>
                    {
                        DealSupplierZoneGroupTier dealZoneGroupTier = dealSupplierZoneGroup.Tiers.Where(itm => previousTierNumber.HasValue ? itm.TierNumber > previousTierNumber.Value : true).FirstOrDefault();
                        if (dealZoneGroupTier == null)
                            return null;
                        return new DealZoneGroupTier() { TierNumber = dealZoneGroupTier.TierNumber, Volume = dealZoneGroupTier.Volume };
                    };

                    UpdateBillingCDRData(costDealProgresses, costDealZoneGroup, record, false, costPropertyNames, getSupplierDealZoneGroupTier, newCostDealProgresses);
                });

            dealProgressManager.UpdateDealProgresses(existingSaleDealProgresses.Union(existingCostDealProgresses));
            dealProgressManager.InsertDealProgresses(newSaleDealProgresses.Union(newCostDealProgresses));

            DataRecordBatch transformedBatch = DataRecordBatch.CreateBatchFromRecords(batchRecords, queueItemType.BatchDescription, recordTypeId);
            switch (batchRecordType)
            {
                case BatchRecordType.Main:
                    if (this.MainOutputStages != null)
                    {
                        foreach (var mainOutputStage in this.MainOutputStages)
                        {
                            context.OutputItems.Add(mainOutputStage, transformedBatch);
                        }
                    }
                    if (this.BillingOutputStages != null)
                    {
                        foreach (var billingOutputStage in this.BillingOutputStages)
                        {
                            context.OutputItems.Add(billingOutputStage, transformedBatch);
                        }
                    }

                    break;
                case BatchRecordType.PartialPriced:
                    if (this.PartialPricedOutputStages != null)
                    {
                        foreach (var partialPricedOutputStage in this.PartialPricedOutputStages)
                        {
                            context.OutputItems.Add(partialPricedOutputStage, transformedBatch);
                        }
                    }

                    if (this.BillingOutputStages != null)
                    {
                        foreach (var billingOutputStage in this.BillingOutputStages)
                        {
                            context.OutputItems.Add(billingOutputStage, transformedBatch);
                        }
                    }
                    break;
            }
        }

        private Dictionary<string, string> BuildPropertyNames(string prefixPropName)
        {
            Dictionary<string, string> propertyNames = new Dictionary<string, string>();

            propertyNames.Add("DurationInSeconds", string.Format("{0}DurationInSeconds", prefixPropName));
            propertyNames.Add("DealId", string.Format("{0}DealId", prefixPropName));
            propertyNames.Add("DealZoneGroupNb", string.Format("{0}DealZoneGroupNb", prefixPropName));
            propertyNames.Add("DealTierNb", string.Format("{0}DealTierNb", prefixPropName));
            propertyNames.Add("DealRateTierNb", string.Format("{0}DealRateTierNb", prefixPropName));
            propertyNames.Add("DealDurInSec", string.Format("{0}DealDurInSec", prefixPropName));
            propertyNames.Add("SecondaryDealTierNb", string.Format("Secondary{0}DealTierNb", prefixPropName));
            propertyNames.Add("SecondaryDealRateTierNb", string.Format("Secondary{0}DealRateTierNb", prefixPropName));
            propertyNames.Add("SecondaryDealDurInSec", string.Format("Secondary{0}DealDurInSec", prefixPropName));

            propertyNames.Add("RateId", string.Format("{0}RateId", prefixPropName));
            propertyNames.Add("RateValue", string.Format("{0}RateValue", prefixPropName));
            propertyNames.Add("Net", string.Format("{0}Net", prefixPropName));
            propertyNames.Add("TariffRuleId", string.Format("{0}TariffRuleId", prefixPropName));
            propertyNames.Add("CurrencyId", string.Format("{0}CurrencyId", prefixPropName));

            return propertyNames;
        }

        private void UpdateBillingCDRData(Dictionary<DealZoneGroup, DealProgress> dealProgresses, DealZoneGroup dealZoneGroup, dynamic record, bool isSale,
            Dictionary<string, string> propertyNames, Func<int?, DealZoneGroupTier> GetNextDealZoneGroupTier, List<DealProgress> newDealProgresses)
        {
            decimal durationInSeconds = record.GetFieldValue(propertyNames["DurationInSeconds"]);

            DealProgress dealProgress;
            if (dealProgresses.TryGetValue(dealZoneGroup, out dealProgress))
            {
                decimal reachedDuration = dealProgress.ReachedDurationInSeconds.HasValue ? dealProgress.ReachedDurationInSeconds.Value : 0;

                if ((dealProgress.TargetDurationInSeconds - reachedDuration) >= durationInSeconds)
                {
                    dealProgress.ReachedDurationInSeconds = reachedDuration + durationInSeconds;
                    SetPrimaryDealData(dealZoneGroup, record, propertyNames, durationInSeconds, dealProgress.CurrentTierNb);
                }
                else
                {
                    decimal primaryTierDurationInSeconds = dealProgress.TargetDurationInSeconds - reachedDuration;
                    decimal secondaryTierDurationInSeconds = durationInSeconds - primaryTierDurationInSeconds;

                    dealProgress.ReachedDurationInSeconds = dealProgress.TargetDurationInSeconds;

                    bool isOnMultipleTier = secondaryTierDurationInSeconds != durationInSeconds;
                    if (isOnMultipleTier)
                    {
                        SetPrimaryDealData(dealZoneGroup, record, propertyNames, primaryTierDurationInSeconds, dealProgress.CurrentTierNb);
                    }

                    DealZoneGroupTier nextDealZoneGroupTier = GetNextDealZoneGroupTier(dealProgress.CurrentTierNb);

                    if (nextDealZoneGroupTier != null)
                    {
                        dealProgress.CurrentTierNb = nextDealZoneGroupTier.TierNumber;
                        dealProgress.TargetDurationInSeconds = nextDealZoneGroupTier.Volume;
                        dealProgress.ReachedDurationInSeconds = secondaryTierDurationInSeconds;

                        if (isOnMultipleTier)
                            SetSecondaryDealData(record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);
                        else
                            SetPrimaryDealData(dealZoneGroup, record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);
                    }
                    else
                    {
                        if (isOnMultipleTier)
                            SetSecondaryDealData(record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);
                    }
                }
            }
            else
            {
                DealZoneGroupTier newDealZoneGroupTier = GetNextDealZoneGroupTier(null);
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
                }
            }
        }

        private void SetPricingData(dynamic record, Dictionary<string, string> propertyNames, CDRPricingData firstPart, CDRPricingData secondPart)
        {
            if (secondPart == null)
            {
                record.SetFieldValue(propertyNames["RateValue"], firstPart.Rate);
                record.SetFieldValue(propertyNames["RateId"], null);
                record.SetFieldValue(propertyNames["Net"], firstPart.Rate * firstPart.DurationInSeconds / 60);
            }
            else
            {
                decimal secondPartNet;
                if (secondPart.DealId.HasValue)
                {
                    record.SetFieldValue(propertyNames["RateId"], null);
                    secondPartNet = secondPart.Rate * secondPart.DurationInSeconds / 60;
                }
                else
                {
                    ApplyTariffRule(record, propertyNames, secondPart.DurationInSeconds);
                }
                decimal rate = firstPart.Rate * firstPart.Percentage + secondPart.Rate * secondPart.Percentage;
                record.SetFieldValue(propertyNames["RateValue"], rate);
            }
        }

        private void ApplyTariffRule(dynamic record, Dictionary<string, string> propertyNames, decimal durationInSeconds)
        {
            int tariffRuleId = record.GetFieldValue(propertyNames["TariffRuleId"]);
            TariffRuleManager tarrifRuleManager = new TariffRuleManager();
            TariffRule tarrifRule = tarrifRuleManager.GetRule(tariffRuleId);

            decimal? extraChargeRate = record.GetFieldValue(propertyNames["ExtraChargeRateValue"]);
            TariffRuleContext context = new TariffRuleContext()
            {
                TargetTime = record.GetFieldValue(propertyNames["AttemptDateTime"]),
                DestinationCurrencyId = record.GetFieldValue(propertyNames["CurrencyId"]),
                Rate = record.GetFieldValue(propertyNames["RateValue"]),
                DurationInSeconds = durationInSeconds,
                ExtraChargeRate = extraChargeRate.HasValue ? extraChargeRate.Value : 0,
                SourceCurrencyId = tarrifRule.Settings.CurrencyId
            };
            tarrifRule.Settings.ApplyTariffRule(context);

            record.SetFieldValue(propertyNames["RateValue"], context.EffectiveDurationInSeconds);
            record.SetFieldValue(propertyNames["RateValue"], context.TotalAmount);
            record.SetFieldValue(propertyNames["RateValue"], context.EffectiveRate);
            record.SetFieldValue(propertyNames["RateValue"], context.ExtraChargeValue);
        }

        private void SetSecondaryDealData(dynamic record, Dictionary<string, string> propertyNames, decimal secondaryTierDurationInSeconds, int? tierNumber)
        {
            if (tierNumber.HasValue)
            {
                record.SetFieldValue(propertyNames["SecondaryDealTierNb"], tierNumber);
                record.SetFieldValue(propertyNames["SecondaryDealRateTierNb"], tierNumber);
            }
            record.SetFieldValue(propertyNames["SecondaryDealDurInSec"], secondaryTierDurationInSeconds);
        }

        private void SetPrimaryDealData(DealZoneGroup dealZoneGroup, dynamic record, Dictionary<string, string> propertyNames, decimal durationInSeconds, int tierNumber)
        {
            record.SetFieldValue(propertyNames["DealId"], dealZoneGroup.DealId);
            record.SetFieldValue(propertyNames["DealZoneGroupNb"], dealZoneGroup.ZoneGroupNb);
            record.SetFieldValue(propertyNames["DealTierNb"], tierNumber);
            record.SetFieldValue(propertyNames["DealRateTierNb"], tierNumber);
            record.SetFieldValue(propertyNames["DealDurInSec"], durationInSeconds);
        }

        private void ManipulateRecords(List<dynamic> batchRecords, Action<DealZoneGroup, dynamic> onSaleDealZoneGroupLoaded, Action<DealZoneGroup, dynamic> onCostDealZoneGroupLoaded)
        {
            foreach (var record in batchRecords)
            {
                int? saleOrigDealId = record.OrigSaleDealId;
                int? saleOrigZoneGroupNb = record.OrigSaleDealZoneGroupNb;
                if (saleOrigDealId.HasValue && saleOrigZoneGroupNb.HasValue)
                {
                    onSaleDealZoneGroupLoaded(new DealZoneGroup() { DealId = saleOrigDealId.Value, ZoneGroupNb = saleOrigZoneGroupNb.Value }, record);
                }

                int? costOrigDealId = record.OrigCostDealId;
                int? costOrigZoneGroupNb = record.OrigCostDealZoneGroupNb;
                if (costOrigDealId.HasValue && costOrigZoneGroupNb.HasValue)
                {
                    onCostDealZoneGroupLoaded(new DealZoneGroup() { DealId = costOrigDealId.Value, ZoneGroupNb = costOrigZoneGroupNb.Value }, record);
                }
            }
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

        private class DealZoneGroupTier
        {
            public int TierNumber { get; set; }
            public decimal Volume { get; set; }
        }


        private enum BatchRecordType { Main, PartialPriced }

        private class CDRPricingData
        {
            public decimal? DealId { get; set; }

            public decimal Rate { get; set; }

            public decimal Percentage { get; set; }

            public decimal DurationInSeconds { get; set; }
        }
    }
}