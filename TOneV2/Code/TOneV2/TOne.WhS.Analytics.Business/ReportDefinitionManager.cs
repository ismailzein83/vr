using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities.BillingReport;

namespace TOne.WhS.Analytics.Business
{
    public class ReportDefinitionManager
    {
        private readonly IReportDefinitionDataManager dataManager;

        public ReportDefinitionManager()
        {
            dataManager = AnalyticsDataManagerFactory.GetDataManager<IReportDefinitionDataManager>();
        }
        public RDLCReportDefinition GetRDLCReportDefinition(int ReportDefinitionId)
        {
            return dataManager.GetRDLCReportDefinition(ReportDefinitionId);
        }
        public List<RDLCReportDefinition> GetAllRDLCReportDefinition()
        {
            return dataManager.GetAllRDLCReportDefinition();
        }

    }
}
