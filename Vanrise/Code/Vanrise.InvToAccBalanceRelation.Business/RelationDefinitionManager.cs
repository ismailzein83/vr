using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.InvToAccBalanceRelation.Entities;

namespace Vanrise.InvToAccBalanceRelation.Business
{
    public class RelationDefinitionManager
    {
        public IEnumerable<RelationDefinitionExtendedSettingsConfig> GetRelationDefinitionExtendedSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<RelationDefinitionExtendedSettingsConfig>(RelationDefinitionExtendedSettingsConfig.EXTENSION_TYPE);
        }
    }
}
