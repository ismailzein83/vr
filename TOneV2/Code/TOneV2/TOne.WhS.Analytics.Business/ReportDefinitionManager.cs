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

            RDLCReportDefinition reportDefinition = new RDLCReportDefinition
            {
                Name = "Losses By Carrier",
                ReportGeneratorFQTN = "TOne.WhS.Analytics.Business.BillingReports.LossesByCarrierReportGenerator, TOne.WhS.Analytics.Business",
                ReportDefinitionId=31,
                ReportURL="~/Client/Modules/WhS_Analytics/Reports/Analytics/rdlLossesByCarrier.rdlc",
                ParameterSettings = new ReportParameterSettings
                {
                    RequiresFromTime=true,
                    RequiresToTime=false,
                    RequiresCustomerId=true,
                    RequiresSupplierId=true,
                    RequiresCustomerAMUId=true,
                    RequiresSupllierAMUId=true,
                    RequiresGroupByCustomer=false,
                    RequiresIsCost=false,
                    RequiresCurrencyId=false,
                    RequiresSupplierGroup=false,
                    RequiresCustomerGroup=false,
                    RequiresGroupBySupplier=false,
                    RequiresIsService=false,
                    RequiresIsCommission=false,
                    RequiresServicesForCustomer=false,
                    RequiresMargin=true,
                    RequiresPageBreak=true
                }
            };
            var ReportDefinitionRDLCFiles = new List<ReportDefinitionRDLCFile>();

            ReportDefinitionRDLCFiles.Add(new ReportDefinitionRDLCFile()
            {
                ReportDefinitionRDLCFileId = 1,
                Name = "Group by Customer",
                Title = "Losses by Carrier",
                RDLCURL = "~/Client/Modules/WhS_Analytics/Reports/Analytics/rdlLossesByCarrier.rdlc"
            });

            ReportDefinitionRDLCFiles.Add(new ReportDefinitionRDLCFile()
            {
                ReportDefinitionRDLCFileId = 2,
                Name = "List Display",
                Title = "Losses by Carrier (List Display)",
                RDLCURL = "~/Client/Modules/WhS_Analytics/Reports/Analytics/rdlLossesByCarrierListView.rdlc"
            });

            reportDefinition.ReportDefinitionRDLCFiles = ReportDefinitionRDLCFiles;
            string serializedReportDefinition = Vanrise.Common.Serializer.Serialize(reportDefinition);

            return dataManager.GetAllRDLCReportDefinition();
        }

    }
}
