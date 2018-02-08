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

--[common].[ExtensionConfiguration]---------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('76F467C2-E440-4B11-A42C-59F69FDDBCB7','Fixed Rate','Fixed Rate','WhS_Deal_SwapDeal_InboundRateCalculationMethod'															,'{"Editor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-inbound-fixed-editor"}'),('EA1454A9-0FA0-4B16-93E8-76533BD504F3','Fixed Rate','Fixed Rate','WhS_Deal_SwapDeal_OutboundRateCalculationMethod'															,'{"Editor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-fixed-editor"}'),('96B9EB7E-D08B-4CCB-AC0D-7FE112EF41D8','Based on other Suppliers'' Cost Rates','Based on other Suppliers'' Cost Rates','WhS_Deal_SwapDeal_InboundRateCalculationMethod'	,'{"Editor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-inbound-suppliers-editor"}'),('91EC1ACA-5D9B-418D-A327-8EE699CE192F','Based on Customers'' Sale Rates','Based on Customers'' Sale Rates','WhS_Deal_SwapDeal_OutboundRateCalculationMethod'				,'{"Editor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-customers-editor"}'),('2BBA4C50-CE06-4F61-9731-D31CE687CABF','Based on other Suppliers'' Cost Rates','Based on other Suppliers'' Cost Rates','WhS_Deal_SwapDeal_OutboundRateCalculationMethod'	,'{"Editor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-suppliers-editor"}'),('3CC81591-9860-4B9D-9755-0E5D7E87A596','VRVommon_VRRuleDefinition','VRRule Definition','VR_Common_VRComponentType'															,'{"Editor":"vr-common-ruledefinition-settings"}'),
('9B2DB188-650C-4F19-8BCD-15BECC653E46','WhS_Deal_SwapDealBuyRouteRule','Swap Deal Buy Route Rule','VRCommon_RuleDefinitionExtendedSettings'								,'{"Editor":"vr-whs-deal-swapdeal-buyrouteruledefinition-settings"}'),
('C49A0C3B-CE93-430B-BE81-8440EEBD87B0','WhS_Deal_SwapDealBuyRouteRuleExtendedSettings_Fixed','Customer','WhS_Deal_SwapDealBuyRouteRuleExtendedSettings'					,'{"Editor":"vr-whs-deal-swapdeal-buyrouterule-fixed"}'),
('43670B8E-C8D6-48DB-AE79-C8957DF1CF54','WhS_Deal_SwapDealBuyRouteRuleExtendedSettings_BySaleDeal','Sale Deal','WhS_Deal_SwapDealBuyRouteRuleExtendedSettings'				,'{"Editor":"vr-whs-deal-swapdeal-buyrouterule-bysaledeal"}'),('0CE291EB-790F-4B24-9DC1-512D457546C5','WhS_Deal_SwapDealRouteRuleCriteria','Swap Deal','WhS_Routing_RouteRuleCriteriaType'												,'{"Editor":"vr-whs-deal-swapdealanalysis-routerulecriteria"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[ExtensionConfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);

--[sec].[Module]---------------------------1701 to 1800---------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('670FD0F9-6DCE-4567-8E12-DC3E5830B549','Deals',null,null,'/images/menu-icons/Deals Module.png',105,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[Id],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
--------------------------------------------------------------------------------------------------------------
END


--[sec].[View]-----------------------------18001 to 19000---------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('95F31CAF-F3F0-4617-B253-031A8FD69866','Volume Commitment','Volume Commitment','#/view/WhS_Deal/Views/VolumeCommitment/VolumeCommitmentManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549','WhS_Deal/VolCommitmentDeal/GetFilteredVolCommitmentDeals',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5),('BAA1EDFB-4DA7-4BEE-ABA0-9E375EA3E3BF','Swap Deal','Swap Deal','#/view/WhS_Deal/Views/SwapDeal/SwapDealManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549','WhS_Deal/SwapDeal/GetFilteredSwapDeals',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',9)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
-------------------------------------------------------------------------------------------------------------
END


--[sec].[BusinessEntityModule]----------1501 to 1600------------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[Name],[ParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('337775AD-C808-4EF4-8256-0BEC2F6D411F','Deals','5A9E78AE-229E-41B9-9DBF-492997B42B61',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[ParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[Name],[ParentId],[BreakInheritance])	values(s.[ID],s.[Name],s.[ParentId],s.[BreakInheritance]);
----------------------------------------------------------------------------------------------------
END


--[sec].[BusinessEntity]----------------4501 to 4800------------------------------------------------
BEGIN
set nocount on;;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('9B51844C-2953-4B96-9D08-AE2F8B9EF109','WhS_Deal_SwapDeal','Swap Deal','337775AD-C808-4EF4-8256-0BEC2F6D411F',0,'["View","Add","Edit"]'),('25F1648E-B081-4EA3-B372-7CD43422E118','WhS_Deal_VolCommitmentDeal','Volume Commitment Deal','337775AD-C808-4EF4-8256-0BEC2F6D411F',0,'["View","Add","Edit"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
----------------------------------------------------------------------------------------------------
END


--[sec].[SystemAction]------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('WhS_Deal/SwapDeal/GetFilteredSwapDeals','WhS_Deal_SwapDeal: View'),('WhS_Deal/SwapDeal/GetDeal',null),('WhS_Deal/SwapDeal/UpdateDeal','WhS_Deal_SwapDeal: Edit'),('WhS_Deal/SwapDeal/AddDeal','WhS_Deal_SwapDeal: Add'),('WhS_Deal/SwapDeal/GetSwapDealSettingData',null),('WhS_Deal/VolCommitmentDeal/GetFilteredVolCommitmentDeals','WhS_Deal_VolCommitmentDeal: View'),('WhS_Deal/VolCommitmentDeal/GetDeal',null),('WhS_Deal/VolCommitmentDeal/UpdateDeal','WhS_Deal_VolCommitmentDeal: Edit'),('WhS_Deal/VolCommitmentDeal/AddDeal','WhS_Deal_VolCommitmentDeal: Add')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);
----------------------------------------------------------------------------------------------------
END


--[common].[setting]--------------------------------------------------------------------------------BEGINset nocount on;;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('8A71FE78-AF3D-40DB-BA46-B37DF712601A','Swap Deal','WhS_Deal_SwapDealSettings','Business Entities','{"Editor":"vr-whs-deal-swapdeal-settings-editor"}','{"$type":"TOne.WhS.Deal.Entities.Settings.SwapDealSettingData, TOne.WhS.Deal.Entities","DefaultCalculationMethodId":"a46e761a-665f-c515-728b-8fbc536da9ff","DefaultInboundRateCalcMethodId":"631a698b-e39e-79de-00a4-1812fa7758f5","InboundCalculationMethods":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[TOne.WhS.Deal.Entities.SwapDealAnalysisInboundRateCalcMethod, TOne.WhS.Deal.Entities]], mscorlib","631a698b-e39e-79de-00a4-1812fa7758f5":{"$type":"TOne.WhS.Deal.MainExtensions.SwapDealAnalysisInboundRateSuppliers, TOne.WhS.Deal.MainExtensions","ConfigId":"96b9eb7e-d08b-4ccb-ac0d-7fe112ef41d8","ItemEditor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-inbound-suppliers-itemeditor","CalculationMethodId":"631a698b-e39e-79de-00a4-1812fa7758f5","Title":"Suppliers"},"248259ff-f865-53b1-6080-75124cead9f2":{"$type":"TOne.WhS.Deal.MainExtensions.SwapDealAnalysisInboundRateFixed, TOne.WhS.Deal.MainExtensions","ConfigId":"76f467c2-e440-4b11-a42c-59f69fddbcb7","ItemEditor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-inbound-fixed-itemeditor","CalculationMethodId":"248259ff-f865-53b1-6080-75124cead9f2","Title":"Fixed"}},"OutboundCalculationMethods":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[TOne.WhS.Deal.Entities.SwapDealAnalysisOutboundRateCalcMethod, TOne.WhS.Deal.Entities]], mscorlib","3856e40a-b8a8-84f2-7788-4728a4eb8396":{"$type":"TOne.WhS.Deal.MainExtensions.SwapDealAnalysisOutboundRateFixed, TOne.WhS.Deal.MainExtensions","ConfigId":"ea1454a9-0fa0-4b16-93e8-76533bd504f3","ItemEditor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-fixed-itemeditor","CalculationMethodId":"3856e40a-b8a8-84f2-7788-4728a4eb8396","Title":"User Defined"},"a5e39e2d-8583-cbe4-329a-cca00742abac":{"$type":"TOne.WhS.Deal.MainExtensions.SwapDealAnalysisOutboundRateSuppliers, TOne.WhS.Deal.MainExtensions","ConfigId":"2bba4c50-ce06-4f61-9731-d31ce687cabf","ItemEditor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-suppliers-itemeditor","CalculationMethodId":"a5e39e2d-8583-cbe4-329a-cca00742abac","Title":"Based on other Suppliers'' Cost Rates"},"a46e761a-665f-c515-728b-8fbc536da9ff":{"$type":"TOne.WhS.Deal.MainExtensions.SwapDealAnalysisOutboundRateCustomers, TOne.WhS.Deal.MainExtensions","ConfigId":"91ec1aca-5d9b-418d-a327-8ee699ce192f","ItemEditor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-customers-itemeditor","CalculationMethodId":"a46e761a-665f-c515-728b-8fbc536da9ff","Title":"Based on Customers'' Sale Rates"}},"GracePeriod":7}',0),('74A4DCC0-5095-4AF4-827D-40054098A0D4','Swap Deal Technical','WhS_Deal_SwapDealTechnicalSettings','Business Entities','{"Editor":"vr-whs-deal-swapdeal-technicalsettings-editor"}','{"$type":"TOne.WhS.Deal.Entities.SwapDealTechnicalSettingData, TOne.WhS.Deal.Entities","SwapDealBuyRouteRuleDefinitionId":"3baf7ff7-2c85-4297-92de-7d333c27dea5"}',1),('45FAB109-7E51-462E-9F13-562604E29790','Deal Technical','WhS_Deal_DealTechnicalSettings','Business Entities','{"Editor":"vr-whs-deal-technicalsettings-editor"}','{"$type":"TOne.WhS.Deal.Entities.DealTechnicalSettingData, TOne.WhS.Deal.Entities","ReprocessDefinitionId":"3b76e3b2-4f31-4d59-8e9d-018ac09a3905","ChunkTime":4,"IntervalOffsetInMinutes":30}',1)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))merge	[common].[setting] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]when not matched by target then	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
----------------------------------------------------------------------------------------------------
END


--[common].[VRComponentType]------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3BAF7FF7-2C85-4297-92DE-7D333C27DEA5','Swap Deal Buy Route Rule','3CC81591-9860-4B9D-9755-0E5D7E87A596','{"$type":"Vanrise.Entities.VRRuleDefinitionSettings, Vanrise.Entities","VRComponentTypeConfigId":"3cc81591-9860-4b9d-9755-0e5d7e87a596","VRRuleDefinitionExtendedSettings":{"$type":"TOne.WhS.Deal.Entities.SwapDealBuyRouteRuleDefinitionSettings, TOne.WhS.Deal.Entities","ConfigId":"9b2db188-650c-4f19-8bcd-15becc653e46"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ConfigID],[Settings]))
merge	[common].[VRComponentType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ConfigID] = s.[ConfigID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ConfigID],[Settings])
	values(s.[ID],s.[Name],s.[ConfigID],s.[Settings]);
----------------------------------------------------------------------------------------------------
END


--[bp].[BPDefinition]-------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('63016D0C-21DE-47EC-A41F-2CF5EABB8A2C','TOne.WhS.Deal.BP.Arguments.DealEvaluatorProcessInput','Deal Evaluator','TOne.WhS.Deal.BP.DealEvaluatorProcess, TOne.WhS.Deal.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"vr-whs-deal-dealevaluator-process","ScheduledExecEditor":"vr-whs-deal-dealevaluator-task","IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"TOne.WhS.Deal.Business.DealEvaluatorBPDefinitionSettings, TOne.WhS.Deal.Business"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Reprocess Logs"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Reprocess"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Reprocess"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([ID],[Name],[Title],[FQTN],[Config])
	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
----------------------------------------------------------------------------------------------------
END

--[reprocess].[ReprocessDefinition]---------------------- 1 to 100----------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3B76E3B2-4F31-4D59-8E9D-018AC09A3905','Deal Evaluator Reprocess','{"$type":"Vanrise.Reprocess.Entities.ReprocessDefinitionSettings, Vanrise.Reprocess.Entities","SourceRecordStorageIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["31cc031c-c1d2-46f4-a0a4-92e2322ce16a","ed4b26d7-8e08-4113-b0b1-c365adfefb50"]},"ExecutionFlowDefinitionId":"25da1ef3-b6f2-40e0-821b-03c632b580e4","StageNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Main CDR Storage Stage","Billing Stats Daily Generation Stage","Partial Priced CDR Storage Stage","Update WhS Balances","Billing Stats XMin Generation Stage","Evaluate Deal Stage"]},"InitiationStageNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Evaluate Deal Stage"]},"StagesToHoldNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Evaluate Deal Stage"]},"StagesToProcessNames":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Update WhS Balances","Main CDR Storage Stage","Billing Stats Daily Update Stage","Billing Stats Daily Generation Stage","Billing Stats XMin Update Stage","Billing Stats XMin Generation Stage","Partial Priced CDR Storage Stage"]},"RecordCountPerTransaction":50000,"ForceUseTempStorage":true}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Settings]))
merge	[reprocess].[ReprocessDefinition] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([Id],[Name],[Settings])
	values(s.[Id],s.[Name],s.[Settings]);
----------------------------------------------------------------------------------------------------
end