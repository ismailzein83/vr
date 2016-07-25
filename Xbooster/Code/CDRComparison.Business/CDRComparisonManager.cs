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

        public CDRComparisonSummary GetCDRComparisonSummary(string tableKey)
        {
            CDRComparisonSummary summary = new CDRComparisonSummary();
            CDRManager cdrManager = new Business.CDRManager();
            MissingCDRManager missingCDRManager = new MissingCDRManager();
            PartialMatchCDRManager partialMatchCDRManager = new PartialMatchCDRManager();
            DisputeCDRManager disputeCDRManager = new Business.DisputeCDRManager();

            summary.SystemCDRsCount = cdrManager.GetCDRsCount(tableKey, false);
            summary.PartnerCDRsCount = cdrManager.GetCDRsCount(tableKey, true);
            summary.SystemMissingCDRsCount = missingCDRManager.GetMissingCDRsCount(tableKey, false);
            summary.PartnerMissingCDRsCount = missingCDRManager.GetMissingCDRsCount(tableKey, true);
            summary.PartialMatchCDRsCount = partialMatchCDRManager.GetPartialMatchCDRsCount(tableKey);
            summary.DisputeCDRsCount = disputeCDRManager.GetDisputeCDRsCount(tableKey);

            summary.DurationOfSystemCDRs = cdrManager.GetDurationOfCDRs(tableKey, false);
            summary.DurationOfPartnerCDRs = cdrManager.GetDurationOfCDRs(tableKey, true);

            summary.DurationOfSystemMissingCDRs = missingCDRManager.GetDurationOfMissingCDRs(tableKey, false);
            summary.DurationOfPartnerMissingCDRs = missingCDRManager.GetDurationOfMissingCDRs(tableKey, true);

            summary.DurationOfSystemPartialMatchCDRs = partialMatchCDRManager.GetDurationOfPartialMatchCDRs(tableKey, false);
            summary.DurationOfPartnerPartialMatchCDRs = partialMatchCDRManager.GetDurationOfPartialMatchCDRs(tableKey, true);
            summary.TotalDurationDifferenceOfPartialMatchCDRs = partialMatchCDRManager.GetTotalDurationDifferenceOfPartialMatchCDRs(tableKey);

            summary.DurationOfSystemDisputeCDRs = disputeCDRManager.GetDurationOfDisputeCDRs(tableKey, false);
            summary.DurationOfPartnerDisputeCDRs = disputeCDRManager.GetDurationOfDisputeCDRs(tableKey, true);

            return summary;
        }
    }
}
