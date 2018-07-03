using System;
using System.Linq;
using System.Text;
using TOne.WhS.Deal.Business;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Entities;
using System.Collections.Generic;

namespace TOne.WhS.Deal.BusinessProcessRules
{
    public class CPValidateZoneCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as AllCountriesToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var dealDefinitionManager = new DealDefinitionManager();
            var countriesToProcess = context.Target as AllCountriesToProcess;
            var customerCountryManager = new CustomerCountryManager();

            ICPParametersContext codePreparationContext = context.GetExtension<ICPParametersContext>();

            var zoneMessages = new List<string>();
            foreach (var countryToProcess in countriesToProcess.Countries)
            {
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
                                            zoneMessages.Add(string.Format("Zone '{0}' in deal '{1}'", existingZone.Name, deal.Name));
                                        }
                                    }
                                }
                            }
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

            return true;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

    }
}
