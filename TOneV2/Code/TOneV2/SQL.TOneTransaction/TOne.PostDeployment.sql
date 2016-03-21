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
--[bp].[BPDefinition]-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('TOne.WhS.Routing.BP.Arguments.BuildRoutesByCodePrefixInput', 'Build Routes By Code Prefix', 'TOne.WhS.Routing.BP.BuildRoutesByCodePrefix, TOne.WhS.Routing.BP', '{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ScheduledExecEditor":"","ManualExecEditor":"vr-whs-routing-buildbycodeprefix","RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/BuildRoutesByCodePrefix.html","ManualExecEditor":"vr-whs-routing-buildbycodeprefix"}'),
('TOne.WhS.Routing.BP.Arguments.RoutingProcessInput', 'Whole Sale Routing Process Process', 'TOne.WhS.Routing.BP.RoutingProcess, TOne.WhS.Routing.BP', '{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ScheduledExecEditor":"","ManualExecEditor":"vr-whs-routing-buildrouteprocess","RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/BuildRouteProcess.html","ManualExecEditor":"vr-whs-routing-buildrouteprocess"}'),
('TOne.WhS.SupplierPriceList.BP.Arguments.SupplierPriceListProcessInput', 'Import Supplier PriceList Process', 'TOne.WhS.SupplierPriceList.BP.ImportSupplierPriceList, TOne.WhS.SupplierPriceList.BP', '{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
('TOne.WhS.Routing.BP.Arguments.RPRoutingProcessInput', 'Routing Product Routing Process', 'TOne.WhS.Routing.BP.RPRoutingProcess, TOne.WhS.Routing.BP', '{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ScheduledExecEditor":"","ManualExecEditor":"vr-whs-routing-rpbuildproduct","RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/RPBuildProductRoutesProcess.html","ManualExecEditor":"vr-whs-routing-rpbuildproduct"}'),
('TOne.WhS.Routing.BP.Arguments.RPBuildCodeMatchesByCodePrefixInput', 'Routing Product Build Code Matches By Code Prefix', 'TOne.WhS.Routing.BP.RPBuildCodeMatchesByCodePrefix, TOne.WhS.Routing.BP', '{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false}'),
('TOne.WhS.Routing.BP.Arguments.RPBuildRoutingProductInput', 'Routing Product Build Process Input', 'TOne.WhS.Routing.BP.RPBuildRoutingProducts, TOne.WhS.Routing.BP', '{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false}'),
('TOne.WhS.CodePreparation.BP.Arguments.CodePreparationInput', 'Code Preparation Process', 'TOne.WhS.CodePreparation.BP.CodePreparation, TOne.WhS.CodePreparation.BP', '{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([Name],[Title],[FQTN],[Config])
	values(s.[Name],s.[Title],s.[FQTN],s.[Config]);
