using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Deal.BP.Activities
{
    public class CheckRetroActiveBeforeDateInput
    {
        public Boolean IsSale { get; set; }

        public Dictionary<DealZoneGroup, DealProgress> DealProgressesBeforeUpdate { get; set; }
    }

    public class CheckRetroActiveBeforeDateOutput
    {
        public HashSet<DateTime> DaysToReprocess { get; set; }
    }


    public sealed class CheckRetroActiveBeforeDate : BaseAsyncActivity<CheckRetroActiveBeforeDateInput, CheckRetroActiveBeforeDateOutput>
    {
        public InArgument<Boolean> IsSale { get; set; }

        public InArgument<Dictionary<DealZoneGroup, DealProgress>> DealProgressesBeforeUpdate { get; set; }

        public OutArgument<HashSet<DateTime>> DaysToReprocess { get; set; }

        protected override CheckRetroActiveBeforeDateOutput DoWorkWithResult(CheckRetroActiveBeforeDateInput inputArgument, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
            //List<DealReprocessInput> itemsToAdd = new List<DealReprocessInput>();

            //HashSet<DealZoneGroup> existingDealZoneGroups = inputArgument.DealProgressesBeforeUpdate.Keys.ToHashSet();
            //Dictionary<DealZoneGroup, DealProgress> dealProgressAfterUpdate = new DealProgressManager().GetDealProgresses(existingDealZoneGroups, inputArgument.IsSale);

            //return new CheckRetroActiveBeforeDateOutput()
            //{
            //    DaysToReprocess = new HashSet<DateTime>(itemsToAdd.Select(itm => itm.FromTime.Date))
            //};
        }

        protected override CheckRetroActiveBeforeDateInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CheckRetroActiveBeforeDateInput()
            {
                IsSale = this.IsSale.Get(context),
                DealProgressesBeforeUpdate = this.DealProgressesBeforeUpdate.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CheckRetroActiveBeforeDateOutput result)
        {
            this.DaysToReprocess.Set(context, result.DaysToReprocess);
        }
    }
}
