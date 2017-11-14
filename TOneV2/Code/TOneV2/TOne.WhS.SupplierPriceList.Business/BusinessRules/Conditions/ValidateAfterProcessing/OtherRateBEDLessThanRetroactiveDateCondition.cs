using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class OtherRateBEDLessThanRetroactiveDateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is ImportedCountry;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var importedCountry = context.Target as ImportedCountry;
            var invalidRateTypes = new HashSet<string>();
            var invalidZoneNames = new HashSet<string>();
            var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            foreach (var importedZone in importedCountry.ImportedZones)
            {
                foreach (var importedRate in importedZone.ImportedOtherRates.Values)
                {
                    if (importedRate.RateTypeId.HasValue && importedRate.BED < importSPLContext.RetroactiveDate && (importedRate.ChangeType == RateChangeType.New || importedRate.ChangeType == RateChangeType.Decrease || importedRate.ChangeType == RateChangeType.Increase))
                    {
                        invalidZoneNames.Add(importedRate.ZoneName);
                        invalidRateTypes.Add(rateTypeManager.GetRateTypeName(importedRate.RateTypeId.Value));
                    }
                }
            }

            if (invalidZoneNames.Count > 0)
            {
                string retroactiveDateString = importSPLContext.RetroactiveDate.ToString(importSPLContext.DateFormat);
                context.Message = string.Format("Changing ({0}) with dates less than retroactive date '{1}' for the following zone(s): ({2})", string.Join(", ", invalidRateTypes), retroactiveDateString, string.Join(", ", invalidZoneNames));
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
