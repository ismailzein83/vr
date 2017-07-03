using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Business.Reader;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public class SetContextActionRateLocator : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SaleRate>> ExistingSaleRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RateToClose>> RatesToClose { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            RatePlanContext ratePlanContext = context.GetRatePlanContext() as RatePlanContext;
            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return;

            IEnumerable<RateToClose> ratesToClose = RatesToClose.Get(context);
            if (ratesToClose == null || ratesToClose.Count() == 0)
                return;

            IEnumerable<RateToClose> normalRatesToClose = ratesToClose.FindAllRecords(x => !x.RateTypeId.HasValue);
            if (normalRatesToClose == null || normalRatesToClose.Count() == 0)
                return;

            IEnumerable<SaleRate> existingSaleRates = ExistingSaleRates.Get(context);
            int sellingProductId = new CarrierAccountManager().GetSellingProductId(ratePlanContext.OwnerId);

            var routingCustomerInfo = new RoutingCustomerInfoDetails()
            {
                SellingProductId = sellingProductId,
                CustomerId = ratePlanContext.OwnerId
            };

            var zoneIds = new List<long>();
            var actionDatesByZoneId = new Dictionary<long, DateTime>();
            DateTime minimumDate;

            SetData(normalRatesToClose, zoneIds, actionDatesByZoneId, out minimumDate);

            var reader = new SaleRateReadRPChanges(existingSaleRates, routingCustomerInfo, zoneIds, minimumDate, actionDatesByZoneId);
            ratePlanContext.ActionRateLocator = new SaleEntityZoneRateLocator(reader);
        }

        #region Private Methods

        private void SetData(IEnumerable<RateToClose> normalRatesToClose, List<long> zoneIds, Dictionary<long, DateTime> actionDatesByZoneId, out DateTime minimumDate)
        {
            minimumDate = DateTime.MaxValue;

            foreach (RateToClose normalRateToClose in normalRatesToClose)
            {
                zoneIds.Add(normalRateToClose.ZoneId);

                if (!actionDatesByZoneId.ContainsKey(normalRateToClose.ZoneId))
                    actionDatesByZoneId.Add(normalRateToClose.ZoneId, normalRateToClose.CloseEffectiveDate);

                if (normalRateToClose.CloseEffectiveDate < minimumDate)
                    minimumDate = normalRateToClose.CloseEffectiveDate;
            }
        }

        #endregion
    }
}
