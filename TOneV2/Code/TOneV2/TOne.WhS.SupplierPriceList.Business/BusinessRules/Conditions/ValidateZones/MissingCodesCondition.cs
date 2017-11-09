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
    public class MissingCodesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as AllImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            AllImportedDataByZone allImportedDataByZone = context.Target as AllImportedDataByZone;
            var invalidZones = new HashSet<string>();

            foreach (var zone in allImportedDataByZone.ImportedDataByZoneList)
            {
                foreach (var importedCode in zone.ImportedCodes)
                {
                    if (string.IsNullOrEmpty(importedCode.Code))
                    {
                        invalidZones.Add(zone.ZoneName);
                        break;
                    }
                }
            }
            if (invalidZones.Count>0)
            {
                context.Message = string.Format("Codes are missing for the following zone(s): {0}.", string.Join(", ", invalidZones));
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
