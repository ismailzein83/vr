﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
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

            if (zoneToProcess.CodesToAdd.Count() > 0 && zoneToProcess.EED.HasValue)
            {
                context.Message = string.Format("Cannot add code to pending closed zone '{0}'", zoneToProcess.ZoneName);
                return false;
            }


            if (zoneToProcess.CodesToMove.Count() > 0 && zoneToProcess.EED.HasValue)
            {
                context.Message = string.Format("Cannot move code to pending closed zone '{0}'", zoneToProcess.ZoneName);
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
