﻿/*
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
--[common].[ExtensionConfiguration]---------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('EA1454A9-0FA0-4B16-93E8-76533BD504F3','Fixed Rate','Fixed Rate','WhS_Deal_SwapDeal_OutboundRateCalculationMethod','{"Editor":"vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-fixed-editor"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[ExtensionConfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);
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
('1437E823-0D44-4176-82F2-5E1D2F4D83EA','Deals Analysis','Deals Analysis Management','#/view/WhS_BusinessEntity/Views/Deal/DealAnalysisManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549','WhS_BE/Deal/GetFilteredDeals',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,2),
('6177DE52-9D79-438D-8B33-ED820FD7D5C5','Deals Management','Deals Management','#/view/WhS_BusinessEntity/Views/Deal/DealManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549','WhS_BE/Deal/GetFilteredDeals',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,3),
('E207E8D2-EF62-484C-A103-97758DBA5FC3','Bilateral Agreement','Bilateral Agreement','#/view/WhS_BusinessEntity/Views/Deal/DealManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549','WhS_BE/Deal/GetFilteredDeals',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,4),
('95F31CAF-F3F0-4617-B253-031A8FD69866','Volume Commitment','Volume Commitment','#/view/WhS_BusinessEntity/Views/Deal/VolumeCommitmentManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549','WhS_BE/Deal/GetFilteredDeals',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,5),
('AE14C61E-F568-4592-BE3E-0BF678AD3A42','Progress Report','Progress Report','#/view/WhS_BusinessEntity/Views/Deal/DealProgressManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549','WhS_BE/Deal/GetFilteredDeals',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,6),
('B16DFE22-64A3-4AB2-AC57-004BA6EA4C69','Over All Progress Report','Over All Progress Report','#/view/WhS_BusinessEntity/Views/Deal/DealManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549','WhS_BE/Deal/GetFilteredDeals',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,7),
('82D7BD5F-4867-43EC-A5AD-3B5CF0612B04','Alerts Management','Alerts Management','#/view/WhS_BusinessEntity/Views/Deal/DealManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549','WhS_BE/Deal/GetFilteredDeals',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,8),
('94CED066-2F04-4397-9AF8-3E91C167B8F5','Alerts History','Alerts History','#/view/WhS_BusinessEntity/Views/Deal/DealManagement','670FD0F9-6DCE-4567-8E12-DC3E5830B549','WhS_BE/Deal/GetFilteredDeals',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,9)
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
