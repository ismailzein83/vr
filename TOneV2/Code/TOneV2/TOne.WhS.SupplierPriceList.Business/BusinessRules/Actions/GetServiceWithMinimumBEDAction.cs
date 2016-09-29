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
            
            
            Dictionary<int, List<ImportedZoneService>> importedZoneServicesByServiceId = new Dictionary<int, List<ImportedZoneService>>();
            foreach (KeyValuePair<int, List<ImportedZoneService>> item in importedDataByZone.ImportedZoneServicesToValidate)
            {
                List<ImportedZoneService> importedZoneServices = new List<ImportedZoneService>();
                importedZoneServices.Add(item.Value.OrderBy(itm => itm.BED).First());
                importedZoneServicesByServiceId.Add(item.Key, importedZoneServices);
            }

            importedDataByZone.ImportedZoneServicesToValidate = importedZoneServicesByServiceId;
        }

        public override ActionSeverity GetSeverity()
        {
            return ActionSeverity.Information;
        }
    }
}
