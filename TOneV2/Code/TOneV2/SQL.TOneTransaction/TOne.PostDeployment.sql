/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/



MERGE INTO bp.[BPDefinition] AS Target 
USING (VALUES 
	(N'TOne.CDRProcess.Arguments.DailyRepricingProcessInput', N'Daily Repricing Process', N'TOne.CDRProcess.DailyRepricingProcess, TOne.CDRProcess', N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":2,"RetryOnProcessFailed":false, "ScheduleTemplateURL":"/Client/Modules/Runtime/Views/WFScheduleTemplates/ScheduleDailyRepricingProcessTemplate.html"}'),
	(N'TOne.CDRProcess.Arguments.TimeRangeRepricingProcessInput', N'Time Range Repricing Process', N'TOne.CDRProcess.DailyRepricingProcess, TOne.CDRProcess', N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":2,"RetryOnProcessFailed":false}'),
	(N'TOne.LCRProcess.Arguments.RoutingProcessInput', N'Routing Process', N'TOne.LCRProcess.RoutingProcess, TOne.LCRProcess', N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
		(N'TOne.LCRProcess.Arguments.DifferentialRoutingProcessInput', N'Differential Routing Process', N'TOne.LCRProcess.DifferentialRoutingProcess, TOne.LCRProcess', N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
		(N'TOne.CDRProcess.Arguments.ImportCDRProcessInput',N'Import CDR Process',N'TOne.CDRProcess.ImportCDRProcess, TOne.CDRProcess',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.CDRProcess.Arguments.BillingCDRsProcessInput',N'Billing CDRs Process',N'TOne.CDRProcess.BillingCDRsProcess, TOne.CDRProcess',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.CDRProcess.Arguments.GenerateStatisticsProcessInput',N'Generate Statistics Process',N'TOne.CDRProcess.GenerateStatisticsProcess, TOne.CDRProcess',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.CDRProcess.Arguments.RawCDRsProcessInput',N'Raw CDRs Process',N'TOne.CDRProcess.RawCDRsProcess, TOne.CDRProcess',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.CDRProcess.Arguments.StoreCDRsInDBProcessInput',N'Store CDRs In DB Process',N'TOne.CDRProcess.StoreCDRsInDBProcess, TOne.CDRProcess',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.CDRProcess.Arguments.StoreInvalidCDRsInDBProcessInput',N'Store Invalid CDRs In DB Process',N'TOne.CDRProcess.StoreInvalidCDRsInDBProcess, TOne.CDRProcess',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.CDRProcess.Arguments.StoreMainCDRsInDBProcessInput',N'Store Main CDRs In DB Process',N'TOne.CDRProcess.StoreMainCDRsInDBProcess, TOne.CDRProcess',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.CDRProcess.Arguments.SaveStatisticsToDBProcessInput',N'SaveStatistics To DB Process',N'TOne.CDRProcess.SaveStatisticsToDBProcess, TOne.CDRProcess',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.CDRProcess.Arguments.SaveDailyStatisticsToDBProcessInput',N'Save Daily Statistics To DB Process',N'TOne.CDRProcess.SaveDailyStatisticsToDBProcess, TOne.CDRProcess',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.CDRProcess.Arguments.GenerateDailyStatisticsProcessInput',N'Generate Daily Statistics Process',N'TOne.CDRProcess.GenerateDailyStatisticsProcess, TOne.CDRProcess',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.WhS.SupplierPriceList.BP.Arguments.SupplierPriceListProcessInput',N'Import Supplier PriceList Process',N'TOne.WhS.SupplierPriceList.BP.ImportSupplierPriceListProcess, TOne.WhS.SupplierPriceList.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.WhS.Routing.BP.Arguments.RoutingProcessInput',N'Whole Sale Routing Process Process',N'TOne.WhS.Routing.BP.RoutingProcess, TOne.WhS.Routing.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false, "Url": "/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/BuildRouteProcess.html"}'),
(N'TOne.WhS.Routing.BP.Arguments.RPRoutingProcessInput',N'Routing Product Routing Process',N'TOne.WhS.Routing.BP.RPRoutingProcess, TOne.WhS.Routing.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/RPBuildProductRoutesProcess.html"}'),
(N'TOne.WhS.Routing.BP.Arguments.BuildRoutesByCodePrefixInput',N'Build Routes By Code Prefix',N'TOne.WhS.Routing.BP.BuildRoutesByCodePrefix, TOne.WhS.Routing.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/BuildRoutesByCodePrefix.html"}'),
(N'TOne.WhS.Routing.BP.Arguments.RPBuildCodeMatchesByCodePrefixInput',N'Routing Product Build Code Matches By Code Prefix',N'TOne.WhS.Routing.BP.RPBuildCodeMatchesByCodePrefix, TOne.WhS.Routing.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false}'),
(N'TOne.WhS.Routing.BP.Arguments.RPBuildRoutingProductInput',N'Routing Product Build Process Input',N'TOne.WhS.Routing.BP.RPBuildRoutingProducts, TOne.WhS.Routing.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false}')


) 
AS Source ([Name], [Title], [FQTN], [Config])
ON Target.[Name] = Source.[Name] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[Title] = Source.[Title],
			[FQTN] = Source.[FQTN],
			[Config]  = Source.[Config]
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Name], [Title], [FQTN], [Config])
VALUES ([Name], [Title], [FQTN], [Config])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;
