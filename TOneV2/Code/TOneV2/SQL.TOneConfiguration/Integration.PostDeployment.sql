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
--[common].[ExtensionConfiguration]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('432AAFD8-62C3-4D2B-99A8-9967D198337F','VR_Integration_AdapterTypeConfig_File','File Receive Adapter'			,'VR_Integration_AdapterTypeConfig','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/FileReceiveAdapterTemplate.html","Editor":"vr-integration-adapter-file","FQTN":"Vanrise.Integration.Adapters.FileReceiveAdapter.FileReceiveAdapter, Vanrise.Integration.Adapters.FileReceiveAdapter"}'),
('396A4933-DF4F-49CD-9799-BF605B9F4597','VR_Integration_AdapterTypeConfig_FTP','FTP Receive Adapter'			,'VR_Integration_AdapterTypeConfig','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/FTPReceiveAdapterTemplate.html","Editor":"vr-integration-adapter-ftp","FQTN":"Vanrise.Integration.Adapters.FTPReceiveAdapter.FTPReceiveAdapter, Vanrise.Integration.Adapters.FTPReceiveAdapter"}'),
('FC06083C-4540-4B0F-89AD-FAC4C04140B8','VR_Integration_AdapterTypeConfig_SFTP','SFTP Receive Adapter'			,'VR_Integration_AdapterTypeConfig','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/FTPReceiveAdapterTemplate.html","Editor":"vr-integration-adapter-ftp","FQTN":"Vanrise.Integration.Adapters.FTPReceiveAdapter.SFTPReceiveAdapter, Vanrise.Integration.Adapters.SFTPReceiveAdapter"}'),
('10323CCB-CBFD-4BBE-91F3-FC80E2D91630','VR_Integration_AdapterTypeConfig_SQL','SQL Receive Adapter'			,'VR_Integration_AdapterTypeConfig','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/DBReceiveAdapterTemplate.html","Editor":"vr-integration-adapter-db","FQTN":"Vanrise.Integration.Adapters.SQLReceiveAdapter.SQLReceiveAdapter, Vanrise.Integration.Adapters.SQLReceiveAdapter"}'),
('105A2927-10C2-4F20-97CE-EDBA90198553','VR_Integration_AdapterTypeConfig_Postgres','Postgres Receive Adapter'	,'VR_Integration_AdapterTypeConfig','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/AdapterPostgresTemplate.html","Editor":"vr-integration-adapter-postgres","FQTN":"Vanrise.Integration.Adapters.PostgresReceiveAdapter.PostgresReceiveAdapter, Vanrise.Integration.Adapters.PostgresReceiveAdapter"}')
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

--[sec].[Module]---------------------------401 to 500---------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('551E5CAE-69CA-478B-B823-8E2CEDBC1841','Data Processes',null,'1037157D-BBC9-4B28-B53F-908936CEC137',null,10,0)--'System Processes'
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

DELETE FROM [sec].[View] WHERE [ID] IN ('3EBA3C0E-28CA-4003-959B-96D30D6747B7','C0231D0B-93E8-45C5-9899-532923814C8C')
--[sec].[View]-----------------------------4001 to 5000-------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FFC528C2-BF74-40E5-B7FE-A3B6F04B76ED','Data Sources','Data Sources Management','#/view/Integration/Views/DataSourceManagement','551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSource/GetFilteredDataSources',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,1)
--('3EBA3C0E-28CA-4003-959B-96D30D6747B7','Log','Log History','#/view/Integration/Views/DataSourceLogManagement','551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSourceLog/GetFilteredDataSourceLogs',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,2),
--('C0231D0B-93E8-45C5-9899-532923814C8C','Imported Batches','Imported Batches','#/view/Integration/Views/DataSourceImportedBatchManagement','551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSourceImportedBatch/GetFilteredDataSourceImportedBatches',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,3)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);
--------------------------------------------------------------------------------------------------------------

END

--[sec].[BusinessEntityModule]-------------401 to 500---------------------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('7CC991E0-A37A-4BE7-A310-B72FB2292914',401,'CDR processing','61451603-E7B9-40C6-AE27-6CBA974E1B3B',2,0),('B0326663-9249-445B-B561-9D65B44919BC',402,'Datasource','B6B8F582-4759-43FB-9220-AA7662C366EA',401,0),('AD4DAF11-F476-4D15-9FF4-98D1A9639111',403,'Configuration rules','7CC991E0-A37A-4BE7-A310-B72FB2292914',401,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[ParentId] = s.[ParentId],[OldParentId] = s.[OldParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])	values(s.[ID],s.[OldId],s.[Name],s.[ParentId],s.[OldParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
END

Delete from [sec].[BusinessEntity] where [Id] in ('4C07BCD1-BBB0-4ABE-A055-B375950ABB59','560F893F-C03F-4DF1-A300-A499ACA3E75A') 
--[sec].[BusinessEntity]-------------------901 to 1200--------------------------------------------------------
BEGIN
set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('CCD65B5B-53BB-4816-B7EA-D8DC58AA513E',901,'VR_Integration_DataSource','Datasource','B0326663-9249-445B-B561-9D65B44919BC',402,0,'["View","Add","Edit","Delete","ImportedBatch","Log"]')--('4C07BCD1-BBB0-4ABE-A055-B375950ABB59',902,'VR_Integration_DataSource_ImportedBatch','Imported Batches','B0326663-9249-445B-B561-9D65B44919BC',402,0,'["View"]'),--('560F893F-C03F-4DF1-A300-A499ACA3E75A',903,'VR_Integration_DataSource_Log','Log','B0326663-9249-445B-B561-9D65B44919BC',402,0,'["View"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);
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
('VR_Integration/DataSourceImportedBatch/GetFilteredDataSourceImportedBatches','VR_Integration_DataSource: ImportedBatch'),
('VR_Integration/DataSourceLog/GetFilteredDataSourceLogs','VR_Integration_DataSource: Log')
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

--[runtime].[SchedulerTaskActionType]-------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[OldID],[Name],[ActionTypeInfo])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68',1,'Data Source','{"URL":"", "SystemType":true, "FQTN":"Vanrise.Integration.Business.DSSchedulerTaskAction, Vanrise.Integration.Business"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldID],[Name],[ActionTypeInfo]))merge	[runtime].[SchedulerTaskActionType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldID] = s.[OldID],[Name] = s.[Name],[ActionTypeInfo] = s.[ActionTypeInfo]when not matched by target then	insert([ID],[OldID],[Name],[ActionTypeInfo])	values(s.[ID],s.[OldID],s.[Name],s.[ActionTypeInfo]);