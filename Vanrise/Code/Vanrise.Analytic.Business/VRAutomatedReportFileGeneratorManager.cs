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
     public class VRAutomatedReportFileGeneratorManager
    {
        #region Public Methods
         public IEnumerable<VRAutomatedReportFileGeneratorConfig> GetFileGeneratorTemplateConfigs()
        {
            return BusinessManagerFactory.GetManager<IExtensionConfigurationManager>().GetExtensionConfigurations<VRAutomatedReportFileGeneratorConfig>(VRAutomatedReportFileGeneratorConfig.EXTENSION_TYPE);
        }

        #endregion
    }
}
