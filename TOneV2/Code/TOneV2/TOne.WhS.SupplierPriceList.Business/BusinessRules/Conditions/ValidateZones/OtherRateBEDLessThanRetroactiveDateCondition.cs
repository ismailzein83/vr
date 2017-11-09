using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class OtherRateBEDLessThanRetroactiveDateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
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

            var invalidRateTypes = new HashSet<string>();
            var invalidZoneNames = new HashSet<string>();
            var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            foreach (ImportedDataByZone zoneData in allData.ImportedDataByZoneList)
            {
                foreach (ImportedRate importedOtherRate in zoneData.ImportedOtherRates.Values.SelectMany(x => x))
                {
                    if (importedOtherRate.BED < importSPLContext.RetroactiveDate)
                    {
                        invalidZoneNames.Add(zoneData.ZoneName);
                        invalidRateTypes.Add(rateTypeManager.GetRateTypeName(importedOtherRate.RateTypeId.Value));
                    }
                }
            }

            if (invalidZoneNames.Count > 0)
            {
                string retroactiveDateString = importSPLContext.RetroactiveDate.ToString(importSPLContext.DateFormat);
                if (invalidZoneNames.Count == 1)
                    context.Message = string.Format("{0} is less than retroactive date '{1}' for the following zones: ({2}))", invalidRateTypes.First(), retroactiveDateString, string.Join(", ", invalidZoneNames));
                else
                    context.Message = string.Format("({0}) are less than retroactive date '{1}' for the following zones: ({2}))", string.Join(", ", invalidRateTypes), retroactiveDateString, string.Join(", ", invalidZoneNames));
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
