using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    public class CodeLenghtCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target as AllZonesToProcess != null;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            AllZonesToProcess allZonesToProcess = context.Target as AllZonesToProcess;
            var invalidZones = new List<string>();

            foreach (var zoneToProcess in allZonesToProcess.Zones)
            {
                foreach (var codeToAdd in zoneToProcess.CodesToAdd)
                {
                    if (codeToAdd.Code.Length > 20)
                    {
                        invalidZones.Add(zoneToProcess.ZoneName);
                        break;
                    }
                }
            }

            if (invalidZones.Count > 0)
            {
                context.Message = string.Format("Following zones have code that exceeds the maximum number of characters (20) : {0}.", string.Join(", ", invalidZones));
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
