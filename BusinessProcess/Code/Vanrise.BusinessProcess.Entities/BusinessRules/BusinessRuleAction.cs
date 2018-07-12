﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Vanrise.BusinessProcess.Entities
{
    public enum ActionSeverity { Information = 0, Warning = 1, Error = 2 };

    public abstract class BusinessRuleAction
    {
        public abstract Guid BPBusinessRuleActionTypeId { get;}

        public abstract void Execute(IBusinessRuleActionExecutionContext context);

        public abstract ActionSeverity GetSeverity();
        public string GetDescription()
        {
            ExtensionConfigurationManager configManager = new ExtensionConfigurationManager();
            return configManager.GetExtensionConfigurationTitle<BPBusinessRuleActionType>(BPBusinessRuleActionTypeId, BPBusinessRuleActionType.EXTENSION_TYPE);
        }
    }
}
