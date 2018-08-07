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

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class OtherRateBEDLessThanValidBEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is DataByZone;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zoneData = context.Target as DataByZone;

            if ((zoneData.IsCustomerCountryNew.HasValue && zoneData.IsCustomerCountryNew.Value) || (zoneData.OtherRatesToChange == null || zoneData.OtherRatesToChange.Count == 0))
                return true;

            var ratePlanContext = context.GetExtension<IRatePlanContext>();

            string partialErrorMessage;
            DateTime newOtherRateMinBED = GetNewOtherRateMinBED(zoneData, ratePlanContext, out partialErrorMessage);

            var invalidRateTypeNames = new List<string>();
            var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

            foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
            {
                var rateTypeIds = Helper.GetRateTypeIds(ratePlanContext.OwnerId, zoneData.ZoneId, DateTime.Now);
                if (rateTypeIds.Contains(otherRateToChange.RateTypeId.Value))
                {
                    if (otherRateToChange.BED == default(DateTime))
                        throw new Vanrise.Entities.DataIntegrityValidationException("otherRateToChange.BED");
                    if (otherRateToChange.BED < newOtherRateMinBED)
                        invalidRateTypeNames.Add(rateTypeManager.GetRateTypeName(otherRateToChange.RateTypeId.Value));
                }
            }

            if (invalidRateTypeNames.Count > 0)
            {
                context.Message = string.Format("Pricing other rates '{0}' for zone '{1}' must be with date greater than or equal to {2}", string.Join(", ", invalidRateTypeNames), zoneData.ZoneName, partialErrorMessage);
                return false;
            }

            return true;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

        #region Private Methods
        private DateTime GetNewOtherRateMinBED(DataByZone zoneData, IRatePlanContext ratePlanContext, out string partialErrorMessage)
        {
            DateTime newOtherRateMinBED;

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
            {
                newOtherRateMinBED = zoneData.BED;

                string zoneBEDString = zoneData.BED.ToString(ratePlanContext.DateFormat);
                partialErrorMessage = string.Format("zone BED '{0}'", zoneBEDString);
            }
            else
            {
                if (!zoneData.SoldOn.HasValue)
                    throw new Vanrise.Entities.DataIntegrityValidationException("zoneData.SoldOn");

                if (zoneData.BED > zoneData.SoldOn.Value)
                {
                    newOtherRateMinBED = zoneData.BED;

                    string zoneBEDString = zoneData.BED.ToString(ratePlanContext.DateFormat);
                    partialErrorMessage = string.Format("zone BED '{0}'", zoneBEDString);
                }
                else
                {
                    newOtherRateMinBED = zoneData.SoldOn.Value;

                    string soldOnString = zoneData.SoldOn.Value.ToString(ratePlanContext.DateFormat);
                    partialErrorMessage = string.Format("sell date '{0}'", soldOnString);
                }
            }

            return newOtherRateMinBED;
        }
        #endregion
    }
}
