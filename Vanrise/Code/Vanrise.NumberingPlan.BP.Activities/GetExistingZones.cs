using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.NumberingPlan.Business;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.BP.Activities
{
    public class GetExistingZonesInput
    {
        public int SellingNumberPlanId { get; set; }
        public DateTime MinimumDate { get; set; }
    }
    public class GetExistingZonesOutput
    {
        public IEnumerable<SaleZone> ExistingZoneEntities { get; set; }
    }
    public sealed class GetExistingZones : Vanrise.BusinessProcess.BaseAsyncActivity<GetExistingZonesInput, GetExistingZonesOutput>
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanID { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZone>> ExistingZoneEntities { get; set; }

        protected override GetExistingZonesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingZonesInput()
            {
                MinimumDate = this.MinimumDate.Get(context),
                SellingNumberPlanId = this.SellingNumberPlanID.Get(context),
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingZoneEntities.Get(context) == null)
                this.ExistingZoneEntities.Set(context, new List<SaleZone>());
            base.OnBeforeExecute(context, handle);
        }

        protected override GetExistingZonesOutput DoWorkWithResult(GetExistingZonesInput inputArgument, AsyncActivityHandle handle)
        {

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            IEnumerable<SaleZone> saleZones = saleZoneManager.GetSaleZonesEffectiveAfter(inputArgument.SellingNumberPlanId, Vanrise.Common.Utilities.Min(inputArgument.MinimumDate, DateTime.Today));
            return new GetExistingZonesOutput()
            {
                ExistingZoneEntities = saleZones
            };
        }
        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingZonesOutput result)
        {
            this.ExistingZoneEntities.Set(context, result.ExistingZoneEntities);
        }
    }
}
