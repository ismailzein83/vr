using System;
using System.Linq;
using TOne.WhS.Deal.Business;
using TOne.WhS.SupplierPriceList;
using System.Collections.Generic;
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
            var countryZones = context.Target as AllZones;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            var zoneMessages = new List<string>();
            foreach (var notImportedZone in countryZones.NotImportedZones.Where(c => c.HasChanged))
            {
                if (notImportedZone.EED.HasValue)
                    zoneMessages.Add(Helper.GetDealZoneMessage(importSPLContext.SupplierId, notImportedZone.ZoneId, notImportedZone.ZoneName, notImportedZone.EED.Value, false));
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
