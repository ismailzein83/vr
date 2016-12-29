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

            foreach (KeyValuePair<int, List<ImportedZoneService>> item in importedDataByZone.ImportedZoneServicesToValidate)
                if (item.Value.Any(itm => itm.BED != item.Value.First().BED))
                {
                    context.Message = string.Format("Zone {0} has services with different BED, minimum BED has been selected", importedDataByZone.ZoneName);
                    return false;
                }

            List<ImportedZoneService> importedZoneServices = new List<ImportedZoneService>();

            foreach (KeyValuePair<int, List<ImportedZoneService>> item in importedDataByZone.ImportedZoneServicesToValidate)
                importedZoneServices.Add(item.Value.First());

            var result = importedZoneServices.All(item => item.BED == importedZoneServices.First().BED);

            if(result == false)
                context.Message = string.Format("Zone {0} has services with different BED, minimum BED has been selected", importedDataByZone.ZoneName);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has services with different BED, minimum BED has been selected", (target as ImportedDataByZone).ZoneName);
        }

    }
}
