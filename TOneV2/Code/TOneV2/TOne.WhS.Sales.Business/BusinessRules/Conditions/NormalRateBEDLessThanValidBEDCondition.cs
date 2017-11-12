using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class NormalRateBEDLessThanValidBEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is DataByZone;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zoneData = context.Target as DataByZone;

            if ((zoneData.IsCustomerCountryNew.HasValue && zoneData.IsCustomerCountryNew.Value) || zoneData.NormalRateToChange == null)
                return true;

            if (zoneData.NormalRateToChange.BED == default(DateTime))
                throw new Vanrise.Entities.DataIntegrityValidationException("zoneData.NormalRateToChange.BED");

            DateTime normalRateBED = zoneData.NormalRateToChange.BED;
            var ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
            {
                if (normalRateBED < zoneData.BED)
                {
                    string zoneBEDString = zoneData.BED.ToString(ratePlanContext.DateFormat);
                    context.Message = string.Format("Pricing zone '{0}' must be with date greater than or equal to zone BED '{1}'", zoneData.ZoneName, zoneBEDString);
                    return false;
                }
            }
            else
            {
                if (!zoneData.SoldOn.HasValue)
                    throw new Vanrise.Entities.DataIntegrityValidationException("zoneData.SoldOn");

                DateTime minNormalRateBED;
                string partialErrorMessage;

                if (zoneData.BED > zoneData.SoldOn.Value)
                {
                    string zoneBEDString = zoneData.BED.ToString(ratePlanContext.DateFormat);
                    minNormalRateBED = zoneData.BED;
                    partialErrorMessage = string.Format("zone BED '{0}'", zoneBEDString);
                }
                else
                {
                    string soldOnString = zoneData.SoldOn.Value.ToString(ratePlanContext.DateFormat);
                    minNormalRateBED = zoneData.SoldOn.Value;
                    partialErrorMessage = string.Format("sell date '{0}'", soldOnString);
                }

                if (normalRateBED < minNormalRateBED)
                {
                    context.Message = string.Format("Pricing zone '{0}' must be with date greater than or equal to {1}", zoneData.ZoneName, partialErrorMessage);
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
