using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using Vanrise.Common;
using System;
using Vanrise.Common.Business;

namespace Vanrise.BusinessProcess.Business
{
    public class BPBusinessRuleActionTypeManager
    {
        #region public methods
        public BPBusinessRuleActionType GetBusinessRuleActionType(Guid bpBusinessRuleActionTypeId)
        {
            var extensions = GetBPBusinessRuleActionTypeConfigs();
            return extensions.FindRecord(x=>x.ExtensionConfigurationId == bpBusinessRuleActionTypeId);
        }

        public IEnumerable<BPBusinessRuleActionType> GetBPBusinessRuleActionTypeConfigs()
        {
            ExtensionConfigurationManager extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<BPBusinessRuleActionType>(BPBusinessRuleActionType.EXTENSION_TYPE);
        }
        #endregion
    }
}