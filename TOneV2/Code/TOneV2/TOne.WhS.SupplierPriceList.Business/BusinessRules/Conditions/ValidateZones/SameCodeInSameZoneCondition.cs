using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SameCodeInSameZoneCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {

            ImportedDataByZone zone = context.Target as ImportedDataByZone;
            var invalidCodes = new HashSet<string>();
            foreach (var importedCode in zone.ImportedCodes)
            {
                if (!string.IsNullOrEmpty(importedCode.Code) && zone.ImportedCodes.Where(x => x.Code == importedCode.Code).Count() > 1)
                    invalidCodes.Add(importedCode.Code);  
            }

            if (invalidCodes.Count>0)
            {
                context.Message = string.Format("Following codes are defined twice in zone {0}: {1}.", zone.ZoneName, string.Join(", ", invalidCodes));
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
