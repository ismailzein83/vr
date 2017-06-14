﻿using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Reprocess.Entities;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Queueing.Entities;
using Vanrise.Common;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.Common.Business;

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

            Dictionary<DealZoneGroup, DealProgress> saleDealProgresses;
            Dictionary<DealZoneGroup, DealProgress> costDealProgresses;
            Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> saleDealDetailedProgresses;
            Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> costDealDetailedProgresses;
            FillDealProgressItems(saleDealZoneGroups, costDealZoneGroups, out saleDealProgresses, out costDealProgresses, out saleDealDetailedProgresses, out costDealDetailedProgresses);


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
                    Func<int, DealZoneGroupTierDetails> getSaleDealZoneGroupTierDetails = GetSaleDealZoneGroupTierDetails(saleDealZoneGroup);
                    UpdateBillingCDRData(saleDealProgresses, newSaleDealProgresses, saleDealDetailedProgresses, newSaleDealDetailedProgresses, saleDealZoneGroup, record, true, salePropertyNames, getSaleDealZoneGroupTierDetails, out firstSaleCDR, out secondSaleCDR);
                }

                int? costOrigDealId = record.OrigCostDealId;
                int? costOrigZoneGroupNb = record.OrigCostDealZoneGroupNb;
                if (costOrigDealId.HasValue && costOrigZoneGroupNb.HasValue)
                {
                    DealZoneGroup costDealZoneGroup = new DealZoneGroup() { DealId = costOrigDealId.Value, ZoneGroupNb = costOrigZoneGroupNb.Value };
                    Func<int, DealZoneGroupTierDetails> getSupplierDealZoneGroupTierDetails = GetSupplierDealZoneGroupTierDetails(costDealZoneGroup);
                    UpdateBillingCDRData(costDealProgresses, newCostDealProgresses, costDealDetailedProgresses, newCostDealDetailedProgresses, costDealZoneGroup, record, false, costPropertyNames, getSupplierDealZoneGroupTierDetails, out firstCostCDR, out secondCostCDR);
                }
                CreateBillingCDRRecords(billingRecords, record, firstSaleCDR, secondSaleCDR, firstCostCDR, secondCostCDR, salePropertyNames, costPropertyNames, recordTypeId);
            }
            DealProgressManager dealProgressManager = new DealProgressManager();
            dealProgressManager.UpdateDealProgresses(existingSaleDealProgresses.Union(existingCostDealProgresses));
            dealProgressManager.InsertDealProgresses(newSaleDealProgresses.Union(newCostDealProgresses));

            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            dealDetailedProgressManager.UpdateDealDetailedProgresses(existingSaleDealDetailedProgresses.Union(existingCostDealDetailedProgresses));
            dealDetailedProgressManager.InsertDealDetailedProgresses(newSaleDealDetailedProgresses.Union(newCostDealDetailedProgresses));

            DataRecordBatch mainTransformedBatch = DataRecordBatch.CreateBatchFromRecords(mainCDRs, queueItemType.BatchDescription, recordTypeId);
            DataRecordBatch partialPricedTransformedBatch = DataRecordBatch.CreateBatchFromRecords(partialPricedCDRs, queueItemType.BatchDescription, recordTypeId);
            DataRecordBatch billingTransformedBatch = DataRecordBatch.CreateBatchFromRecords(billingRecords, queueItemType.BatchDescription, recordTypeId);

            SendOutputData(context, mainTransformedBatch, partialPricedTransformedBatch, billingTransformedBatch);
        }

        private void FillDealProgressItems(HashSet<DealZoneGroup> saleDealZoneGroups, HashSet<DealZoneGroup> costDealZoneGroups, out Dictionary<DealZoneGroup, DealProgress> saleDealProgresses, out Dictionary<DealZoneGroup, DealProgress> costDealProgresses,
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
            saleDealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgresses(saleDealZoneGroups, true);
            if (saleDealDetailedProgresses == null)
                saleDealDetailedProgresses = new Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress>();

            costDealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgresses(costDealZoneGroups, false);
            if (costDealDetailedProgresses == null)
                costDealDetailedProgresses = new Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress>();
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

            billingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.Net], decimal.Round(firstNet, 10));
            billingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.PricedDurationInSeconds], firstDurationInSeconds);

            secondBillingCDRRecord.SetFieldValue(otherPropertyNames[PropertyName.Net], decimal.Round(secondNet, 10));
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

            secondBillingCDRRecord.SetFieldValue(propertyNames[PropertyName.Net], decimal.Round(net, 10));
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

            billingCDRRecord.SetFieldValue(propertyNames[PropertyName.Net], decimal.Round(net, 10));
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

        private Func<int, DealZoneGroupTierDetails> GetSaleDealZoneGroupTierDetails(DealZoneGroup saleDealZoneGroup)
        {
            Func<int, DealZoneGroupTierDetails> getSaleDealZoneGroupTierDetails = (targetTierNumber) =>
            {
                return new DealDefinitionManager().GetSaleDealZoneGroupTierDetails(saleDealZoneGroup.DealId, saleDealZoneGroup.ZoneGroupNb, targetTierNumber);
            };
            return getSaleDealZoneGroupTierDetails;
        }

        private Func<int, DealZoneGroupTierDetails> GetSupplierDealZoneGroupTierDetails(DealZoneGroup costDealZoneGroup)
        {
            Func<int, DealZoneGroupTierDetails> getSaleDealZoneGroupTierDetails = (targetTierNumber) =>
            {
                return new DealDefinitionManager().GetSupplierDealZoneGroupTierDetails(costDealZoneGroup.DealId, costDealZoneGroup.ZoneGroupNb, targetTierNumber);
            };
            return getSaleDealZoneGroupTierDetails;
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

        private void UpdateBillingCDRData(Dictionary<DealZoneGroup, DealProgress> dealProgresses, List<DealProgress> newDealProgresses, Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses,
            List<DealDetailedProgress> newDealDetailedProgresses, DealZoneGroup dealZoneGroup, dynamic record, bool isSale, Dictionary<PropertyName, string> propertyNames,
            Func<int, DealZoneGroupTierDetails> GetDealZoneGroupTierDetails, out CDRPricingDataOutput firstPartOutput, out CDRPricingDataOutput secondPartOutput)
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
                    DealZoneGroupTierDetails currentDealZoneGroupTierDetails = GetDealZoneGroupTierDetails(dealProgress.CurrentTierNb);

                    CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, durationInSeconds, 100, currentDealZoneGroupTierDetails, currentDealZoneGroupTierDetails.CurrencyId);
                    SetPricingData(record, propertyNames, firstPartInput, null, out firstPartOutput, out secondPartOutput);
                    AdjustDealDetailedProgress(dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, dealProgress.CurrentTierNb, durationInSeconds, record, propertyNames, isSale);
                }
                else
                {
                    decimal primaryTierDurationInSeconds = dealProgress.TargetDurationInSeconds.Value - reachedDuration;
                    decimal secondaryTierDurationInSeconds = durationInSeconds - primaryTierDurationInSeconds;

                    dealProgress.ReachedDurationInSeconds = dealProgress.TargetDurationInSeconds;
                    DealZoneGroupTierDetails previousDealZoneGroupTierDetails = null;

                    bool isOnMultipleTier = primaryTierDurationInSeconds > 0;
                    if (isOnMultipleTier)
                    {
                        previousDealZoneGroupTierDetails = GetDealZoneGroupTierDetails(dealProgress.CurrentTierNb);
                        SetPrimaryDealData(dealZoneGroup, record, propertyNames, primaryTierDurationInSeconds, dealProgress.CurrentTierNb);
                        AdjustDealDetailedProgress(dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, dealProgress.CurrentTierNb, primaryTierDurationInSeconds, record, propertyNames, isSale);
                    }

                    int nextTierNb = dealProgress.CurrentTierNb + 1;
                    DealZoneGroupTierDetails nextDealZoneGroupTier = GetDealZoneGroupTierDetails(nextTierNb);

                    if (nextDealZoneGroupTier != null)
                    {
                        dealProgress.CurrentTierNb = nextDealZoneGroupTier.TierNumber;
                        dealProgress.TargetDurationInSeconds = nextDealZoneGroupTier.VolumeInSeconds;
                        dealProgress.ReachedDurationInSeconds = secondaryTierDurationInSeconds;

                        if (isOnMultipleTier)
                        {
                            SetSecondaryDealData(record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);

                            CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, primaryTierDurationInSeconds, primaryTierDurationInSeconds / durationInSeconds * 100, previousDealZoneGroupTierDetails, previousDealZoneGroupTierDetails.CurrencyId);
                            CDRPricingDataInput secondPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, secondaryTierDurationInSeconds, secondaryTierDurationInSeconds / durationInSeconds * 100, nextDealZoneGroupTier, nextDealZoneGroupTier.CurrencyId);
                            SetPricingData(record, propertyNames, firstPartInput, secondPartInput, out firstPartOutput, out secondPartOutput);
                        }
                        else
                        {
                            SetPrimaryDealData(dealZoneGroup, record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);
                            CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, primaryTierDurationInSeconds, 100, nextDealZoneGroupTier, nextDealZoneGroupTier.CurrencyId);
                            SetPricingData(record, propertyNames, firstPartInput, null, out firstPartOutput, out secondPartOutput);
                        }
                        AdjustDealDetailedProgress(dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, nextDealZoneGroupTier.TierNumber, secondaryTierDurationInSeconds, record, propertyNames, isSale);
                    }
                    else // second part outside deal
                    {
                        if (isOnMultipleTier)
                        {
                            SetSecondaryDealData(record, propertyNames, secondaryTierDurationInSeconds, nextDealZoneGroupTier.TierNumber);

                            CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, primaryTierDurationInSeconds, primaryTierDurationInSeconds / durationInSeconds * 100, previousDealZoneGroupTierDetails, previousDealZoneGroupTierDetails.CurrencyId);

                            int recordCurrencyId = record.GetFieldValue(propertyNames[PropertyName.CurrencyId]);
                            CDRPricingDataInput secondPartInput = BuildCDRPricingDataInput(record, propertyNames, null, secondaryTierDurationInSeconds, secondaryTierDurationInSeconds / durationInSeconds * 100, null, recordCurrencyId);
                            SetPricingData(record, propertyNames, firstPartInput, secondPartInput, out firstPartOutput, out secondPartOutput);
                        }
                        AdjustDealDetailedProgress(dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, null, secondaryTierDurationInSeconds, record, propertyNames, isSale);
                    }
                }
            }
            else
            {
                DealZoneGroupTierDetails newDealZoneGroupTierDetails = GetDealZoneGroupTierDetails(0);
                if (newDealZoneGroupTierDetails != null)
                {
                    dealProgress = new DealProgress()
                    {
                        CurrentTierNb = newDealZoneGroupTierDetails.TierNumber,
                        DealID = dealZoneGroup.DealId,
                        IsSale = isSale,
                        ReachedDurationInSeconds = durationInSeconds,
                        TargetDurationInSeconds = newDealZoneGroupTierDetails.VolumeInSeconds.HasValue ? newDealZoneGroupTierDetails.VolumeInSeconds : null,
                        ZoneGroupNb = dealZoneGroup.ZoneGroupNb
                    };
                    dealProgresses.Add(dealZoneGroup, dealProgress);
                    newDealProgresses.Add(dealProgress);

                    SetPrimaryDealData(dealZoneGroup, record, propertyNames, durationInSeconds, dealProgress.CurrentTierNb);
                    CDRPricingDataInput firstPartInput = BuildCDRPricingDataInput(record, propertyNames, dealProgress.DealID, durationInSeconds, 100, newDealZoneGroupTierDetails, newDealZoneGroupTierDetails.CurrencyId);
                    SetPricingData(record, propertyNames, firstPartInput, null, out firstPartOutput, out secondPartOutput);
                    AdjustDealDetailedProgress(dealDetailedProgresses, newDealDetailedProgresses, dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, newDealZoneGroupTierDetails.TierNumber, durationInSeconds, record, propertyNames, isSale);
                }
            }
        }

        private void AdjustDealDetailedProgress(Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses, List<DealDetailedProgress> newDealDetailedProgresses,
            int dealId, int zoneGroupNb, int? tierNb, decimal durationInSeconds, dynamic record, Dictionary<PropertyName, string> propertyNames, bool isSale)
        {
            int intervalOffset = 30;
            DateTime attemptDateTime = record.GetFieldValue(propertyNames[PropertyName.AttemptDateTime]);
            DateTime fromTime = new DateTime(attemptDateTime.Year, attemptDateTime.Month, attemptDateTime.Day, attemptDateTime.Hour, ((int)(attemptDateTime.Minute / intervalOffset)) * intervalOffset, 0);
            DateTime toTime = fromTime.AddMinutes(intervalOffset);

            DealDetailedZoneGroupTier dealDetailedZoneGroupTier = new DealDetailedZoneGroupTier()
            {
                DealID = dealId,
                FromTime = fromTime,
                RateTierNb = tierNb,
                TierNb = tierNb,
                ToTime = toTime,
                ZoneGroupNb = zoneGroupNb
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
                    DealID = dealId,
                    FromTime = fromTime,
                    RateTierNb = tierNb,
                    TierNb = tierNb,
                    ToTime = toTime,
                    ZoneGroupNb = zoneGroupNb,
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
                return new CDRPricingDataInput() { DurationInSeconds = durationInSeconds };

            long zoneId = record.GetFieldValue(propertyNames[PropertyName.Zone]);
            decimal zoneRate;
            if (dealZoneGroupTierDetails.ExceptionRates == null || !dealZoneGroupTierDetails.ExceptionRates.TryGetValue(zoneId, out zoneRate))
                zoneRate = dealZoneGroupTierDetails.Rate;

            return new CDRPricingDataInput()
            {
                DealId = dealId,
                DurationInSeconds = durationInSeconds,
                Percentage = percentage,
                Rate = zoneRate,
                CurrencyId = currencyId
            };
        }

        private void SetPricingData(dynamic record, Dictionary<PropertyName, string> propertyNames, CDRPricingDataInput firstPartInput, CDRPricingDataInput secondPartInput, out CDRPricingDataOutput firstPartOutput,
            out CDRPricingDataOutput secondPartOutput)
        {
            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
            decimal firstPartNet = firstPartInput.Rate * firstPartInput.DurationInSeconds / 60;
            firstPartOutput = new CDRPricingDataOutput() { DealId = firstPartInput.DealId.Value, PricedDurationInSeconds = firstPartInput.DurationInSeconds, DurationInSeconds = firstPartInput.DurationInSeconds, Net = firstPartNet, Percentage = firstPartInput.Percentage };

            if (secondPartInput == null)
            {
                secondPartOutput = null;
                record.SetFieldValue(propertyNames[PropertyName.CurrencyId], firstPartInput.CurrencyId);
                record.SetFieldValue(propertyNames[PropertyName.RateValue], decimal.Round(firstPartInput.Rate, 10));
                record.SetFieldValue(propertyNames[PropertyName.RateId], null);
                record.SetFieldValue(propertyNames[PropertyName.Net], decimal.Round(firstPartNet, 10));
                record.SetFieldValue(propertyNames[PropertyName.ExtraChargeRateValue], null);
                record.SetFieldValue(propertyNames[PropertyName.ExtraChargeValue], null);
                record.SetFieldValue(propertyNames[PropertyName.PricedDurationInSeconds], firstPartInput.DurationInSeconds);
            }
            else
            {
                int secondCurrencyId;
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
                    secondCurrencyId = secondPartInput.CurrencyId;
                }
                else
                {
                    decimal secondaryRate;
                    ApplyTariffRule(record, propertyNames, secondPartInput.DurationInSeconds, out secondPartNet, out secondaryRate, out secondPartDuration);
                    secondPartInput.Rate = secondaryRate;
                    secondPartPercentage = (firstPartInput.DurationInSeconds + secondPartDuration) / secondPartDuration * 100;
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
                }

                decimal rate = convertedFirstPartRate * firstPartInput.Percentage / 100 + secondPartInput.Rate * secondPartPercentage / 100;
                record.SetFieldValue(propertyNames[PropertyName.RateValue], decimal.Round(rate, 10));

                secondPartOutput = new CDRPricingDataOutput() { DealId = secondPartInput.DealId, PricedDurationInSeconds = secondPartDuration, DurationInSeconds = secondPartInput.DurationInSeconds, Net = secondPartNet, Percentage = secondPartPercentage };

                record.SetFieldValue(propertyNames[PropertyName.Net], decimal.Round(totalNet, 10));
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
            public int CurrencyId { get; set; }
        }

        private enum PropertyName
        {
            DurationInSeconds, PricedDurationInSeconds, DealId, DealZoneGroupNb, DealTierNb, DealRateTierNb, DealDurInSec, SecondaryDealTierNb,
            SecondaryDealRateTierNb, SecondaryDealDurInSec, RateId, RateValue, Net, TariffRuleId, CurrencyId, ExtraChargeRateValue, ExtraChargeValue, Zone, AttemptDateTime,
            CDRPartDurInSec, CDRPartNb
        }
    }
}