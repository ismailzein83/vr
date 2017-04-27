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
    public class MoveCodeToPendingEffectiveZoneCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zoneTopProcess = context.Target as ZoneToProcess;

            if (zoneTopProcess.ChangeType == ZoneChangeType.New || zoneTopProcess.ChangeType == ZoneChangeType.Renamed)
                return true;

            if (zoneTopProcess.CodesToAdd.Count() > 0 && zoneTopProcess.BED > DateTime.Today.Date)
            {
                context.Message = string.Format("Cannot add codes to the pending zone {0}", zoneTopProcess.ZoneName);
                return false;
            }

            if (zoneTopProcess.CodesToMove.Count() > 0 && zoneTopProcess.BED > DateTime.Today.Date)
            {
                context.Message = string.Format("Cannot move codes to the pending zone {0}", zoneTopProcess.ZoneName);
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
