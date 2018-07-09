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
        public override Guid BPBusinessRuleActionTypeId
        {
            get { return new Guid("2C3ED299-C955-44B3-A708-E9B53A24CB0E"); }
        }
        public override void Execute(IBusinessRuleActionExecutionContext context)
        {
            ImportedDataByZone importedDataByZone = context.Target as ImportedDataByZone;


            Dictionary<int, List<ImportedZoneService>> importedZoneServicesByServiceId = new Dictionary<int, List<ImportedZoneService>>();
            DateTime minZoneServiceBED = importedDataByZone.ImportedZoneServicesToValidate.SelectMany(item => item.Value).Min(itm => itm.BED);
            foreach (KeyValuePair<int, List<ImportedZoneService>> item in importedDataByZone.ImportedZoneServicesToValidate)
            {
                List<ImportedZoneService> importedZoneServices = new List<ImportedZoneService>();
                ImportedZoneService importedZoneService = item.Value.First();
                importedZoneServices.Add(new ImportedZoneService()
                {
                    BED = minZoneServiceBED,
                    ZoneName = importedZoneService.ZoneName,
                    ServiceId = importedZoneService.ServiceId,
                    EED = importedZoneService.EED
                });

                importedZoneServicesByServiceId.Add(item.Key, importedZoneServices);
            }

            importedDataByZone.ImportedZoneServicesToValidate = importedZoneServicesByServiceId;
        }

        public override ActionSeverity GetSeverity()
        {
            return ActionSeverity.Warning;
        }
        public override string GetDescription()
        {
            return "warning";
        }
    }
}
