using System;
using System.Linq;
using Vanrise.Entities;
using TOne.WhS.Deal.Business;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;

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
            var dealDefinitionManager = new DealDefinitionManager();
            var countryToProcess = context.Target as CountryToProcess;
            var customerCountryManager = new CustomerCountryManager();

            ICPParametersContext codePreparationContext = context.GetExtension<ICPParametersContext>();

            var zoneMessages = new List<string>();

            foreach (var carrierAccountInfo in codePreparationContext.Customers)
            {
                var soldCountries = customerCountryManager.GetSoldCountries(carrierAccountInfo.CarrierAccountId, codePreparationContext.EffectiveDate);

                if (soldCountries == null)
                    continue;

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
                                    var dealId = dealDefinitionManager.IsZoneIncludedInDeal(carrierAccountInfo.CarrierAccountId, existingZone.ZoneId,
                                            codePreparationContext.EffectiveDate, true);
                                    if (dealId.HasValue)
                                    {
                                        var deal = new DealDefinitionManager().GetDealDefinition(dealId.Value);
                                        zoneMessages.Add(string.Format("zone '{0}' in deal '{1}'", existingZone.Name, deal.Name));
                                    }
                                }
                            }
                        }
                        if (zoneMessages.Any())
                        {
                            string zoneMessageString = string.Join(",", zoneMessages);
                            context.Message = String.Format("Following closed zones are included in effective deals : {0}", zoneMessageString);
                            return false;
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
