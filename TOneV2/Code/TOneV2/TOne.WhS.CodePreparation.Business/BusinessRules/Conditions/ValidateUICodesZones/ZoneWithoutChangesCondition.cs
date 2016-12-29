using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class ZoneWithoutChangesCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as NewZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            NewZone newZone = context.Target as NewZone;
            var result = newZone.hasChanges;

            if(result == false)
                context.Message = string.Format("Zone {0} has no codes, creation of zone has been canceled", newZone.Name);

            return newZone.hasChanges;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has no codes, creation of zone has been canceled", (target as NewZone).Name);
        }
    }
}
