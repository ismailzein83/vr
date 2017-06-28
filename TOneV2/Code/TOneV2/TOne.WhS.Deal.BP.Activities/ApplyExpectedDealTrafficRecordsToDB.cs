using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Business;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
    public class ApplyExpectedDealTrafficRecordsToDBInput
    {
        public Boolean IsSale { get; set; }

        public Dictionary<DealZoneGroup, DealProgress> DealProgresses { get; set; }

        public Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> DealDetailedProgresses { get; set; }

        public Dictionary<DealZoneGroup, DealProgressData> ExpectedDealProgressData { get; set; }

        public Dictionary<DealDetailedZoneGroupTier, DealBillingSummary> ExpectedDealBillingSummaryRecords { get; set; }
    }

    public class ApplyExpectedDealTrafficRecordsToDBOutput
    {
        public HashSet<DateTime> DaysToReprocess { get; set; }
    }

    public sealed class ApplyExpectedDealTrafficRecordsToDB : BaseAsyncActivity<ApplyExpectedDealTrafficRecordsToDBInput, ApplyExpectedDealTrafficRecordsToDBOutput>
    {
        [RequiredArgument]
        public InArgument<Boolean> IsSale { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<DealZoneGroup, DealProgress>> DealProgresses { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress>> DealDetailedProgresses { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<DealZoneGroup, DealProgressData>> ExpectedDealProgressData { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<DealDetailedZoneGroupTier, DealBillingSummary>> ExpectedDealBillingSummaryRecords { get; set; }

        [RequiredArgument]
        public OutArgument<HashSet<DateTime>> DaysToReprocess { get; set; }

        protected override ApplyExpectedDealTrafficRecordsToDBOutput DoWorkWithResult(ApplyExpectedDealTrafficRecordsToDBInput inputArgument, AsyncActivityHandle handle)
        {
            int intervalOffset = new ConfigManager().GetDealTechnicalSettingIntervalOffset();

            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            List<DealDetailedProgress> dealDetailedProgressToAdd = new List<DealDetailedProgress>();
            List<DealDetailedProgress> dealDetailedProgressToUpdate = new List<DealDetailedProgress>();
            HashSet<long> dealDetailedProgressesToKeep = new HashSet<long>();

            DealProgressManager dealProgressManager = new DealProgressManager();
            List<DealProgress> dealProgressesToAdd = new List<DealProgress>();
            List<DealProgress> dealProgressesToUpdate = new List<DealProgress>();

            Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses = inputArgument.DealDetailedProgresses;

            DealDetailedProgress currentDealDetailedProgresses;

            foreach (var kvp_expectedDealBillingSummaryRecord in inputArgument.ExpectedDealBillingSummaryRecords)
            {
                DealDetailedZoneGroupTier dealDetailedZoneGroupTier = kvp_expectedDealBillingSummaryRecord.Key;
                DealBillingSummary expectedDealBillingSummary = kvp_expectedDealBillingSummaryRecord.Value;

                if (!dealDetailedProgresses.TryGetValue(dealDetailedZoneGroupTier, out currentDealDetailedProgresses))
                {
                    dealDetailedProgressToAdd.Add(BuildDealDetailedProgress(expectedDealBillingSummary, null, intervalOffset));
                }
                else
                {
                    dealDetailedProgressesToKeep.Add(currentDealDetailedProgresses.DealDetailedProgressId);
                    if (!dealDetailedProgressManager.AreEqual(currentDealDetailedProgresses, expectedDealBillingSummary))
                        dealDetailedProgressToUpdate.Add(BuildDealDetailedProgress(expectedDealBillingSummary, currentDealDetailedProgresses.DealDetailedProgressId, intervalOffset));
                }
            }

            IEnumerable<DealDetailedProgress> dealDetailedProgressesToDelete = dealDetailedProgresses.FindAllRecords(itm => !dealDetailedProgressesToKeep.Contains(itm.DealDetailedProgressId));

            DealProgress expectedDealProgress;
            DealProgress currentDealProgress;

            foreach (var kvp_dealProgressData in inputArgument.ExpectedDealProgressData)
            {
                DealZoneGroup dealZoneGroup = kvp_dealProgressData.Key;
                DealProgressData dealProgressData = kvp_dealProgressData.Value;

                if (inputArgument.DealProgresses != null && inputArgument.DealProgresses.TryGetValue(dealZoneGroup, out currentDealProgress))
                {
                    expectedDealProgress = BuildDealProgress(inputArgument.IsSale, currentDealProgress.DealProgressId, dealProgressData);
                    if (!expectedDealProgress.IsEqual(currentDealProgress))
                        dealProgressesToUpdate.Add(expectedDealProgress);
                }
                else
                {
                    expectedDealProgress = BuildDealProgress(inputArgument.IsSale, null, dealProgressData);
                    dealProgressesToAdd.Add(expectedDealProgress);
                }
            }
            bool shouldUpdateDealTrafficRecords = false;
            if (dealProgressesToAdd.Count > 0)
            {
                shouldUpdateDealTrafficRecords = true;
                dealProgressManager.InsertDealProgresses(dealProgressesToAdd);
            }

            if (dealProgressesToUpdate.Count > 0)
            {
                shouldUpdateDealTrafficRecords = true;
                dealProgressManager.UpdateDealProgresses(dealProgressesToUpdate);
            }

            ApplyExpectedDealTrafficRecordsToDBOutput output = null;

            List<DateTime> daysToReprocess = new List<DateTime>();

            if (dealDetailedProgressToAdd.Count > 0)
            {
                shouldUpdateDealTrafficRecords = true;
                dealDetailedProgressManager.InsertDealDetailedProgresses(dealDetailedProgressToAdd);
                daysToReprocess.AddRange(dealDetailedProgressToAdd.Select(itm => itm.FromTime.Date));
            }

            if (dealDetailedProgressToUpdate.Count > 0)
            {
                shouldUpdateDealTrafficRecords = true;
                dealDetailedProgressManager.UpdateDealDetailedProgresses(dealDetailedProgressToUpdate);
                daysToReprocess.AddRange(dealDetailedProgressToUpdate.Select(itm => itm.FromTime.Date));
            }

            if (dealDetailedProgressesToDelete != null && dealDetailedProgressesToDelete.Count() > 0)
            {
                shouldUpdateDealTrafficRecords = true;
                dealDetailedProgressManager.DeleteDealDetailedProgresses(dealDetailedProgressesToDelete.Select(itm => itm.DealDetailedProgressId).ToList());
                daysToReprocess.AddRange(dealDetailedProgressesToDelete.Select(itm => itm.FromTime.Date));
            }

            if (daysToReprocess.Count > 0)
                output = new ApplyExpectedDealTrafficRecordsToDBOutput() { DaysToReprocess = new HashSet<DateTime>(daysToReprocess) };

            string isSaleAsString = inputArgument.IsSale ? "Sale" : "Cost";

            if (shouldUpdateDealTrafficRecords)
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Apply Expected {0} Deal Traffic Records To DataBase is done", isSaleAsString), null);
            else
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("{0} Deal Traffic Records is already synchronized", isSaleAsString), null);

            return output;
        }

        protected override ApplyExpectedDealTrafficRecordsToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyExpectedDealTrafficRecordsToDBInput
            {
                IsSale = this.IsSale.Get(context),
                DealProgresses = this.DealProgresses.Get(context),
                DealDetailedProgresses = this.DealDetailedProgresses.Get(context),
                ExpectedDealProgressData = this.ExpectedDealProgressData.Get(context),
                ExpectedDealBillingSummaryRecords = this.ExpectedDealBillingSummaryRecords.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyExpectedDealTrafficRecordsToDBOutput result)
        {
            this.DaysToReprocess.Set(context, result != null ? result.DaysToReprocess : null);
        }

        #region Private Methods

        private DealDetailedProgress BuildDealDetailedProgress(DealBillingSummary dealBillingSummary, long? dealDetailedProgressID, int intervalOffset)
        {
            DealDetailedProgress dealDetailedProgress = new DealDetailedProgress()
            {
                DealId = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.ZoneGroupNb,
                IsSale = dealBillingSummary.IsSale,
                TierNb = dealBillingSummary.TierNb,
                RateTierNb = dealBillingSummary.RateTierNb,
                FromTime = dealBillingSummary.BatchStart,
                ToTime = dealBillingSummary.BatchStart.AddMinutes(intervalOffset),
                ReachedDurationInSeconds = dealBillingSummary.DurationInSeconds
            };

            if (dealDetailedProgressID.HasValue)
                dealDetailedProgress.DealDetailedProgressId = dealDetailedProgressID.Value;

            return dealDetailedProgress;
        }

        private DealProgress BuildDealProgress(bool isSale, long? dealProgressID, DealProgressData dealProgressData)
        {
            DealProgress dealProgress = new DealProgress()
            {
                IsSale = isSale,
                DealId = dealProgressData.DealId,
                ZoneGroupNb = dealProgressData.ZoneGroupNb,
                CurrentTierNb = dealProgressData.CurrentTierNb,
                ReachedDurationInSeconds = dealProgressData.ReachedDurationInSeconds,
                TargetDurationInSeconds = dealProgressData.TargetDurationInSeconds
            };

            if (dealProgressID.HasValue)
                dealProgress.DealProgressId = dealProgressID.Value;

            return dealProgress;
        }

        #endregion
    }
}