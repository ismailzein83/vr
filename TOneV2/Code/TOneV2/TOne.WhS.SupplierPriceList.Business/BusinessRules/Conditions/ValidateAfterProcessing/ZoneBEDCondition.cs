using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    class ZoneBEDCondition : BusinessRuleCondition
    {
      
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target as ImportedZone != null;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedZone importedZone = context.Target as ImportedZone;

            if(importedZone.ChangeType == ZoneChangeType.New || importedZone.ChangeType == ZoneChangeType.Renamed)
            {
                return (Vanrise.Common.ExtensionMethods.VRLessThanOrEqual(DateTime.Today.Date, importedZone.BED));
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has been opened in a period less than system period", (target as ImportedZone).ZoneName);
        }

    }
}
