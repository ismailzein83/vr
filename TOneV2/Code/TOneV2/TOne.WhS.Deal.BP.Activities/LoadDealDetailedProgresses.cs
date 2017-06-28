using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
    public class LoadDealDetailedProgressesInput
    {
        public HashSet<DealZoneGroup> AffectedDealZoneGroups { get; set; }

        public Boolean IsSale { get; set; }
    }

    public class LoadDealDetailedProgressesOutput
    {
        public Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> DealDetailedProgresses { get; set; }
    }

    public sealed class LoadDealDetailedProgresses : BaseAsyncActivity<LoadDealDetailedProgressesInput, LoadDealDetailedProgressesOutput>
    {
        [RequiredArgument]
        public InArgument<Boolean> IsSale { get; set; }

        [RequiredArgument]
        public InArgument<HashSet<DealZoneGroup>> AffectedDealZoneGroups { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress>> DealDetailedProgresses { get; set; }

        protected override LoadDealDetailedProgressesOutput DoWorkWithResult(LoadDealDetailedProgressesInput inputArgument, AsyncActivityHandle handle)
        {
            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            Dictionary<DealDetailedZoneGroupTier, DealDetailedProgress> dealDetailedProgresses = dealDetailedProgressManager.GetDealDetailedProgresses(inputArgument.AffectedDealZoneGroups, inputArgument.IsSale);

            string isSaleAsString = inputArgument.IsSale ? "Sale" : "Cost";
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Load {0} Deal Detailed Progress Table is done", isSaleAsString), null);

            return new LoadDealDetailedProgressesOutput()
            {
                DealDetailedProgresses = dealDetailedProgresses
            }; 
        }

        protected override LoadDealDetailedProgressesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadDealDetailedProgressesInput
            {
                AffectedDealZoneGroups = this.AffectedDealZoneGroups.Get(context),
                IsSale = this.IsSale.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadDealDetailedProgressesOutput result)
        {
            this.DealDetailedProgresses.Set(context, result.DealDetailedProgresses);
        }
    }
}