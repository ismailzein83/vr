using RecordAnalysis.Entities;
using System.Collections.Generic;
using Vanrise.Common.Business;

namespace RecordAnalysis.Business
{
    public class C4SwitchManager
    {
        public IEnumerable<C4SwitchSettingsConfig> GetC4SwitchTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<C4SwitchSettingsConfig>(C4SwitchSettingsConfig.EXTENSION_TYPE);
        }
    }
}
