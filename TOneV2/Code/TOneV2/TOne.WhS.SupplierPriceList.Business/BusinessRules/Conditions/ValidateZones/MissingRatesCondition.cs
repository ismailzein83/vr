using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class MissingRatesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as AllImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            AllImportedDataByZone allImportedDataByZone = context.Target as AllImportedDataByZone;
            var invalidZones = new HashSet<string>();

            foreach (var zone in allImportedDataByZone.ImportedDataByZoneList)
            {
                if (zone.ImportedNormalRates.Count == 0)
                    invalidZones.Add(zone.ZoneName);
            }

            if (invalidZones.Count > 0)
            {
                context.Message = string.Format("Rates are missing for the following zone(s): {0}.", string.Join(", ", invalidZones));
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
