using System;
using System.Linq;
using TOne.WhS.Deal.Business;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Entities;

namespace TOne.WhS.Deal.BusinessProcessRules
{
    public class CPValidateZoneCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as CountryToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            CountryToProcess countryToProcess = context.Target as CountryToProcess;
            ICPParametersContext codePreparationContext = context.GetExtension<ICPParametersContext>();
            CustomerCountryManager customerCountryManager = new CustomerCountryManager();
            foreach (var carrierAccountInfo in codePreparationContext.Customers)
            {
                var soldCountries = customerCountryManager.GetSoldCountries(carrierAccountInfo.CarrierAccountId, codePreparationContext.EffectiveDate);
                var soldCountry = soldCountries.FirstOrDefault(c => c.CountryId == countryToProcess.CountryId);
                if (soldCountry != null)
                {
                    if (countryToProcess.ZonesToProcess != null)
                    {
                        foreach (var zoneToProcess in countryToProcess.ZonesToProcess)
                        {
                            if (zoneToProcess.ChangeType == ZoneChangeType.Deleted || zoneToProcess.ChangeType == ZoneChangeType.PendingClosed)
                            {
                                if (zoneToProcess.ExistingZones == null)
                                    throw new DataIntegrityValidationException("ZoneToProcess has no existing zones");
                                foreach (var existingZone in zoneToProcess.ExistingZones)
                                {
                                    if (dealDefinitionManager.IsSaleZoneIncludedInDeal(carrierAccountInfo.CarrierAccountId, existingZone.ZoneId, existingZone.BED))
                                    {
                                        context.Message = string.Format("Zone '{0}' is included in a deal", existingZone.Name);
                                        return false;
                                    }
                                }
                            }
                        }
                    }
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
 