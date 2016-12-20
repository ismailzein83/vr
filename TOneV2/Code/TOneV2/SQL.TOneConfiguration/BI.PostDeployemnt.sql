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
begin
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
----------------------------------------------------------------------------------------------------
end

--[common].[ExtensionConfiguration]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('82B4CD55-3500-4158-ACCD-E5D244390746','VR_Sec_BI','BI','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html","EnableAdd":true}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))
merge	[common].[ExtensionConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[ConfigType],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);

--[sec].[Module]------------------------------1301 to 1400------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('42BE0E81-EAEE-490D-A342-028FB111DE19','Dynamic Management',null,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,11,0),
('EB303A61-929A-4D33-BF50-18F40308BC86','Reports & Dashboards',null,null,'/images/menu-icons/busines intel.png',35,1)
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

end

--[sec].[View]-----------------------------13001 to 14000------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1A2C8311-F0CE-40EC-A864-D815EF65301C','Widgets','Widgets Management','#/view/Security/Views/WidgetsPages/WidgetManagement','42BE0E81-EAEE-490D-A342-028FB111DE19','VR_Sec/Widget/GetFilteredWidgets',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,2),
('21F5BF24-36A9-4CFF-B027-FC61AC57A838','Pages','Dynamic Pages Management','#/view/Security/Views/DynamicPages/DynamicPageManagement','42BE0E81-EAEE-490D-A342-028FB111DE19','VR_Sec/View/GetFilteredViews & VR_Sec/View/GetFilteredDynamicViews',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,3)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);
---------------------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]------------------3601 to 3900----------------------------------------------------------
begin

set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('38630117-4929-42D1-8685-5F5AE815DFE1',3601,'VR_Sec_Widget','BI Widget','4C9719E3-F818-454D-9977-01A9668E7ABA',3,0,'["View","Add","Edit","Delete"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);
----------------------------------------------------------------------------------------------------------------
end

--[sec].[systemaction]------------------------------------------------------------------------------
begin

set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Sec/Widget/GetFilteredWidgets','VR_Sec_Widget:View'),
('VR_Sec/Widget/AddWidget','VR_Sec_Widget:Add'),
('VR_Sec/Widget/UpdateWidget','VR_Sec_Widget:Edit'),
('VR_Sec/Widget/DeleteWidget','VR_Sec_Widget:Delete')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[systemaction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);
----------------------------------------------------------------------------------------------------
end
