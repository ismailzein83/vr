﻿using System;
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
                Name = "Detailed Carrier Summary",
                ReportGeneratorFQTN = "TOne.Analytics.Business.BillingReports.DetailedCarrierSummaryReportGenerator, TOne.Analytics.Business",
                ReportURL = "~/Reports/Analytics/rdlCarrierDetailedSummary.rdlc",
                ParameterSettings = new TOne.Entities.ReportParameterSettings
                {
                    RequiresFromTime = true,
                    RequiresToTime = true,
                    RequiresCustomerId = true,
                    RequiresSupplierId = true,
                    RequiresMargin = false,
                    RequiresCustomerAMUId = true,
                    RequiresSupllierAMUId = true,

                    RequiresGroupBySupplier = false,
                    RequiresGroupByCustomer = false,
                    RequiresCurrencyId = false,
                    RequiresCustomerGroup = false,
                    RequiresIsCommission = false,
                    RequiresIsCost = false,
                    RequiresIsService = false,
                    RequiresServicesForCustomer = false,
                    RequiresSupplierGroup= false,
                    RequiresZoneId = false 
               
                },
                ReportDefinitionId = 11
            };

            string serializedReportDefinition = Vanrise.Common.Serializer.Serialize(reportDefinition);
        }
    }
}
