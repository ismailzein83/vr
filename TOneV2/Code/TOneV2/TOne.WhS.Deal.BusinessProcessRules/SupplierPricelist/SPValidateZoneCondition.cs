using System;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.Deal.BusinessProcessRules
{
    public class SPValidateZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is ImportedCountry;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCountry importedCountry = context.Target as ImportedCountry;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            if (importedCountry.ImportedCodes == null || importedCountry.ImportedCodes.Count == 0)
                return true;

            foreach (var importedZone in importedCountry.ImportedZones)
            {
                if (importedZone.EED.HasValue && IsZoneLinkedToDeal(importSPLContext.SupplierId, importedZone.ZoneName, importedZone.BED))
                {
                    context.Message = string.Format("Cannot end zone: {0} if it's related to a deal", importedZone.ZoneName);
                    return false;
                }
            }
            return true;
        }

        private bool IsZoneLinkedToDeal(int supplierId, string zoneName, DateTime effectiveAfter)
        {
            return false;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
