﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.BP.Arguments
{
    public class BalanceAlertThresholdUpdateProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            throw new NotImplementedException();
        }

        public Guid AlertRuleTypeId { get; set; }
    }
}
