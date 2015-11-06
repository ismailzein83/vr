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
       public List<TemplateConfig> GetNormalizeNumberActionSettingsTemplates()
       {
           TemplateConfigManager manager = new TemplateConfigManager();
           return manager.GetTemplateConfigurations(Constants.NormalizeNumberActionConfigType);
       }
    }
}
