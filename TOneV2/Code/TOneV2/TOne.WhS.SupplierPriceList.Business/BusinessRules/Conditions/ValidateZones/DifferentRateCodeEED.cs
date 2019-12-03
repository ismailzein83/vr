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
        //This rule should be last rule to execute in "ValidateZones"
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target as ImportedDataByZone != null;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedDataByZone importedZone = context.Target as ImportedDataByZone;
            var messages = new List<string>();

            if (importedZone.ImportedNormalRates.All(item => item.EED.HasValue))
            {
                DateTime? importedRateEED = importedZone.ImportedNormalRates.Max(item => item.EED);
                DateTime? maxCodeEED = new DateTime();
                foreach (var importedCode in importedZone.ImportedCodes)
                {
                    maxCodeEED = importedCode.EED.VRGreaterThan(maxCodeEED)
                      ? importedCode.EED
                      : maxCodeEED;
                }
                if (importedRateEED.HasValue && maxCodeEED.HasValue && maxCodeEED.Value != importedRateEED.Value)
                    messages.Add($"Code EED is different than rate EED in zone {importedZone.ZoneName}");
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
