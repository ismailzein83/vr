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
	(N'TOne.WhS.SupplierPriceList.BP.Arguments.SupplierPriceListProcessInput',N'Import Supplier PriceList Process',N'TOne.WhS.SupplierPriceList.BP.ImportSupplierPriceList, TOne.WhS.SupplierPriceList.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(N'TOne.WhS.Routing.BP.Arguments.RoutingProcessInput',N'Whole Sale Routing Process Process',N'TOne.WhS.Routing.BP.RoutingProcess, TOne.WhS.Routing.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ScheduledExecEditor":"","ManualExecEditor":"vr-whs-routing-buildrouteprocess","RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/BuildRouteProcess.html"}'),
(N'TOne.WhS.Routing.BP.Arguments.RPRoutingProcessInput',N'Routing Product Routing Process',N'TOne.WhS.Routing.BP.RPRoutingProcess, TOne.WhS.Routing.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ScheduledExecEditor":"","ManualExecEditor":"vr-whs-routing-rpbuildproduct","RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/RPBuildProductRoutesProcess.html"}'),
(N'TOne.WhS.Routing.BP.Arguments.BuildRoutesByCodePrefixInput',N'Build Routes By Code Prefix',N'TOne.WhS.Routing.BP.BuildRoutesByCodePrefix, TOne.WhS.Routing.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ScheduledExecEditor":"","ManualExecEditor":"vr-whs-routing-buildbycodeprefix","RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/BuildRoutesByCodePrefix.html"}'),
(N'TOne.WhS.Routing.BP.Arguments.RPBuildCodeMatchesByCodePrefixInput',N'Routing Product Build Code Matches By Code Prefix',N'TOne.WhS.Routing.BP.RPBuildCodeMatchesByCodePrefix, TOne.WhS.Routing.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false}'),
(N'TOne.WhS.Routing.BP.Arguments.RPBuildRoutingProductInput',N'Routing Product Build Process Input',N'TOne.WhS.Routing.BP.RPBuildRoutingProducts, TOne.WhS.Routing.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false}'),
(N'TOne.WhS.CodePreparation.BP.Arguments.CodePreparationInput',N'Code Preparation Process',N'TOne.WhS.CodePreparation.BP.CodePreparation, TOne.WhS.CodePreparation.BP',N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}')
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
