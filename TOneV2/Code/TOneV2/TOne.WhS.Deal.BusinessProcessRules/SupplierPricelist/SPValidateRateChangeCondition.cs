using System;
using System.Linq;
using TOne.WhS.Deal.Business;
using System.Collections.Generic;
using TOne.WhS.SupplierPriceList;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;

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
            var dealDefinitionManager = new DealDefinitionManager();
            var countryZones = context.Target as AllZones;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            var zoneMessages = new List<string>();
            foreach (var importedZone in countryZones.ImportedZones.Zones)
            {
                var importedRate = importedZone.ImportedNormalRate;
                var rate = importedRate.NewRates.First();
                DateTime effectivedate = DateTime.Now;

                if (importedRate.ChangeType == RateChangeType.Deleted)
                    effectivedate = importedRate.EED.Value;
                if (importedRate.ChangeType == RateChangeType.Decrease || importedRate.ChangeType == RateChangeType.Increase)
                    effectivedate = importedRate.BED;

                string dealMessage = Helper.GetDealZoneMessage(importSPLContext.SupplierId, rate.Zone.ZoneId, importedRate.ZoneName, effectivedate, false);
                if (dealMessage != null)
                    zoneMessages.Add(dealMessage);
            }
            if (zoneMessages.Any())
            {
                string zoneMessageString = string.Join(",", zoneMessages);
                context.Message = String.Format("Modified rates cannot be done for zones included in deals. Following zones are : {0}", zoneMessageString);
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
