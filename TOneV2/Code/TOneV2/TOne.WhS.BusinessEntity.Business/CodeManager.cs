using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CodeManager
    {
        public List<CodeCriteria> GetCodeCriterias(int codeCriteriaGroupConfigId, CodeCriteriaGroupSettings settings)
        {
            TemplateConfigManager templateConfigManager = new TemplateConfigManager();
            CodeCriteriaGroupBehavior codeCriteriaGroupBehavior = templateConfigManager.GetBehavior<CodeCriteriaGroupBehavior>(codeCriteriaGroupConfigId);
            if (codeCriteriaGroupBehavior != null)
                return codeCriteriaGroupBehavior.GetCodeCriterias(settings);
            else
                return null;
        }

        public List<Vanrise.Entities.TemplateConfig> GetCodeCriteriaGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CodeCriteriaGroupConfigType);
        }
    }
}
