using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class NormalRateBEDLessThanRetroactiveDateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllImportedDataByZone;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var allData = context.Target as AllImportedDataByZone;

            if (allData.ImportedDataByZoneList == null || allData.ImportedDataByZoneList.Count() == 0)
                return true;

            var invalidZoneNames = new HashSet<string>();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            foreach (ImportedDataByZone zoneData in allData.ImportedDataByZoneList)
            {
                foreach (var importedNormalRate in zoneData.ImportedNormalRates)
                {
                    if (importedNormalRate.BED < importSPLContext.RetroactiveDate)
                    {
                        invalidZoneNames.Add(zoneData.ZoneName);
                        break;
                    }
                }
            }

            if (invalidZoneNames.Count > 0)
            {
                string retroactiveDateString = importSPLContext.RetroactiveDate.ToString(importSPLContext.DateFormat);
                context.Message = string.Format("Rates are less than retroactive date {0} for the following zones: {1}.", retroactiveDateString, string.Join(", ", invalidZoneNames));
                return false;
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
