using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class CompanyDefinitionManager
    {
        public IEnumerable<CompanyDefinitionConfig> GetCompanyDefinitionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<CompanyDefinitionConfig>(CompanyDefinitionConfig.EXTENSION_TYPE).OrderByDescending(x => x.Name);
        }
    }
}
