﻿using System;
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
    public class AddMoveCodeToPendingClosedZoneCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {

            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            foreach (CodeToAdd codeToAdd in zoneToProcess.CodesToAdd)
            {
                ExistingZone existingZone = zoneToProcess.ExistingZones.FindRecord(item=>item.ZoneEntity.Name.Equals(codeToAdd.ZoneName, StringComparison.InvariantCultureIgnoreCase));

                if (existingZone != null && existingZone.EED.HasValue)
                    return false;
            }



            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                ExistingZone existingZone = zoneToProcess.ExistingZones.FindRecord(item => item.ZoneEntity.Name.Equals(codeToMove.ZoneName, StringComparison.InvariantCultureIgnoreCase));

                if (existingZone != null && existingZone.EED.HasValue)
                    return false;
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Can not move or add code to the pending closed zone {0}", (target as ZoneToProcess).ZoneName);
        }

    }
}
