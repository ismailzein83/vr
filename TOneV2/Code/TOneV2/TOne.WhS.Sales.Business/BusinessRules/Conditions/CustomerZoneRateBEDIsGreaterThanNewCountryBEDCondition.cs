using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class CustomerZoneRateBEDIsGreaterThanNewCountryBEDCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is DataByZone;
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            var zoneData = context.Target as DataByZone;

            if (!zoneData.IsCustomerCountryNew.HasValue || !zoneData.IsCustomerCountryNew.Value)
                return true;

            string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(zoneData.CountryId);

            if (!zoneData.SoldOn.HasValue)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("BED of country '{0}' of zone '{0}' was not found", countryName, zoneData.ZoneName));

            var errorMessages = new List<string>();

            if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.BED > zoneData.SoldOn.Value)
                errorMessages.Add(string.Format("normal rate BED '{0}'", UtilitiesManager.GetDateTimeAsString(zoneData.NormalRateToChange.BED)));

            if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0)
            {
                var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

                foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                {
                    if (otherRateToChange.BED > zoneData.SoldOn.Value)
                    {
                        string rateTypeName = rateTypeManager.GetRateTypeName(otherRateToChange.RateTypeId.Value);
                        string formattedRateTypeName = (!string.IsNullOrWhiteSpace(rateTypeName)) ? rateTypeName.ToLower() : null;
                        errorMessages.Add(string.Format("{0} rate BED: '{1}'", formattedRateTypeName, UtilitiesManager.GetDateTimeAsString(otherRateToChange.BED)));
                    }
                }
            }

            if (errorMessages.Count > 0)
            {
                string soldOnAsString = UtilitiesManager.GetDateTimeAsString(zoneData.SoldOn.Value);
                string errorMessagesAsString = string.Join(", ", errorMessages);

                context.Message =
                    string.Format("The following rates of zone '{0}' must have a BED that is equal to '{1}' that of the new country '{2}': {3}", zoneData.ZoneName, soldOnAsString, countryName, errorMessagesAsString);

                return false;
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            var zoneData = target as DataByZone;
            string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(zoneData.CountryId);
            string soldOnAsString = (zoneData.SoldOn.HasValue) ? UtilitiesManager.GetDateTimeAsString(zoneData.SoldOn.Value) : null;
            return string.Format("Rates of zone '{0}' of new country '{1}' must have the same BED '{2}' as that of the country", zoneData.ZoneName, countryName, soldOnAsString);
        }
    }
}
