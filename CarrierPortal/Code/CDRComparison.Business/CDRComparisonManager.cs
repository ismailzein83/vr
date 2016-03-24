using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace CDRComparison.Business
{
    public class CDRComparisonManager
    {
        public IEnumerable<TemplateConfig> GetCDRSourceTemplateConfigs()
        {
            var templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.CDRSourceConfigType);
        }

        public IEnumerable<TemplateConfig> GetFileReaderTemplateConfigs()
        {
            var templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.FileReaderConfigType);
        }
    }
}
