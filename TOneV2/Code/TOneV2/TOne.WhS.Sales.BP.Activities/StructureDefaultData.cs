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
        public InArgument<DefaultRoutingProductToAdd> DefaultRoutingProductToAdd { get; set; }

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
            DefaultRoutingProductToAdd defaultRoutingProductToAdd = DefaultRoutingProductToAdd.Get(context);

            var rpLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(effectiveOn));
            SaleEntityZoneRoutingProduct defaultRP;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
                defaultRP = rpLocator.GetSellingProductDefaultRoutingProduct(ownerId);
            else
            {
                int? sellingProductId = new CustomerSellingProductManager().GetEffectiveSellingProductId(ownerId, effectiveOn, false);
                if (!sellingProductId.HasValue) {
                    string effectiveOnAsString = UtilitiesManager.GetDateTimeAsString(effectiveOn);
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' is not assigned to a SellingProduct on '{1}'", ownerId, effectiveOnAsString));
                }
                defaultRP = rpLocator.GetCustomerDefaultRoutingProduct(ownerId, sellingProductId.Value);
            }

            var defaultData = new DefaultData() { DefaultRoutingProductToAdd = defaultRoutingProductToAdd };

            if (defaultRP != null)
            {
                defaultData.CurrentDefaultRoutingProduct = new OwnerDefaultRP()
                {
                    RoutingProductId = defaultRP.RoutingProductId,
                    Source = (ownerType == SalePriceListOwnerType.SellingProduct) ? SaleEntityZoneRoutingProductSource.ProductDefault : SaleEntityZoneRoutingProductSource.CustomerDefault,
                    BED = defaultRP.BED,
                    EED = defaultRP.EED
                };
            }

            DefaultData.Set(context, defaultData);
        }
    }
}
