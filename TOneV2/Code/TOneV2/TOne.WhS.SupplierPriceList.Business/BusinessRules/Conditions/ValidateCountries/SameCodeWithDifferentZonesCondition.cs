using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SameCodeWithDifferentZonesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCountry != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCountry country = context.Target as ImportedCountry;
            var messages = new List<string>();
            var duplicatedCodes = new List<string>();
            foreach (var importedCode in country.ImportedCodes)
            {
                if (!duplicatedCodes.Contains(importedCode.Code))
                {
                    var zones = country.ImportedCodes
                        .Where(x => x.Code == importedCode.Code && !x.ZoneName.Equals(importedCode.ZoneName, StringComparison.InvariantCultureIgnoreCase))
                        .Select(item => item.ZoneName);

                    if (zones.Count() > 0)
                    {
                        var invalidZones = new List<string>();
                        invalidZones.Add(importedCode.ZoneName);
                        foreach (var zoneName in zones)
                        {
                            if (!invalidZones.Any(item => item.ToLower() == zoneName.ToLower()))
                                invalidZones.Add(zoneName);
                        }
                        
                        duplicatedCodes.Add(importedCode.Code);
                        messages.Add(string.Format("Duplicate Code '{0}' found in ({1}).", importedCode.Code, string.Join(", ",invalidZones)));
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

    }
}
