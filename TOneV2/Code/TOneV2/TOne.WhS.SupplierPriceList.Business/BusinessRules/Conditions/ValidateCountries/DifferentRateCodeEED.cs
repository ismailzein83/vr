using System;
using System.Linq;
using Vanrise.Common;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class DifferentRateCodeEED : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCountry != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCountry importedCountry = context.Target as ImportedCountry;
            var messages = new List<string>();

            foreach (var importedZone in importedCountry.ImportedZones)
            {
                DateTime? importedRateEED = importedZone.ImportedNormalRate.EED;
                DateTime? maxCodeEED = null;

                foreach (var importedCode in importedZone.ImportedCodes)
                {
                    maxCodeEED = importedCode.EED.VRGreaterThan(maxCodeEED)
                      ? importedCode.EED
                      : maxCodeEED;
                }

                if (importedRateEED.HasValue && maxCodeEED.HasValue && maxCodeEED.Value != importedRateEED.Value)
                    messages.Add($"Code EED is different than rate EED in zone {importedZone.ZoneName}");
                if (importedRateEED.HasValue && importedZone.ImportedCodes.Any(item => !item.EED.HasValue))
                    messages.Add($"Cannot set rate EED for zone {importedZone.ZoneName} if no related code EED exist");
            }

            if (messages.Count > 0)
            {
                context.Message = string.Join(",", messages);
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
