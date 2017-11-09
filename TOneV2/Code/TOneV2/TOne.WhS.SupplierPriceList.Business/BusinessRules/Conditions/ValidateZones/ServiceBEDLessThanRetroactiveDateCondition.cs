using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ServiceBEDLessThanRetroactiveDateCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllImportedDataByZone;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var allData = context.Target as AllImportedDataByZone;

            if (allData.ImportedDataByZoneList == null || allData.ImportedDataByZoneList.Count() == 0)
                return true;

            var invalidZoneNames = new HashSet<string>();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            foreach (ImportedDataByZone zoneData in allData.ImportedDataByZoneList)
            {
                foreach (ImportedZoneService importedService in zoneData.ImportedZoneServicesToValidate.Values.SelectMany(x => x))
                {
                    if (importedService.BED < importSPLContext.RetroactiveDate)
                    {
                        invalidZoneNames.Add(zoneData.ZoneName);
                        break;
                    }
                }
            }

            if (invalidZoneNames.Count > 0)
            {
                string retroactiveDateString = importSPLContext.RetroactiveDate.ToString(importSPLContext.DateFormat);
                context.Message = string.Format("Services are less than retroactive date {0} for the following zones: {1}.", retroactiveDateString, string.Join(", ", invalidZoneNames));
                return false;
            }

            return true;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
