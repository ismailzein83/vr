﻿using System;
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
                context.Message = string.Format("Creation of Zone {0} has been canceled because it does not contains codes", newZone.Name);

            return newZone.hasChanges;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
