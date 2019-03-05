using RecordAnalysis.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common.Business;

namespace RecordAnalysis.Business
{
    public class C4SwitchManager
    {
        public static readonly Guid BeDefinitionId = new Guid("9e7ecdc0-e19b-43d2-9edb-ddc7bbc0f764");

        public IEnumerable<C4SwitchSettingsConfig> GetC4SwitchTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<C4SwitchSettingsConfig>(C4SwitchSettingsConfig.EXTENSION_TYPE);
        }
    }
}
