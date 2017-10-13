using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class CodeBEDOutOfRangeCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is ImportedCountry;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            ImportedCountry importedCountry = context.Target as ImportedCountry;

            if (importedCountry.ImportedCodes == null || importedCountry.ImportedCodes.Count == 0)
                return true;

            var invalidZoneNames = new HashSet<string>();

            DateTime today = DateTime.Today;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            foreach (ImportedCode importedCode in importedCountry.ImportedCodes)
            {
                if (importedCode.ChangeType == CodeChangeType.New || importedCode.ChangeType == CodeChangeType.Moved)
                {
                    if (importedCode.BED < today || importedCode.BED > importSPLContext.CodeEffectiveDate)
                        invalidZoneNames.Add(importedCode.ZoneName);
                }
            }

            if (invalidZoneNames.Count > 0)
            {
                string todayString = today.ToString(importSPLContext.DateFormat);
                string codeEffectiveDateString = importSPLContext.CodeEffectiveDate.ToString(importSPLContext.DateFormat);
                context.Message = string.Format("BEDs of some of the codes of the following zones must be between '{0}' and '{1}': {2}", todayString, codeEffectiveDateString, string.Join(", ", invalidZoneNames));
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
