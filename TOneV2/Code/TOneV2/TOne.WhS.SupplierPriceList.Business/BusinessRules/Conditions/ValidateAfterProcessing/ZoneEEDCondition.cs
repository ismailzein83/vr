using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ZoneEEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is CountryNotImportedZones;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var countryNotImportedZones = context.Target as CountryNotImportedZones;

            if (countryNotImportedZones.NotImportedZones == null || countryNotImportedZones.NotImportedZones.Count() == 0)
                return true;

            var invalidZoneNames = new HashSet<string>();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            foreach (NotImportedZone notImportedZone in countryNotImportedZones.NotImportedZones)
            {
                if (notImportedZone.HasChanged && notImportedZone.EED.HasValue && notImportedZone.EED.Value < importSPLContext.CodeEffectiveDate)
                    invalidZoneNames.Add(notImportedZone.ZoneName);
            }

            if (invalidZoneNames.Count > 0)
            {
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryNotImportedZones.CountryId);
                string codeEffectiveDateString = importSPLContext.CodeEffectiveDate.ToString(importSPLContext.DateFormat);
                context.Message = string.Format("EEDs of some of the zones of country '{0}' must be greater than or equal to '{1}': {2}", countryName, codeEffectiveDateString, string.Join(", ", invalidZoneNames));
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
