using System;
using System.Linq;
using System.Text;
using TOne.WhS.Deal.Business;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.Deal.BusinessProcessRules
{
    public class SPValidateRateChangeCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is List<ImportedZone>;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var dealDefinitionManager = new DealDefinitionManager();
            var importedZones = context.Target as List<ImportedZone>;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            StringBuilder sb = new StringBuilder();
            foreach (var importedZone in importedZones)
            {
                var importedRate = importedZone.ImportedNormalRate;
                var newZone = importedZone.NewZones.FirstOrDefault();
                if (newZone != null)
                {
                    if (importedRate.EED.HasValue)
                    {
                        var dealId = dealDefinitionManager.IsZoneIncludedInDeal(importSPLContext.SupplierId,
                            newZone.ZoneId, importedRate.EED.Value, false);
                        if (dealId.HasValue)
                        {
                            var deal = new DealDefinitionManager().GetDealDefinition(dealId.Value);
                            sb.AppendFormat("zone '{0}' in deal '{1}'", importedRate.ZoneName, deal.Name);
                        }
                    }
                }
            }
            if (sb.Length > 1)
            {
                context.Message = String.Format("Cannot perform rate Change if zones are linked to deal : {0}", sb.ToString());
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
