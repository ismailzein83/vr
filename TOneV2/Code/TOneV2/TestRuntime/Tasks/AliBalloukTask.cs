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
            //TOne.Entities.RDLCReportDefinition reportDefinition = new TOne.Entities.RDLCReportDefinition
            //{
            //    Name = "Zone Profit",
            //    ReportGeneratorFQTN = "TOne.Analytics.Business.BillingReports.ZoneProfitReportGenerator, TOne.Analytics.Business",
            //    ReportURL = "~/Reports/Analytics/rdlZoneProfits.rdlc",
            //    ParameterSettings = new TOne.Entities.ReportParameterSettings
            //    {
            //        RequiresFromTime = true,
            //        RequiresToTime = true,
            //        RequiresCustomerAMUId = true ,
            //        RequiresCustomerId = true ,
            //        RequiresGroupByCustomer = true ,
            //        RequiresSupllierAMUId = true ,
            //        RequiresSupllierId = true

            //    },
            //    ReportDefinitionId = 1
            //};


            TOne.Entities.RDLCReportDefinition reportDefinition = new TOne.Entities.RDLCReportDefinition
            {
                Name = "Zone Summary",
                ReportGeneratorFQTN = "TOne.Analytics.Business.BillingReports.ZoneSummaryReportGenerator, TOne.Analytics.Business",
                ReportURL = "~/Reports/Analytics/rdlZoneSummary.rdlc",
                ParameterSettings = new TOne.Entities.ReportParameterSettings
                {
                    RequiresFromTime = true,
                    RequiresToTime = true,
                    RequiresCustomerAMUId = true,
                    RequiresGroupBySupplier = true,
                    RequiresGroupByCustomer = false,
                    RequiresSupllierAMUId = true,
                    RequiresSupllierId = true,
                    RequiresCurrencyId = true,
                    RequiresCustomerGroup = true,
                    RequiresIsCommission = true,
                    RequiresIsCost = true,
                    RequiresIsService = true,
                    RequiresServicesForCustomer = true,
                    RequiresSupplierGroup= true,
                    RequiresCustomerId = true
                },
                ReportDefinitionId = 2
            };

            string serializedReportDefinition = Vanrise.Common.Serializer.Serialize(reportDefinition);
        }
    }
}
