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
--[common].[ExtensionConfiguration]---------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('76F467C2-E440-4B11-A42C-59F69FDDBCB7','Fixed Rate','Fixed Rate','WhS_Deal_SwapDeal_InboundRateCalculationMethod'															,'{"Editor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-inbound-fixed-editor"}'),('EA1454A9-0FA0-4B16-93E8-76533BD504F3','Fixed Rate','Fixed Rate','WhS_Deal_SwapDeal_OutboundRateCalculationMethod'															,'{"Editor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-fixed-editor"}'),('96B9EB7E-D08B-4CCB-AC0D-7FE112EF41D8','Based on other Suppliers'' Cost Rates','Based on other Suppliers'' Cost Rates','WhS_Deal_SwapDeal_InboundRateCalculationMethod'	,'{"Editor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-inbound-suppliers-editor"}'),('91EC1ACA-5D9B-418D-A327-8EE699CE192F','Based on Customers'' Sale Rates','Based on Customers'' Sale Rates','WhS_Deal_SwapDeal_OutboundRateCalculationMethod'				,'{"Editor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-customers-editor"}'),('2BBA4C50-CE06-4F61-9731-D31CE687CABF','Based on other Suppliers'' Cost Rates','Based on other Suppliers'' Cost Rates','WhS_Deal_SwapDeal_OutboundRateCalculationMethod'	,'{"Editor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-suppliers-editor"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[ExtensionConfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);
--[sec].[Module]---------------------------1701 to 1800---------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('670FD0F9-6DCE-4567-8E12-DC3E5830B549','Deals',null,null,'/images/menu-icons/Deals Module.png',15,0)
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
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('95F31CAF-F3F0-4617-B253-031A8FD69866','Volume Commitment','Volume Commitment Management','#/view/WhS_Deal/Views/VolumeCommitment/VolumeCommitmentManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549',null,null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,5),('BAA1EDFB-4DA7-4BEE-ABA0-9E375EA3E3BF','Swap Deal','Swap Deal Management','#/view/WhS_Deal/Views/SwapDeal/SwapDealManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549',null,null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,9)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);
-------------------------------------------------------------------------------------------------------------
END

--[sec].[BusinessEntityModule]----------1501 to 1600------------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('FD41836C-146A-404A-9D4D-124C42757F3D',1501,'Deals','D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',201,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[ParentId] = s.[ParentId],[OldParentId] = s.[OldParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])	values(s.[ID],s.[OldId],s.[Name],s.[ParentId],s.[OldParentId],s.[BreakInheritance]);
----------------------------------------------------------------------------------------------------
END

--[sec].[BusinessEntity]----------------4501 to 4800------------------------------------------------
BEGIN
set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('F6B59F7B-280D-47B9-87B8-8E433DF407E1',4501,'WhS_BE_Deal','Deal','FD41836C-146A-404A-9D4D-124C42757F3D',1501,0,'["View","Add","Edit"]'),('F45F9A2A-3C91-42B5-84FE-E6C5329D38DC',4502,'WhS_BE_Deal_Analysis','Deal Analysis','FD41836C-146A-404A-9D4D-124C42757F3D',1501,0,'["View","Add","Edit"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);
----------------------------------------------------------------------------------------------------
END

--[sec].[SystemAction]------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('WhS_BE/Deal/GetFilteredDeals','WhS_BE_Deal: View'),
('WhS_BE/Deal/GetDeal','WhS_BE_Deal: View'),
('WhS_BE/Deal/AddDeal','WhS_BE_Deal: Add'),
('WhS_BE/Deal/UpdateDeal','WhS_BE_Deal: Edit')
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

--[common].[setting]------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('8A71FE78-AF3D-40DB-BA46-B37DF712601A',null,'Swap Deal','WhS_Deal_SwapDealSettings','Business Entities','{"Editor":"vr-whs-deal-swapdeal-settings-editor"}','{"$type":"TOne.WhS.Deal.Entities.Settings.SwapDealSettingData, TOne.WhS.Deal.Entities","DefaultCalculationMethodId":"a46e761a-665f-c515-728b-8fbc536da9ff","DefaultInboundRateCalcMethodId":"631a698b-e39e-79de-00a4-1812fa7758f5","InboundCalculationMethods":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[TOne.WhS.Deal.Entities.SwapDealAnalysisInboundRateCalcMethod, TOne.WhS.Deal.Entities]], mscorlib","631a698b-e39e-79de-00a4-1812fa7758f5":{"$type":"TOne.WhS.Deal.MainExtensions.SwapDealAnalysisInboundRateSuppliers, TOne.WhS.Deal.MainExtensions","ConfigId":"96b9eb7e-d08b-4ccb-ac0d-7fe112ef41d8","ItemEditor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-inbound-suppliers-itemeditor","CalculationMethodId":"631a698b-e39e-79de-00a4-1812fa7758f5","Title":"Suppliers"},"248259ff-f865-53b1-6080-75124cead9f2":{"$type":"TOne.WhS.Deal.MainExtensions.SwapDealAnalysisInboundRateFixed, TOne.WhS.Deal.MainExtensions","ConfigId":"76f467c2-e440-4b11-a42c-59f69fddbcb7","ItemEditor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-inbound-fixed-itemeditor","CalculationMethodId":"248259ff-f865-53b1-6080-75124cead9f2","Title":"Fixed"}},"OutboundCalculationMethods":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[TOne.WhS.Deal.Entities.SwapDealAnalysisOutboundRateCalcMethod, TOne.WhS.Deal.Entities]], mscorlib","3856e40a-b8a8-84f2-7788-4728a4eb8396":{"$type":"TOne.WhS.Deal.MainExtensions.SwapDealAnalysisOutboundRateFixed, TOne.WhS.Deal.MainExtensions","ConfigId":"ea1454a9-0fa0-4b16-93e8-76533bd504f3","ItemEditor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-fixed-itemeditor","CalculationMethodId":"3856e40a-b8a8-84f2-7788-4728a4eb8396","Title":"User Defined"},"a5e39e2d-8583-cbe4-329a-cca00742abac":{"$type":"TOne.WhS.Deal.MainExtensions.SwapDealAnalysisOutboundRateSuppliers, TOne.WhS.Deal.MainExtensions","ConfigId":"2bba4c50-ce06-4f61-9731-d31ce687cabf","ItemEditor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-suppliers-itemeditor","CalculationMethodId":"a5e39e2d-8583-cbe4-329a-cca00742abac","Title":"Based on other Suppliers'' Cost Rates"},"a46e761a-665f-c515-728b-8fbc536da9ff":{"$type":"TOne.WhS.Deal.MainExtensions.SwapDealAnalysisOutboundRateCustomers, TOne.WhS.Deal.MainExtensions","ConfigId":"91ec1aca-5d9b-418d-a327-8ee699ce192f","ItemEditor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-customers-itemeditor","CalculationMethodId":"a46e761a-665f-c515-728b-8fbc536da9ff","Title":"Based on Customers'' Sale Rates"}},"GracePeriod":7}',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))merge	[common].[setting] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]when not matched by target then	insert([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])	values(s.[ID],s.[OldId],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);

