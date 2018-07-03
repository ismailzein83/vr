using System;
using System.Text;
using TOne.WhS.Deal.Business;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList;

namespace TOne.WhS.Deal.BusinessProcessRules
{
    public class SPValidateClosedZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllZones;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            var countryZones = context.Target as AllZones;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            var zoneMessages = new List<string>();
            foreach (var notImportedZone in countryZones.NotImportedZones)
            {
                if (notImportedZone.EED.HasValue)
                {
                    var dealId = dealDefinitionManager.IsZoneIncludedInDeal(importSPLContext.SupplierId,
                        notImportedZone.ZoneId, notImportedZone.EED.Value, false);
                    if (dealId.HasValue)
                    {
                        var deal = new DealDefinitionManager().GetDealDefinition(dealId.Value);
                        zoneMessages.Add(string.Format("zone '{0}' in deal '{1}'", notImportedZone.ZoneName, deal.Name));
                    }
                }

            }
            if (zoneMessages.Any())
            {
                string zoneMessageString = string.Join(",", zoneMessages);
                context.Message = String.Format("Following closed zones are included in effective deals : {0}", zoneMessageString);
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
