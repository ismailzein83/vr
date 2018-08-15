using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    public class VRAutomatedReportHandlerManager
    {

        #region Public Methods
        public IEnumerable<VRAutomatedReportHandlerSettingsConfig> GetAutomatedReportHandlerTemplateConfigs()
        {
            return BusinessManagerFactory.GetManager<IExtensionConfigurationManager>().GetExtensionConfigurations<VRAutomatedReportHandlerSettingsConfig>(VRAutomatedReportHandlerSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<VRAutomatedReportHandlerSettingsActionTypeConfig> GetAutomatedReportHandlerActionTypesTemplateConfigs()
        {
            return BusinessManagerFactory.GetManager<IExtensionConfigurationManager>().GetExtensionConfigurations<VRAutomatedReportHandlerSettingsActionTypeConfig>(VRAutomatedReportHandlerSettingsActionTypeConfig.EXTENSION_TYPE);
        }

        #endregion

    }
}
