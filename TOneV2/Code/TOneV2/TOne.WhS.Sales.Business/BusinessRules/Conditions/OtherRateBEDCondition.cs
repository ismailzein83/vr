using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class OtherRateBEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllDataByZone;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var ratePlanContext = context.GetExtension<IRatePlanContext>();
            //TODO: Throw a new data integrity validation exception

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

                int sellingProductId = carrierAccountManager.GetSellingProductId(ratePlanContext.OwnerId);


                var customerZoneRateHistoryReader = ratePlanContext.CustomerZoneRateHistoryReader;
                var lasRateLocator = ratePlanContext.LastRateLocator;
                var customerZoneRateHistoryLocator = new CustomerZoneRateHistoryLocator(customerZoneRateHistoryReader);
                var allZoneData = context.Target as AllDataByZone;

                List<string> zoneWithInvalidBED = new List<string>();

                foreach (var zoneData in allZoneData.DataByZoneList)
                {
                    if (zoneData.OtherRatesToChange.Count() > 0)
                    {
                        var currentCustomerRate = customerZoneRateHistoryLocator.GetCustomerZoneRateHistoryRecord(ratePlanContext.OwnerId, sellingProductId, zoneData.ZoneName, null, zoneData.CountryId, zoneData.NormalRateToChange.BED, ratePlanContext.CurrencyId, ratePlanContext.LongPrecision);
                        var lastCustomerRate = lasRateLocator.GetCustomerZoneRate(ratePlanContext.OwnerId, sellingProductId, zoneData.ZoneId);

                        if (lastCustomerRate == null)
                            throw new VRBusinessException(string.Format("Zone {0} has no rates set neither for customer nor for selling product", zoneData.ZoneName));

                        if (zoneData.OtherRatesToChange.First().BED < lastCustomerRate.Rate.BED && (currentCustomerRate != null && zoneData.NormalRateToChange.NormalRate == currentCustomerRate.Rate))
                        {
                            zoneWithInvalidBED.Add(zoneData.ZoneName);
                        }
                    }
                }

                if (zoneWithInvalidBED.Count() > 0)
                {
                    context.Message = string.Format("Zone(s) '{0}' are having other rates with BED less than the BED of their normal rates", string.Join(",", zoneWithInvalidBED));
                    return false;
                }
            }
            return true;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}


