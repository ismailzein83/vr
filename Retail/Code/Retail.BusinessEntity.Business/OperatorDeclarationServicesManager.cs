using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Retail.BusinessEntity.Business
{
   public class OperatorDeclarationServicesManager
    {
       public IEnumerable<OperatorDirectionServicesMappedCellExtensionConfiguration> GetMappedCellsExtensionConfigs()
       {
           var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
           return extensionConfigManager.GetExtensionConfigurations<OperatorDirectionServicesMappedCellExtensionConfiguration>(OperatorDirectionServicesMappedCellExtensionConfiguration.EXTENSION_TYPE);
       }
    }
}
