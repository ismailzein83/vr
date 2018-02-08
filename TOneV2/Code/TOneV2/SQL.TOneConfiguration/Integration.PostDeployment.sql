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
('FC06083C-4540-4B0F-89AD-FAC4C04140B8','VR_Integration_AdapterTypeConfig_SFTP','SFTP Receive Adapter'			,'VR_Integration_AdapterTypeConfig','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/FTPReceiveAdapterTemplate.html","Editor":"vr-integration-adapter-sftp","FQTN":"Vanrise.Integration.Adapters.SFTPReceiveAdapter.SFTPReceiveAdapter, Vanrise.Integration.Adapters.SFTPReceiveAdapter"}'),
('10323CCB-CBFD-4BBE-91F3-FC80E2D91630','VR_Integration_AdapterTypeConfig_SQL','SQL Receive Adapter'			,'VR_Integration_AdapterTypeConfig','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/DBReceiveAdapterTemplate.html","Editor":"vr-integration-adapter-db","FQTN":"Vanrise.Integration.Adapters.SQLReceiveAdapter.SQLReceiveAdapter, Vanrise.Integration.Adapters.SQLReceiveAdapter"}'),
('105A2927-10C2-4F20-97CE-EDBA90198553','VR_Integration_AdapterTypeConfig_Postgres','Postgres Receive Adapter'	,'VR_Integration_AdapterTypeConfig','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/AdapterPostgresTemplate.html","Editor":"vr-integration-adapter-postgres","FQTN":"Vanrise.Integration.Adapters.PostgresReceiveAdapter.PostgresReceiveAdapter, Vanrise.Integration.Adapters.PostgresReceiveAdapter"}'),
('A694ECE9-CB4B-42D5-9150-4DD4C499BBE6','VR_Integration_AdapterTypeConfig_MySQL','MySql Receive Adapter'	,'VR_Integration_AdapterTypeConfig','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/AdapterMySQLTemplate.html","Editor":"vr-integration-adapter-mysql","FQTN":"Vanrise.Integration.Adapters.MySQLReceiveAdapter.MySQLReceiveAdapter, Vanrise.Integration.Adapters.MySQLReceiveAdapter"}'),

('EA1F0775-08B9-4638-AC42-A2FD11E008EF','VR_Common_ObjectType_FailedBatchInfo','Failed Batch Info','VR_Common_ObjectType'																			,'{"Editor":"vr-integration-faildbatchinfo-objecttype", "PropertyEvaluatorExtensionType": "VR_Integration_FailedBatchInfo_PropertyEvaluator"}'),
('708F5F03-F14E-4487-B916-5617013E8B3E','VR_Common_PropertyEvaluator_FailedBatchInfo','Failed Batch Info Property','VR_Integration_FailedBatchInfo_PropertyEvaluator'										,'{"Editor":"vr-integration-faildbatchinfo-propertyevaluator"}')

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

--[sec].[View]-----------------------------4001 to 5000-------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FFC528C2-BF74-40E5-B7FE-A3B6F04B76ED','Data Sources','Data Sources','#/view/Integration/Views/DataSourceManagement','551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSource/GetFilteredDataSources',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',1)
--('3EBA3C0E-28CA-4003-959B-96D30D6747B7','Log','Log History','#/view/Integration/Views/DataSourceLogManagement','551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSourceLog/GetFilteredDataSourceLogs',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2),
--('C0231D0B-93E8-45C5-9899-532923814C8C','Imported Batches','Imported Batches','#/view/Integration/Views/DataSourceImportedBatchManagement','551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSourceImportedBatch/GetFilteredDataSourceImportedBatches',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',3)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
--------------------------------------------------------------------------------------------------------------

END


--[sec].[BusinessEntity]-------------------901 to 1200--------------------------------------------------------
BEGIN
set nocount on;;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('CCD65B5B-53BB-4816-B7EA-D8DC58AA513E','VR_Integration_DataProcesses','Data Processes','B6B8F582-4759-43FB-9220-AA7662C366EA',0,'["View Data Sources","Add Data Sources","Edit Data Sources","View Logs","View Reprocess Logs","Start Reprocess"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
END


--[sec].[SystemAction]------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Integration/DataSource/GetDataSources',null),
('VR_Integration/DataSource/GetFilteredDataSources','VR_Integration_DataProcesses: View Data Sources'),
('VR_Integration/DataSource/GetDataSource','VR_Integration_DataProcesses: View Data Sources'),
('VR_Integration/DataSource/GetDataSourceAdapterTypes',null),
('VR_Integration/DataSource/GetExecutionFlows',null),
('VR_Integration/DataSource/AddExecutionFlow',null),
('VR_Integration/DataSource/GetExecutionFlowDefinitions',null),
('VR_Integration/DataSource/AddDataSource','VR_Integration_DataProcesses: Add Data Sources'),
('VR_Integration/DataSource/UpdateDataSource','VR_Integration_DataProcesses: Edit Data Sources'),
('VR_Integration/DataSourceImportedBatch/GetFilteredDataSourceImportedBatches','VR_Integration_DataProcesses: View Logs'),
('VR_Integration/DataSourceLog/GetFilteredDataSourceLogs','VR_Integration_DataProcesses: View Logs')
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

--[runtime].[SchedulerTaskActionType]-------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[ActionTypeInfo])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','Data Source','{"URL":"","SystemType":true,"FQTN":"Vanrise.Integration.Business.DSSchedulerTaskAction, Vanrise.Integration.Business","Security":{"$type":"Vanrise.Runtime.Entities.ActionTypeInfoSecurity, Vanrise.Runtime.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":["View Data Sources"]}]},"ConfigurePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":["Edit Data Sources"]}]},"RunPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":["Edit Data Sources"]}]}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[ActionTypeInfo]))merge	[runtime].[SchedulerTaskActionType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[ActionTypeInfo] = s.[ActionTypeInfo]when not matched by target then	insert([ID],[Name],[ActionTypeInfo])	values(s.[ID],s.[Name],s.[ActionTypeInfo]);--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
begin
set nocount on;;with cte_data([ID],[Name],[Title],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('E522907C-AEBD-48B6-82F4-FE55238942F2','VR_Integration_DataSource','Data Source','{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-integration-datasource-selector","GroupSelectorUIControl":"","ManagerFQTN":"Vanrise.Integration.Business.DataSourceManager,Vanrise.Integration.Business","IdType":"System.Guid"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[Settings]))merge	[genericdata].[BusinessEntityDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[Settings]);
----------------------------------------------------------------------------------------------------
end--[logging].[LoggableEntity]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[UniqueName],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('3BD49160-FBB8-41B6-951D-CD7024CC2DB9','VR_Integration_DataSource','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Integration_DataSource_ViewHistoryItem"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[UniqueName],[Settings]))merge	[logging].[LoggableEntity] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]when not matched by target then	insert([ID],[UniqueName],[Settings])	values(s.[ID],s.[UniqueName],s.[Settings]);--common.[VRObjectTypeDefinition]-----------------------------------------------------------------------------------------------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('69B266CE-E309-426F-B672-B0E61D3211FC','Data Source Failed Batch Info','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoObjectType, Vanrise.Integration.MainExtensions","ConfigId":"ea1f0775-08b9-4638-ac42-a2fd11e008ef"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Message":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Message","Description":"Message","PropertyEvaluator":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoPropertyEvaluator, Vanrise.Integration.MainExtensions","FailedBatchInfoField":0,"ConfigId":"708f5f03-f14e-4487-b916-5617013e8b3e"}},"IsEmpty":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"IsEmpty","Description":"Is Empty","PropertyEvaluator":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoPropertyEvaluator, Vanrise.Integration.MainExtensions","FailedBatchInfoField":1,"ConfigId":"708f5f03-f14e-4487-b916-5617013e8b3e"}},"DataSourceName":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"DataSourceName","Description":"Data Source Name","PropertyEvaluator":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoPropertyEvaluator, Vanrise.Integration.MainExtensions","FailedBatchInfoField":4,"ConfigId":"708f5f03-f14e-4487-b916-5617013e8b3e"}},"DataSourceId":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"DataSourceId","Description":"Data Source Id","PropertyEvaluator":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoPropertyEvaluator, Vanrise.Integration.MainExtensions","FailedBatchInfoField":3,"ConfigId":"708f5f03-f14e-4487-b916-5617013e8b3e"}},"BatchDescription":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"BatchDescription","Description":"Batch Description","PropertyEvaluator":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoPropertyEvaluator, Vanrise.Integration.MainExtensions","FailedBatchInfoField":2,"ConfigId":"708f5f03-f14e-4487-b916-5617013e8b3e"}}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[VRObjectTypeDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);----------------------------------------------------------------------------------------------------end--[common].[MailMessageType]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('0F64DA0B-E2D0-4421-BEB9-32C6E749F8F1','Data Source Failed Batch Mail Template Type','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Failed Batch Info":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Failed Batch Info","VRObjectTypeDefinitionId":"69b266ce-e309-426f-b672-b0e61d3211fc"},"User":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"User","VRObjectTypeDefinitionId":"e3887cc9-1fbb-44d1-b1e3-7a0922400550"}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[MailMessageType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);