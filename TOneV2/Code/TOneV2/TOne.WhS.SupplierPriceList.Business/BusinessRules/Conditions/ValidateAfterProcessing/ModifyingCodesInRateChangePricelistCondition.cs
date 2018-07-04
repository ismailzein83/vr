using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ModifyingCodesInRateChangePricelistCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllZones;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            if (importSPLContext.SupplierPricelistType == BusinessEntity.Entities.SupplierPricelistType.RateChange)
            {
                List<string> zonesWithCodeChange = new List<string>();
                var allZones = context.Target as AllZones;
                AllImportedZones allImportedZones = allZones.ImportedZones;

                foreach (var importedZone in allImportedZones.Zones)
                {
                    if (importedZone.ImportedCodes != null && importedZone.ImportedCodes.Count() > 0)
                        foreach (var importedCode in importedZone.ImportedCodes)
                        {
                            if (importedCode.ChangeType != CodeChangeType.NotChanged)
                            {
                                zonesWithCodeChange.Add(importedZone.ZoneName);
                                break;
                            }
                        }

                }
                if (zonesWithCodeChange.Count() > 0)
                {
                    context.Message = string.Format("Cannot change code(s) in Rate Change pricelist on following zone(s) :'{0}'", string.Join(",", zonesWithCodeChange));
                    return false;
                }
            }
            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
