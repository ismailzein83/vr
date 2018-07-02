using System;
using System.Text;
using TOne.WhS.Deal.Business;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.Deal.BusinessProcessRules
{
    public class SPValidateClosedZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is List<NotImportedZone>;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            var notImportedZones = context.Target as List<NotImportedZone>;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            StringBuilder sb = new StringBuilder();
            foreach (var notImportedZone in notImportedZones)
            {
                if (notImportedZone.EED.HasValue)
                {
                    var dealId = dealDefinitionManager.IsZoneIncludedInDeal(importSPLContext.SupplierId,
                        notImportedZone.ZoneId, notImportedZone.EED.Value, false);
                    if (dealId.HasValue)
                    {
                        var deal = new DealDefinitionManager().GetDealDefinition(dealId.Value);
                        sb.AppendFormat("zone '{0}' in deal '{1}'", notImportedZone.ZoneName, deal.Name);
                    }
                }

            }
            if (sb.Length > 1)
            {
                context.Message = String.Format("Cannot close Zone if its linked to deal : {0}", sb.ToString());
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
