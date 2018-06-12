using System;
using System.Linq;
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
            CountryToProcess countryToProcess = context.Target as CountryToProcess;
            ICPParametersContext codePreparationContext = context.GetExtension<ICPParametersContext>();
            CustomerCountryManager customerCountryManager = new CustomerCountryManager();
            var customers = new CarrierAccountManager().GetCustomersBySellingNumberPlanId(codePreparationContext.SellingNumberPlanId);

            foreach (var carrierAccountInfo in customers)
            {
                var soldCountries = customerCountryManager.GetSoldCountries(carrierAccountInfo.CarrierAccountId, codePreparationContext.EffectiveDate);

                var soldCountry = soldCountries.FirstOrDefault(c => c.CountryId == countryToProcess.CountryId);
                if (soldCountry != null)
                {
                    //check zone of this country
                    foreach (var zoneToProcess in countryToProcess.ZonesToProcess)
                    {
                        // if(is)
                    }
                }
            }

            // if (IsZoneLinkedToDeal(z))
            return true;

        }

        private bool IsZoneLinkedToDeal(int customerId, long zoneId, DateTime effectiveAfter)
        {
            return false;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

    }
}
