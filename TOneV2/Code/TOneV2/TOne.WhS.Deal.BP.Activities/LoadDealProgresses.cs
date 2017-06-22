using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Deal.BP.Activities
{
    public class LoadDealProgressesInput
    {
        public Boolean IsSale { get; set; }

        public HashSet<DealZoneGroup> AffectedDealZoneGroups { get; set; }
    }

    public class LoadDealProgressesOutput
    {
        public Dictionary<DealZoneGroup, DealProgress> DealProgresses { get; set; }
    }

    public sealed class LoadDealProgresses : BaseAsyncActivity<LoadDealProgressesInput, LoadDealProgressesOutput>
    {
        [RequiredArgument]
        public InArgument<Boolean> IsSale { get; set; }

        [RequiredArgument]
        public InArgument<HashSet<DealZoneGroup>> AffectedDealZoneGroups { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<DealZoneGroup, DealProgress>> DealProgresses { get; set; }

        protected override LoadDealProgressesOutput DoWorkWithResult(LoadDealProgressesInput inputArgument, AsyncActivityHandle handle)
        {
            DealProgressManager dealProgressManager = new DealProgressManager();
            Dictionary<DealZoneGroup, DealProgress> dealProgresses = dealProgressManager.GetDealProgresses(inputArgument.AffectedDealZoneGroups, inputArgument.IsSale);

            return new LoadDealProgressesOutput()
            {
                DealProgresses = dealProgresses
            };
        }

        protected override LoadDealProgressesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadDealProgressesInput
            {
                IsSale = this.IsSale.Get(context),
                AffectedDealZoneGroups = this.AffectedDealZoneGroups.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadDealProgressesOutput result)
        {
            this.DealProgresses.Set(context, result.DealProgresses);
        }
    }
}