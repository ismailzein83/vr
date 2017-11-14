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
    public class NormalRateBEDLessThanRetroactiveDateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is ImportedCountry;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var importedCountry = context.Target as ImportedCountry;
            var invalidZoneNames = new HashSet<string>();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            foreach (var importedRate in importedCountry.ImportedRates)
            {
                if (importedRate.RateTypeId == null && importedRate.BED < importSPLContext.RetroactiveDate && (importedRate.ChangeType == RateChangeType.New || importedRate.ChangeType == RateChangeType.Decrease || importedRate.ChangeType==RateChangeType.Increase)  )
                {
                    invalidZoneNames.Add(importedRate.ZoneName);
                }
            }

            if (invalidZoneNames.Count > 0)
            {
                string retroactiveDateString = importSPLContext.RetroactiveDate.ToString(importSPLContext.DateFormat);
                context.Message = string.Format("Changing rates with dates less than retroactive date {0} for the following zone(s): {1}.", retroactiveDateString, string.Join(", ", invalidZoneNames));
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
