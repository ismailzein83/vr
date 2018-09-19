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
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FFC528C2-BF74-40E5-B7FE-A3B6F04B76ED','Data Sources','Data Sources','#/view/Integration/Views/DataSourceManagement'						,'551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSource/GetFilteredDataSources',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',1),
('6E8852CC-6D8A-4B5F-9EBC-C5F21B7EA38B','Data Source Alerts','Data Source Alerts',NULL														,'551E5CAE-69CA-478B-B823-8E2CEDBC1841',NULL,NULL,NULL,'{"$type":"Vanrise.Notification.Entities.VRNotificationViewSettings, Vanrise.Notification.Entities","Settings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Notification.Entities.VRNotificationViewSettingItem, Vanrise.Notification.Entities]], mscorlib","$values":[{"$type":"Vanrise.Notification.Entities.VRNotificationViewSettingItem, Vanrise.Notification.Entities","VRNotificationTypeId":"7364c3e5-1138-4dcf-9ddb-70bebf246b67"}]}}','A196C40A-30B5-4297-B7B0-4344C41CE5A2',NULL)
--('3EBA3C0E-28CA-4003-959B-96D30D6747B7','Log','Log History','#/view/Integration/Views/DataSourceLogManagement'							,'551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSourceLog/GetFilteredDataSourceLogs',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2),
--('C0231D0B-93E8-45C5-9899-532923814C8C','Imported Batches','Imported Batches','#/view/Integration/Views/DataSourceImportedBatchManagement','551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Integration/DataSourceImportedBatch/GetFilteredDataSourceImportedBatches',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',3)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
--------------------------------------------------------------------------------------------------------------

END


--[sec].[BusinessEntity]-------------------901 to 1200--------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('CCD65B5B-53BB-4816-B7EA-D8DC58AA513E','VR_Integration_DataProcesses','Data Processes','B6B8F582-4759-43FB-9220-AA7662C366EA',0,'["View Data Sources","Add Data Sources","Edit Data Sources","View Logs","View Reprocess Logs","Start Reprocess", "Manage Import Process"]')
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

--[runtime].[SchedulerTaskActionType]---------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ActionTypeInfo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','Data Source','{"URL":"","SystemType":true,"FQTN":"Vanrise.Integration.Business.DSSchedulerTaskAction, Vanrise.Integration.Business","Security":{"$type":"Vanrise.Runtime.Entities.ActionTypeInfoSecurity, Vanrise.Runtime.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":["View Data Sources"]}]},"ConfigurePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":["Edit Data Sources"]}]},"RunPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"CCD65B5B-53BB-4816-B7EA-D8DC58AA513E","PermissionOptions":["Edit Data Sources"]}]}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ActionTypeInfo]))
merge	[runtime].[SchedulerTaskActionType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ActionTypeInfo] = s.[ActionTypeInfo]
when not matched by target then
	insert([ID],[Name],[ActionTypeInfo])
	values(s.[ID],s.[Name],s.[ActionTypeInfo]);


--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('E522907C-AEBD-48B6-82F4-FE55238942F2','VR_Integration_DataSource','Data Source','{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-integration-datasource-selector","GroupSelectorUIControl":"","ManagerFQTN":"Vanrise.Integration.Business.DataSourceManager,Vanrise.Integration.Business","IdType":"System.Guid"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);
----------------------------------------------------------------------------------------------------
end

--[logging].[LoggableEntity]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[UniqueName],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3BD49160-FBB8-41B6-951D-CD7024CC2DB9','VR_Integration_DataSource','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Integration_DataSource_ViewHistoryItem"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[UniqueName],[Settings]))
merge	[logging].[LoggableEntity] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[UniqueName],[Settings])
	values(s.[ID],s.[UniqueName],s.[Settings]);

--common.[VRObjectTypeDefinition]-------------------------------------------------------------
----------------------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('69B266CE-E309-426F-B672-B0E61D3211FC','Data Source Failed Batch Info','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoObjectType, Vanrise.Integration.MainExtensions","ConfigId":"ea1f0775-08b9-4638-ac42-a2fd11e008ef"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Message":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Message","Description":"Message","PropertyEvaluator":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoPropertyEvaluator, Vanrise.Integration.MainExtensions","FailedBatchInfoField":0,"ConfigId":"708f5f03-f14e-4487-b916-5617013e8b3e"}},"IsEmpty":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"IsEmpty","Description":"Is Empty","PropertyEvaluator":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoPropertyEvaluator, Vanrise.Integration.MainExtensions","FailedBatchInfoField":1,"ConfigId":"708f5f03-f14e-4487-b916-5617013e8b3e"}},"DataSourceName":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"DataSourceName","Description":"Data Source Name","PropertyEvaluator":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoPropertyEvaluator, Vanrise.Integration.MainExtensions","FailedBatchInfoField":4,"ConfigId":"708f5f03-f14e-4487-b916-5617013e8b3e"}},"DataSourceId":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"DataSourceId","Description":"Data Source Id","PropertyEvaluator":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoPropertyEvaluator, Vanrise.Integration.MainExtensions","FailedBatchInfoField":3,"ConfigId":"708f5f03-f14e-4487-b916-5617013e8b3e"}},"BatchDescription":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"BatchDescription","Description":"Batch Description","PropertyEvaluator":{"$type":"Vanrise.Integration.MainExtensions.FailedBatchInfoPropertyEvaluator, Vanrise.Integration.MainExtensions","FailedBatchInfoField":2,"ConfigId":"708f5f03-f14e-4487-b916-5617013e8b3e"}}}}'),
('601A8EA4-29E5-43AA-839F-96D00467FD37','Data Source Summary','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordObjectType, Vanrise.GenericData.MainExtensions","ConfigId":"bbc57155-0412-4371-83e5-1917a8bea468","RecordTypeId":"3bd1fed8-44c7-4f33-93fd-d8276fbe07ad"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Data Source":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Data Source","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"DataSource","UseDescription":true}},"Last Hour Max Batch Time":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last Hour Max Batch Time","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"LastHourMaxBatchTime","UseDescription":false}},"Last Hour Nb Batches":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last Hour Nb Batches","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"LastHourNbBatches","UseDescription":false}},"Last Hour Total Record Count":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last Hour Total Record Count","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"LastHourTotalRecordCount","UseDescription":false}},"Last Hour Max Batch Record Count":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last Hour Max Batch Record Count","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"LastHourMaxBatchRecordCount","UseDescription":false}},"Last Hour Min Batch Record Count":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last Hour Min Batch Record Count","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"LastHourMinBatchRecordCount","UseDescription":false}},"Last Hour Max Batch Size":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last Hour Max Batch Size","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"LastHourMaxBatchSize","UseDescription":false}},"Last Hour Min Batch Size":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last Hour Min Batch Size","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"LastHourMinBatchSize","UseDescription":false}},"Last Hour Nb Invalid Batches":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last Hour Nb Invalid Batches","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"LastHourNbInvalidBatches","UseDescription":false}},"Last Hour Nb Empty Batches":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last Hour Nb Empty Batches","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"LastHourNbEmptyBatches","UseDescription":false}},"Last Hour Nb Of Mins Since Last Batch":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last Hour Nb Of Mins Since Last Batch","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"LastHourNbOfMinutesSinceLastBatch","UseDescription":false}},"Last 24 hrs Max Batch Time":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last 24 hrs Max Batch Time","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Last24HoursMaxBatchTime","UseDescription":false}},"Last 24 hrs Nb Batches":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last 24 hrs Nb Batches","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Last24HoursNbBatches","UseDescription":false}},"Last 24 hrs Total Record Count":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last 24 hrs Total Record Count","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Last24HoursTotalRecordCount","UseDescription":false}},"Last 24 hrs Max Batch Record Count":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last 24 hrs Max Batch Record Count","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Last24HoursMaxBatchRecordCount","UseDescription":false}},"Last 24 hrs Min Batch Record Count":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last 24 hrs Min Batch Record Count","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Last24HoursMinBatchRecordCount","UseDescription":false}},"Last 24 hrs Max Batch Size":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last 24 hrs Max Batch Size","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Last24HoursMaxBatchSize","UseDescription":false}},"Last 24 hrs Min Batch Size":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last 24 hrs Min Batch Size","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Last24HoursMinBatchSize","UseDescription":false}},"Last 24 hrs Nb Invalid Batches":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last 24 hrs Nb Invalid Batches","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Last24HoursNbInvalidBatches","UseDescription":false}},"Last 24 hrs Nb Empty Batches":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last 24 hrs Nb Empty Batches","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Last24HoursNbEmptyBatches","UseDescription":false}},"Last 24 hrs Nb Of Minutes Since Last Batch":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Last 24 hrs Nb Of Minutes Since Last Batch","PropertyEvaluator":{"$type":"Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions","ConfigId":"f663bf74-99db-4746-8cbc-e74198e1786c","FieldName":"Last24HoursNbOfMinutesSinceLastBatch","UseDescription":false}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[VRObjectTypeDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);
----------------------------------------------------------------------------------------------------
end

--[common].[MailMessageType]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('0F64DA0B-E2D0-4421-BEB9-32C6E749F8F1','Data Source Failed Batch','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Failed Batch Info":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Failed Batch Info","VRObjectTypeDefinitionId":"69b266ce-e309-426f-b672-b0e61d3211fc"},"User":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"User","VRObjectTypeDefinitionId":"e3887cc9-1fbb-44d1-b1e3-7a0922400550"}}}'),
('35EE68DE-BD17-4F66-8452-A8F0B7B06306','Data Source Summary','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Data Source Summary":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Data Source Summary","VRObjectTypeDefinitionId":"601a8ea4-29e5-43aa-839f-96d00467fd37"}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[MailMessageType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);



--[common].[MailMessageTemplate]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[MessageTypeID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('8CDB71F5-02AB-442E-8E23-8ED3FD46446A','Data Source Error(Default)','0F64DA0B-E2D0-4421-BEB9-32C6E749F8F1','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","From":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"support@vanrise.com"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"User\",\"Email\")"},"BCC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Failed Batch Info\",\"DataSourceName\") Error"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Dears,<div>Data Source&nbsp;<span style=\"background-color: rgb(215, 235, 238); color: rgb(35, 82, 124); font-family: &quot;Open Sans&quot;; font-size: 11px; text-decoration-line: underline; white-space: nowrap;\">@Model.GetVal(\"Failed Batch Info\",\"DataSourceName\")</span>&nbsp;raised the below error.</div><div>Data Source Id:<a ng-class=\"::dataItem.columnsValues.col_4.cellClass\" ng-click=\"$parent.ctrl.onColumnClicked($parent.ctrl.columnDefs[3], dataItem , $event)\" class=\"ng-binding span-summary\" style=\"box-sizing: border-box; background-color: rgb(215, 235, 238); color: rgb(35, 82, 124); text-decoration-line: underline; font-size: 11px; outline: 0px; font-family: &quot;Open Sans&quot;; white-space: nowrap; cursor: pointer;\">&nbsp;@Model.GetVal(\"Failed Batch Info\",\"DataSourceId\")</a></div><div>Batch:&nbsp;<a ng-class=\"::dataItem.columnsValues.col_4.cellClass\" ng-click=\"$parent.ctrl.onColumnClicked($parent.ctrl.columnDefs[3], dataItem , $event)\" class=\"ng-binding span-summary\" style=\"box-sizing: border-box; background-color: rgb(215, 235, 238); color: rgb(35, 82, 124); text-decoration-line: underline; font-size: 11px; outline: 0px; font-family: &quot;Open Sans&quot;; white-space: nowrap; cursor: pointer;\">@Model.GetVal(\"Failed Batch Info\",\"BatchDescription\")</a></div><div>Message:&nbsp;<span style=\"background-color: rgb(215, 235, 238); color: rgb(35, 82, 124); font-family: &quot;Open Sans&quot;; font-size: 11px; text-decoration-line: underline; white-space: nowrap;\">@Model.GetVal(\"Failed Batch Info\",\"Message\")</span></div>"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[MessageTypeID],[Settings]))
merge	[common].[MailMessageTemplate] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[MessageTypeID] = s.[MessageTypeID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[MessageTypeID],[Settings])
	values(s.[ID],s.[Name],s.[MessageTypeID],s.[Settings]);


--[genericdata].[DataRecordType]--------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3BD1FED8-44C7-4F33-93FD-D8276FBE07AD','DataSourceSummary',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DataSource","Title":"DataSource","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"e522907c-aebd-48b6-82f4-fe55238942f2","IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastHourMaxBatchTime","Title":"Last Hour Max Batch Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","RuntimeEditor":"vr-genericdata-fieldtype-datetime-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-datetime-viewereditor","DataType":0,"IsNullable":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastHourNbBatches","Title":"Last Hour Nb Batches","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastHourTotalRecordCount","Title":"Last Hour Total Record Count","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastHourMaxBatchRecordCount","Title":"Last Hour Max Batch Record Count","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastHourMinBatchRecordCount","Title":"Last Hour Min Batch Record Count","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastHourMaxBatchSize","Title":"Last Hour Max Batch Size","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":0,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastHourMinBatchSize","Title":"Last Hour Min Batch Size","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":0,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastHourNbInvalidBatches","Title":"Last Hour Nb Invalid Batches","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastHourNbEmptyBatches","Title":"Last Hour Nb Empty Batches","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LastHourNbOfMinutesSinceLastBatch","Title":"Last Hour Nb Of Mins Since Last Batch","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":0,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Last24HoursMaxBatchTime","Title":"Last 24 hrs Max Batch Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","RuntimeEditor":"vr-genericdata-fieldtype-datetime-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-datetime-viewereditor","DataType":0,"IsNullable":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Last24HoursNbBatches","Title":"Last 24 hrs Nb Batches","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Last24HoursTotalRecordCount","Title":"Last 24 hrs Total Record Count","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Last24HoursMaxBatchRecordCount","Title":"Last 24 hrs Max Batch Record Count","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Last24HoursMinBatchRecordCount","Title":"Last 24 hrs Min Batch Record Count","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Last24HoursMaxBatchSize","Title":"Last 24 hrs Max Batch Size","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":0,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Last24HoursMinBatchSize","Title":"Last 24 hrs Min Batch Size","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":0,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Last24HoursNbInvalidBatches","Title":"Last 24 hrs Nb Invalid Batches","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Last24HoursNbEmptyBatches","Title":"Last 24 hrs Nb Empty Batches","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Last24HoursNbOfMinutesSinceLastBatch","Title":"Last 24 hrs Nb Of Minutes Since Last Batch","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":0,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false}]}',NULL,'{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities","DateTimeField":"LastHourImportedBatchTime"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings]))
merge	[genericdata].[datarecordtype] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentID] = s.[ParentID],[Fields] = s.[Fields],[ExtraFieldsEvaluator] = s.[ExtraFieldsEvaluator],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
	values(s.[ID],s.[Name],s.[ParentID],s.[Fields],s.[ExtraFieldsEvaluator],s.[Settings]);
----------------------------------------------------------------------------------------------------
end


--[genericdata].[DataRecordStorage]-----------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D1B13442-463F-4572-88C3-BA52BE1C53F4','Data Source Business Storage','3BD1FED8-44C7-4F33-93FD-D8276FBE07AD','D76E65B5-21BE-4C0B-92AF-D38FEAB10C68','{"$type":"Vanrise.GenericData.Business.BusinessObjectDataRecordStorageSettings, Vanrise.GenericData.Business","Settings":{"$type":"Vanrise.GenericData.Business.BusinessObjectDataProviderSettings, Vanrise.GenericData.Business","ExtendedSettings":{"$type":"Vanrise.Integration.MainExtensions.BusinessObjectDataStore.DataSourceBusinessObjectDataProviderSettings, Vanrise.Integration.MainExtensions","ConfigId":"345363b4-3d89-45ab-85d1-687674c77dfc","DoesSupportFilterOnAllFields":false}},"DateTimeField":"LastHourImportedBatchTime","EnableUseCaching":false,"RequiredLimitResult":false,"DontReflectToDB":false,"DenyAPICall":false,"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"FieldsPermissions":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordStorageFieldsPermission, Vanrise.GenericData.Entities]], mscorlib","$values":[]}}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State]))
merge	[genericdata].[DataRecordStorage] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DataRecordTypeID] = s.[DataRecordTypeID],[DataStoreID] = s.[DataStoreID],[Settings] = s.[Settings],[State] = s.[State]
when not matched by target then
	insert([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
	values(s.[ID],s.[Name],s.[DataRecordTypeID],s.[DataStoreID],s.[Settings],s.[State]);
----------------------------------------------------------------------------------------------------
end

--[common].[VRComponentType]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('0A6EE0DA-DE15-47FB-9C1A-4C252DD5FB6C','Data Source Summary Evaluator','AA969BAD-225D-4D83-B76C-68BFDBC0F045','{"$type":"Vanrise.GenericData.Entities.DataRecordRuleEvaluatorDefinitionSettings, Vanrise.GenericData.Entities","VRComponentTypeConfigId":"aa969bad-225d-4d83-b76c-68bfdbc0f045","DataRecordStorageIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["d1b13442-463f-4572-88c3-ba52be1c53f4"]},"AlertRuleTypeId":"84d60857-53a6-4e91-979e-2ede68063268","AreDatesHardCoded":true,"Security":{"$type":"Vanrise.GenericData.Entities.DataRecordRuleEvaluatorDefinitionSecurity, Vanrise.GenericData.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"StartInstancePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}}'),
('7364C3E5-1138-4DCF-9DDB-70BEBF246B67','Data Source Summary Notification','FDD73530-067F-4160-AB71-7852303C785C','{"$type":"Vanrise.Notification.Entities.VRNotificationTypeSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"fdd73530-067f-4160-ab71-7852303c785c","VRAlertLevelDefinitionId":"48fc148c-299a-4717-bd03-401bb79c082e","ExtendedSettings":{"$type":"Vanrise.GenericData.Notification.DataRecordNotificationTypeSettings, Vanrise.GenericData.Notification","ConfigId":"e64c51a2-08e0-4b7d-96f0-9ff1848a72fa","SearchRuntimeEditor":"vr-genericdata-datarecordnotificationtypesettings-searcheditor","BodyRuntimeEditor":"vr-genericdata-datarecordnotificationtypesettings-bodyeditor","DataRecordTypeId":"3bd1fed8-44c7-4f33-93fd-d8276fbe07ad","GridColumnDefinitions":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"DataSource","Header":"DataSource"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"LastHourMaxBatchTime","Header":"Last Hour Max Batch Time"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"LastHourNbBatches","Header":"Last Hour Nb Batches"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"LastHourTotalRecordCount","Header":"Last Hour Total Record Count"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"LastHourMaxBatchRecordCount","Header":"Last Hour Max Batch Record Count"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"LastHourMinBatchRecordCount","Header":"Last Hour Min Batch Record Count"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"LastHourMaxBatchSize","Header":"Last Hour Max Batch Size"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"LastHourMinBatchSize","Header":"Last Hour Min Batch Size"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"LastHourNbInvalidBatches","Header":"Last Hour Nb Invalid Batches"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"LastHourNbEmptyBatches","Header":"Last Hour Nb Empty Batches"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"LastHourNbOfMinutesSinceLastBatch","Header":"Last Hour Nb Of Mins Since Last Batch"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Last24HoursMaxBatchTime","Header":"Last 24 hrs Max Batch Time"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Last24HoursNbBatches","Header":"Last 24 hrs Nb Batches"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Last24HoursTotalRecordCount","Header":"Last 24 hrs Total Record Count"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Last24HoursMaxBatchRecordCount","Header":"Last 24 hrs Max Batch Record Count"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Last24HoursMinBatchRecordCount","Header":"Last 24 hrs Min Batch Record Count"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Last24HoursMaxBatchSize","Header":"Last 24 hrs Max Batch Size"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Last24HoursMinBatchSize","Header":"Last 24 hrs Min Batch Size"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Last24HoursNbInvalidBatches","Header":"Last 24 hrs Nb Invalid Batches"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Last24HoursNbEmptyBatches","Header":"Last 24 hrs Nb Empty Batches"},{"$type":"Vanrise.GenericData.Notification.NotificationGridColumnDefinition, Vanrise.GenericData.Notification","FieldName":"Last24HoursNbOfMinutesSinceLastBatch","Header":"Last 24 hrs Nb Of Minutes Since Last Batch"}]}},"Security":{"$type":"Vanrise.Notification.Entities.VRNotificationTypeSecurity, Vanrise.Notification.Entities","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"ccd65b5b-53bb-4816-b7ea-d8dc58aa513e","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Data Sources"]}}]}},"HideRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}}'),
('C70C12B8-06A3-4CCB-A72F-5C83827C8D12','Send Email (Data Source Summary)','D96F17C8-29D7-4C0C-88DC-9D5FBCA2178F','{"$type":"Vanrise.Notification.Entities.VRActionDefinitionSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"d96f17c8-29d7-4c0c-88dc-9d5fbca2178f","ExtendedSettings":{"$type":"Vanrise.GenericData.Notification.DataRecordSendEmailDefinitionSettings, Vanrise.GenericData.Notification","ConfigId":"3b904e8c-2ac0-43db-a4ef-425869d40544","RuntimeEditor":"vr-genericdata-datarecord-vraction-sendemail","DataRecordTypeId":"3bd1fed8-44c7-4f33-93fd-d8276fbe07ad","MailMessageTypeId":"35ee68de-bd17-4f66-8452-a8f0b7b06306","DataRecordObjectName":"Data Source Summary","ObjectFieldMappings":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Notification.ObjectFieldMapping, Vanrise.GenericData.Notification]], mscorlib","$values":[]}}}')
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

--[VRNotification].[VRAlertRuleType]----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('84D60857-53A6-4E91-979E-2EDE68063268','Data Source Summary','{"$type":"Vanrise.GenericData.Notification.DataRecordAlertRuleTypeSettings, Vanrise.GenericData.Notification","ConfigId":"434f7b1e-8b93-4144-9150-e24bf3eb4efb","SettingEditor":"vr-genericdata-datarecordalertrule-extendedsettings","DataRecordTypeId":"3bd1fed8-44c7-4f33-93fd-d8276fbe07ad","IdentificationFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Notification.AlertRuleTypeRecordField, Vanrise.GenericData.Notification]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Notification.AlertRuleTypeRecordField, Vanrise.GenericData.Notification","Name":"DataSource"}]},"NotificationTypeId":"7364c3e5-1138-4dcf-9ddb-70bebf246b67","Security":{"$type":"Vanrise.Notification.Entities.AlertRuleTypeSecurity, Vanrise.Notification.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"ccd65b5b-53bb-4816-b7ea-d8dc58aa513e","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Data Sources"]}}]}},"AddPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"ccd65b5b-53bb-4816-b7ea-d8dc58aa513e","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Add Data Sources"]}}]}},"EditPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"ccd65b5b-53bb-4816-b7ea-d8dc58aa513e","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Edit Data Sources"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[VRNotification].[VRAlertRuleType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);