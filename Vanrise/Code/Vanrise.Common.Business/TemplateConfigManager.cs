using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class TemplateConfigManager
    {
        public List<Entities.TemplateConfig> GetTemplateConfigurations(string configType)
        {
            ITemplateConfigDataManager manager = CommonDataManagerFactory.GetDataManager<ITemplateConfigDataManager>();
            return manager.GetTemplateConfigurations(configType);
        }

        public List<Entities.TemplateConfig> GetAllTemplateConfigurations()
        {
            ITemplateConfigDataManager manager = CommonDataManagerFactory.GetDataManager<ITemplateConfigDataManager>();
            return manager.GetTemplateConfigurations();
        }

        public T GetBehavior<T>(int configId) where T : class
        {
            var allTemplates = GetAllTemplateConfigurations();
            if (allTemplates != null)
            {
                Vanrise.Entities.TemplateConfig templateConfig = allTemplates.FirstOrDefault(itm => itm.TemplateConfigID == configId);
                if (templateConfig != null)
                {
                    Type t = Type.GetType(templateConfig.BehaviorFQTN);
                    return Activator.CreateInstance(t) as T;
                }
            }
            return null;
        }
    }
}
