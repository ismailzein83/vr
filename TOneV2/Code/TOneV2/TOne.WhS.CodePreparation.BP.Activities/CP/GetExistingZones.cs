using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class GetExistingZones : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanID { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZone>> ExistingZoneEntities { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            int sellingNumberPlanID = context.GetValue(this.SellingNumberPlanID);
            DateTime minDate = context.GetValue(this.MinimumDate);

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            List<SaleZone> saleZones = saleZoneManager.GetSaleZonesEffectiveAfter(sellingNumberPlanID, minDate);
            ExistingZoneEntities.Set(context, saleZones);
        }
    }
}
