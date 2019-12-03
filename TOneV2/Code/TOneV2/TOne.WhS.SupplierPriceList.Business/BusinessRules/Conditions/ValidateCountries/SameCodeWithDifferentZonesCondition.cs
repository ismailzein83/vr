using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SameCodeWithDifferentZonesCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target as ImportedCountry != null;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var country = context.Target as ImportedCountry;
            var messages = new List<string>();
            var duplicatedCodes = new List<string>();
            foreach (var importedCode in country.ImportedCodes)
            {
                if (!duplicatedCodes.Contains(importedCode.Code))
                {
                    HashSet<string> zoneNames = GetZoneNamesWithDuplicateCodes(importedCode, country.ImportedCodes);

                    if (zoneNames.Count() > 0)
                    {
                        duplicatedCodes.Add(importedCode.Code);
                        messages.Add(string.Format("Duplicate Code '{0}' found in ({1}).", importedCode.Code, string.Join(", ", zoneNames)));
                    }
                }
            }
            if (messages.Count > 0)
            {
                context.Message = string.Join(" ", messages);
                return false;
            }
            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
        private HashSet<string> GetZoneNamesWithDuplicateCodes(ImportedCode codeToCompare, IEnumerable<ImportedCode> importedCodes)
        {
           var zoneNames = new HashSet<string>();
            foreach (var importedCode in importedCodes)
            {
                if (codeToCompare.Code == importedCode.Code && !importedCode.ZoneName.Equals(codeToCompare.ZoneName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (importedCode.EED.HasValue && codeToCompare.EED.HasValue || (!importedCode.EED.HasValue && !codeToCompare.EED.HasValue))
                    {
                        zoneNames.Add(importedCode.ZoneName);
                        zoneNames.Add(codeToCompare.ZoneName);
                    }
                }
            }
            return zoneNames;
        }
    }
}
