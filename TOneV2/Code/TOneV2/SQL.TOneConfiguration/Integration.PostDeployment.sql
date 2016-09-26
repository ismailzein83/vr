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
--[sec].[Module]---------------------------401 to 500---------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('551E5CAE-69CA-478B-B823-8E2CEDBC1841','Data Sources',null,'50624672-CD25-44FD-8580-0E3AC8E34C71',null,10,0)
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

--[sec].[View]-----------------------------4001 to 5000-------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FFC528C2-BF74-40E5-B7FE-A3B6F04B76ED','Management','Data Sources Management','#/view/Integration/Views/DataSourceManagement','551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSource/GetFilteredDataSources',null,null,null,0,1),
('3EBA3C0E-28CA-4003-959B-96D30D6747B7','Log','Log History','#/view/Integration/Views/DataSourceLogManagement','551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSourceLog/GetFilteredDataSourceLogs',null,null,null,0,2),
('C0231D0B-93E8-45C5-9899-532923814C8C','Imported Batches','Imported Batches','#/view/Integration/Views/DataSourceImportedBatchManagement','551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSourceImportedBatch/GetFilteredDataSourceImportedBatches',null,null,null,0,3)
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
--------------------------------------------------------------------------------------------------------------

END

--[sec].[BusinessEntityModule]-------------401 to 500---------------------------------------------------------
BEGIN
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(401,'CDR processing',2,0),
(402,'Datasource',401,0),
(403,'Configuration rules',401,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([Id],[Name],[ParentId],[BreakInheritance])
	values(s.[Id],s.[Name],s.[ParentId],s.[BreakInheritance]);
set identity_insert [sec].[BusinessEntityModule] off;
--------------------------------------------------------------------------------------------------------------
END


--[sec].[BusinessEntity]-------------------901 to 1200--------------------------------------------------------
BEGIN
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(901,'VR_Integration_DataSource','Datasource',402,0,'["View","Add","Edit","Delete"]'),
(902,'VR_Integration_DataSource_ImportedBatch','Imported batches',402,0,'["View"]'),
(903,'VR_Integration_DataSource_Log','Log',402,0,'["View"]')
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
--------------------------------------------------------------------------------------------------------------
END


--[sec].[SystemAction]------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Integration/DataSource/GetDataSources',null),
('VR_Integration/DataSource/GetFilteredDataSources','VR_Integration_DataSource: View'),
('VR_Integration/DataSource/GetDataSource',null),
('VR_Integration/DataSource/GetDataSourceAdapterTypes',null),
('VR_Integration/DataSource/GetExecutionFlows',null),
('VR_Integration/DataSource/AddExecutionFlow',null),
('VR_Integration/DataSource/GetExecutionFlowDefinitions',null),
('VR_Integration/DataSource/AddDataSource','VR_Integration_DataSource: Add'),
('VR_Integration/DataSource/UpdateDataSource','VR_Integration_DataSource: Edit'),
('VR_Integration/DataSource/DeleteDataSource','VR_Integration_DataSource: Delete'),
('VR_Integration/DataSourceImportedBatch/GetFilteredDataSourceImportedBatches','VR_Integration_DataSource_ImportedBatch: View'),
('VR_Integration/DataSourceLog/GetFilteredDataSourceLogs','VR_Integration_DataSource_Log: View')
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

