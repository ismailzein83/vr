using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class RenamedZonesCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllImportedZones;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            List<string> renamedZones = new List<string>();
            AllImportedZones allImportedZones = context.Target as AllImportedZones;
            if (allImportedZones.Zones == null || allImportedZones.Zones.Count() == 0)
                return true;
            foreach (var importedZone in allImportedZones.Zones)
            {
                if (importedZone.ChangeType == ZoneChangeType.Renamed)
                    renamedZones.Add(importedZone.ZoneName);
            }
            if (renamedZones.Count() > 0)
            {
                context.Message = string.Format("The following zone(s) has been renamed :'{0}'", string.Join(",", renamedZones));
                return false;
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}




   

