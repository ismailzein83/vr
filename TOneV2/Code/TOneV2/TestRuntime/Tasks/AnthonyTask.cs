using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TestRuntime.Tasks
{
    public class AnthonyTask : ITask
    {
        public void Execute()
        {
            //string result = BuildSerializedDatabaseDefinitions();

            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);

            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingRuntimeService);

            CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingDistributorRuntimeService);

            DataGroupingExecutorRuntimeService dataGroupingExecutorRuntimeService = new Vanrise.Common.Business.DataGroupingExecutorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingExecutorRuntimeService);

            DataGroupingDistributorRuntimeService dataGroupingDistributorRuntimeService = new Vanrise.Common.Business.DataGroupingDistributorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingDistributorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            Console.ReadKey();
        }

        string BuildSerializedDatabaseDefinitions()
        {
            Dictionary<Guid, DBReplicationDatabaseDefinition> databaseDefinitions = new Dictionary<Guid, DBReplicationDatabaseDefinition>();
            DBReplicationDatabaseDefinition configurationDefinition = new DBReplicationDatabaseDefinition()
            {
                Name = "Configuration",
                Tables = new List<DBReplicationTableDefinition>()
            };
            FillConfiguration(configurationDefinition);
            databaseDefinitions.Add(Guid.NewGuid(), configurationDefinition);

            DBReplicationDatabaseDefinition businessEntitiesDefinition = new DBReplicationDatabaseDefinition()
            {
                Name = "Business Entities",
                Tables = new List<DBReplicationTableDefinition>()
            };
            FillBusinessEntities(businessEntitiesDefinition);
            databaseDefinitions.Add(Guid.NewGuid(), businessEntitiesDefinition);

            DBReplicationDatabaseDefinition cdrDefinition = new DBReplicationDatabaseDefinition()
            {
                Name = "CDRs",
                Tables = new List<DBReplicationTableDefinition>()
            };
            FillCDRs(cdrDefinition);
            databaseDefinitions.Add(Guid.NewGuid(), cdrDefinition);

            DBReplicationDatabaseDefinition analyticDefinition = new DBReplicationDatabaseDefinition()
            {
                Name = "Analytics",
                Tables = new List<DBReplicationTableDefinition>()
            };
            FillAnalytics(analyticDefinition);
            databaseDefinitions.Add(Guid.NewGuid(), analyticDefinition);

            DBReplicationDatabaseDefinition transactionDefinition = new DBReplicationDatabaseDefinition()
            {
                Name = "Transaction",
                Tables = new List<DBReplicationTableDefinition>()
            };
            FillTransaction(transactionDefinition);
            databaseDefinitions.Add(Guid.NewGuid(), transactionDefinition);

            DBReplicationDefinition DBReplicationDefinition = new Vanrise.Entities.DBReplicationDefinition()
            {
                Name = "Database Replication",
                VRComponentTypeId = new Guid("BB07A3B5-E519-4A6C-B4C6-695069BBB64A"),
                Settings = new DBReplicationDefinitionSettings() { DatabaseDefinitions = databaseDefinitions }
            };
            string result = Vanrise.Common.Serializer.Serialize(DBReplicationDefinition.Settings);
            return result;
        }
        private void FillConfiguration(DBReplicationDatabaseDefinition configurationDefinition)
        {
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "City", TableSchema = "common" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "Country", TableSchema = "common" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "Currency", TableSchema = "common" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CurrencyExchangeRate", TableSchema = "common" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "File", TableSchema = "common", IdColumn = "ID", ChunkSize = 1000 });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "IDManager", TableSchema = "common" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RateType", TableSchema = "common" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "Region", TableSchema = "common" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "Type", TableSchema = "common" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "VRRule", TableSchema = "common" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "VRSequence", TableSchema = "common" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "VRTimeZone", TableSchema = "common" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BEParentChildRelation", TableSchema = "genericdata" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BusinessEntityStatusHistory", TableSchema = "genericdata" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "GenericBusinessEntity", TableSchema = "genericdata" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "Rule", TableSchema = "rules" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RuleType", TableSchema = "rules" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "AccountUsage", TableSchema = "VR_AccountBalance" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "AccountUsageOverride", TableSchema = "VR_AccountBalance" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BalanceClosingPeriod", TableSchema = "VR_AccountBalance" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BalanceHistory", TableSchema = "VR_AccountBalance" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BillingTransaction", TableSchema = "VR_AccountBalance" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BillingTransactionType", TableSchema = "VR_AccountBalance" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "LiveBalance", TableSchema = "VR_AccountBalance" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BillingPeriodInfo", TableSchema = "VR_Invoice" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "Invoice", TableSchema = "VR_Invoice" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "InvoiceAccount", TableSchema = "VR_Invoice" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "InvoiceItem", TableSchema = "VR_Invoice" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "InvoiceSequence", TableSchema = "VR_Invoice" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "InvoiceSetting", TableSchema = "VR_Invoice" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "PartnerInvoiceSetting", TableSchema = "VR_Invoice" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "VRAlertLevel", TableSchema = "VRNotification" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "VRAlertRule", TableSchema = "VRNotification" });

        }

        private void FillBusinessEntities(DBReplicationDatabaseDefinition configurationDefinition)
        {
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CarrierFinancialAccount", TableSchema = "TOneWhS_AccBalance" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "AccountManager", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "ANumberGroup", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "ANumberSaleCode", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "ANumberSupplierCode", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CarrierAccount", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CarrierAccountStatusHistory", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CarrierProfile", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CodeGroup", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CustomerBillingRecurringCharge", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CustomerCountry", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CustomerRecurringCharges", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CustomerRecurringChargesType", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CustomerSellingProduct", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CustomerZone", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "FinancialAccount", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "PointOfInterconnect", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RoutingProduct", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SaleCode", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SaleEntityRoutingProduct", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SaleEntityService", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SalePriceList", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SalePricelistCodeChange", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SalePricelistCustomerChange", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SalePricelistRateChange", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SalePricelistRPChange", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SalePriceListSnapShot", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SalePriceListTemplate", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SaleRate", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SaleZone", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SellingNumberPlan", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SellingProduct", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "StateBackup", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierBillingRecurringCharge", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierCode", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierPriceList", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierRate", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierRecurringCharges", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierRecurringChargesType", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierZone", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierZoneService", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "Switch", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SwitchConnectivity", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SwitchReleaseCause", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "VolumeCommitment", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "ZoneServiceConfig", TableSchema = "TOneWhS_BE" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "ReportDefinition", TableSchema = "TOneWhS_Billing" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CaseReason", TableSchema = "TOneWhS_Case" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CaseWorkGroup", TableSchema = "TOneWhS_Case" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CustomerFaultTicket", TableSchema = "TOneWhS_Case" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "InternationalReleaseCode", TableSchema = "TOneWhS_Case" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierFaultTicket", TableSchema = "TOneWhS_Case" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CodePreparation", TableSchema = "TOneWhs_CP" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SaleCode_Preview", TableSchema = "TOneWhs_CP" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SaleRate_Preview", TableSchema = "TOneWhs_CP" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SaleZone_Preview", TableSchema = "TOneWhs_CP" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SaleZoneRoutingProduct_Preview", TableSchema = "TOneWhs_CP" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "Deal", TableSchema = "TOneWhS_Deal" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "DealZoneRate", TableSchema = "TOneWhS_Deal" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CarrierInvoiceAccount", TableSchema = "TOneWhS_Invoice" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "InvoiceComparisonTemplate", TableSchema = "TOneWhS_Invoice" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RouteSyncDefinition", TableSchema = "TOneWhS_RouteSync" });
            //configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RoutingDatabase", TableSchema = "TOneWhS_Routing" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "PricingTemplate", TableSchema = "TOneWhS_Sales" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RatePlan", TableSchema = "TOneWhS_Sales" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RP_DefaultRoutingProduct_Preview", TableSchema = "TOneWhS_Sales" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RP_DefaultService_Preview", TableSchema = "TOneWhS_Sales" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RP_RatePlanPreview_Summary", TableSchema = "TOneWhS_Sales" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RP_SaleRate_Preview", TableSchema = "TOneWhS_Sales" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RP_SaleZoneRoutingProduct_Preview", TableSchema = "TOneWhS_Sales" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RP_SaleZoneService_Preview", TableSchema = "TOneWhS_Sales" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "RP_Subscriber_Preview", TableSchema = "TOneWhS_Sales" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "ReceivedSupplierPricelist", TableSchema = "TOneWhS_SPL" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierCode_Preview", TableSchema = "TOneWhS_SPL" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierOtherRate_Preview", TableSchema = "TOneWhS_SPL" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierPriceListTemplate", TableSchema = "TOneWhS_SPL" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "SupplierZoneRate_Preview", TableSchema = "TOneWhS_SPL" });

        }

        private void FillAnalytics(DBReplicationDatabaseDefinition configurationDefinition)
        {
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BillingStats30Min", TableSchema = "TOneWhS_Analytics", FilterDateTimeColumn = "BatchStart" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BillingStatsDaily", TableSchema = "TOneWhS_Analytics", FilterDateTimeColumn = "BatchStart" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "TrafficStats15Min", TableSchema = "TOneWhS_Analytics", FilterDateTimeColumn = "BatchStart" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "TrafficStatsDaily", TableSchema = "TOneWhS_Analytics", FilterDateTimeColumn = "BatchStart" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "AffectedDealZoneGroupsToDelete", TableSchema = "TOneWhS_Deal" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "DealDetailedProgress", TableSchema = "TOneWhS_Deal" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "DealProgress", TableSchema = "TOneWhS_Deal" });
        }

        private void FillCDRs(DBReplicationDatabaseDefinition configurationDefinition)
        {
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BillingCDR_Failed", TableSchema = "TOneWhS_CDR", FilterDateTimeColumn = "AttemptDateTime" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BillingCDR_Interconnect", TableSchema = "TOneWhS_CDR", FilterDateTimeColumn = "AttemptDateTime" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BillingCDR_Invalid", TableSchema = "TOneWhS_CDR", FilterDateTimeColumn = "AttemptDateTime" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BillingCDR_Main", TableSchema = "TOneWhS_CDR", FilterDateTimeColumn = "AttemptDateTime" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BillingCDR_PartialPriced", TableSchema = "TOneWhS_CDR", FilterDateTimeColumn = "AttemptDateTime" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "CDR", TableSchema = "TOneWhS_CDR", FilterDateTimeColumn = "AttemptDateTime" });
        }

        private void FillTransaction(DBReplicationDatabaseDefinition configurationDefinition)
        {
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BPInstance", TableSchema = "bp" });
            configurationDefinition.Tables.Add(new DBReplicationTableDefinition() { TableName = "BPInstance_Archived", TableSchema = "bp", IdColumn = "ID", ChunkSize = 500000 });
        }
    }
}