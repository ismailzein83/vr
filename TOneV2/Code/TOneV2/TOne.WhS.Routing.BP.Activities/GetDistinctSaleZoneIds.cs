using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class GetDistinctSaleZoneIds : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }
        [RequiredArgument]
        public InArgument<bool> IsEffectiveInFuture { get; set; }
        [RequiredArgument]
        public InOutArgument<IOrderedEnumerable<long>> SaleZoneIds { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();

            var saleZoneIds = saleZoneManager.GetSaleZoneIds(this.EffectiveOn.Get(context), this.IsEffectiveInFuture.Get(context));
            this.SaleZoneIds.Set(context, saleZoneIds);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Getting Distinct Sale Zone Ids is done", null);
        }
    }
}
