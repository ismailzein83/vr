using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    public class GetExistingSaleZonesInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class GetExistingSaleZonesOutput
    {
        public IEnumerable<SaleZone> ExistingSaleZones { get; set; }
    }

    public class GetExistingSaleZones : BaseAsyncActivity<GetExistingSaleZonesInput, GetExistingSaleZonesOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZone>> ExistingSaleZones { get; set; }
        
        #endregion

        protected override GetExistingSaleZonesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingSaleZonesInput()
            {
                OwnerType = this.OwnerType.Get(context),
                OwnerId = this.OwnerId.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingSaleZones.Get(context) == null)
                this.ExistingSaleZones.Set(context, new List<SaleZone>());
            base.OnBeforeExecute(context, handle);
        }

        protected override GetExistingSaleZonesOutput DoWorkWithResult(GetExistingSaleZonesInput inputArgument, AsyncActivityHandle handle)
        {
            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            int ownerId = inputArgument.OwnerId;
            DateTime minimumDate = inputArgument.MinimumDate;

            var saleZoneManager = new SaleZoneManager();
            int sellingNumberPlanId = GetSellingNumberPlanId(ownerType, ownerId);
            IEnumerable<SaleZone> saleZones = saleZoneManager.GetSaleZonesEffectiveAfter(sellingNumberPlanId, minimumDate);

            if (saleZones == null || saleZones.Count() == 0)
                throw new NullReferenceException("saleZones");

            return new GetExistingSaleZonesOutput()
            {
                ExistingSaleZones = saleZones
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingSaleZonesOutput result)
        {
            this.ExistingSaleZones.Set(context, result.ExistingSaleZones);
        }

        #region Private Methods

        private int GetSellingNumberPlanId(SalePriceListOwnerType ownerType, int ownerId)
        {
            var ratePlanManager = new RatePlanManager();
            int? sellingNumberPlanId = ratePlanManager.GetOwnerSellingNumberPlanId(ownerType, ownerId);

            if (!sellingNumberPlanId.HasValue)
                throw new NullReferenceException("sellingNumberPlanId");

            return sellingNumberPlanId.Value;
        }
        
        #endregion
    }
}
