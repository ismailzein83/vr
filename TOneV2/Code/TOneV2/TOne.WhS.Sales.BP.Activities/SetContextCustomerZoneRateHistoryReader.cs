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
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public class SetContextCustomerZoneRateHistoryReader : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingRate>> CustomerRates { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            var ratePlanContext = context.GetRatePlanContext() as RatePlanContext;

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return;

            IEnumerable<ExistingRate> customerRates = CustomerRates.Get(context);
            IEnumerable<SaleRate> mappedCustomerRates = (customerRates != null) ? customerRates.MapRecords(x => x.RateEntity) : null;

            ratePlanContext.CustomerZoneRateHistoryReader = new CustomerZoneRateHistoryReader(ratePlanContext.InheritedRates, mappedCustomerRates);

            RoutingCustomerInfoDetails customerInfo = new RoutingCustomerInfoDetails
            {
                CustomerId = ratePlanContext.OwnerId,
                SellingProductId = new CarrierAccountManager().GetSellingProductId(ratePlanContext.OwnerId)
            };
            List<RoutingCustomerInfoDetails> customerInfos = new List<RoutingCustomerInfoDetails> { customerInfo };
            ratePlanContext.LastRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadLastRateNoCache(customerInfos, ratePlanContext.EffectiveDate));

        }
    }
}
