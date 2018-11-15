using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class ReportGenerationCustomCodeSettingsManager
    {
        #region Public Methods
        public IEnumerable<ReportGenerationCustomCodeDefinitionInfo> GetReportGenerationCustomCodeSettingsInfo()
        {
            VRComponentTypeManager vrComponentTypeManager = new VRComponentTypeManager();
            Func<ReportGenerationCustomCodeSettings, bool> filterExpression = (customCodeSettings) =>
            {
                return true;
            };
            return vrComponentTypeManager.GetComponentTypes<ReportGenerationCustomCodeSettings, ReportGenerationCustomCodeDefinition>().MapRecords(ReportGenerationCustomCodeDefinitionInfoMapper);

        }
        #endregion

        #region Private Methods
        private ReportGenerationCustomCodeDefinitionInfo ReportGenerationCustomCodeDefinitionInfoMapper(ReportGenerationCustomCodeDefinition reportGenerationCustomCodeDefinition)
        {
            return new ReportGenerationCustomCodeDefinitionInfo
            {
                DefinitionId = reportGenerationCustomCodeDefinition.VRComponentTypeId,
                Name = reportGenerationCustomCodeDefinition.Name
            };
        }
        #endregion
    }
}
