using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRuntime.Tasks
{
    public class AliBalloukTask : ITask
    {
        public void Execute()
        {
            TOne.Entities.RDLCReportDefinition reportDefinition = new TOne.Entities.RDLCReportDefinition
            {
                Name = "Zone Profit",
                ReportGeneratorFQTN = "TOne.Analytics.Business.BillingReports.ZoneProfitReportGenerator, TOne.Analytics.Business",
                ReportURL = "~/Reports/Analytics/rdlZoneProfits.rdlc",
                ParameterSettings = new TOne.Entities.ReportParameterSettings
                {
                    RequiresFromTime = true,
                    RequiresToTime = true
                },
                ReportDefinitionId = 1
            };

            string serializedReportDefinition = Vanrise.Common.Serializer.Serialize(reportDefinition);
        }
    }
}
