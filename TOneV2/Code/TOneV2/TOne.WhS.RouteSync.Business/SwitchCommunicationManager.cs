using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Business
{
    public class SwitchCommunicationManager
    {
        public IEnumerable<SwitchCommunicationConfig> GetSwitchCommunicationTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SwitchCommunicationConfig>(SwitchCommunicationConfig.EXTENSION_TYPE);
        }
    }
}