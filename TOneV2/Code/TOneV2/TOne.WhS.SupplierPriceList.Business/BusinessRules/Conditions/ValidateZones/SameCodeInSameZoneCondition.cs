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

            foreach (var importedCode in zone.ImportedCodes)
            {
                if (zone.ImportedCodes.Where(x => x.Code == importedCode.Code).Count() > 1)
                {
                    context.Message = string.Format("Can not add Code {0} because Zone {1} contains this Code multiple times", importedCode.Code, zone.ZoneName);
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has one or more same code", (target as ImportedDataByZone).ZoneName);
        }

    }
}
