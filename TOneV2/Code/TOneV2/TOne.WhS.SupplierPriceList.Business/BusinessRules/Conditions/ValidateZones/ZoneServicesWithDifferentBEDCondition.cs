using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ZoneServicesWithDifferentBEDCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedDataByZone importedDataByZone = context.Target as ImportedDataByZone;

            foreach (ImportedZoneService importedZoneService in importedDataByZone.ImportedZoneServices)
            {
                if (importedDataByZone.ImportedZoneServices.FindRecord(item => item.ServiceId == importedZoneService.ServiceId && item.BED != importedZoneService.BED) != null)
                    return false;
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has services with different begin effective date, minimum begin effective date has been selected", (target as ImportedDataByZone).ZoneName);
        }

    }
}
