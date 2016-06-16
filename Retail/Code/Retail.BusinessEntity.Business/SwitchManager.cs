using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
namespace Retail.BusinessEntity.Business
{
    public class SwitchManager 
    {
        #region ctor/Local Variables

        #endregion

        #region Public Methods
        public IEnumerable<SwitchIntegrationConfig> GetSwitchSettingsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<SwitchIntegrationConfig>(SwitchIntegrationConfig.EXTENSION_TYPE);
        }
       
        #endregion

    }
}
