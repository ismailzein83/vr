using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Business
{
    public class OtherRateEEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllDataByZone;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var allZoneData = context.Target as AllDataByZone;
            var ratePlanContext = context.GetExtension<IRatePlanContext>();
            if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer)
            {
                HashSet<string> zoneWithInvalidEED = new HashSet<string>();
                foreach (var zoneData in allZoneData.DataByZoneList)
                {
                    var otherRatesToClose = zoneData.OtherRatesToClose;
                    if (otherRatesToClose.Count() > 0)
                    {
                        var otherRateToClose = otherRatesToClose.FirstOrDefault(x => x.CloseEffectiveDate != null);
                        if (otherRateToClose != null)
                        {
                            var firstCloseEffectiveDate = otherRateToClose.CloseEffectiveDate;

                            foreach (var otherRate in otherRatesToClose)
                            {
                                if (otherRate.CloseEffectiveDate != firstCloseEffectiveDate)
                                {
                                    zoneWithInvalidEED.Add(zoneData.ZoneName);
                                }
                            }
                        }
                    }
                }
                if (zoneWithInvalidEED.Count() > 0)
                {
                    context.Message = string.Format("Cannot close other rates with different EEDs on zone(s): '{0}' ", string.Join(",", zoneWithInvalidEED));
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
