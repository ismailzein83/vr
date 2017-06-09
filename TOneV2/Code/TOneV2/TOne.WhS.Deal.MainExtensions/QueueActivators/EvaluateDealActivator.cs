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

            List<dynamic> billingRecords = new List<dynamic>();

            foreach (var record in batchRecords)
            {
                CDRPricingDataOutput firstSaleCDR = null;
                CDRPricingDataOutput secondSaleCDR = null;

                CDRPricingDataOutput firstCostCDR = null;
                CDRPricingDataOutput secondCostCDR = null;

                int? saleOrigDealId = record.OrigSaleDealId;
                int? saleOrigZoneGroupNb = record.OrigSaleDealZoneGroupNb;
                if (saleOrigDealId.HasValue && saleOrigZoneGroupNb.HasValue)
                {
                    DealZoneGroup saleDealZoneGroup = new DealZoneGroup() { DealId = saleOrigDealId.Value, ZoneGroupNb = saleOrigZoneGroupNb.Value };
                    Func<int, DealZoneGroupTier> getSaleDealZoneGroupTier = GetSaleDealZoneGroupTier(saleDealZoneGroup);
                    UpdateBillingCDRData(saleDealProgresses, saleDealZoneGroup, record, true, salePropertyNames, getSaleDealZoneGroupTier, newSaleDealProgresses, out firstSaleCDR, out secondSaleCDR);
                }

                int? costOrigDealId = record.OrigCostDealId;
                int? costOrigZoneGroupNb = record.OrigCostDealZoneGroupNb;
                if (costOrigDealId.HasValue && costOrigZoneGroupNb.HasValue)
                {
                    DealZoneGroup costDealZoneGroup = new DealZoneGroup() { DealId = costOrigDealId.Value, ZoneGroupNb = costOrigZoneGroupNb.Value };
                    Func<int, DealZoneGroupTier> getSupplierDealZoneGroupTier = GetSupplierDealZoneGroupTier(costDealZoneGroup);
                    UpdateBillingCDRData(costDealProgresses, costDealZoneGroup, record, false, costPropertyNames, getSupplierDealZoneGroupTier, newCostDealProgresses, out firstCostCDR, out secondCostCDR);
                }
                CreateBillingCDRRecords(billingRecords, record, firstSaleCDR, secondSaleCDR, firstCostCDR, secondCostCDR, salePropertyNames, costPropertyNames, recordTypeId);
            }

            dealProgressManager.UpdateDealProgresses(existingSaleDealProgresses.Union(existingCostDealProgresses));
            dealProgressManager.InsertDealProgresses(newSaleDealProgresses.Union(newCostDealProgresses));

            DataRecordBatch mainTransformedBatch = DataRecordBatch.CreateBatchFromRecords(mainCDRs, queueItemType.BatchDescription, recordTypeId);
            DataRecordBatch partialPricedTransformedBatch = DataRecordBatch.CreateBatchFromRecords(partialPricedCDRs, queueItemType.BatchDescription, recordTypeId);
            DataRecordBatch billingTransformedBatch = DataRecordBatch.CreateBatchFromRecords(billingRecords, queueItemType.BatchDescription, recordTypeId);

            SendOutputData(context, mainTransformedBatch, partialPricedTransformedBatch, billingTransformedBatch);
        }

        private void CreateBillingCDRRecords(List<dynamic> billingRecords, dynamic record, CDRPricingDataOutput firstSaleCDR, CDRPricingDataOutput secondSaleCDR, CDRPricingDataOutput firstCostCDR,
            CDRPricingDataOutput secondCostCDR, Dictionary<PropertyName, string> salePropertyNames, Dictionary<PropertyName, string> costPropertyNames, Guid recordTypeId)
        {
            dynamic billingCDRRecord = record.CloneRecord(recordTypeId);
            if (firstSaleCDR == null && firstCostCDR == null)
            {
                billingRecords.Add(billingCDRRecord);
            }

            else if (firstSaleCDR != null && firstCostCDR == null)
            {
                AddBillingCDRs(billingCDRRecord, billingRecords, salePropertyNames, firstSaleCDR, secondSaleCDR, recordTypeId);
            }

            else if (firstSaleCDR == null && firstCostCDR != null)
            {
                AddBillingCDRs(billingCDRRecord, billingRecords, costPropertyNames, firstCostCDR, secondCostCDR, recordTypeId);
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

        private void AddBillingCDRs(dynamic billingCDRRecord, List<dynamic> billingRecords, Dictionary<PropertyName, string> salePropertyNames, CDRPricingDataOutput firstSaleCDR,
            CDRPricingDataOutput secondSaleCDR, Dictionary<PropertyName, string> costPropertyNames, CDRPricingDataOutput firstCostCDR, CDRPricingDataOutput secondCostCDR, Guid recordTypeId)
        {
            dynamic secondBillingCDRRecord = billingCDRRecord.CloneRecord(recordTypeId);
            dynamic thirdBillingCDRRecord = billingCDRRecord.CloneRecord(recordTypeId);

            decimal totalSaleCDRPricedDuration = firstSaleCDR.PricedDurationInSeconds + secondSaleCDR.PricedDurationInSeconds;
            decimal firstSaleCDRRatio = firstSaleCDR.PricedDurationInSeconds / totalSaleCDRPricedDuration;
            decimal secondSaleCDRRatio = secondSaleCDR.PricedDurationInSeconds / totalSaleCDRPricedDuration;

            decimal totalCostCDRPricedDuration = firstCostCDR.PricedDurationInSeconds + secondCostCDR.PricedDurationInSeconds;
            decimal firstCostCDRRatio = firstCostCDR.PricedDurationInSeconds / totalCostCDRPricedDuration;
            decimal secondCostCDRRatio = secondCostCDR.PricedDurationInSeconds / totalCostCDRPricedDuration;

            decimal durationInSeconds = firstCostCDR.DurationInSeconds + secondCostCDR.DurationInSeconds;

            if (firstSaleCDRRatio > firstCostCDRRatio)
            {
                decimal firstDurationInSeconds = firstCostCDR.PricedDurationInSeconds * durationInSeconds / totalCostCDRPricedDuration;
                SetPrimaryBillingCDRRecordData(billingCDRRecord, costPropertyNames, firstCostCDR.Net, firstDurationInSeconds, firstCostCDR.PricedDurationInSeconds, 1);

                decimal firstSaleCDRPricedDuration = firstCostCDR.PricedDurationInSeconds * totalSaleCDRPricedDuration / totalCostCDRPricedDuration;
                decimal firstSaleCDRNet = firstSaleCDR.Net * firstSaleCDRPricedDuration / firstSaleCDR.PricedDurationInSeconds;
                SetPrimaryBillingCDRRecordData(billingCDRRecord, salePropertyNames, firstSaleCDRNet, firstDurationInSeconds, firstSaleCDRPricedDuration, 1);

                decimal secondSaleCDRPricedDuration = firstSaleCDR.PricedDurationInSeconds - firstSaleCDRPricedDuration;
                decimal secondSaleCDRNet = firstSaleCDR.Net - firstSaleCDRNet;
                decimal secondDurationInSeconds = secondSaleCDRPricedDuration * durationInSeconds / totalSaleCDRPricedDuration;
                SetPrimaryBillingCDRRecordData(secondBillingCDRRecord, salePropertyNames, secondSaleCDRNet, secondDurationInSeconds, secondSaleCDRPricedDuration, 2);


                decimal secondCostCDRPricedDuration = secondSaleCDRPricedDuration * totalCostCDRPricedDuration / totalSaleCDRPricedDuration;
                decimal secondCostCDRNet = secondCostCDR.Net * secondCostCDRPricedDuration / secondCostCDR.PricedDurationInSeconds;
                SetSecondaryBillingCDRRecordData(secondBillingCDRRecord, costPropertyNames, secondCostCDRNet, secondDurationInSeconds, secondCostCDRPricedDuration, 2);

                decimal? costExtraChargeValue = secondBillingCDRRecord.GetFieldValue(costPropertyNames[PropertyName.ExtraChargeValue]);
                decimal secondCostCDRExtaChargeValue = 0;
                if (costExtraChargeValue.HasValue)
                {
                    secondCostCDRExtaChargeValue = costExtraChargeValue.Value * secondCostCDRPricedDuration / secondCostCDR.PricedDurationInSeconds;
                    secondBillingCDRRecord.SetFieldValue(costPropertyNames[PropertyName.ExtraChargeValue], secondCostCDRExtaChargeValue);
                }

                decimal thirdDurationInSeconds = durationInSeconds - firstDurationInSeconds - secondDurationInSeconds;
                SetSecondaryBillingCDRRecordData(thirdBillingCDRRecord, salePropertyNames, secondSaleCDR.Net, thirdDurationInSeconds, secondSaleCDR.PricedDurationInSeconds, 3);

                decimal thirdCostCDRNet = secondCostCDR.Net - secondCostCDRNet;
                decimal thirdCostCDRPricedDuration = secondCostCDR.PricedDurationInSeconds - secondCostCDRPricedDuration;
                SetSecondaryBillingCDRRecordData(thirdBillingCDRRecord, costPropertyNames, thirdCostCDRNet, thirdDurationInSeconds, thirdCostCDRPricedDuration, 3);

                if (costExtraChargeValue.HasValue)
                {
                    decimal thirdCostCDRExtaChargeValue = costExtraChargeValue.Value - secondCostCDRExtaChargeValue;
                    thirdBillingCDRRecord.SetFieldValue(costPropertyNames[PropertyName.ExtraChargeValue], thirdCostCDRExtaChargeValue);
                }
            }
            else
            {
                decimal firstDurationInSeconds = firstSaleCDR.PricedDurationInSeconds * durationInSeconds / totalSaleCDRPricedDuration;
                SetPrimaryBillingCDRRecordData(billingCDRRecord, salePropertyNames, firstSaleCDR.Net, firstDurationInSeconds, firstSaleCDR.PricedDurationInSeconds, 1);

                decimal firstCostCDRPricedDuration = firstSaleCDR.PricedDurationInSeconds * totalCostCDRPricedDuration / totalSaleCDRPricedDuration;
                decimal firstCostCDRNet = firstCostCDR.Net * firstCostCDRPricedDuration / firstCostCDR.PricedDurationInSeconds;
                SetPrimaryBillingCDRRecordData(billingCDRRecord, costPropertyNames, firstCostCDRNet, firstDurationInSeconds, firstCostCDRPricedDuration, 1);

                decimal secondCostCDRPricedDuration = firstCostCDR.PricedDurationInSeconds - firstCostCDRPricedDuration;
                decimal secondCostCDRNet = firstCostCDR.Net - firstCostCDRNet;
                decimal secondDurationInSeconds = secondCostCDRPricedDuration * durationInSeconds / totalSaleCDRPricedDuration;
                SetPrimaryBillingCDRRecordData(secondBillingCDRRecord, costPropertyNames, secondCostCDRNet, secondDurationInSeconds, secondCostCDRPricedDuration, 2);


                decimal secondSaleCDRPricedDuration = secondCostCDRPricedDuration * totalSaleCDRPricedDuration / totalCostCDRPricedDuration;
                decimal secondSaleCDRNet = secondSaleCDR.Net * secondSaleCDRPricedDuration / secondSaleCDR.PricedDurationInSeconds;
                SetSecondaryBillingCDRRecordData(secondBillingCDRRecord, salePropertyNames, secondSaleCDRNet, secondDurationInSeconds, secondSaleCDRPricedDuration, 2);

                decimal? saleExtraChargeValue = secondBillingCDRRecord.GetFieldValue(salePropertyNames[PropertyName.ExtraChargeValue]);
                decimal secondSaleCDRExtaChargeValue = 0;
                if (saleExtraChargeValue.HasValue)
                {
                    secondSaleCDRExtaChargeValue = saleExtraChargeValue.Value * secondSaleCDRPricedDuration / secondSaleCDR.PricedDurationInSeconds;
                    secondBillingCDRRecord.SetFieldValue(salePropertyNames[PropertyName.ExtraChargeValue], secondSaleCDRExtaChargeValue);
                }

                decimal thirdDurationInSeconds = durationInSeconds - firstDurationInSeconds - secondDurationInSeconds;
                SetSecondaryBillingCDRRecordData(thirdBillingCDRRecord, costPropertyNames, secondCostCDR.Net, thirdDurationInSeconds, secondCostCDR.PricedDurationInSeconds, 3);

                decimal thirdSaleCDRNet = secondSaleCDR.Net - secondSaleCDRNet;
                decimal thirdSaleCDRPricedDuration = secondSaleCDR.PricedDurationInSeconds - secondSaleCDRPricedDuration;
                SetSecondaryBillingCDRRecordData(thirdBillingCDRRecord, salePropertyNames, thirdSaleCDRNet, thirdDurationInSeconds, thirdSaleCDRPricedDuration, 3);

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

        private void AddBillingCDRs(dynamic billingCDRRecord, List<dynamic> billingRecords, Dictionary<PropertyName, string> propertyNames, CDRPricingDataOutput firstCDR,
            CDRPricingDataOutput secondCDR, CDRPricingDataOutput otherCDR, Dictionary<PropertyName, string> otherPropertyNames, Guid recordTypeId)
        {
            dynamic secondBillingCDRRecord = billingCDRRecord.CloneRecord(recordTypeId);
            SetPrimaryBillingCDRRecordData(billingCDRRecord, propertyNames, firstCDR.Net, firstCDR.DurationInSeconds, firstCDR.PricedDurationInSeconds, 1);
            SetSecondaryBillingCDRRecordData(secondBillingCDRRecord, propertyNames, secondCDR.Net, secondCDR.DurationInSeconds, secondCDR.PricedDurationInSeconds, 2);

            decimal firstNet = otherCDR.Net * firstCDR.Percentage / 100;
            decimal secondNet = otherCDR.Net * secondCDR.Percentage / 100;
            decimal firstDurationInSeconds = otherCDR.PricedDurationInSeconds * firstCDR.Percentage / 100;
            decimal secondDurationInSeconds = otherCDR.PricedDurationInSeconds * secondCDR.Percentage / 100;

            billingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.Net], firstNet);
            billingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.PricedDurationInSeconds], firstDurationInSeconds);

            secondBillingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.Net], secondNet);
            secondBillingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.PricedDurationInSeconds], secondDurationInSeconds);

            billingRecords.Add(billingCDRRecord);
            billingRecords.Add(secondBillingCDRRecord);
        }

        private void AddBillingCDRs(dynamic billingCDRRecord, List<dynamic> billingRecords, Dictionary<PropertyName, string> propertyNames, CDRPricingDataOutput firstCDR,
            CDRPricingDataOutput secondCDR, Guid recordTypeId)
        {
            if (secondCDR != null)
            {
                dynamic secondBillingCDRRecord = billingCDRRecord.CloneRecord(recordTypeId);

                SetPrimaryBillingCDRRecordData(billingCDRRecord, propertyNames, firstCDR.Net, firstCDR.DurationInSeconds, firstCDR.PricedDurationInSeconds, 1);
                SetSecondaryBillingCDRRecordData(secondBillingCDRRecord, propertyNames, secondCDR.Net, secondCDR.DurationInSeconds, secondCDR.PricedDurationInSeconds, 2);

                billingRecords.Add(secondBillingCDRRecord);
            }

            billingRecords.Add(billingCDRRecord);
        }

        private static void SetSecondaryBillingCDRRecordData(dynamic secondBillingCDRRecord, Dictionary<PropertyName, string> propertyNames, decimal net, decimal durationInSeconds, decimal pricedDurationInSeconds, int cdrPartNb)
        {
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.DealTierNb], secondBillingCDRRecord.GetFieldValue(propertyNames[PropertyName.SecondaryDealTierNb]));
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.DealRateTierNb], secondBillingCDRRecord.GetFieldValue(propertyNames[PropertyName.SecondaryDealRateTierNb]));
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.DealDurInSec], secondBillingCDRRecord.GetFieldValue(propertyNames[PropertyName.SecondaryDealDurInSec]));

            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealTierNb], null);
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealRateTierNb], null);
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealDurInSec], null);

            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.Net], net);
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], pricedDurationInSeconds);

            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.CDRPartNb], cdrPartNb);
            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.CDRPartDurInSec], durationInSeconds);
        }

        private static void SetPrimaryBillingCDRRecordData(dynamic billingCDRRecord, Dictionary<PropertyName, string> propertyNames, decimal net, decimal durationInSeconds, decimal pricedDurationInSeconds, int cdrPartNb)
        {
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealTierNb], null);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealRateTierNb], null);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.SecondaryDealDurInSec], null);

            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.RateId], null);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.ExtraChargeRateValue], null);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.ExtraChargeValue], null);

            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.Net], net);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], pricedDurationInSeconds);

            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.CDRPartNb], cdrPartNb);
            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.CDRPartDurInSec], durationInSeconds);
        }

        private void SendOutputData(IQueueActivatorExecutionContext context, DataRecordBatch mainTransformedBatch, DataRecordBatch partialPricedTransformedBatch, DataRecordBatch billingTransformedBatch)
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

            propertyNames.Add(PropertyName.Zone, string.Format("{0}ZoneId", secondaryPrefixPropName));

            return propertyNames;
        }

        private void UpdateBillingCDRData(Dictionary<DealZoneGroup, DealProgress> dealProgresses, DealZoneGroup dealZoneGroup, dynamic record, bool isSale,
            Dictionary<PropertyName, string> propertyNames, Func<int, DealZoneGroupTier> GetDealZoneGroupTier, List<DealProgress> newDealProgresses, out CDRPricingDataOutput firstPartOutput,
            out CDRPricingDataOutput secondPartOutput)
        {
            firstPartOutput = null; secondPartOutput = null;
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

                    CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, durationInSeconds, 100, currentDealZoneGroupTier);
                    SetPricingData(record, propertyNames, firstPartInput, null, out firstPartOutput, out secondPartOutput);
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

                    int nextTierNb = dealProgress.CurrentTierNb + 1;
                    DealZoneGroupTier nextDealZoneGroupTier = GetDealZoneGroupTier(nextTierNb);

                    if (nextDealZoneGroupTier != null)
                    {
                        dealProgress.CurrentTierNb = nextDealZoneGroupTier.TierNumber;
                        dealProgress.TargetDurationInSeconds = nextDealZoneGroupTier.VolumeInSeconds;
                        dealProgress.ReachedDurationInSeconds = secondaryTierDurationInSeconds;

                        if (isOnMultipleTier)
                        {
                            SetSecondaryDealData(record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);

                            CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, primaryTierDurationInSeconds, primaryTierDurationInSeconds / durationInSeconds * 100, previousDealZoneGroupTier);
                            CDRPricingDataInput secondPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, secondaryTierDurationInSeconds, secondaryTierDurationInSeconds / durationInSeconds * 100, nextDealZoneGroupTier);
                            SetPricingData(record, propertyNames, firstPartInput, secondPartInput, out firstPartOutput, out secondPartOutput);
                        }
                        else
                        {
                            SetPrimaryDealData(dealZoneGroup, record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);
                            CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, primaryTierDurationInSeconds, 100, nextDealZoneGroupTier);
                            SetPricingData(record, propertyNames, firstPartInput, null, out firstPartOutput, out secondPartOutput);
                        }
                    }
                    else
                    {
                        if (isOnMultipleTier)
                        {
                            SetSecondaryDealData(record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);

                            CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, primaryTierDurationInSeconds, primaryTierDurationInSeconds / durationInSeconds * 100, previousDealZoneGroupTier);
                            CDRPricingDataInput secondPartInput = BuildCDRPricingDataInput(record, propertyNames, null, secondaryTierDurationInSeconds, secondaryTierDurationInSeconds / durationInSeconds * 100, null);
                            SetPricingData(record, propertyNames, firstPartInput, secondPartInput, out firstPartOutput, out secondPartOutput);
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
                        TargetDurationInSeconds = newDealZoneGroupTier.VolumeInSeconds.HasValue ? newDealZoneGroupTier.VolumeInSeconds : null,
                        ZoneGroupNb = dealZoneGroup.ZoneGroupNb
                    };
                    dealProgresses.Add(dealZoneGroup, dealProgress);
                    newDealProgresses.Add(dealProgress);

                    SetPrimaryDealData(dealZoneGroup, record, propertyNames, durationInSeconds, dealProgress.CurrentTierNb);
                    CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, durationInSeconds, 100, newDealZoneGroupTier);
                    SetPricingData(record, propertyNames, firstPartInput, null, out firstPartOutput, out secondPartOutput);
                }
            }
        }

        private CDRPricingDataInput BuildCDRPricingDataInput(dynamic record, Dictionary<PropertyName, string> propertyNames, int? dealId, decimal durationInSeconds,
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

        private void SetPricingData(dynamic record, Dictionary<PropertyName, string> propertyNames, CDRPricingDataInput firstPartInput, CDRPricingDataInput secondPartInput, out CDRPricingDataOutput firstPartOutput,
            out CDRPricingDataOutput secondPartOutput)
        {
            decimal firstPartNet = firstPartInput.Rate * firstPartInput.DurationInSeconds / 60;
            firstPartOutput = new CDRPricingDataOutput() { DealId = firstPartInput.DealId.Value, PricedDurationInSeconds = firstPartInput.DurationInSeconds, DurationInSeconds = firstPartInput.DurationInSeconds, Net = firstPartNet, Percentage = firstPartInput.Percentage };

            if (secondPartInput == null)
            {
                secondPartOutput = null;
                record.SetFieldValue(propertyNames[PropertyName.RateValue], firstPartInput.Rate);
                record.SetFieldValue(propertyNames[PropertyName.RateId], null);
                record.SetFieldValue(propertyNames[PropertyName.Net], firstPartNet);
                record.SetFieldValue(propertyNames[PropertyName.ExtraChargeRateValue], null);
                record.SetFieldValue(propertyNames[PropertyName.ExtraChargeValue], null);
                record.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], firstPartInput.DurationInSeconds);
            }
            else
            {
                decimal secondPartNet;
                decimal secondPartDuration;
                decimal secondPartPercentage;
                if (secondPartInput.DealId.HasValue)
                {
                    record.SetFieldValue(propertyNames[PropertyName.RateId], null);
                    record.SetFieldValue(propertyNames[PropertyName.ExtraChargeRateValue], null);
                    record.SetFieldValue(propertyNames[PropertyName.ExtraChargeValue], null);
                    secondPartNet = secondPartInput.Rate * secondPartInput.DurationInSeconds / 60;
                    secondPartDuration = secondPartInput.DurationInSeconds;
                    secondPartPercentage = secondPartInput.Percentage;
                }
                else
                {
                    decimal secondaryRate;
                    ApplyTariffRule(record, propertyNames, secondPartInput.DurationInSeconds, out secondPartNet, out secondaryRate, out secondPartDuration);
                    secondPartInput.Rate = secondaryRate;
                    secondPartPercentage = (firstPartInput.DurationInSeconds + secondPartDuration) / secondPartDuration * 100;
                }
                decimal rate = firstPartInput.Rate * firstPartInput.Percentage / 100 + secondPartInput.Rate * secondPartPercentage / 100;
                record.SetFieldValue(propertyNames[PropertyName.RateValue], rate);

                decimal totalNet = firstPartInput.Rate * firstPartInput.DurationInSeconds / 60 + secondPartNet;

                secondPartOutput = new CDRPricingDataOutput() { DealId = secondPartInput.DealId, PricedDurationInSeconds = secondPartDuration, DurationInSeconds = secondPartInput.DurationInSeconds, Net = secondPartNet, Percentage = secondPartPercentage };

                record.SetFieldValue(propertyNames[PropertyName.Net], totalNet);
                record.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], (firstPartInput.DurationInSeconds + secondPartDuration));
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
            public decimal PricedDurationInSeconds { get; set; }
            public decimal Net { get; set; }
            public decimal Percentage { get; set; }
        }

        private class CDRPricingDataInput
        {
            public decimal? DealId { get; set; }
            public decimal Rate { get; set; }
            public decimal Percentage { get; set; }
            public decimal DurationInSeconds { get; set; }
        }

        private enum PropertyName
        {
            DurationInSeconds, PricedDurationInSeconds, DealId, DealZoneGroupNb, DealTierNb, DealRateTierNb, DealDurInSec, SecondaryDealTierNb,
            SecondaryDealRateTierNb, SecondaryDealDurInSec, RateId, RateValue, Net, TariffRuleId, CurrencyId, ExtraChargeRateValue, ExtraChargeValue, Zone, AttemptDateTime,
            CDRPartDurInSec, CDRPartNb
        }
    }
}