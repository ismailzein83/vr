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



GO--delete useless views from ClearVoice product such 'My Scheduler Service', 'Style Definitions', 'Organizational Charts', Lookups, etc...
delete from [sec].[View] where [Id] in ('C65ED28A-36D0-4047-BEC5-030D35B02308','66DE2441-8A96-41E7-94EA-9F8AF38A3515','604B2CB5-B839-4E51-8D13-3C1C84D05DEE','DCF8CA21-852C-41B9-9101-6990E545509D','25994374-CB99-475B-8047-3CDB7474A083','9F691B87-4936-4C4C-A757-4B3E12F7E1D9', 'E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9', '0F111ADC-B7F6-46A4-81BC-72FFDEB305EB', '4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712')
GO
--[sec].[View]------------------------17001 to 18000------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7079BD63-BFE2-4519-9B1B-8158A2F3A12A','Event Logs','Event Logs',null,'BAAF681E-AB1C-4A64-9A35-3F3951398881',null,null,null,'{"$type":"Vanrise.Common.Business.MasterLogViewSettings, Vanrise.Common.Business","Items":[{"PermissionName":"VRCommon_System_Log: View General Logs","Directive":"vr-log-entry-search","Title":"General"},{"PermissionName":"VR_Integration_DataProcesses: View Logs","Directive":"vr-integration-log-search","Title":"Data Source"},{"PermissionName":"VR_Integration_DataProcesses: View Logs","Directive":"vr-integration-importedbatch-search","Title":"Imported Batch"},{"PermissionName":"VR_Integration_DataProcesses: View Logs", "Directive":"bp-instance-log-search","Title":"Business Process"},{"PermissionName":"VRCommon_System_Log: View Action Audit","Directive":"vr-common-actionaudit-search","Title":"Action Audit"}]}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',15),
('AFD3C5EB-1745-4AEC-82DD-83FFBDE81900','Mediation Definitions','Mediation Definitions Management','#/view/Mediation_Generic/Views/MediationDefinition/MediationDefinitionManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','Mediation_Generic/MediationSettingDefinition/GetFilteredMediationSettingDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',60)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);

--[sec].[BusinessEntity]-------------------5101 to 5400-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3ADD847F-311E-43FF-917B-38EBB1152A4B','BusinessProcess_BP_Mediation','Mediation','04493174-83F0-44D6-BBE4-DBEB8B57875A',0,'["View", "StartInstance", "ScheduleTask"]')
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

--[bp].[BPDefinition]----------------------4001 to 5000------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('AAD944E5-AF00-480E-A318-23E835A123FF','Mediation.Generic.BP.Arguments.MediationProcessInput','Mediation','Mediation.Generic.BP.MediationProcess, Mediation.Generic.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"mediation-generic-mediationprocess-manual","ScheduledExecEditor":"mediation-generic-mediationprocess","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3ADD847F-311E-43FF-917B-38EBB1152A4B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3ADD847F-311E-43FF-917B-38EBB1152A4B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3ADD847F-311E-43FF-917B-38EBB1152A4B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}')
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

--[genericdata].[DataStore]---------------------201 to 300------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('DAFC9E00-12C6-48E5-95DB-F2FA073D131A','Mediation Store','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataStoreSettings, Vanrise.GenericData.SQLDataStorage","ConnectionStringName":"Mediation_GenericRecord_DBConnString","ConfigId":1}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[genericdata].[DataStore] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);



--[genericdata].[DataRecordType]----------------201 to 300------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;;with cte_data([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('1D7B5D3C-4AC6-42A3-A6E8-5A014E2EDF94','ParsedCDR',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_VERSIONID","Title":"TC_VERSIONID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_LOGTYPE","Title":"TC_LOGTYPE","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_CALLID","Title":"TC_CALLID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_TIMESTAMP","Title":"TC_TIMESTAMP","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_DISCONNECTREASON","Title":"TC_DISCONNECTREASON","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_CALLPROGRESSSTATE","Title":"TC_CALLPROGRESSSTATE","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_ACCOUNT","Title":"TC_ACCOUNT","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_ORIGINATORID","Title":"TC_ORIGINATORID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_ORIGINATORNUMBER","Title":"TC_ORIGINATORNUMBER","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_ORIGINALFROMNUMBER","Title":"TC_ORIGINALFROMNUMBER","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_ORIGINALDIALEDNUMBER","Title":"TC_ORIGINALDIALEDNUMBER","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_TERMINATORID","Title":"TC_TERMINATORID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_TERMINATORNUMBER","Title":"TC_TERMINATORNUMBER","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_INCOMINGGWID","Title":"TC_INCOMINGGWID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_OUTGOINGGWID","Title":"TC_OUTGOINGGWID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_TRANSFERREDCALLID","Title":"TC_TRANSFERREDCALLID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_TERMINATORIP","Title":"TC_TERMINATORIP","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_ORIGINATORIP","Title":"TC_ORIGINATORIP","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}}]}',null,null),
('EAE69ABE-1E94-460C-8B6E-77DBC7DAB1D6','CookedCDR',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CallId","Title":"Call Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ConnectDateTime","Title":"Connect DateTime","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":0,"IsNullable":true,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DiconnectDateTime","Title":"Disconnect DateTime","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":0,"IsNullable":true,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DisconnectReason","Title":"Disconnect Reason","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CallProgressState","Title":"Call Progress State","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Account","Title":"Account","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OriginatorId","Title":"Orginator Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OriginatorNumber","Title":"Originator Number","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OriginatorFromNumber","Title":"Originator From Number","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OriginalDialedNumber","Title":"Original Dialed Number","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TerminatorId","Title":"Terminator Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TerminatorNumber","Title":"Terminator Number","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"IncomingGwId","Title":"Incoming Gw Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OutgoingGwId","Title":"Outgoing Gw Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TransferredCallId","Title":"Transferred Call Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DurationInSeconds","Title":"Duration In Seconds","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","DataPrecision":0,"DataType":0,"IsNullable":false,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OriginatorIp","Title":"Originator Ip","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TerminatorIp","Title":"Terminator Ip","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","OrderType":1}}]}',null,null)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings]))merge	[genericdata].[datarecordtype] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[ParentID] = s.[ParentID],[Fields] = s.[Fields],[ExtraFieldsEvaluator] = s.[ExtraFieldsEvaluator],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])	values(s.[ID],s.[Name],s.[ParentID],s.[Fields],s.[ExtraFieldsEvaluator],s.[Settings]);

--[genericdata].[DataRecordStorage]-----------201 to 300--------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('0B1837DF-C8CE-4B2A-B07E-1A9F75408741','Cooked CDR Record Storage','EAE69ABE-1E94-460C-8B6E-77DBC7DAB1D6','DAFC9E00-12C6-48E5-95DB-F2FA073D131A','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageSettings, Vanrise.GenericData.SQLDataStorage","TableName":"CookedCDR","TableSchema":"Mediation_Generic","Columns":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CallId","SQLDataType":"nvarchar(100)","ValueExpression":"CallId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ConnectDateTime","SQLDataType":"datetime","ValueExpression":"ConnectDateTime"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DisconnectDateTime","SQLDataType":"datetime","ValueExpression":"DiconnectDateTime"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DisconnectReason","SQLDataType":"nvarchar(100)","ValueExpression":"DisconnectReason"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CallProgressState","SQLDataType":"nvarchar(100)","ValueExpression":"CallProgressState"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"Account","SQLDataType":"nvarchar(100)","ValueExpression":"Account"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginatorId","SQLDataType":"nvarchar(100)","ValueExpression":"OriginatorId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginatorNumber","SQLDataType":"nvarchar(100)","ValueExpression":"OriginatorNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginatorFromNumber","SQLDataType":"nvarchar(100)","ValueExpression":"OriginatorFromNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginalDialedNumber","SQLDataType":"nvarchar(100)","ValueExpression":"OriginalDialedNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TerminatorId","SQLDataType":"nvarchar(100)","ValueExpression":"TerminatorId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TerminatorNumber","SQLDataType":"nvarchar(100)","ValueExpression":"TerminatorNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"IncomingGwId","SQLDataType":"nvarchar(100)","ValueExpression":"IncomingGwId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OutgoingGwId","SQLDataType":"nvarchar(100)","ValueExpression":"OutgoingGwId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TransferredCallId","SQLDataType":"nvarchar(100)","ValueExpression":"TransferredCallId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DurationInSeconds","SQLDataType":"decimal(10,4)","ValueExpression":"DurationInSeconds"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"AttemptDateTime","SQLDataType":"datetime","ValueExpression":"AttemptDateTime"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"SendCallType","SQLDataType":"tinyint","ValueExpression":"SendCallType"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ReveiveCallType","SQLDataType":"tinyint","ValueExpression":"ReceiveCallType"}]},"NullableFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.NullableField, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[]},"IncludeQueueItemId":false,"DateTimeField":"ConnectDateTime","RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}',null)
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

--[genericdata].[DataTransformationDefinition]-------1001 to 2000-----------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('C8B4BC22-65B7-447D-ACA0-633F14C175D4','ParsedToCookedTransformation','Parsed To Cooked Transformation','{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationDefinition, Vanrise.GenericData.Transformation.Entities","DataTransformationDefinitionId":"c8b4bc22-65b7-447d-aca0-633f14c175d4","Name":"ParsedToCookedTransformation","Title":"Parsed To Cooked Transformation","RecordTypes":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities","RecordName":"parsedCDRs","DataRecordTypeId":"1d7b5d3c-4ac6-42a3-a6e8-5a014e2edf94","IsArray":true},{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities","RecordName":"cookedCDR","DataRecordTypeId":"eae69abe-1e94-460c-8b6e-77dbc7dab1d6","IsArray":false}]},"MappingSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.ForLoopStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"d7ce9698-2721-467e-9820-1ed44b446d0d","Steps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.IfElseStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"9cf3c165-1921-4f83-990d-03b82a04aa5a","Condition":"parsedCDRItem.TC_LOGTYPE ==  \"START\"","ThenSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_TIMESTAMP","Target":"cookedCDR.ConnectDateTime"}]},"ElseSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.IfElseStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"9cf3c165-1921-4f83-990d-03b82a04aa5a","Condition":"parsedCDRItem.TC_LOGTYPE == \"STOP\"","ThenSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_TIMESTAMP","Target":"cookedCDR.DiconnectDateTime"}]},"ElseSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[]}}]}},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_DISCONNECTREASON","Target":"cookedCDR.DisconnectReason"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_ORIGINATORID","Target":"cookedCDR.OriginatorId"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_ORIGINATORNUMBER","Target":"cookedCDR.OriginatorNumber"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_ORIGINALFROMNUMBER","Target":"cookedCDR.OriginatorFromNumber"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_TERMINATORID","Target":"cookedCDR.TerminatorId"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_TERMINATORNUMBER","Target":"cookedCDR.TerminatorNumber"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_ACCOUNT","Target":"cookedCDR.Account"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_INCOMINGGWID","Target":"cookedCDR.IncomingGwId"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_TRANSFERREDCALLID","Target":"cookedCDR.TransferredCallId"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_CALLID","Target":"cookedCDR.CallId"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_TERMINATORIP","Target":"cookedCDR.TerminatorIp"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"parsedCDRItem.TC_TERMINATORIP","Target":"cookedCDR.OriginatorIp"}]},"ArrayVariableName":"parsedCDRs","IterationVariableName":"parsedCDRItem"},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.IfElseStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"9cf3c165-1921-4f83-990d-03b82a04aa5a","Condition":"cookedCDR.ConnectDateTime.HasValue && cookedCDR.DiconnectDateTime.HasValue","ThenSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"(decimal)(cookedCDR.DiconnectDateTime.Value - cookedCDR.ConnectDateTime.Value).TotalSeconds","Target":"cookedCDR.DurationInSeconds"}]},"ElseSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"00e8e50c-017e-44ed-96a9-6d4291a9c4b6","Source":"0","Target":"cookedCDR.DurationInSeconds"}]}}]}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Details]))
merge	[genericdata].[DataTransformationDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Title],[Details])
	values(s.[ID],s.[Name],s.[Title],s.[Details]);

--[queue].[QueueActivatorConfig]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [queue].[QueueActivatorConfig] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(501,'Mediation Staging Records Queue Activator', '{ "QueueActivatorConfigId": "501" , "Name": " Mediation Staging Records Queue Activator" ,"Title" : " Mediation Staging Records Queue Activator", "Editor" :"mediation-generic-queueactivator-storestagingrecords"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge   [queue].[QueueActivatorConfig] as t
using     cte_data as s
on     1=1 and t.[ID] = s.[ID]
when matched then
    update set
    [Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
    insert([ID],[Name],[Details])
    values(s.[ID],s.[Name],s.[Details]);
set identity_insert [queue].[QueueActivatorConfig] off;

--[common].[Setting]---------------------------501 to 600-------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('509E467B-4562-4CA6-A32E-E50473B74D2C','Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Mediation","VersionNumber":"version 0.9 ~ 2017-04-28"}}',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);


--[queue].[ExecutionFlowDefinition]---------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[ExecutionTree],[Stages])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('DCC30943-F1E3-44D2-81F0-4AB49EADFD31','MediationExecutionFlow', 'Mediation Execution Flow', NULL, '{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Mediation Store Batch","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":"1d7b5d3c-4ac6-42a3-a6e8-5a014e2edf94","BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Mediation.Generic.QueueActivators.StoreStagingRecordsQueueActivator, Mediation.Generic.QueueActivators","MediationDefinitionId":1,"ConfigId":501}}]}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ExecutionTree],[Stages]))
merge	[queue].[ExecutionFlowDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ExecutionTree] = s.[ExecutionTree],[Stages] = s.[Stages]
when not matched by target then
	insert([ID],[Name],[Title],[ExecutionTree],[Stages])
	values(s.[ID],s.[Name],s.[Title],s.[ExecutionTree],s.[Stages]);


--[queue].[ExecutionFlow]-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;

;with cte_data([ID],[Name],[ExecutionFlowDefinitionID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('8F27D74A-F254-4E98-AE43-05AE61566484','Mediation Flow','DCC30943-F1E3-44D2-81F0-4AB49EADFD31')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ExecutionFlowDefinitionID]))
merge	[queue].[ExecutionFlow] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ExecutionFlowDefinitionID] = s.[ExecutionFlowDefinitionID]
when not matched by target then
	insert([ID],[Name],[ExecutionFlowDefinitionID])
	values(s.[ID],s.[Name],s.[ExecutionFlowDefinitionID]);

--[runtime].[ScheduleTask]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('6D3AB9F6-DF06-4DA1-814A-D40CF4A6A558','Mediation Data Source Task', 0, 0, '295B4FAC-DBF9-456F-855E-60D0B176F86B', 'B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68', '{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"},"StartEffDate":"2016-06-12T10:35:00+03:00"}', 1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId]))
merge	[runtime].[ScheduleTask] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[IsEnabled] = s.[IsEnabled],[TaskType] = s.[TaskType],[TriggerTypeId] = s.[TriggerTypeId],[ActionTypeId] = s.[ActionTypeId],[TaskSettings] = s.[TaskSettings],[OwnerId] = s.[OwnerId]
when not matched by target then
	insert([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
	values(s.[ID],s.[Name],s.[IsEnabled],s.[TaskType],s.[TriggerTypeId],s.[ActionTypeId],s.[TaskSettings],s.[OwnerId]);



--[integration].[DataSource]----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;

;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3643F87B-8C5F-4C8F-BB39-3218198FD8C1','File Data Source - MultiNet','396A4933-DF4F-49CD-9799-BF605B9F4597', null,'6D3AB9F6-DF06-4DA1-814A-D40CF4A6A558', '{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".txt","Mask":"","Directory":"/MultiNet/CDRs","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","DirectorytoMoveFile":"/MultiNet/Processed","ActionAfterImport":2,"BasedOnLastModifiedTime":false,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\n            var cdrs = new List<dynamic>();\n            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();\n            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(\"ParsedCDR\");\n            try\n            {\n                System.IO.StreamReader sr = ImportedData.StreamReader;\n                while (!sr.EndOfStream)\n                {\n                    string currentLine = sr.ReadLine();\n                    if (string.IsNullOrEmpty(currentLine))\n                        continue;\n                    string[] rowData = currentLine.Split(',');\n                    dynamic cdr = Activator.CreateInstance(mediationCDRRuntimeType) as dynamic;\n                    cdr.TC_VERSIONID = rowData[0].Trim(''\"'');\n                    cdr.TC_CALLID = rowData[13].Trim(''\"'');\n                    cdr.TC_LOGTYPE = rowData[1].Trim(''\"'');\n                    cdr.TC_TIMESTAMP = DateTime.ParseExact(rowData[3].Trim(''\"''), \"yyyyMMddHHmmss:fff\", System.Globalization.CultureInfo.InvariantCulture);\n\n                    cdr.TC_DISCONNECTREASON = rowData[4].Trim(''\"'');\n                    cdr.TC_CALLPROGRESSSTATE = rowData[5].Trim(''\"'');\n                    cdr.TC_ACCOUNT = rowData[6].Trim(''\"'');\n                    cdr.TC_ORIGINATORID = rowData[7].Trim(''\"'');\n                    cdr.TC_ORIGINATORNUMBER = rowData[8].Trim(''\"'');\n                    cdr.TC_ORIGINALFROMNUMBER = rowData[9].Trim(''\"'');\n                    cdr.TC_ORIGINALDIALEDNUMBER = rowData[10].Trim(''\"'');\n                    cdr.TC_TERMINATORID = rowData[11].Trim(''\"'');\n                    cdr.TC_TERMINATORNUMBER = rowData[12].Trim(''\"'');\n                    cdr.TC_INCOMINGGWID = rowData[15].Trim(''\"'');\n                    cdr.TC_OUTGOINGGWID = rowData[16].Trim(''\"'');\n                    cdr.TC_TRANSFERREDCALLID = rowData[20].Trim(''\"'');\n                    cdr.TC_TERMINATORIP = rowData[33].Trim(''\"'');\n                    cdr.TC_ORIGINATORIP = rowData[32].Trim(''\"'');\n                    cdrs.Add(cdr);\n\n                }\n            }\n            catch (Exception ex)\n            {                \n                throw ex;\n            }\n\n            MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, \"#RECORDSCOUNT# of Raw CDRs\", \"ParsedCDR\");\n            mappedBatches.Add(\"Mediation Store Batch\", batch);\n            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            return result;","ExecutionFlowId":"8f27d74a-f254-4e98-ae43-05ae61566484"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings]))
merge	[integration].[DataSource] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[AdapterID] = s.[AdapterID],[AdapterState] = s.[AdapterState],[TaskId] = s.[TaskId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
	values(s.[ID],s.[Name],s.[AdapterID],s.[AdapterState],s.[TaskId],s.[Settings]);




