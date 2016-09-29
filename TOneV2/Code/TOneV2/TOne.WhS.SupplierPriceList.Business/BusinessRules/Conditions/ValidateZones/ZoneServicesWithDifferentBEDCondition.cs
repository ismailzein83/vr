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
                    return false;

            List<ImportedZoneService> importedZoneServices = new List<ImportedZoneService>();

            foreach (KeyValuePair<int, List<ImportedZoneService>> item in importedDataByZone.ImportedZoneServicesToValidate)
                importedZoneServices.Add(item.Value.First());

            bool result;

            if (result = importedZoneServices.All(item => item.BED != importedZoneServices.First().BED))
            {
                DateTime zoneServiceWithMinBED = importedZoneServices.Min(item => item.BED);

                importedDataByZone.ImportedZoneServiceGroup = new ImportedZoneServiceGroup()
                {
                    ServiceIds = importedZoneServices.Select(item => item.ServiceId).ToList(),
                    ZoneName = importedDataByZone.ZoneName,
                    BED = zoneServiceWithMinBED,
                    EED = importedZoneServices.First().EED
                };
            }

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has services with different begin effective date, minimum begin effective date has been selected", (target as ImportedDataByZone).ZoneName);
        }

    }
}
