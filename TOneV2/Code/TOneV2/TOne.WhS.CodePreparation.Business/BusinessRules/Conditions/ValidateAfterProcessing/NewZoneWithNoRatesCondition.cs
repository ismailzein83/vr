using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
    public class NewZoneWithNoRatesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            bool result = true;
            ZoneToProcess zoneTopProcess = context.Target as ZoneToProcess;

            if (zoneTopProcess.ChangeType == ZoneChangeType.New)
                result = zoneTopProcess.RatesToAdd.Count() > 0;

            if (result == false)
                context.Message = string.Format("Zone '{0}' has been created without rates", zoneTopProcess.ZoneName);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone '{0}' has been created without rates", (target as ZoneToProcess).ZoneName);
        }

    }
}
