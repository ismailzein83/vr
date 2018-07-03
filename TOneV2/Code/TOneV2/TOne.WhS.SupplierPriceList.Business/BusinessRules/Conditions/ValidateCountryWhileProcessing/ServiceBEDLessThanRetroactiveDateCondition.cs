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
            return target is ImportedCountry;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var importedCountry = context.Target as ImportedCountry;

            var invalidZoneNames = new HashSet<string>();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            foreach (ImportedZone zoneData in importedCountry.ImportedZones)
            {
                if (zoneData.ImportedZoneServiceGroup != null && zoneData.ImportedZoneServiceGroup.BED < importSPLContext.RetroactiveDate && zoneData.ImportedZoneServiceGroup.ChangeType == ZoneServiceChangeType.New)
                {
                    invalidZoneNames.Add(zoneData.ZoneName);
                }
            }

            if (invalidZoneNames.Count > 0)
            {
                string retroactiveDateString = importSPLContext.RetroactiveDate.ToString(importSPLContext.DateFormat);
                context.Message = string.Format("Adding services in dates less than retroactive date {0} for the following zone(s): {1}.", retroactiveDateString, string.Join(", ", invalidZoneNames));
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
