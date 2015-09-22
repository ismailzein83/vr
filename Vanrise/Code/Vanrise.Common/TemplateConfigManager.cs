using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;

namespace Vanrise.Common
{
    public class TemplateConfigManager
    {
        public List<Entities.TemplateConfig> GetTemplateConfigurations(string configType)
        {
            ITemplateConfigDataManager manager = CommonDataManagerFactory.GetDataManager<ITemplateConfigDataManager>();
            return manager.GetTemplateConfigurations(configType);
        }
    }
}
