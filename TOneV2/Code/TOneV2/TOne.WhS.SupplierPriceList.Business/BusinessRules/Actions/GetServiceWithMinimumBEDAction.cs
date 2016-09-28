using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class GetServiceWithMinimumBEDAction : BusinessRuleAction
    {
        public override void Execute(IBusinessRuleActionExecutionContext context)
        {
            ImportedDataByZone importedDataByZone = context.Target as ImportedDataByZone;
            Dictionary<int, ImportedZoneService> importedZoneServicesByServiceId = new Dictionary<int, ImportedZoneService>();

            if (importedDataByZone != null)
            {
                foreach (ImportedZoneService importedZoneService in importedDataByZone.ImportedZoneServices)
                {
                    IEnumerable<ImportedZoneService> tmp = importedDataByZone.ImportedZoneServices.FindAllRecords(item => item.ServiceId == importedZoneService.ServiceId).OrderBy(itm => itm.BED);
                    ImportedZoneService importedZoneServiceWithMinBED = importedDataByZone.ImportedZoneServices.FindAllRecords(item => item.ServiceId == importedZoneService.ServiceId).OrderBy(itm => itm.BED).FirstOrDefault();

                    ImportedZoneService tmpImportedZoneService;

                    if (!importedZoneServicesByServiceId.TryGetValue(importedZoneService.ServiceId, out tmpImportedZoneService))
                        importedZoneServicesByServiceId.Add(importedZoneService.ServiceId, importedZoneServiceWithMinBED);
                }

                importedDataByZone.ImportedZoneServices = importedZoneServicesByServiceId.Values.ToList();
            }
        }

        public override ActionSeverity GetSeverity()
        {
            return ActionSeverity.Information;
        }
    }
}
