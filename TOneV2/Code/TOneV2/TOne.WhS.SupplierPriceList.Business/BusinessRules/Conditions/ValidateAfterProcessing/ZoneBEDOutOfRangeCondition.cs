using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ZoneBEDOutOfRangeCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is ImportedCountry;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var importedCountry = context.Target as ImportedCountry;

            if (importedCountry.ImportedZones == null || importedCountry.ImportedZones.Count == 0)
                return true;

            DateTime today = DateTime.Today;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            var invalidZoneNames = new List<string>();

            foreach (ImportedZone importedZone in importedCountry.ImportedZones)
            {
                if (importedZone.ChangeType == ZoneChangeType.New || importedZone.ChangeType == ZoneChangeType.Renamed)
                {
                    if (importedZone.BED < today || importedZone.BED > importSPLContext.CodeEffectiveDate)
                        invalidZoneNames.Add(importedZone.ZoneName);
                }
            }

            if (invalidZoneNames.Count > 0)
            {
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(importedCountry.CountryId);
                string todayString = today.ToString(importSPLContext.DateFormat);
                string codeEffectiveDateString = importSPLContext.CodeEffectiveDate.ToString(importSPLContext.DateFormat);
                context.Message = string.Format("BEDs of some of the zones of country '{0}' must be between '{1}' and '{2}': {3}", countryName, todayString, codeEffectiveDateString, string.Join(", ", invalidZoneNames));
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
