using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
   public class GenericLKUPDefinitionManager
    {
       public IEnumerable<GenericLKUPDefinitionConfig> GetGenericLKUPDefintionTemplateConfigs()
       {
           var extensionConfiguration = new ExtensionConfigurationManager();
           return extensionConfiguration.GetExtensionConfigurations<GenericLKUPDefinitionConfig>(GenericLKUPDefinitionConfig.EXTENSION_TYPE);
       }

       public GenericLKUPDefinitionExtendedSettings GetGenericLKUPDefinitionExtendedSetings(Guid BusinessEntityDefinitionId)
       {

           IBusinessEntityDefinitionManager manager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IBusinessEntityDefinitionManager>();
           var genericLKUPDefinition=manager.GetBusinessEntityDefinition(BusinessEntityDefinitionId);
           if (genericLKUPDefinition != null)
           {
               var genericLKUPBEDefinitionSettings = genericLKUPDefinition.Settings as GenericLKUPBEDefinitionSettings;
               return genericLKUPBEDefinitionSettings.ExtendedSettings;
           }
           return null;
          
       }

    }
}
