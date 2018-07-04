using System;
using System.Linq;
using System.Collections.Generic;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class RenamedZonesCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllZones;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var renamedZones = new List<string>();
            var allZones = context.Target as AllZones;
            var allImportedZones = allZones.ImportedZones;

            if (allImportedZones.Zones == null || allImportedZones.Zones.Count() == 0)
                return true;

            foreach (var importedZone in allImportedZones.Zones)
            {
                if (importedZone.ChangeType == ZoneChangeType.Renamed)
                    renamedZones.Add(importedZone.ZoneName);
            }
            if (renamedZones.Any())
            {
                context.Message = string.Format("The following zone(s) have been renamed :'{0}'", string.Join(",", renamedZones));
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






