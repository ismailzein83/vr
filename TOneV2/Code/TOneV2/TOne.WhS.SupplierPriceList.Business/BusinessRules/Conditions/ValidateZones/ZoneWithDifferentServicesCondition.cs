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
    public class ZoneWithDifferentServicesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedDataByZone importedDataByZone = context.Target as ImportedDataByZone;

            Dictionary<int, List<ImportedZoneService>> importedZoneServicesByServiceId = new Dictionary<int, List<ImportedZoneService>>();

            if (importedDataByZone.ImportedZoneServices.Count > 0)
            {
                foreach (ImportedZoneService importedZoneService in importedDataByZone.ImportedZoneServices)
                {
                    IEnumerable<ImportedZoneService> currentZoneService = importedDataByZone.ImportedZoneServices.FindAllRecords(item => item.ServiceId == importedZoneService.ServiceId);

                    List<ImportedZoneService> tmpImportedZoneServices;

                    if (!importedZoneServicesByServiceId.TryGetValue(importedZoneService.ServiceId, out tmpImportedZoneServices))
                    {
                        tmpImportedZoneServices = new List<ImportedZoneService>();
                        tmpImportedZoneServices.Add(importedZoneService);
                        importedZoneServicesByServiceId.Add(importedZoneService.ServiceId, tmpImportedZoneServices);
                    }
                    else
                        tmpImportedZoneServices.Add(importedZoneService);

                }

                int firstZoneServicesCount = importedZoneServicesByServiceId.Values.First().Count();

                return importedZoneServicesByServiceId.Values.All(item => item.Count() == firstZoneServicesCount);
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has different services", (target as ImportedDataByZone).ZoneName);
        }

    }
}
