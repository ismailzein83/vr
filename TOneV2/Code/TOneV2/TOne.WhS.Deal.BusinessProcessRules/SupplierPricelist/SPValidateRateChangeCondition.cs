using System;
using System.Linq;
using TOne.WhS.SupplierPriceList;
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
            return target is AllZones;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var allZones = context.Target as AllZones;

            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            var zoneMessages = new List<string>();
            foreach (var importedZone in allZones.ImportedZones.Zones)
            {
                var importedRate = importedZone.ImportedNormalRate;

                if (importedRate.NewRates == null || importedRate.NewRates.Count == 0)
                    continue;

                var rate = importedRate.NewRates.First();
                string dealMessage = Helper.GetDealZoneMessage(importSPLContext.SupplierId, rate.Zone.ZoneId, importedRate.ZoneName, DateTime.Now, false);
                if (dealMessage != null)
                    zoneMessages.Add(dealMessage);

            }

            if (zoneMessages.Any())
            {
                string zoneMessageString = string.Join(",", zoneMessages);
                context.Message = $"Modified rates cannot be done for zones included in deals. Following zones are : {zoneMessageString}";
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
