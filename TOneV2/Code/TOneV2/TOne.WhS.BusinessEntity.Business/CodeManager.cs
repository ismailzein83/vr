using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CodeManager
    {
        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public List<Vanrise.Entities.TemplateConfig> GetCodeCriteriaGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CodeCriteriaGroupConfigType);
        }
        #endregion

        #region Private Methods
        #endregion

        #region  Mappers
        #endregion
      
    }
}
