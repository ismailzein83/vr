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
--[sec].[WidgetDefinition]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[WidgetDefinition] on;
;with cte_data([ID],[Name],[DirectiveName],[Setting])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Report','vr-bi-datagrid','{"DirectiveTemplateURL":"vr-bi-datagrid-template","Sections":[1]}'),
(2,'Chart','vr-bi-chart','{"DirectiveTemplateURL":"vr-bi-chart-template","Sections":[1]}'),
(3,'Summary','vr-bi-summary','{"DirectiveTemplateURL":"vr-bi-summary-template","Sections":[0]} ')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DirectiveName],[Setting]))
merge	[sec].[WidgetDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DirectiveName] = s.[DirectiveName],[Setting] = s.[Setting]
when not matched by target then
	insert([ID],[Name],[DirectiveName],[Setting])
	values(s.[ID],s.[Name],s.[DirectiveName],s.[Setting]);
set identity_insert [sec].[WidgetDefinition] off;

--[sec].[Module]------------------------------1301 to 1400------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1301,'Dynamic Management',null,1,null,45,0),
(1302,'Business Intelligence',null,null,'/images/menu-icons/busines intel.png',16,1)
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
set identity_insert [sec].[Module] off;

--[sec].[viewtype]---------------------------301 to 400---------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(301,'VR_Sec_BI','BI','{"ViewTypeId":1,"Name":"VR_Sec_BI","Title":"Business Intelligence","Editor":"/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html","EnableAdd":true}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Details]))
merge	[sec].[viewtype] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Title],[Details])
	values(s.[ID],s.[Name],s.[Title],s.[Details]);

--[sec].[View]-----------------------------13001 to 14000------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(13001,'Widgets','Widgets Management','#/view/Security/Views/WidgetsPages/WidgetManagement',1301,'Root/Administration Module/Dynamic Pages:View',null,null,null,0,1),
(13002,'Pages','Dynamic Pages Management','#/view/Security/Views/DynamicPages/DynamicPageManagement',1301,'Root/Administration Module/Dynamic Pages:View',null,null,null,0,2)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
set identity_insert [sec].[View] off;

--[sec].[BusinessEntity]------------------3601 to 3900----------------------------------------------------------
----------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(3601,'BI Dynamic Pages','BI Dynamic Pages',2,1,'["View", "Add", "Edit", "Delete","Validate"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
set identity_insert [sec].[BusinessEntity] off;

--[sec].[Permission]--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(0,'1',1,'3601','[{"Name":"View","Value":1}, {"Name":"Edit","Value":1}, {"Name":"Delete","Value":1}, {"Name":"Add","Value":1}, {"Name":"Validate","Value":1}]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags]))
merge	[sec].[Permission] as t
using	cte_data as s
on		1=1 and t.[HolderType] = s.[HolderType] and t.[HolderId] = s.[HolderId] and t.[EntityType] = s.[EntityType] and t.[EntityId] = s.[EntityId]
when matched then
	update set
	[PermissionFlags] = s.[PermissionFlags]
when not matched by target then
	insert([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
	values(s.[HolderType],s.[HolderId],s.[EntityType],s.[EntityId],s.[PermissionFlags]);