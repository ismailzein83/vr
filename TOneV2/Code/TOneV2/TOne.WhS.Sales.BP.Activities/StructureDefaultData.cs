using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class StructureDefaultData : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<DefaultServiceToAdd> DefaultServiceToAdd { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<DefaultData> DefaultData { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int ownerId = OwnerId.Get(context);
            DateTime effectiveOn = EffectiveOn.Get(context);
            DefaultServiceToAdd defaultServiceToAdd = DefaultServiceToAdd.Get(context);

            var defaultData = new DefaultData()
            {
                OwnerType = ownerType,
                DefaultServiceToAdd = defaultServiceToAdd
            };

            var serviceLocator = new SaleEntityServiceLocator(new SaleEntityServiceReadWithCache(effectiveOn));
            SaleEntityService currentDefaultService;
            SaleEntityServiceSource targetSource;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                targetSource = SaleEntityServiceSource.ProductDefault;
                currentDefaultService = serviceLocator.GetSellingProductDefaultService(ownerId);
            }
            else
            {
                targetSource = SaleEntityServiceSource.CustomerDefault;

                var ratePlanManager = new RatePlanManager();
                int? sellingProductId = ratePlanManager.GetSellingProductId(ownerType, ownerId, effectiveOn, false);
                if (!sellingProductId.HasValue)
                    throw new NullReferenceException("sellingProductId");
                currentDefaultService = serviceLocator.GetCustomerDefaultService(ownerId, sellingProductId.Value);
            }

            if (currentDefaultService != null && currentDefaultService.Source == targetSource)
            {
                defaultData.CurrentServices = currentDefaultService.Services;
            }

            DefaultData.Set(context, defaultData);
        }
    }
}
