using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class GetExistingSaleZones : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZone>> ExistingSaleZones { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            SalePriceListOwnerType ownerType = this.OwnerType.Get(context);
            int ownerId = this.OwnerId.Get(context);
            DateTime minimumDate = this.MinimumDate.Get(context);

            var saleZoneManager = new SaleZoneManager();
            int sellingNumberPlanId = this.GetSellingNumberPlanId(ownerType, ownerId);
            IEnumerable<SaleZone> saleZones = saleZoneManager.GetSaleZonesEffectiveAfter(sellingNumberPlanId, minimumDate);

            if (saleZones == null || saleZones.Count() == 0)
                throw new NullReferenceException("saleZones");

            this.ExistingSaleZones.Set(context, saleZones);
        }

        #region Private Methods

        private int GetSellingNumberPlanId(SalePriceListOwnerType ownerType, int ownerId)
        {
            int? sellingNumberPlanId = null;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                var sellingProductManager = new SellingProductManager();
                sellingNumberPlanId = sellingProductManager.GetSellingNumberPlanId(ownerId);
            }
            else if (ownerType == SalePriceListOwnerType.Customer)
            {
                var carrierAccountManager = new CarrierAccountManager();
                sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(ownerId, CarrierAccountType.Customer);
            }

            if (!sellingNumberPlanId.HasValue)
                throw new NullReferenceException("sellingNumberPlanId");

            return sellingNumberPlanId.Value;
        }
        
        #endregion
    }
}
