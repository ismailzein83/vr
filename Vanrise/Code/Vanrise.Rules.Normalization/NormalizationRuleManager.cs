using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Rules.Normalization
{
   public  class NormalizationRuleManager
    {
       public IEnumerable<NormalizeNumberActionSettingsConfig> GetNormalizeNumberActionSettingsTemplates()
       {
           ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
           return manager.GetExtensionConfigurations<NormalizeNumberActionSettingsConfig>(NormalizeNumberActionSettingsConfig.EXTENSION_TYPE);
       }
    }
}
