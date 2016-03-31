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

        public CDRComparisonSummary GetCDRComparisonSummary()
        {
            CDRComparisonSummary summary = new CDRComparisonSummary();
            CDRManager cdrManager = new Business.CDRManager();
            MissingCDRManager missingCDRManager = new MissingCDRManager();
            PartialMatchCDRManager partialMatchCDRManager = new PartialMatchCDRManager();
            DisputeCDRManager disputeCDRManager = new Business.DisputeCDRManager();
            summary.MissingCDRsCount = missingCDRManager.GetMissingCDRsCount();
            summary.PartialMatchCDRsCount = partialMatchCDRManager.GetPartialMatchCDRsCount();
            summary.AllCDRsCount = cdrManager.GetAllCDRsCount();
            summary.DisputeCDRsCount = disputeCDRManager.GetDisputeCDRsCount();
            return summary;
        }
    }
}
