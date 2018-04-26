using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Business
{
    public class OtherRateBEDCondition : BusinessRuleCondition
    {
      /*  public override bool ShouldValidate(IRuleTarget target)
        {
            return target is DataByZone;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zoneData = context.Target as DataByZone;
            if (zoneData.EED != null && zoneData.OtherRatesToChange.Count() > 0 && zoneData.OtherRatesToChange.First().BED < zoneData.EED)
            {
                context.Message = string.Format("Other rate BED on  '{0}' must be after pending normal rate BED", zoneData.ZoneName);
                return false;
            }
            return true;
           
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }*/
    }
}

