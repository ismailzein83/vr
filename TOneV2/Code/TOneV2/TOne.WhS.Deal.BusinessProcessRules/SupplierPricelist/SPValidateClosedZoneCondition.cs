using System;
using System.Linq;
using System.Collections.Generic;
using TOne.WhS.SupplierPriceList;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.SupplierPriceList.Entities;

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
            var allZones = context.Target as AllZones;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            var zoneMessages = new HashSet<string>();
            foreach (var notImportedZone in allZones.NotImportedZones)
            {
                string dealMessage = Helper.GetDealZoneMessage(importSPLContext.SupplierId, notImportedZone.ZoneId, notImportedZone.ZoneName, DateTime.Now, false);
                if (dealMessage != null)
                    zoneMessages.Add(dealMessage);
            }

            if (zoneMessages.Any())
            {
                string zoneMessageString = string.Join(",", zoneMessages);
                context.Message = $"Following closed zones are included in effective deals : {zoneMessageString}";
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
