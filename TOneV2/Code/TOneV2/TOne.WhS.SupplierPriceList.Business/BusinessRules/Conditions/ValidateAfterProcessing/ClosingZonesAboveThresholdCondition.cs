using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ClosingZonesAboveThresholdCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllZones;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            AllZones allZones = context.Target as AllZones;
            if (allZones != null)
            {
                if (allZones.AllImportedZones != null && allZones.NotImportedZones != null && (allZones.NotImportedZones.Count() > allZones.AllImportedZones.Zones.Count()))
                {
                    context.Message = "More than 50% of zones will be closed";
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
