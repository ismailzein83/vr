using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Deal.BP.Activities
{
    public class LoadPreviousAffectedDealZoneGroupsInput
    {
        public Boolean IsSale { get; set; }
    }

    public class LoadPreviousAffectedDealZoneGroupsOutput
    {
        public HashSet<DealZoneGroup> AffectedDealZoneGroups { get; set; }
    }

    public sealed class LoadPreviousAffectedDealZoneGroups : BaseAsyncActivity<LoadPreviousAffectedDealZoneGroupsInput, LoadPreviousAffectedDealZoneGroupsOutput>
    {
        [RequiredArgument]
        public InArgument<Boolean> IsSale { get; set; }

        [RequiredArgument]
        public OutArgument<HashSet<DealZoneGroup>> AffectedDealZoneGroups { get; set; }

        protected override LoadPreviousAffectedDealZoneGroupsOutput DoWorkWithResult(LoadPreviousAffectedDealZoneGroupsInput inputArgument, AsyncActivityHandle handle)
        {
            DealProgressManager dealProgressManager = new DealProgressManager();
            IEnumerable<DealZoneGroup> affectedDealZoneGroups = dealProgressManager.GetAffectedDealZoneGroups(inputArgument.IsSale);

            return new LoadPreviousAffectedDealZoneGroupsOutput()
            {
                AffectedDealZoneGroups = affectedDealZoneGroups != null ? affectedDealZoneGroups.ToHashSet() : null
            };
        }

        protected override LoadPreviousAffectedDealZoneGroupsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadPreviousAffectedDealZoneGroupsInput
            {
                IsSale = this.IsSale.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadPreviousAffectedDealZoneGroupsOutput result)
        {
            this.AffectedDealZoneGroups.Set(context, result.AffectedDealZoneGroups);
        }
    }
}