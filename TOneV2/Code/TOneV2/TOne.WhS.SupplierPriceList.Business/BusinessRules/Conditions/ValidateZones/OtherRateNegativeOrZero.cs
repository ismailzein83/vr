using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    class OtherRateNegativeOrZero : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllImportedDataByZone;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            AllImportedDataByZone allImportedDataByZone = context.Target as AllImportedDataByZone;
            var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();
            var messages = new List<string>();

            foreach (var importedZone in allImportedDataByZone.ImportedDataByZoneList)
            {
                if (importedZone != null && importedZone.ImportedOtherRates != null && importedZone.ImportedOtherRates.Count > 0)
                {
                    var invalidOtherRate = new HashSet<string>();
                    foreach (KeyValuePair<int, List<ImportedRate>> importedOtherRate in importedZone.ImportedOtherRates)
                    {

                        if (importedOtherRate.Value.Any(item => item.Rate < 0))
                        {
                            string rateTypeName = rateTypeManager.GetRateTypeName(importedOtherRate.Key);
                            invalidOtherRate.Add(rateTypeName);
                        }
                    }
                    if (invalidOtherRate.Any())
                    messages.Add(string.Format("({0}) for zone {1}", string.Join(", ",invalidOtherRate), importedZone.ZoneName));
                }
            }

            if (messages.Any())
            {
                context.Message = string.Format("Other rate can not be negative. Violated other rates: {0}.", string.Join(", ", messages));
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