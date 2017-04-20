using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business.GenericLKUP
{
   public class GenericLKUPDefinitionManager
    {
       public IEnumerable<GenericLKUPDefinitionConfig> GetGenericLKUPDefintionTemplateConfigs()
       {
           var extensionConfiguration = new ExtensionConfigurationManager();
           return extensionConfiguration.GetExtensionConfigurations<GenericLKUPDefinitionConfig>(GenericLKUPDefinitionConfig.EXTENSION_TYPE);
       }

    }
}
