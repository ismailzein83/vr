using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Queueing.Entities;
using Vanrise.Reprocess.Entities;
using Vanrise.Rules.Pricing;

namespace TOne.WhS.Deal.MainExtensions.QueueActivators
{
    public class EvaluateDealActivator : QueueActivator, IReprocessStageActivator
    {
        private List<int> cdrTypesToEvaluate = new List<int>() { 1, 4 };

        public List<string> MainOutputStages { get; set; }
        public List<string> PartialPricedOutputStages { get; set; }
        public List<string> BillingOutputStages { get; set; }
        public List<string> TrafficOutputStages { get; set; }


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

            HashSet<DealZoneGroup> saleDealZoneGroups;
            HashSet<DealZoneGroup> costDealZoneGroups;

            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);
            List<dynamic> mainCDRs;
            List<dynamic> partialPricedCDRs;
            List<dynamic> billingRecords = new List<dynamic>();// for billing stats and traffic stats
            List<dynamic> trafficRecords = new List<dynamic>();// for traffic stats

            DateTime minAttemptDateTime; DateTime maxAttemptDateTime;
            PrepareLists(batchRecords, out saleDealZoneGroups, out costDealZoneGroups, out mainCDRs, out partialPricedCDRs, out minAttemptDateTime, out maxAttemptDateTime);

            if (saleDealZoneGroups.Count > 0 || costDealZoneGroups.Count > 0)
            {
                Dictionary<DealZoneGroup, DealProgress> saleDealProgresses;
                Dictionary<DealZoneGroup, DealProgress> costDealProgresses;
                Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> saleDealDetailedProgresses;
                Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> costDealDetailedProgresses;
                LoadDealProgressItems(saleDealZoneGroups, costDealZoneGroups, minAttemptDateTime, maxAttemptDateTime, out saleDealProgresses, out costDealProgresses, out saleDealDetailedProgresses, out costDealDetailedProgresses);

                Dictionary<PropertyName, string> salePropertyNames = BuildPropertyNames("Sale", "Sale");
                Dictionary<PropertyName, string> costPropertyNames = BuildPropertyNames("Cost", "Supplier");

                List<DealProgress> newSaleDealProgresses = new List<DealProgress>();
                List<DealProgress> newCostDealProgresses = new List<DealProgress>();
                List<DealProgress> existingSaleDealProgresses = saleDealProgresses.Values.ToList();
                List<DealProgress> existingCostDealProgresses = costDealProgresses.Values.ToList();

                List<DealDetailedProgress> newSaleDealDetailedProgresses = new List<DealDetailedProgress>();
                List<DealDetailedProgress> newCostDealDetailedProgresses = new List<DealDetailedProgress>();
                List<DealDetailedProgress> existingSaleDealDetailedProgresses = saleDealDetailedProgresses.Values.ToList();
                List<DealDetailedProgress> existingCostDealDetailedProgresses = costDealDetailedProgresses.Values.ToList();

                foreach (var record in batchRecords)
                {
                    int cdrType = record.Type;
                    if (!cdrTypesToEvaluate.Contains(cdrType))
                    {
                        trafficRecords.Add(record);
                        continue;
                    }

                    CDRPricingDataOutput firstSaleCDR = null;
                    CDRPricingDataOutput secondSaleCDR = null;

                    CDRPricingDataOutput firstCostCDR = null;
                    CDRPricingDataOutput secondCostCDR = null;

                    DateTime attemptDateTime = record.GetFieldValue(salePropertyNames[PropertyName.AttemptDateTime]);

                    int? saleOrigDealId = record.OrigSaleDealId;
                    int? saleOrigZoneGroupNb = record.OrigSaleDealZoneGroupNb;
                    if (saleOrigDealId.HasValue && saleOrigZoneGroupNb.HasValue)
                    {
                        var saleZoneId = record.GetFieldValue(salePropertyNames[PropertyName.Zone]);
                        DealZoneGroup saleDealZoneGroup = new DealZoneGroup() { DealId = saleOrigDealId.Value, ZoneGroupNb = saleOrigZoneGroupNb.Value };
                        Func<int, DealZoneGroupTierDetails> getSaleDealZoneGroupTierDetails = GetSaleDealZoneGroupTierDetails(saleDealZoneGroup, saleZoneId, attemptDateTime);
                        UpdateBillingCDRData(saleDealProgresses, newSaleDealProgresses, saleDealDetailedProgresses, newSaleDealDetailedProgresses, saleDealZoneGroup, record, true, salePropertyNames, getSaleDealZoneGroupTierDetails, out firstSaleCDR, out secondSaleCDR);
                    }

                    int? costOrigDealId = record.OrigCostDealId;
                    int? costOrigZoneGroupNb = record.OrigCostDealZoneGroupNb;
                    if (costOrigDealId.HasValue && costOrigZoneGroupNb.HasValue)
                    {
                        var supplierZoneId = record.GetFieldValue(costPropertyNames[PropertyName.Zone]);
                        DealZoneGroup costDealZoneGroup = new DealZoneGroup() { DealId = costOrigDealId.Value, ZoneGroupNb = costOrigZoneGroupNb.Value };
                        Func<int, DealZoneGroupTierDetails> getSupplierDealZoneGroupTierDetails = GetSupplierDealZoneGroupTierDetails(costDealZoneGroup, supplierZoneId, attemptDateTime);
                        UpdateBillingCDRData(costDealProgresses, newCostDealProgresses, costDealDetailedProgresses, newCostDealDetailedProgresses, costDealZoneGroup, record, false, costPropertyNames, getSupplierDealZoneGroupTierDetails, out firstCostCDR, out secondCostCDR);
                    }
                    CreateBillingStatsCDRRecords(billingRecords, record, salePropertyNames, firstSaleCDR, secondSaleCDR, costPropertyNames, firstCostCDR, secondCostCDR, recordTypeId);
                }
                DealProgressManager dealProgressManager = new DealProgressManager();
                dealProgressManager.UpdateDealProgresses(existingSaleDealProgresses.Union(existingCostDealProgresses));
                dealProgressManager.InsertDealProgresses(newSaleDealProgresses.Union(newCostDealProgresses));

                DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
                dealDetailedProgressManager.UpdateDealDetailedProgresses(existingSaleDealDetailedProgresses.Union(existingCostDealDetailedProgresses));
                dealDetailedProgressManager.InsertDealDetailedProgresses(newSaleDealDetailedProgresses.Union(newCostDealDetailedProgresses));
            }
            else
            {
                billingRecords = mainCDRs.Union(partialPricedCDRs).ToList();
            }

            DataRecordBatch mainTransformedBatch = DataRecordBatch.CreateBatchFromRecords(mainCDRs, queueItemType.BatchDescription, recordTypeId);
            DataRecordBatch partialPricedTransformedBatch = DataRecordBatch.CreateBatchFromRecords(partialPricedCDRs, queueItemType.BatchDescription, recordTypeId);
            DataRecordBatch billingTransformedBatch = DataRecordBatch.CreateBatchFromRecords(billingRecords, queueItemType.BatchDescription, recordTypeId);
            DataRecordBatch trafficTransformedBatch = DataRecordBatch.CreateBatchFromRecords(billingRecords.Union(trafficRecords).ToList(), queueItemType.BatchDescription, recordTypeId);

            SendOutputData(context, mainTransformedBatch, partialPricedTransformedBatch, billingTransformedBatch, trafficTransformedBatch);
        }

        private void PrepareLists(List<dynamic> batchRecords, out HashSet<DealZoneGroup> saleDealZoneGroups, out HashSet<DealZoneGroup> costDealZoneGroups, out List<dynamic> mainCDRs, out List<dynamic> partialPricedCDRs,
            out DateTime minAttemptDateTime, out DateTime maxAttemptDateTime)
        {
            saleDealZoneGroups = new HashSet<DealZoneGroup>();
            costDealZoneGroups = new HashSet<DealZoneGroup>();
            mainCDRs = new List<dynamic>();
            partialPricedCDRs = new List<dynamic>();

            minAttemptDateTime = DateTime.MaxValue;
            maxAttemptDateTime = DateTime.MinValue;

            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();

            foreach (var record in batchRecords)
            {
                int cdrType = record.Type;
                if (!cdrTypesToEvaluate.Contains(cdrType))
                    continue;

                DateTime attemptDateTime = record.AttemptDateTime;
                if (attemptDateTime > maxAttemptDateTime)
                    maxAttemptDateTime = attemptDateTime;

                if (attemptDateTime < minAttemptDateTime)
                    minAttemptDateTime = attemptDateTime;

                decimal? recordSaleRateId = record.SaleRateId;
                decimal? recordSaleCurrencyId = record.SaleCurrencyId;
                decimal? recordCostRateId = record.CostRateId;
                decimal? recordCostCurrencyId = record.CostCurrencyId;

                bool saleValid = recordSaleRateId.HasValue && recordSaleCurrencyId.HasValue;
                bool costValid = recordCostRateId.HasValue && recordCostCurrencyId.HasValue;

                if (!saleValid && !costValid)
                    throw new VRBusinessException(string.Format("Sale Part and Cost Part doesn't exist for CDR Id: {0}", record.CDRId));

                if (saleValid && costValid)
                    mainCDRs.Add(record);
                else
                    partialPricedCDRs.Add(record);

                dealDefinitionManager.FillOrigSaleDealValues(record);
                dealDefinitionManager.FillOrigCostDealValues(record);

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
        }

        private void LoadDealProgressItems(HashSet<DealZoneGroup> saleDealZoneGroups, HashSet<DealZoneGroup> costDealZoneGroups, DateTime minAttemptDateTime, DateTime maxAttemptDateTime,
            out Dictionary<DealZoneGroup, DealProgress> saleDealProgresses, out Dictionary<DealZoneGroup, DealProgress> costDealProgresses,
            out Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> saleDealDetailedProgresses, out Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> costDealDetailedProgresses)
        {
            DealProgressManager dealProgressManager = new DealProgressManager();
            saleDealProgresses = dealProgressManager.GetDealProgresses(saleDealZoneGroups, true);
            if (saleDealProgresses == null)
                saleDealProgresses = new Dictionary<DealZoneGroup, DealProgress>();

            costDealProgresses = dealProgressManager.GetDealProgresses(costDealZoneGroups, false);
            if (costDealProgresses == null)
                costDealProgresses = new Dictionary<DealZoneGroup, DealProgress>();

            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            saleDealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgresses(saleDealZoneGroups, true, minAttemptDateTime, maxAttemptDateTime);
            if (saleDealDetailedProgresses == null)
                saleDealDetailedProgresses = new Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress>();

            costDealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgresses(costDealZoneGroups, false, minAttemptDateTime, maxAttemptDateTime);
            if (costDealDetailedProgresses == null)
                costDealDetailedProgresses = new Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress>();
        }


        private Func<int, DealZoneGroupTierDetails> GetSaleDealZoneGroupTierDetails(DealZoneGroup saleDealZoneGroup, long saleZoneId, DateTime attemptDateTime)
        {
            Func<int, DealZoneGroupTierDetails> getSaleDealZoneGroupTierDetails = (targetTierNumber) =>
            {
                return new DealDefinitionManager().GetSaleDealZoneGroupTierDetails(saleDealZoneGroup.DealId, saleDealZoneGroup.ZoneGroupNb, targetTierNumber, saleZoneId, attemptDateTime);
            };
            return getSaleDealZoneGroupTierDetails;
        }

        private Func<int, DealZoneGroupTierDetails> GetSupplierDealZoneGroupTierDetails(DealZoneGroup costDealZoneGroup, long supplierZoneId, DateTime attemptDateTime)
        {
            Func<int, DealZoneGroupTierDetails> getSupplierDealZoneGroupTierDetails = (targetTierNumber) =>
            {
                return new DealDefinitionManager().GetSupplierDealZoneGroupTierDetails(costDealZoneGroup.DealId, costDealZoneGroup.ZoneGroupNb, targetTierNumber, supplierZoneId, attemptDateTime);
            };
            return getSupplierDealZoneGroupTierDetails;
        }

        private Dictionary<PropertyName, string> BuildPropertyNames(string prefixPropName, string secondaryPrefixPropName)
        {
            Dictionary<PropertyName, string> propertyNames = new Dictionary<PropertyName, string>();

            propertyNames.Add(PropertyName.AttemptDateTime, "AttemptDateTime");
            propertyNames.Add(PropertyName.DurationInSeconds, "DurationInSeconds");
            propertyNames.Add(PropertyName.CDRPartDurInSec, "CDRPartDurInSec");
            propertyNames.Add(PropertyName.CDRPartNb, "CDRPartNb");

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

            propertyNames.Add(PropertyName.OrigRateId, string.Format("Orig{0}RateId", prefixPropName));
            propertyNames.Add(PropertyName.OrigRateValue, string.Format("Orig{0}RateValue", prefixPropName));
            propertyNames.Add(PropertyName.OrigNet, string.Format("Orig{0}Net", prefixPropName));
            propertyNames.Add(PropertyName.OrigExtraChargeRateValue, string.Format("Orig{0}ExtraChargeRateValue", prefixPropName));
            propertyNames.Add(PropertyName.OrigExtraChargeValue, string.Format("Orig{0}ExtraChargeValue", prefixPropName));
            propertyNames.Add(PropertyName.OrigPricedDurationInSeconds, string.Format("Orig{0}DurationInSeconds", prefixPropName));
            propertyNames.Add(PropertyName.OrigCurrencyId, string.Format("Orig{0}CurrencyId", prefixPropName));

            propertyNames.Add(PropertyName.Zone, string.Format("{0}ZoneId", secondaryPrefixPropName));

            return propertyNames;
        }

        private void UpdateBillingCDRData(Dictionary<DealZoneGroup, DealProgress> dealProgresses, List<DealProgress> newDealProgresses, Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses,
            List<DealDetailedProgress> newDealDetailedProgresses, DealZoneGroup dealZoneGroup, dynamic record, bool isSale, Dictionary<PropertyName, string> propertyNames,
            Func<int, DealZoneGroupTierDetails> getDealZoneGroupTierDetails, out CDRPricingDataOutput firstPartOutput, out CDRPricingDataOutput secondPartOutput)
        {
            firstPartOutput = null; secondPartOutput = null;
            decimal pricedDurationInSeconds = record.GetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds]);

            DealProgress dealProgress;
            if (dealProgresses.TryGetValue(dealZoneGroup, out dealProgress))
            {
                decimal reachedDuration = dealProgress.ReachedDurationInSeconds.HasValue ? dealProgress.ReachedDurationInSeconds.Value : 0;

                if (!dealProgress.TargetDurationInSeconds.HasValue || (dealProgress.TargetDurationInSeconds - reachedDuration) >= pricedDurationInSeconds)
                {
                    dealProgress.ReachedDurationInSeconds = reachedDuration + pricedDurationInSeconds;
                    DealZoneGroupTierDetails currentDealZoneGroupTierDetails = getDealZoneGroupTierDetails(dealProgress.CurrentTierNb);

                    if (currentDealZoneGroupTierDetails != null)
                    {
                        SetPrimaryDealData(record, propertyNames, dealZoneGroup, dealProgress.CurrentTierNb, dealProgress.CurrentTierNb, pricedDurationInSeconds);
                        CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealId, pricedDurationInSeconds, 100, currentDealZoneGroupTierDetails, currentDealZoneGroupTierDetails.CurrencyId);
                        SetPricingData(record, propertyNames, firstPartInput, null, out firstPartOutput, out secondPartOutput);
                        AdjustDealDetailedProgress(record, propertyNames, isSale, dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, dealProgress.CurrentTierNb, pricedDurationInSeconds);
                    }
                    else
                    {
                        AdjustDealDetailedProgress(record, propertyNames, isSale, dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, null, pricedDurationInSeconds);
                    }
                }
                else
                {
                    decimal primaryTierDurationInSeconds = dealProgress.TargetDurationInSeconds.Value - reachedDuration;
                    decimal secondaryTierDurationInSeconds = pricedDurationInSeconds - primaryTierDurationInSeconds;

                    dealProgress.ReachedDurationInSeconds = dealProgress.TargetDurationInSeconds;
                    DealZoneGroupTierDetails previousDealZoneGroupTierDetails = null;

                    bool isOnMultipleTier = primaryTierDurationInSeconds > 0;
                    if (isOnMultipleTier)
                    {
                        previousDealZoneGroupTierDetails = getDealZoneGroupTierDetails(dealProgress.CurrentTierNb);
                        if (previousDealZoneGroupTierDetails == null)//current tier has been deleted and no more tiers exist after
                        {
                            AdjustDealDetailedProgress(record, propertyNames, isSale, dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, null, pricedDurationInSeconds);
                            return;
                        }

                        SetPrimaryDealData(record, propertyNames, dealZoneGroup, dealProgress.CurrentTierNb, dealProgress.CurrentTierNb, primaryTierDurationInSeconds);
                        AdjustDealDetailedProgress(record, propertyNames, isSale, dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, dealProgress.CurrentTierNb, primaryTierDurationInSeconds);
                    }

                    int nextTierNb = dealProgress.CurrentTierNb + 1;
                    DealZoneGroupTierDetails nextDealZoneGroupTier = getDealZoneGroupTierDetails(nextTierNb);

                    if (nextDealZoneGroupTier != null)
                    {
                        dealProgress.CurrentTierNb = nextDealZoneGroupTier.TierNb;
                        dealProgress.TargetDurationInSeconds = nextDealZoneGroupTier.VolumeInSeconds;
                        dealProgress.ReachedDurationInSeconds = 0;

                        if (isOnMultipleTier)
                        {
                            dealProgress.ReachedDurationInSeconds = secondaryTierDurationInSeconds;
                            SetSecondaryDealData(record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNb, nextDealZoneGroupTier.TierNb);

                            CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealId, primaryTierDurationInSeconds, primaryTierDurationInSeconds / pricedDurationInSeconds * 100, previousDealZoneGroupTierDetails, previousDealZoneGroupTierDetails.CurrencyId);
                            CDRPricingDataInput secondPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealId, secondaryTierDurationInSeconds, secondaryTierDurationInSeconds / pricedDurationInSeconds * 100, nextDealZoneGroupTier, nextDealZoneGroupTier.CurrencyId);
                            SetPricingData(record, propertyNames, firstPartInput, secondPartInput, out firstPartOutput, out secondPartOutput);
                            AdjustDealDetailedProgress(record, propertyNames, isSale, dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, nextDealZoneGroupTier.TierNb, secondaryTierDurationInSeconds);
                        }
                        else
                        {
                            UpdateBillingCDRData(dealProgresses, newDealProgresses, dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup, record, isSale, propertyNames,
                                getDealZoneGroupTierDetails, out firstPartOutput, out secondPartOutput);
                        }
                    }
                    else // second part outside deal
                    {
                        if (isOnMultipleTier)
                        {
                            SetSecondaryDealData(record, propertyNames, secondaryTierDurationInSeconds, null, null);

                            CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealId, primaryTierDurationInSeconds, primaryTierDurationInSeconds / pricedDurationInSeconds * 100, previousDealZoneGroupTierDetails, previousDealZoneGroupTierDetails.CurrencyId);

                            int recordCurrencyId = record.GetFieldValue(propertyNames[PropertyName.CurrencyId]);
                            CDRPricingDataInput secondPartInput = BuildCDRPricingDataInput(record, propertyNames, null, secondaryTierDurationInSeconds, secondaryTierDurationInSeconds / pricedDurationInSeconds * 100, null, recordCurrencyId);
                            SetPricingData(record, propertyNames, firstPartInput, secondPartInput, out firstPartOutput, out secondPartOutput);
                        }
                        AdjustDealDetailedProgress(record, propertyNames, isSale, dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, null, secondaryTierDurationInSeconds);
                    }
                }
            }
            else
            {
                DealZoneGroupTierDetails newDealZoneGroupTierDetails = getDealZoneGroupTierDetails(1);
                if (newDealZoneGroupTierDetails != null)
                {
                    dealProgress = new DealProgress()
                    {
                        DealId = dealZoneGroup.DealId,
                        ZoneGroupNb = dealZoneGroup.ZoneGroupNb,
                        CurrentTierNb = newDealZoneGroupTierDetails.TierNb,
                        IsSale = isSale,
                        ReachedDurationInSeconds = 0,
                        TargetDurationInSeconds = newDealZoneGroupTierDetails.VolumeInSeconds
                    };
                    dealProgresses.Add(dealZoneGroup, dealProgress);
                    newDealProgresses.Add(dealProgress);

                    UpdateBillingCDRData(dealProgresses, newDealProgresses, dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup, record, isSale, propertyNames,
                        getDealZoneGroupTierDetails, out firstPartOutput, out secondPartOutput);
                }
            }
        }

        private void AdjustDealDetailedProgress(dynamic record, Dictionary<PropertyName, string> propertyNames, bool isSale, Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses,
            List<DealDetailedProgress> newDealDetailedProgresses, int dealId, int zoneGroupNb, int? tierNb, decimal durationInSeconds)
        {
            int intervalOffsetInMinutes = new TOne.WhS.Deal.Business.ConfigManager().GetDealTechnicalSettingIntervalOffsetInMinutes();
            DateTime attemptDateTime = record.GetFieldValue(propertyNames[PropertyName.AttemptDateTime]);
            DateTime fromTime = new DateTime(attemptDateTime.Year, attemptDateTime.Month, attemptDateTime.Day, attemptDateTime.Hour, ((int)(attemptDateTime.Minute / intervalOffsetInMinutes)) * intervalOffsetInMinutes, 0);
            DateTime toTime = fromTime.AddMinutes(intervalOffsetInMinutes);

            DealDetailedZoneGroupTier dealDetailedZoneGroupTier = new DealDetailedZoneGroupTier()
            {
                DealId = dealId,
                ZoneGroupNb = zoneGroupNb,
                TierNb = tierNb,
                RateTierNb = tierNb,
                FromTime = fromTime,
                ToTime = toTime
            };
            DealDetailedProgress tempDealDetailedProgress;
            if (dealDetailedProgresses.TryGetValue(dealDetailedZoneGroupTier, out tempDealDetailedProgress))
            {
                tempDealDetailedProgress.ReachedDurationInSeconds += durationInSeconds;
            }
            else
            {
                DealDetailedProgress newDealDetailedProgress = new DealDetailedProgress()
                {
                    DealId = dealId,
                    ZoneGroupNb = zoneGroupNb,
                    TierNb = tierNb,
                    RateTierNb = tierNb,
                    FromTime = fromTime,
                    ToTime = toTime,
                    ReachedDurationInSeconds = durationInSeconds,
                    IsSale = isSale
                };
                dealDetailedProgresses.Add(dealDetailedZoneGroupTier, newDealDetailedProgress);
                newDealDetailedProgresses.Add(newDealDetailedProgress);
            }
        }

        private CDRPricingDataInput BuildCDRPricingDataInput(dynamic record, Dictionary<PropertyName, string> propertyNames, int? dealId, decimal durationInSeconds,
            decimal percentage, DealZoneGroupTierDetails dealZoneGroupTierDetails, int currencyId)
        {
            if (dealZoneGroupTierDetails == null)
                return new CDRPricingDataInput() { PricedDurationInSeconds = durationInSeconds };

            return new CDRPricingDataInput()
            {
                DealId = dealId,
                PricedDurationInSeconds = durationInSeconds,
                Percentage = percentage,
                Rate = dealZoneGroupTierDetails.Rate,
                CurrencyId = currencyId
            };
        }

        private void SetPricingData(dynamic record, Dictionary<PropertyName, string> propertyNames, CDRPricingDataInput firstPartInput, CDRPricingDataInput secondPartInput, out CDRPricingDataOutput firstPartOutput,
            out CDRPricingDataOutput secondPartOutput)
        {
            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
            decimal firstPartNet = firstPartInput.Rate * firstPartInput.PricedDurationInSeconds / 60;
            firstPartOutput = new CDRPricingDataOutput() { DealId = firstPartInput.DealId.Value, PricedDurationInSeconds = firstPartInput.PricedDurationInSeconds, Net = firstPartNet, Percentage = firstPartInput.Percentage, Rate = firstPartInput.Rate };

            if (secondPartInput == null)
            {
                secondPartOutput = null;
                record.SetFieldValue(propertyNames[PropertyName.CurrencyId], firstPartInput.CurrencyId);
                record.SetFieldValue(propertyNames[PropertyName.RateValue], firstPartInput.Rate);
                record.SetFieldValue(propertyNames[PropertyName.RateId], null);
                record.SetFieldValue(propertyNames[PropertyName.Net], firstPartNet);
                record.SetFieldValue(propertyNames[PropertyName.ExtraChargeRateValue], null);
                record.SetFieldValue(propertyNames[PropertyName.ExtraChargeValue], null);
                record.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], firstPartInput.PricedDurationInSeconds);
            }
            else
            {
                int secondCurrencyId;
                decimal secondPartNet;
                decimal secondPartDuration = secondPartInput.PricedDurationInSeconds;
                decimal secondPartPercentage;
                if (secondPartInput.DealId.HasValue)
                {
                    record.SetFieldValue(propertyNames[PropertyName.CurrencyId], firstPartInput.CurrencyId);
                    record.SetFieldValue(propertyNames[PropertyName.RateId], null);
                    record.SetFieldValue(propertyNames[PropertyName.ExtraChargeRateValue], null);
                    record.SetFieldValue(propertyNames[PropertyName.ExtraChargeValue], null);
                    secondPartNet = secondPartInput.Rate * secondPartInput.PricedDurationInSeconds / 60;
                    secondPartPercentage = secondPartInput.Percentage;
                    secondCurrencyId = secondPartInput.CurrencyId;
                }
                else
                {
                    decimal secondaryRate;
                    ApplyTariffRule(record, propertyNames, secondPartInput.PricedDurationInSeconds, out secondPartNet, out secondaryRate);
                    secondPartInput.Rate = secondaryRate;
                    secondPartPercentage = secondPartDuration / (firstPartInput.PricedDurationInSeconds + secondPartDuration) * 100;
                    firstPartOutput.Percentage = 100 - secondPartPercentage;
                    secondCurrencyId = record.GetFieldValue(propertyNames[PropertyName.CurrencyId]);
                }

                decimal totalNet;
                decimal convertedFirstPartRate;

                if (firstPartInput.CurrencyId == secondCurrencyId)
                {
                    totalNet = firstPartNet + secondPartNet;
                    convertedFirstPartRate = firstPartInput.Rate;
                }
                else
                {
                    DateTime attemptDateTime = record.GetFieldValue(propertyNames[PropertyName.AttemptDateTime]);
                    record.SetFieldValue(propertyNames[PropertyName.CurrencyId], secondCurrencyId);

                    firstPartOutput.Net = currencyExchangeRateManager.ConvertValueToCurrency(firstPartNet, firstPartInput.CurrencyId, secondCurrencyId, attemptDateTime);
                    totalNet = firstPartOutput.Net + secondPartNet;

                    convertedFirstPartRate = currencyExchangeRateManager.ConvertValueToCurrency(firstPartInput.Rate, firstPartInput.CurrencyId, secondCurrencyId, attemptDateTime);
                    firstPartOutput.Rate = convertedFirstPartRate;
                }

                decimal rate = convertedFirstPartRate * firstPartInput.Percentage / 100 + secondPartInput.Rate * secondPartPercentage / 100;
                record.SetFieldValue(propertyNames[PropertyName.RateValue], rate);

                secondPartOutput = new CDRPricingDataOutput() { DealId = secondPartInput.DealId, PricedDurationInSeconds = secondPartDuration, Net = secondPartNet, Percentage = secondPartPercentage, Rate = secondPartInput.Rate };

                record.SetFieldValue(propertyNames[PropertyName.Net], totalNet);
                record.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], (firstPartInput.PricedDurationInSeconds + secondPartDuration));
            }
        }

        private void ApplyTariffRule(dynamic record, Dictionary<PropertyName, string> propertyNames, decimal durationInSeconds, out decimal totalAmount, out decimal effectiveRate)
        {
            int tariffRuleId = record.GetFieldValue(propertyNames[PropertyName.TariffRuleId]);
            decimal origRate = record.GetFieldValue(propertyNames[PropertyName.OrigRateValue]);

            TariffRule tarrifRule = new TariffRuleManager().GetRule(tariffRuleId);
            TariffRuleCalculateAmountContext context = new TariffRuleCalculateAmountContext() { DurationInSeconds = durationInSeconds, EffectiveRate = origRate };
            tarrifRule.Settings.CalculateAmount(context);

            totalAmount = context.Amount;
            effectiveRate = origRate;
        }

        private void SetPrimaryDealData(dynamic record, Dictionary<PropertyName, string> propertyNames, DealZoneGroup dealZoneGroup, int tierNumber, int rateTierNumber, decimal durationInSeconds)
        {
            record.SetFieldValue(propertyNames[PropertyName.DealId], dealZoneGroup.DealId);
            record.SetFieldValue(propertyNames[PropertyName.DealZoneGroupNb], dealZoneGroup.ZoneGroupNb);
            record.SetFieldValue(propertyNames[PropertyName.DealTierNb], tierNumber);
            record.SetFieldValue(propertyNames[PropertyName.DealRateTierNb], rateTierNumber);
            record.SetFieldValue(propertyNames[PropertyName.DealDurInSec], durationInSeconds);
        }

        private void SetSecondaryDealData(dynamic record, Dictionary<PropertyName, string> propertyNames, decimal? secondaryTierDurationInSeconds, int? tierNumber, int? rateTierNumber)
        {
            if (tierNumber.HasValue)
            {
                record.SetFieldValue(propertyNames[PropertyName.SecondaryDealTierNb], tierNumber.Value);
                record.SetFieldValue(propertyNames[PropertyName.SecondaryDealRateTierNb], rateTierNumber.Value);
            }
            else
            {
                record.SetFieldValue(propertyNames[PropertyName.SecondaryDealTierNb], null);
                record.SetFieldValue(propertyNames[PropertyName.SecondaryDealRateTierNb], null);
            }
            record.SetFieldValue(propertyNames[PropertyName.SecondaryDealDurInSec], secondaryTierDurationInSeconds);
        }

        private void CreateBillingStatsCDRRecords(List<dynamic> billingRecords, dynamic record, Dictionary<PropertyName, string> salePropertyNames, CDRPricingDataOutput firstSaleCDR,
            CDRPricingDataOutput secondSaleCDR, Dictionary<PropertyName, string> costPropertyNames, CDRPricingDataOutput firstCostCDR, CDRPricingDataOutput secondCostCDR, Guid recordTypeId)
        {
            dynamic billingCDRRecord = record.CloneRecord(recordTypeId);
            if (firstSaleCDR == null && firstCostCDR == null)
            {
                billingRecords.Add(billingCDRRecord);
            }

            else if (firstSaleCDR != null && firstCostCDR == null)
            {
                AddBillingCDRs(billingCDRRecord, billingRecords, salePropertyNames, firstSaleCDR, secondSaleCDR, recordTypeId, costPropertyNames);
            }

            else if (firstSaleCDR == null && firstCostCDR != null)
            {
                AddBillingCDRs(billingCDRRecord, billingRecords, costPropertyNames, firstCostCDR, secondCostCDR, recordTypeId, salePropertyNames);
            }

            else if (firstSaleCDR != null && firstCostCDR != null)
            {
                if (secondSaleCDR == null && secondCostCDR == null)
                {
                    billingRecords.Add(billingCDRRecord);
                }
                else if (secondSaleCDR != null && secondCostCDR == null)
                {
                    AddBillingCDRs(billingCDRRecord, billingRecords, salePropertyNames, firstSaleCDR, secondSaleCDR, firstCostCDR, costPropertyNames, recordTypeId);
                }
                else if (secondSaleCDR == null && secondCostCDR != null)
                {
                    AddBillingCDRs(billingCDRRecord, billingRecords, costPropertyNames, firstCostCDR, secondCostCDR, firstSaleCDR, salePropertyNames, recordTypeId);
                }
                else if (secondSaleCDR != null && secondCostCDR != null)
                {
                    AddBillingCDRs(billingCDRRecord, billingRecords, salePropertyNames, firstSaleCDR, secondSaleCDR, costPropertyNames, firstCostCDR, secondCostCDR, recordTypeId);
                }
            }
        }

        private void AddBillingCDRs(dynamic billingCDRRecord, List<dynamic> billingRecords, Dictionary<PropertyName, string> propertyNames, CDRPricingDataOutput firstCDR,
            CDRPricingDataOutput secondCDR, Guid recordTypeId, Dictionary<PropertyName, string> otherPropertyNames)
        {
            if (secondCDR != null)
            {
                decimal durationInSeconds = billingCDRRecord.GetFieldValue(otherPropertyNames[PropertyName.DurationInSeconds]);
                decimal cdrFirstDurationInSeconds = durationInSeconds * firstCDR.Percentage / 100;
                decimal cdrSecondDurationInSeconds = durationInSeconds - cdrFirstDurationInSeconds;

                dynamic secondBillingCDRRecord = billingCDRRecord.CloneRecord(recordTypeId);

                SetPrimaryBillingCDRRecordData(billingCDRRecord, propertyNames, firstCDR.Net, cdrFirstDurationInSeconds, firstCDR.PricedDurationInSeconds, 1, firstCDR.Rate, firstCDR.PricedDurationInSeconds);
                SetSecondaryBillingCDRRecordData(secondBillingCDRRecord, propertyNames, secondCDR.Net, cdrSecondDurationInSeconds, secondCDR.PricedDurationInSeconds, 2, secondCDR.Rate, secondCDR.PricedDurationInSeconds);

                decimal otherCDRNet = billingCDRRecord.GetFieldValue(otherPropertyNames[PropertyName.Net]);
                decimal otherCDRPricedDuration = billingCDRRecord.GetFieldValue(otherPropertyNames[PropertyName.PricedDurationInSeconds]);
                decimal? otherCDRExtraCharge = billingCDRRecord.GetFieldValue(otherPropertyNames[PropertyName.ExtraChargeValue]);

                decimal otherCDRFirstNet = otherCDRNet * firstCDR.Percentage / 100;
                decimal otherCDRSecondNet = otherCDRNet - otherCDRFirstNet;
                decimal otherCDRFirstPricedDurationInSeconds = otherCDRPricedDuration * firstCDR.Percentage / 100;
                decimal otherCDRSecondPricedDurationInSeconds = otherCDRPricedDuration - otherCDRFirstPricedDurationInSeconds;

                billingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.Net], otherCDRFirstNet);
                billingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.PricedDurationInSeconds], otherCDRFirstPricedDurationInSeconds);

                secondBillingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.Net], otherCDRSecondNet);
                secondBillingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.PricedDurationInSeconds], otherCDRSecondPricedDurationInSeconds);

                if (otherCDRExtraCharge.HasValue)
                {
                    decimal otherCDRFirstExtraCharge = otherCDRExtraCharge.Value * firstCDR.Percentage / 100;
                    decimal otherCDRSecondExtraCharge = otherCDRExtraCharge.Value - otherCDRFirstExtraCharge;

                    billingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.ExtraChargeValue], otherCDRFirstExtraCharge);
                    secondBillingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.ExtraChargeValue], otherCDRSecondExtraCharge);
                }

                billingRecords.Add(secondBillingCDRRecord);
            }

            billingRecords.Add(billingCDRRecord);
        }

        private void AddBillingCDRs(dynamic billingCDRRecord, List<dynamic> billingRecords, Dictionary<PropertyName, string> propertyNames, CDRPricingDataOutput firstCDR,
            CDRPricingDataOutput secondCDR, CDRPricingDataOutput otherCDR, Dictionary<PropertyName, string> otherPropertyNames, Guid recordTypeId)
        {
            decimal durationInSeconds = billingCDRRecord.GetFieldValue(otherPropertyNames[PropertyName.DurationInSeconds]);
            decimal cdrFirstDurationInSeconds = durationInSeconds * firstCDR.Percentage / 100;
            decimal cdrSecondDurationInSeconds = durationInSeconds - cdrFirstDurationInSeconds;

            dynamic secondBillingCDRRecord = billingCDRRecord.CloneRecord(recordTypeId);
            SetPrimaryBillingCDRRecordData(billingCDRRecord, propertyNames, firstCDR.Net, cdrFirstDurationInSeconds, firstCDR.PricedDurationInSeconds, 1, firstCDR.Rate, firstCDR.PricedDurationInSeconds);
            SetSecondaryBillingCDRRecordData(secondBillingCDRRecord, propertyNames, secondCDR.Net, cdrSecondDurationInSeconds, secondCDR.PricedDurationInSeconds, 2, secondCDR.Rate, secondCDR.PricedDurationInSeconds);

            decimal otherCDRFirstNet = otherCDR.Net * firstCDR.Percentage / 100;
            decimal otherCDRSecondNet = otherCDR.Net - otherCDRFirstNet;
            decimal otherCDRFirstPricedDurationInSeconds = otherCDR.PricedDurationInSeconds * firstCDR.Percentage / 100;
            decimal otherCDRSecondPricedDurationInSeconds = otherCDR.PricedDurationInSeconds - otherCDRFirstPricedDurationInSeconds;

            decimal otherCDRDealDurationInSeconds = billingCDRRecord.GetFieldValue(otherPropertyNames[PropertyName.DealDurInSec]);
            decimal otherCDRFirstDealDurationInSeconds = otherCDRDealDurationInSeconds * firstCDR.Percentage / 100;
            decimal otherCDRSecondDealDurationInSeconds = otherCDRDealDurationInSeconds - otherCDRFirstDealDurationInSeconds;

            billingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.Net], otherCDRFirstNet);
            billingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.PricedDurationInSeconds], otherCDRFirstPricedDurationInSeconds);
            billingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.DealDurInSec], otherCDRFirstDealDurationInSeconds);

            secondBillingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.Net], otherCDRSecondNet);
            secondBillingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.PricedDurationInSeconds], otherCDRSecondPricedDurationInSeconds);
            secondBillingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.DealDurInSec], otherCDRSecondDealDurationInSeconds);

            billingRecords.Add(billingCDRRecord);
            billingRecords.Add(secondBillingCDRRecord);
        }

        private void AddBillingCDRs(dynamic billingCDRRecord, List<dynamic> billingRecords, Dictionary<PropertyName, string> salePropertyNames, CDRPricingDataOutput firstSaleCDR,
            CDRPricingDataOutput secondSaleCDR, Dictionary<PropertyName, string> costPropertyNames, CDRPricingDataOutput firstCostCDR, CDRPricingDataOutput secondCostCDR, Guid recordTypeId)
        {
            dynamic secondBillingCDRRecord = billingCDRRecord.CloneRecord(recordTypeId);
            dynamic thirdBillingCDRRecord = billingCDRRecord.CloneRecord(recordTypeId);

            decimal totalSaleCDRPricedDuration = firstSaleCDR.PricedDurationInSeconds + secondSaleCDR.PricedDurationInSeconds;
            decimal firstSaleCDRRatio = firstSaleCDR.PricedDurationInSeconds / totalSaleCDRPricedDuration;
            decimal secondSaleCDRRatio = 1 - firstSaleCDRRatio;

            decimal totalCostCDRPricedDuration = firstCostCDR.PricedDurationInSeconds + secondCostCDR.PricedDurationInSeconds;
            decimal firstCostCDRRatio = firstCostCDR.PricedDurationInSeconds / totalCostCDRPricedDuration;
            decimal secondCostCDRRatio = 1 - firstCostCDRRatio;

            decimal durationInSeconds = billingCDRRecord.GetFieldValue(salePropertyNames[PropertyName.DurationInSeconds]);

            if (firstSaleCDRRatio > firstCostCDRRatio)
            {
                decimal firstDurationInSeconds = firstCostCDRRatio * durationInSeconds;
                SetPrimaryBillingCDRRecordData(billingCDRRecord, costPropertyNames, firstCostCDR.Net, firstDurationInSeconds, firstCostCDR.PricedDurationInSeconds, 1, firstCostCDR.Rate, firstCostCDR.PricedDurationInSeconds);

                decimal firstSaleCDRPricedDuration = firstCostCDRRatio * totalSaleCDRPricedDuration;
                decimal firstSaleCDRNet = firstSaleCDR.Net * firstSaleCDRPricedDuration / firstSaleCDR.PricedDurationInSeconds;
                SetPrimaryBillingCDRRecordData(billingCDRRecord, salePropertyNames, firstSaleCDRNet, firstDurationInSeconds, firstSaleCDRPricedDuration, 1, firstSaleCDR.Rate, firstSaleCDRPricedDuration);

                decimal secondSaleCDRPricedDuration = firstSaleCDR.PricedDurationInSeconds - firstSaleCDRPricedDuration;
                decimal secondSaleCDRNet = firstSaleCDR.Net - firstSaleCDRNet;
                decimal secondDurationInSeconds = secondSaleCDRPricedDuration * durationInSeconds / totalSaleCDRPricedDuration;
                SetPrimaryBillingCDRRecordData(secondBillingCDRRecord, salePropertyNames, secondSaleCDRNet, secondDurationInSeconds, secondSaleCDRPricedDuration, 2, firstSaleCDR.Rate, secondSaleCDRPricedDuration);

                decimal secondCostCDRPricedDuration = secondSaleCDRPricedDuration * totalCostCDRPricedDuration / totalSaleCDRPricedDuration;
                decimal secondCostCDRNet = secondCostCDR.Net * secondCostCDRPricedDuration / secondCostCDR.PricedDurationInSeconds;
                SetSecondaryBillingCDRRecordData(secondBillingCDRRecord, costPropertyNames, secondCostCDRNet, secondDurationInSeconds, secondCostCDRPricedDuration, 2, secondCostCDR.Rate, secondCostCDRPricedDuration);

                decimal? costExtraChargeValue = secondBillingCDRRecord.GetFieldValue(costPropertyNames[PropertyName.ExtraChargeValue]);
                decimal secondCostCDRExtaChargeValue = 0;
                if (costExtraChargeValue.HasValue)
                {
                    secondCostCDRExtaChargeValue = costExtraChargeValue.Value * secondCostCDRPricedDuration / secondCostCDR.PricedDurationInSeconds;
                    secondBillingCDRRecord.SetFieldValue(costPropertyNames[PropertyName.ExtraChargeValue], secondCostCDRExtaChargeValue);
                }

                decimal thirdDurationInSeconds = durationInSeconds - firstDurationInSeconds - secondDurationInSeconds;
                SetSecondaryBillingCDRRecordData(thirdBillingCDRRecord, salePropertyNames, secondSaleCDR.Net, thirdDurationInSeconds, secondSaleCDR.PricedDurationInSeconds, 3, secondSaleCDR.Rate, secondSaleCDR.PricedDurationInSeconds);

                decimal thirdCostCDRNet = secondCostCDR.Net - secondCostCDRNet;
                decimal thirdCostCDRPricedDuration = secondCostCDR.PricedDurationInSeconds - secondCostCDRPricedDuration;
                SetSecondaryBillingCDRRecordData(thirdBillingCDRRecord, costPropertyNames, thirdCostCDRNet, thirdDurationInSeconds, thirdCostCDRPricedDuration, 3, secondCostCDR.Rate, thirdCostCDRPricedDuration);

                if (costExtraChargeValue.HasValue)
                {
                    decimal thirdCostCDRExtaChargeValue = costExtraChargeValue.Value - secondCostCDRExtaChargeValue;
                    thirdBillingCDRRecord.SetFieldValue(costPropertyNames[PropertyName.ExtraChargeValue], thirdCostCDRExtaChargeValue);
                }
            }
            else
            {
                decimal firstDurationInSeconds = firstSaleCDRRatio * durationInSeconds;
                SetPrimaryBillingCDRRecordData(billingCDRRecord, salePropertyNames, firstSaleCDR.Net, firstDurationInSeconds, firstSaleCDR.PricedDurationInSeconds, 1, firstSaleCDR.Rate, firstSaleCDR.PricedDurationInSeconds);

                decimal firstCostCDRPricedDuration = firstSaleCDRRatio * totalCostCDRPricedDuration;
                decimal firstCostCDRNet = firstCostCDR.Net * firstCostCDRPricedDuration / firstCostCDR.PricedDurationInSeconds;
                SetPrimaryBillingCDRRecordData(billingCDRRecord, costPropertyNames, firstCostCDRNet, firstDurationInSeconds, firstCostCDRPricedDuration, 1, firstCostCDR.Rate, firstCostCDRPricedDuration);

                decimal secondCostCDRPricedDuration = firstCostCDR.PricedDurationInSeconds - firstCostCDRPricedDuration;
                decimal secondCostCDRNet = firstCostCDR.Net - firstCostCDRNet;
                decimal secondDurationInSeconds = secondCostCDRPricedDuration * durationInSeconds / totalSaleCDRPricedDuration;
                SetPrimaryBillingCDRRecordData(secondBillingCDRRecord, costPropertyNames, secondCostCDRNet, secondDurationInSeconds, secondCostCDRPricedDuration, 2, firstCostCDR.Rate, secondCostCDRPricedDuration);

                decimal secondSaleCDRPricedDuration = secondCostCDRPricedDuration * totalSaleCDRPricedDuration / totalCostCDRPricedDuration;
                decimal secondSaleCDRNet = secondSaleCDR.Net * secondSaleCDRPricedDuration / secondSaleCDR.PricedDurationInSeconds;
                SetSecondaryBillingCDRRecordData(secondBillingCDRRecord, salePropertyNames, secondSaleCDRNet, secondDurationInSeconds, secondSaleCDRPricedDuration, 2, secondSaleCDR.Rate, secondSaleCDRPricedDuration);

                decimal? saleExtraChargeValue = secondBillingCDRRecord.GetFieldValue(salePropertyNames[PropertyName.ExtraChargeValue]);
                decimal secondSaleCDRExtaChargeValue = 0;
                if (saleExtraChargeValue.HasValue)
                {
                    secondSaleCDRExtaChargeValue = saleExtraChargeValue.Value * secondSaleCDRPricedDuration / secondSaleCDR.PricedDurationInSeconds;
                    secondBillingCDRRecord.SetFieldValue(salePropertyNames[PropertyName.ExtraChargeValue], secondSaleCDRExtaChargeValue);
                }

                decimal thirdDurationInSeconds = durationInSeconds - firstDurationInSeconds - secondDurationInSeconds;
                SetSecondaryBillingCDRRecordData(thirdBillingCDRRecord, costPropertyNames, secondCostCDR.Net, thirdDurationInSeconds, secondCostCDR.PricedDurationInSeconds, 3, secondCostCDR.Rate, secondCostCDR.PricedDurationInSeconds);

                decimal thirdSaleCDRNet = secondSaleCDR.Net - secondSaleCDRNet;
                decimal thirdSaleCDRPricedDuration = secondSaleCDR.PricedDurationInSeconds - secondSaleCDRPricedDuration;
                SetSecondaryBillingCDRRecordData(thirdBillingCDRRecord, salePropertyNames, thirdSaleCDRNet, thirdDurationInSeconds, thirdSaleCDRPricedDuration, 3, secondSaleCDR.Rate, thirdSaleCDRPricedDuration);

                if (saleExtraChargeValue.HasValue)
                {
                    decimal thirdSaleCDRExtaChargeValue = saleExtraChargeValue.Value - secondSaleCDRExtaChargeValue;
                    thirdBillingCDRRecord.SetFieldValue(salePropertyNames[PropertyName.ExtraChargeValue], thirdSaleCDRExtaChargeValue);
                }
            }

            billingRecords.Add(billingCDRRecord);
            billingRecords.Add(secondBillingCDRRecord);
            billingRecords.Add(thirdBillingCDRRecord);
        }

        private void SetPrimaryBillingCDRRecordData(dynamic billingCDRRecord, Dictionary<PropertyName, string> propertyNames, decimal net, decimal durationInSeconds, decimal pricedDurationInSeconds, int cdrPartNb, decimal rate, decimal dealDurationInSeconds)
        {
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealTierNb], null);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealRateTierNb], null);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealDurInSec], null);

            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.RateId], null);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.ExtraChargeRateValue], null);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.ExtraChargeValue], null);

            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.RateValue], rate);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.Net], net);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], pricedDurationInSeconds);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.DealDurInSec], dealDurationInSeconds);

            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.CDRPartNb], cdrPartNb);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.CDRPartDurInSec], durationInSeconds);
        }

        private void SetSecondaryBillingCDRRecordData(dynamic secondBillingCDRRecord, Dictionary<PropertyName, string> propertyNames, decimal net, decimal durationInSeconds, decimal pricedDurationInSeconds, int cdrPartNb, decimal rate, decimal dealDurationInSeconds)
        {
            int? secondaryTierNb = secondBillingCDRRecord.GetFieldValue(propertyNames[PropertyName.SecondaryDealTierNb]);
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.DealTierNb], secondaryTierNb);
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.DealRateTierNb], secondBillingCDRRecord.GetFieldValue(propertyNames[PropertyName.SecondaryDealRateTierNb]));

            if (secondaryTierNb.HasValue)
                secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.DealDurInSec], dealDurationInSeconds);
            else
            {
                secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.DealId], null);
                secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.DealZoneGroupNb], null);
                secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.DealDurInSec], null);
            }

            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealTierNb], null);
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealRateTierNb], null);
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealDurInSec], null);

            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.RateValue], rate);
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.Net], net);
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], pricedDurationInSeconds);

            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.CDRPartNb], cdrPartNb);
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.CDRPartDurInSec], durationInSeconds);
        }

        private void SendOutputData(IQueueActivatorExecutionContext context, DataRecordBatch mainTransformedBatch, DataRecordBatch partialPricedTransformedBatch,
            DataRecordBatch billingTransformedBatch, DataRecordBatch trafficTransformedBatch)
        {
            if (mainTransformedBatch != null && mainTransformedBatch.GetRecordCount() > 0 && this.MainOutputStages != null)
            {
                foreach (var mainOutputStage in this.MainOutputStages)
                {
                    context.OutputItems.Add(mainOutputStage, mainTransformedBatch);
                }
            }

            if (partialPricedTransformedBatch != null && partialPricedTransformedBatch.GetRecordCount() > 0 && this.PartialPricedOutputStages != null)
            {
                foreach (var partialPricedOutputStage in this.PartialPricedOutputStages)
                {
                    context.OutputItems.Add(partialPricedOutputStage, partialPricedTransformedBatch);
                }

            }

            if (billingTransformedBatch != null && billingTransformedBatch.GetRecordCount() > 0 && this.BillingOutputStages != null)
            {
                foreach (var billingOutputStage in this.BillingOutputStages)
                {
                    context.OutputItems.Add(billingOutputStage, billingTransformedBatch);
                }
            }

            if (trafficTransformedBatch != null && trafficTransformedBatch.GetRecordCount() > 0 && this.TrafficOutputStages != null)
            {
                foreach (var trafficOutputStage in this.TrafficOutputStages)
                {
                    context.OutputItems.Add(trafficOutputStage, trafficTransformedBatch);
                }
            }
        }

        #endregion

        #region IReprocessStageActivator

        public object InitializeStage(Vanrise.Reprocess.Entities.IReprocessStageActivatorInitializingContext context)
        {
            return null;
        }

        public void ExecuteStage(Vanrise.Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> saleDealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgressesByDate(true, null, context.To);
            Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> costDealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgressesByDate(false, null, context.To);

            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();

            Dictionary<DealZoneGroup, Dictionary<DateTime, SortedList<DealDetailedData, DealDuration>>> saleDealDetailedProgressesDict;
            Dictionary<DealZoneGroup, DealDetailedProgress> previousSaleDealDetailedProgressesDict;
            Dictionary<DealZoneGroup, Dictionary<DateTime, SortedList<DealDetailedData, DealDuration>>> costDealDetailedProgressesDict;
            Dictionary<DealZoneGroup, DealDetailedProgress> previousCostDealDetailedProgressesDict;
            BuildDealDetailedProgressDict(context.From, saleDealDetailedProgresses, out saleDealDetailedProgressesDict, out previousSaleDealDetailedProgressesDict);
            BuildDealDetailedProgressDict(context.From, costDealDetailedProgresses, out costDealDetailedProgressesDict, out previousCostDealDetailedProgressesDict);

            Dictionary<PropertyName, string> salePropertyNames = BuildPropertyNames("Sale", "Sale");
            Dictionary<PropertyName, string> costPropertyNames = BuildPropertyNames("Cost", "Supplier");

            QueueExecutionFlowStage queueExecutionFlowStage = context.QueueExecutionFlowStage;
            var queueItemType = queueExecutionFlowStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;

            List<string> validMainOutputStages = BuildValidOutputStage(context.StageNames, this.MainOutputStages);
            List<string> validPartialPricedOutputStages = BuildValidOutputStage(context.StageNames, this.PartialPricedOutputStages);
            List<string> validBillingOutputStages = BuildValidOutputStage(context.StageNames, this.BillingOutputStages);
            List<string> validTrafficOutputStages = BuildValidOutputStage(context.StageNames, this.TrafficOutputStages);

            context.DoWhilePreviousRunning(() =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = context.InputQueue.TryDequeue((reprocessBatch) =>
                    {
                        GenericDataRecordBatch genericDataRecordBatch = reprocessBatch as GenericDataRecordBatch;
                        if (genericDataRecordBatch == null)
                            throw new Exception(String.Format("reprocessBatch should be of type 'Reprocess.Entities.GenericDataRecordBatch'. and not of type '{0}'", reprocessBatch.GetType()));

                        List<dynamic> mainCDRs = new List<dynamic>();
                        List<dynamic> partialPricedCDRs = new List<dynamic>();
                        List<dynamic> billingRecords = new List<dynamic>(); //for billing stats and traffic stats
                        List<dynamic> trafficRecords = new List<dynamic>(); //for traffic stats

                        foreach (var record in genericDataRecordBatch.Records)
                        {
                            int cdrType = record.Type;
                            if (!cdrTypesToEvaluate.Contains(cdrType))
                            {
                                trafficRecords.Add(record);
                                continue;
                            }

                            dealDefinitionManager.FillOrigSaleDealValues(record);
                            dealDefinitionManager.FillOrigCostDealValues(record);

                            decimal? recordSaleRateId = record.OrigSaleRateId;
                            decimal? recordSaleCurrencyId = record.OrigSaleCurrencyId;
                            decimal? recordCostRateId = record.OrigCostRateId;
                            decimal? recordCostCurrencyId = record.OrigCostCurrencyId;

                            bool saleValid = recordSaleRateId.HasValue && recordSaleCurrencyId.HasValue;
                            bool costValid = recordCostRateId.HasValue && recordCostCurrencyId.HasValue;

                            if (!saleValid && !costValid)
                                throw new VRBusinessException(string.Format("Sale Part and Cost Part doesn't exist for CDR Id: {0}", record.CDRId));

                            if (saleValid && costValid)
                                mainCDRs.Add(record);
                            else
                                partialPricedCDRs.Add(record);

                            CDRPricingDataOutput firstSaleCDR = null;
                            CDRPricingDataOutput secondSaleCDR = null;

                            CDRPricingDataOutput firstCostCDR = null;
                            CDRPricingDataOutput secondCostCDR = null;

                            int intervalOffsetInMinutes = new TOne.WhS.Deal.Business.ConfigManager().GetDealTechnicalSettingIntervalOffsetInMinutes();
                            DateTime attemptDateTime = record.GetFieldValue(salePropertyNames[PropertyName.AttemptDateTime]);//same value for sale and cost
                            DateTime batchStart = new DateTime(attemptDateTime.Year, attemptDateTime.Month, attemptDateTime.Day, attemptDateTime.Hour, ((int)(attemptDateTime.Minute / intervalOffsetInMinutes)) * intervalOffsetInMinutes, 0);

                            int? saleOrigDealId = record.OrigSaleDealId;
                            int? saleOrigZoneGroupNb = record.OrigSaleDealZoneGroupNb;
                            if (saleOrigDealId.HasValue && saleOrigZoneGroupNb.HasValue)
                            {
                                var saleZoneId = record.GetFieldValue(salePropertyNames[PropertyName.Zone]);
                                DealZoneGroup saleDealZoneGroup = new DealZoneGroup() { DealId = saleOrigDealId.Value, ZoneGroupNb = saleOrigZoneGroupNb.Value };
                                Func<int, DealZoneGroupTierDetails> getSaleDealZoneGroupTierDetails = GetSaleDealZoneGroupTierDetails(saleDealZoneGroup, saleZoneId, attemptDateTime);
                                UpdateReprocessBillingCDRData(saleDealDetailedProgressesDict.GetRecord(saleDealZoneGroup), previousSaleDealDetailedProgressesDict.GetRecord(saleDealZoneGroup), record, true, salePropertyNames, batchStart, getSaleDealZoneGroupTierDetails, saleDealZoneGroup, context.From, out firstSaleCDR, out secondSaleCDR);
                            }
                            else
                            {
                                FillDataFromOriginalValues(record, salePropertyNames);
                            }

                            int? costOrigDealId = record.OrigCostDealId;
                            int? costOrigZoneGroupNb = record.OrigCostDealZoneGroupNb;
                            if (costOrigDealId.HasValue && costOrigZoneGroupNb.HasValue)
                            {
                                var supplierZoneId = record.GetFieldValue(costPropertyNames[PropertyName.Zone]);
                                DealZoneGroup costDealZoneGroup = new DealZoneGroup() { DealId = costOrigDealId.Value, ZoneGroupNb = costOrigZoneGroupNb.Value };
                                Func<int, DealZoneGroupTierDetails> getSupplierDealZoneGroupTierDetails = GetSupplierDealZoneGroupTierDetails(costDealZoneGroup, supplierZoneId, attemptDateTime);
                                UpdateReprocessBillingCDRData(costDealDetailedProgressesDict.GetRecord(costDealZoneGroup), previousCostDealDetailedProgressesDict.GetRecord(costDealZoneGroup), record, false, costPropertyNames, batchStart, getSupplierDealZoneGroupTierDetails, costDealZoneGroup, context.From, out firstCostCDR, out secondCostCDR);
                            }
                            else
                            {
                                FillDataFromOriginalValues(record, costPropertyNames);
                            }
                            CreateBillingStatsCDRRecords(billingRecords, record, salePropertyNames, firstSaleCDR, secondSaleCDR, costPropertyNames, firstCostCDR, secondCostCDR, recordTypeId);
                        }

                        if (validMainOutputStages != null && mainCDRs.Count > 0)
                        {
                            Vanrise.Reprocess.Entities.GenericDataRecordBatch mainGenericDataRecordBatch = new Vanrise.Reprocess.Entities.GenericDataRecordBatch() { Records = mainCDRs };
                            foreach (var mainOutputStageName in validMainOutputStages)
                            {
                                context.EnqueueBatch(mainOutputStageName, mainGenericDataRecordBatch);
                            }
                        }

                        if (validPartialPricedOutputStages != null && partialPricedCDRs.Count > 0)
                        {
                            Vanrise.Reprocess.Entities.GenericDataRecordBatch partialPricedGenericDataRecordBatch = new Vanrise.Reprocess.Entities.GenericDataRecordBatch() { Records = partialPricedCDRs };
                            foreach (var partialPricedOutputStageName in validPartialPricedOutputStages)
                            {
                                context.EnqueueBatch(partialPricedOutputStageName, partialPricedGenericDataRecordBatch);
                            }
                        }

                        if (validBillingOutputStages != null && billingRecords.Count > 0)
                        {
                            Vanrise.Reprocess.Entities.GenericDataRecordBatch billingGenericDataRecordBatch = new Vanrise.Reprocess.Entities.GenericDataRecordBatch() { Records = billingRecords };
                            foreach (var billingOutputStageName in validBillingOutputStages)
                            {
                                context.EnqueueBatch(billingOutputStageName, billingGenericDataRecordBatch);
                            }
                        }

                        if (validTrafficOutputStages != null && (billingRecords.Count > 0 || trafficRecords.Count > 0))
                        {
                            Vanrise.Reprocess.Entities.GenericDataRecordBatch trafficGenericDataRecordBatch = new Vanrise.Reprocess.Entities.GenericDataRecordBatch() { Records = billingRecords.Union(trafficRecords).ToList() };
                            foreach (var trafficOutputStageName in validTrafficOutputStages)
                            {
                                context.EnqueueBatch(trafficOutputStageName, trafficGenericDataRecordBatch);
                            }
                        }
                    });
                } while (!context.ShouldStop() && hasItem);

            });
        }

        private void UpdateReprocessBillingCDRData(Dictionary<DateTime, SortedList<DealDetailedData, DealDuration>> currentdealDetailedProgresses, DealDetailedProgress previousDealDetailedProgress,
            dynamic record, bool isSale, Dictionary<PropertyName, string> propertyNames, DateTime batchStart, Func<int, DealZoneGroupTierDetails> getDealZoneGroupTierDetails, DealZoneGroup dealZoneGroup,
            DateTime firstBatchStart, out CDRPricingDataOutput firstPartOutput, out CDRPricingDataOutput secondPartOutput)
        {
            firstPartOutput = null; secondPartOutput = null;
            SortedList<DealDetailedData, DealDuration> dealDetailedProgressList = currentdealDetailedProgresses.GetRecord(batchStart);

            decimal pricedDurationInSeconds = record.GetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds]);
            if (dealDetailedProgressList != null && dealDetailedProgressList.Count > 0)
            {
                var firstDealDetailedProgressItem = dealDetailedProgressList.First();
                DealDetailedData dealDetailedProgress = firstDealDetailedProgressItem.Key;
                DealDuration dealDuration = firstDealDetailedProgressItem.Value;

                if (dealDetailedProgressList.Count == 1 || (dealDetailedProgress.ReachedDurationInSeconds - dealDuration.AssignedDurationInSeconds) >= pricedDurationInSeconds)
                {
                    if (dealDetailedProgress.TierNb.HasValue)
                    {
                        DealZoneGroupTierDetails currentDealZoneGroupTierDetails = getDealZoneGroupTierDetails(dealDetailedProgress.RateTierNb.Value);
                        if (currentDealZoneGroupTierDetails != null)
                        {
                            SetPrimaryDealData(record, propertyNames, dealZoneGroup, dealDetailedProgress.TierNb.Value, dealDetailedProgress.RateTierNb.Value, pricedDurationInSeconds);
                            SetSecondaryDealData(record, propertyNames, null, null, null);
                            CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealDetailedProgress.DealId, pricedDurationInSeconds, 100, currentDealZoneGroupTierDetails, currentDealZoneGroupTierDetails.CurrencyId);
                            SetPricingData(record, propertyNames, firstPartInput, null, out firstPartOutput, out secondPartOutput);
                        }
                        else
                        {
                            FillDataFromOriginalValues(record, propertyNames);
                            return;
                        }
                    }
                    else
                    {
                        FillDataFromOriginalValues(record, propertyNames);
                    }

                    dealDuration.AssignedDurationInSeconds += pricedDurationInSeconds;
                }
                else
                {
                    decimal firstTierDurationInSeconds = dealDetailedProgress.ReachedDurationInSeconds - dealDuration.AssignedDurationInSeconds;
                    CDRPricingDataInput firstPartInput = null;

                    if (firstTierDurationInSeconds > 0)
                    {
                        DealZoneGroupTierDetails currentDealZoneGroupTierDetails = getDealZoneGroupTierDetails(dealDetailedProgress.RateTierNb.Value);
                        if (currentDealZoneGroupTierDetails != null)
                        {
                            SetPrimaryDealData(record, propertyNames, dealZoneGroup, dealDetailedProgress.TierNb.Value, dealDetailedProgress.RateTierNb.Value, firstTierDurationInSeconds);
                            firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealDetailedProgress.DealId, firstTierDurationInSeconds, firstTierDurationInSeconds / pricedDurationInSeconds * 100, currentDealZoneGroupTierDetails, currentDealZoneGroupTierDetails.CurrencyId);
                        }
                        else
                        {
                            FillDataFromOriginalValues(record, propertyNames);
                            return;
                        }
                    }

                    decimal remainingTierDurationInSeconds = pricedDurationInSeconds - firstTierDurationInSeconds;
                    dealDetailedProgressList.RemoveAt(0);

                    firstDealDetailedProgressItem = dealDetailedProgressList.First();
                    dealDetailedProgress = firstDealDetailedProgressItem.Key;
                    dealDuration = firstDealDetailedProgressItem.Value;

                    if (firstTierDurationInSeconds > 0)
                    {
                        CDRPricingDataInput secondPartInput;
                        if (dealDetailedProgress.TierNb.HasValue)
                        {
                            DealZoneGroupTierDetails currentDealZoneGroupTierDetails = getDealZoneGroupTierDetails(dealDetailedProgress.RateTierNb.Value);
                            if (currentDealZoneGroupTierDetails != null)
                            {
                                SetSecondaryDealData(record, propertyNames, remainingTierDurationInSeconds, dealDetailedProgress.TierNb.Value, dealDetailedProgress.RateTierNb.Value);
                                secondPartInput = BuildCDRPricingDataInput(record, propertyNames, dealDetailedProgress.DealId, remainingTierDurationInSeconds, remainingTierDurationInSeconds / pricedDurationInSeconds * 100, currentDealZoneGroupTierDetails, currentDealZoneGroupTierDetails.CurrencyId);
                            }
                            else
                            {
                                SetSecondaryDealData(record, propertyNames, remainingTierDurationInSeconds, null, null);
                                int recordCurrencyId = record.GetFieldValue(propertyNames[PropertyName.CurrencyId]);
                                secondPartInput = BuildCDRPricingDataInput(record, propertyNames, dealDetailedProgress.DealId, remainingTierDurationInSeconds, remainingTierDurationInSeconds / pricedDurationInSeconds * 100, null, recordCurrencyId);
                            }
                        }
                        else
                        {
                            SetSecondaryDealData(record, propertyNames, remainingTierDurationInSeconds, null, null);
                            int recordCurrencyId = record.GetFieldValue(propertyNames[PropertyName.CurrencyId]);
                            secondPartInput = BuildCDRPricingDataInput(record, propertyNames, dealDetailedProgress.DealId, remainingTierDurationInSeconds, remainingTierDurationInSeconds / pricedDurationInSeconds * 100, null, recordCurrencyId);
                        }
                        SetPricingData(record, propertyNames, firstPartInput, secondPartInput, out firstPartOutput, out secondPartOutput);
                        dealDuration.AssignedDurationInSeconds += remainingTierDurationInSeconds;
                    }
                    else
                    {
                        if (dealDetailedProgress.TierNb.HasValue)
                        {
                            UpdateReprocessBillingCDRData(currentdealDetailedProgresses, previousDealDetailedProgress, record, isSale, propertyNames, batchStart, getDealZoneGroupTierDetails,
                                dealZoneGroup, firstBatchStart, out firstPartOutput, out secondPartOutput);
                        }
                        else
                        {
                            FillDataFromOriginalValues(record, propertyNames);
                            dealDuration.AssignedDurationInSeconds += pricedDurationInSeconds;
                        }
                    }
                }
            }
            else
            {
                int intervalOffsetInMinutes = new TOne.WhS.Deal.Business.ConfigManager().GetDealTechnicalSettingIntervalOffsetInMinutes();
                DateTime currentBatchStart = batchStart.AddMinutes(-1 * intervalOffsetInMinutes);
                bool tierFound = false;
                while (!tierFound && currentBatchStart >= firstBatchStart)
                {
                    var item = currentdealDetailedProgresses.GetRecord(currentBatchStart);
                    if (item == null)
                    {
                        currentBatchStart = currentBatchStart.AddMinutes(-1 * intervalOffsetInMinutes);
                        continue;
                    }

                    tierFound = true;
                    var lastDealDetailedProgressItem = item.Last();
                    DealDetailedData lastDealDetailedProgress = lastDealDetailedProgressItem.Key;

                    if (lastDealDetailedProgress.TierNb.HasValue)
                    {
                        DealZoneGroupTierDetails lastDealZoneGroupTierDetails = getDealZoneGroupTierDetails(lastDealDetailedProgress.RateTierNb.Value);
                        if (lastDealZoneGroupTierDetails != null)
                        {
                            SetPrimaryDealData(record, propertyNames, dealZoneGroup, lastDealDetailedProgress.TierNb.Value, lastDealDetailedProgress.RateTierNb.Value, pricedDurationInSeconds);
                            SetSecondaryDealData(record, propertyNames, null, null, null);
                            CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealZoneGroup.DealId, pricedDurationInSeconds, 100, lastDealZoneGroupTierDetails, lastDealZoneGroupTierDetails.CurrencyId);
                            SetPricingData(record, propertyNames, firstPartInput, null, out firstPartOutput, out secondPartOutput);
                        }
                        else
                        {
                            FillDataFromOriginalValues(record, propertyNames);
                        }
                    }
                    else
                    {
                        FillDataFromOriginalValues(record, propertyNames);
                    }
                }

                if (!tierFound)
                {
                    if (previousDealDetailedProgress != null)
                    {
                        if (previousDealDetailedProgress.TierNb.HasValue)
                        {
                            DealZoneGroupTierDetails currentDealZoneGroupTierDetails = getDealZoneGroupTierDetails(previousDealDetailedProgress.RateTierNb.Value);
                            if (currentDealZoneGroupTierDetails != null)
                            {
                                SetPrimaryDealData(record, propertyNames, dealZoneGroup, previousDealDetailedProgress.TierNb.Value, previousDealDetailedProgress.RateTierNb.Value, pricedDurationInSeconds);
                                SetSecondaryDealData(record, propertyNames, null, null, null);
                                CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealZoneGroup.DealId, pricedDurationInSeconds, 100, currentDealZoneGroupTierDetails, currentDealZoneGroupTierDetails.CurrencyId);
                                SetPricingData(record, propertyNames, firstPartInput, null, out firstPartOutput, out secondPartOutput);
                            }
                            else
                            {
                                FillDataFromOriginalValues(record, propertyNames);
                            }
                        }
                        else
                        {
                            FillDataFromOriginalValues(record, propertyNames);
                        }
                    }
                    else
                    {
                        DealZoneGroupTierDetails currentDealZoneGroupTierDetails = getDealZoneGroupTierDetails(1);
                        if (currentDealZoneGroupTierDetails != null)
                        {
                            SetPrimaryDealData(record, propertyNames, dealZoneGroup, currentDealZoneGroupTierDetails.TierNb, currentDealZoneGroupTierDetails.TierNb, pricedDurationInSeconds);
                            SetSecondaryDealData(record, propertyNames, null, null, null);
                            CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealZoneGroup.DealId, pricedDurationInSeconds, 100, currentDealZoneGroupTierDetails, currentDealZoneGroupTierDetails.CurrencyId);
                            SetPricingData(record, propertyNames, firstPartInput, null, out firstPartOutput, out secondPartOutput);
                        }
                        else
                        {
                            FillDataFromOriginalValues(record, propertyNames);
                        }
                    }
                }
            }
        }

        private void FillDataFromOriginalValues(dynamic record, Dictionary<PropertyName, string> propertyNames)
        {
            record.SetFieldValue(propertyNames[PropertyName.RateId], record.GetFieldValue(propertyNames[PropertyName.OrigRateId]));
            record.SetFieldValue(propertyNames[PropertyName.RateValue], record.GetFieldValue(propertyNames[PropertyName.OrigRateValue]));
            record.SetFieldValue(propertyNames[PropertyName.Net], record.GetFieldValue(propertyNames[PropertyName.OrigNet]));
            record.SetFieldValue(propertyNames[PropertyName.ExtraChargeRateValue], record.GetFieldValue(propertyNames[PropertyName.OrigExtraChargeRateValue]));
            record.SetFieldValue(propertyNames[PropertyName.ExtraChargeValue], record.GetFieldValue(propertyNames[PropertyName.OrigExtraChargeValue]));
            record.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], record.GetFieldValue(propertyNames[PropertyName.OrigPricedDurationInSeconds]));
            record.SetFieldValue(propertyNames[PropertyName.CurrencyId], record.GetFieldValue(propertyNames[PropertyName.OrigCurrencyId]));

            record.SetFieldValue(propertyNames[PropertyName.SecondaryDealTierNb], null);
            record.SetFieldValue(propertyNames[PropertyName.SecondaryDealRateTierNb], null);
            record.SetFieldValue(propertyNames[PropertyName.SecondaryDealDurInSec], null);

            record.SetFieldValue(propertyNames[PropertyName.DealId], null);
            record.SetFieldValue(propertyNames[PropertyName.DealZoneGroupNb], null);
            record.SetFieldValue(propertyNames[PropertyName.DealTierNb], null);
            record.SetFieldValue(propertyNames[PropertyName.DealRateTierNb], null);
            record.SetFieldValue(propertyNames[PropertyName.DealDurInSec], null);
        }

        private class DealDetailedDataComparer : IComparer<DealDetailedData>
        {
            public int Compare(DealDetailedData firstDealDetailedData, DealDetailedData secondDealDetailedData)
            {
                if (!firstDealDetailedData.TierNb.HasValue)
                    return 1;

                if (!secondDealDetailedData.TierNb.HasValue)
                    return -1;

                return firstDealDetailedData.TierNb.Value < secondDealDetailedData.TierNb.Value ? -1 : 1;
            }
        }

        private void BuildDealDetailedProgressDict(DateTime firstBatchStart, Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses,
            out Dictionary<DealZoneGroup, Dictionary<DateTime, SortedList<DealDetailedData, DealDuration>>> currentDealDetailedProgressesDict,
            out Dictionary<DealZoneGroup, DealDetailedProgress> previousDealDetailedProgressesDict)
        {
            currentDealDetailedProgressesDict = null;
            previousDealDetailedProgressesDict = null;

            if (dealDetailedProgresses == null)
                return;

            currentDealDetailedProgressesDict = new Dictionary<DealZoneGroup, Dictionary<DateTime, SortedList<DealDetailedData, DealDuration>>>();
            previousDealDetailedProgressesDict = new Dictionary<DealZoneGroup, DealDetailedProgress>();

            foreach (var dealDetailedProgressItem in dealDetailedProgresses)
            {
                DealDetailedZoneGroupTier dealDetailedZoneGroupTier = dealDetailedProgressItem.Key;
                DealDetailedProgress dealDetailedProgress = dealDetailedProgressItem.Value;
                DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = dealDetailedZoneGroupTier.DealId, ZoneGroupNb = dealDetailedZoneGroupTier.ZoneGroupNb };

                if (dealDetailedZoneGroupTier.FromTime >= firstBatchStart)
                {
                    Dictionary<DateTime, SortedList<DealDetailedData, DealDuration>> dealDetailedZoneGroupTierByDate = currentDealDetailedProgressesDict.GetOrCreateItem(dealZoneGroup);
                    SortedList<DealDetailedData, DealDuration> dealDetailedProgressList = dealDetailedZoneGroupTierByDate.GetOrCreateItem(dealDetailedZoneGroupTier.FromTime, () => { return new SortedList<DealDetailedData, DealDuration>(new DealDetailedDataComparer()); });
                    dealDetailedProgressList.Add(new DealDetailedData() { DealId = dealDetailedProgress.DealId, ReachedDurationInSeconds = dealDetailedProgress.ReachedDurationInSeconds, TierNb = dealDetailedProgress.TierNb, RateTierNb = dealDetailedProgress.RateTierNb }, new DealDuration() { AssignedDurationInSeconds = 0 });
                }
                else
                {
                    DealDetailedProgress previousDealDetailedProgress;
                    if (previousDealDetailedProgressesDict.TryGetValue(dealZoneGroup, out previousDealDetailedProgress))
                    {
                        if (!previousDealDetailedProgress.TierNb.HasValue)
                            continue;

                        if (!dealDetailedProgress.TierNb.HasValue)
                        {
                            previousDealDetailedProgressesDict[dealZoneGroup] = dealDetailedProgress;
                            continue;
                        }

                        if (previousDealDetailedProgress.TierNb.Value < dealDetailedProgress.TierNb.Value)
                            previousDealDetailedProgressesDict[dealZoneGroup] = dealDetailedProgress;
                    }
                    else
                    {
                        previousDealDetailedProgressesDict.Add(dealZoneGroup, dealDetailedProgress);
                    }
                }
            }
        }

        private List<string> BuildValidOutputStage(List<string> currentStages, List<string> outputStagesToCheck)
        {
            if (outputStagesToCheck != null && currentStages != null)
            {
                List<string> validOutputStages = new List<string>();
                foreach (var stageToCheck in outputStagesToCheck)
                {
                    if (currentStages.Contains(stageToCheck))
                        validOutputStages.Add(stageToCheck);
                }
                return validOutputStages.Count > 0 ? validOutputStages : null;
            }
            return null;
        }

        public void FinalizeStage(Vanrise.Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {
        }

        public int? GetStorageRowCount(Vanrise.Reprocess.Entities.IReprocessStageActivatorGetStorageRowCountContext context)
        {
            return null;
        }

        public void CommitChanges(Vanrise.Reprocess.Entities.IReprocessStageActivatorCommitChangesContext context)
        {
        }

        public void DropStorage(Vanrise.Reprocess.Entities.IReprocessStageActivatorDropStorageContext context)
        {
        }

        public List<string> GetOutputStages(List<string> stageNames)
        {
            if (MainOutputStages == null && PartialPricedOutputStages == null && BillingOutputStages == null && TrafficOutputStages == null)
                return null;

            if (stageNames == null)
                return null;

            List<string> allStages = new List<string>();
            if (MainOutputStages != null)
                allStages.AddRange(MainOutputStages);

            if (PartialPricedOutputStages != null)
                allStages.AddRange(PartialPricedOutputStages);

            if (BillingOutputStages != null)
                allStages.AddRange(BillingOutputStages);

            if (TrafficOutputStages != null)
                allStages.AddRange(TrafficOutputStages);

            Func<string, bool> filterExpression = (itemObject) => stageNames.Contains(itemObject);

            IEnumerable<string> filteredStages = allStages.FindAllRecords(filterExpression);
            return filteredStages != null ? filteredStages.ToList() : null;
        }

        public Vanrise.Queueing.BaseQueue<Vanrise.Reprocess.Entities.IReprocessBatch> GetQueue()
        {
            return new Vanrise.Queueing.MemoryQueue<Vanrise.Reprocess.Entities.IReprocessBatch>();
        }

        public List<Vanrise.Reprocess.Entities.BatchRecord> GetStageBatchRecords(Vanrise.Reprocess.Entities.IReprocessStageActivatorPreparingContext context)
        {
            return null;
        }

        #endregion

        #region Private Classes

        private enum PropertyName
        {
            DurationInSeconds, PricedDurationInSeconds, DealId, DealZoneGroupNb, DealTierNb, DealRateTierNb, DealDurInSec, SecondaryDealTierNb,
            SecondaryDealRateTierNb, SecondaryDealDurInSec, RateId, RateValue, Net, TariffRuleId, CurrencyId, ExtraChargeRateValue, ExtraChargeValue, Zone, AttemptDateTime,
            CDRPartDurInSec, CDRPartNb, OrigRateId, OrigRateValue, OrigNet, OrigExtraChargeRateValue, OrigExtraChargeValue, OrigPricedDurationInSeconds, OrigCurrencyId
        }

        private struct DealDetailedData
        {
            public int DealId { get; set; }
            public int? TierNb { get; set; }
            public int? RateTierNb { get; set; }
            public decimal ReachedDurationInSeconds { get; set; }
        }

        private class DealDuration
        {
            public decimal AssignedDurationInSeconds { get; set; }
        }

        private class CDRPricingDataOutput
        {
            public decimal? DealId { get; set; }
            public decimal PricedDurationInSeconds { get; set; }
            public decimal Net { get; set; }
            public decimal Percentage { get; set; }
            public decimal Rate { get; set; }
        }

        private class CDRPricingDataInput
        {
            public decimal? DealId { get; set; }
            public decimal Rate { get; set; }
            public decimal Percentage { get; set; }
            public decimal PricedDurationInSeconds { get; set; }
            public int CurrencyId { get; set; }
        }

        #endregion
    }
}