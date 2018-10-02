using System;
using Vanrise.Common;
using System.ComponentModel;

namespace Vanrise.BusinessProcess.Entities
{
    public enum ActionSeverity {
        [Description("Information")]
        Information = 0,

        [Description("Warning")]
        Warning = 1,

        [Description("Error")]
        Error = 2
    };

    public abstract class BusinessRuleAction
    {
        public abstract Guid BPBusinessRuleActionTypeId { get;}

        public abstract void Execute(IBusinessRuleActionExecutionContext context);

        public abstract ActionSeverity GetSeverity();
        public string GetDescription()
        {
            IExtensionConfigurationManager configManager = Vanrise.Common.BusinessManagerFactory.GetManager<IExtensionConfigurationManager>();
            return configManager.GetExtensionConfigurationTitle<BPBusinessRuleActionType>(BPBusinessRuleActionTypeId, BPBusinessRuleActionType.EXTENSION_TYPE);
        }
    }
}
