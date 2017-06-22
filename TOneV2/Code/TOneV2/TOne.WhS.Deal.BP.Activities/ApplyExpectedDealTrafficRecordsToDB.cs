using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Business;
using Vanrise.Common;

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
            List<DealDetailedProgress> dealDetailedProgressToAdd = new List<DealDetailedProgress>();
            List<DealDetailedProgress> dealDetailedProgressToUpdate = new List<DealDetailedProgress>();
            HashSet<long> dealDetailedProgressesToKeep = new HashSet<long>();

            List<DealProgress> dealProgressesToAdd = new List<DealProgress>();
            List<DealProgress> dealProgressesToUpdate = new List<DealProgress>();

            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses = inputArgument.DealDetailedProgresses;

            DealDetailedProgress currentDealDetailedProgresses;

            foreach (var kvp_expectedDealBillingSummaryRecord in inputArgument.ExpectedDealBillingSummaryRecords)
            {
                DealDetailedZoneGroupTier dealDetailedZoneGroupTier = kvp_expectedDealBillingSummaryRecord.Key;
                DealBillingSummary expectedDealBillingSummary = kvp_expectedDealBillingSummaryRecord.Value;

                if (!dealDetailedProgresses.TryGetValue(dealDetailedZoneGroupTier, out currentDealDetailedProgresses))
                {
                    dealDetailedProgressToAdd.Add(BuildDealDetailedProgress(expectedDealBillingSummary, null));
                }
                else
                {
                    dealDetailedProgressesToKeep.Add(currentDealDetailedProgresses.DealDetailedProgressID);
                    if (!dealDetailedProgressManager.AreEqual(currentDealDetailedProgresses, expectedDealBillingSummary))
                        dealDetailedProgressToUpdate.Add(BuildDealDetailedProgress(expectedDealBillingSummary, currentDealDetailedProgresses.DealDetailedProgressID));
                }
            }

            IEnumerable<DealDetailedProgress> dealDetailedProgressesToDelete = dealDetailedProgresses.FindAllRecords(itm => !dealDetailedProgressesToKeep.Contains(itm.DealDetailedProgressID));

            DealProgress expectedDealProgress;
            DealProgress currentDealProgress;

            foreach (var kvp_dealProgressData in inputArgument.ExpectedDealProgressData)
            {
                DealZoneGroup dealZoneGroup = kvp_dealProgressData.Key;
                DealProgressData dealProgressData = kvp_dealProgressData.Value;

                if (inputArgument.DealProgresses != null && inputArgument.DealProgresses.TryGetValue(dealZoneGroup, out currentDealProgress))
                {
                    expectedDealProgress = BuildDealProgress(inputArgument.IsSale, currentDealProgress.DealProgressID, dealProgressData);
                    if (!expectedDealProgress.IsEqual(currentDealProgress))
                        dealProgressesToUpdate.Add(expectedDealProgress);
                }
                else
                {
                    expectedDealProgress = BuildDealProgress(inputArgument.IsSale, null, dealProgressData);
                    dealProgressesToAdd.Add(expectedDealProgress);
                }
            }

            if (dealDetailedProgressToAdd.Count == 0 && dealDetailedProgressToUpdate.Count == 0 && (dealDetailedProgressesToDelete == null || dealDetailedProgressesToDelete.Count() == 0))
                return null;

            List<DateTime> daysToReprocess = new List<DateTime>();

            if (dealDetailedProgressToAdd.Count > 0)
            {
                dealDetailedProgressManager.InsertDealDetailedProgresses(dealDetailedProgressToAdd);
                daysToReprocess.AddRange(dealDetailedProgressToAdd.Select(itm => itm.FromTime.Date));
            }

            if (dealDetailedProgressToUpdate.Count > 0)
            {
                dealDetailedProgressManager.UpdateDealDetailedProgresses(dealDetailedProgressToUpdate);
                daysToReprocess.AddRange(dealDetailedProgressToUpdate.Select(itm => itm.FromTime.Date));
            }

            if (dealDetailedProgressesToDelete != null && dealDetailedProgressesToDelete.Count() > 0)
            {
                dealDetailedProgressManager.DeleteDealDetailedProgresses(dealDetailedProgressesToDelete.Select(itm => itm.DealDetailedProgressID).ToList());
                daysToReprocess.AddRange(dealDetailedProgressesToDelete.Select(itm => itm.FromTime.Date));
            }

            return new ApplyExpectedDealTrafficRecordsToDBOutput()
            {
                DaysToReprocess = daysToReprocess.Count > 0 ? new HashSet<DateTime>(daysToReprocess) : null
            };
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
            this.DaysToReprocess.Set(context, result.DaysToReprocess);
        }

        #region Private Methods

        private DealDetailedProgress BuildDealDetailedProgress(DealBillingSummary dealBillingSummary, long? dealDetailedProgressID)
        {
            DealDetailedProgress dealDetailedProgress = new DealDetailedProgress()
            {
                DealID = dealBillingSummary.DealId,
                ZoneGroupNb = dealBillingSummary.DealZoneGroupNb,
                IsSale = dealBillingSummary.IsSale,
                TierNb = dealBillingSummary.DealTierNb,
                RateTierNb = dealBillingSummary.DealRateTierNb,
                FromTime = dealBillingSummary.BatchStart,
                ToTime = dealBillingSummary.BatchStart.AddMinutes(30),
                ReachedDurationInSeconds = dealBillingSummary.DurationInSeconds
            };

            if (dealDetailedProgressID.HasValue)
                dealDetailedProgress.DealDetailedProgressID = dealDetailedProgressID.Value;

            return dealDetailedProgress;
        }

        private DealProgress BuildDealProgress(bool isSale, long? dealProgressID, DealProgressData dealProgressData)
        {
            DealProgress dealProgress = new DealProgress()
            {
                IsSale = isSale,
                DealID = dealProgressData.DealId,
                ZoneGroupNb = dealProgressData.ZoneGroupNb,
                CurrentTierNb = dealProgressData.CurrentTierNb,
                ReachedDurationInSeconds = dealProgressData.ReachedDurationInSeconds,
                TargetDurationInSeconds = dealProgressData.TargetDurationInSeconds
            };

            if (dealProgressID.HasValue)
                dealProgress.DealProgressID = dealProgressID.Value;

            return dealProgress;
        }

        #endregion
    }
}