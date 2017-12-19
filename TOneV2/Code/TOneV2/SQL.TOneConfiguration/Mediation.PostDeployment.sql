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

--[common].[ExtensionConfiguration]---------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('F1D57186-49CE-4BF9-B4B6-46DDCE93E9EC','Mediation_MediationOutputHandler_OtherMediation','Other Mediation Handler','Mediation_MediationOutputHandler','{"Editor":"mediation-outputhandler-othermediation-editor"}'),('14F0A218-77B5-4417-AB80-6A9386A7BB49','Mediation_MediationOutputHandler_StoreRecords','Store Records Handler','Mediation_MediationOutputHandler','{"Editor":"mediation-outputhandler-storerecords-editor"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[ExtensionConfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);


GO--delete useless views from ClearVoice product such 'My Scheduler Service', 'Style Definitions', 'Organizational Charts', Lookups, etc...
delete from [sec].[View] where [Id] in ('C65ED28A-36D0-4047-BEC5-030D35B02308','66DE2441-8A96-41E7-94EA-9F8AF38A3515','604B2CB5-B839-4E51-8D13-3C1C84D05DEE','DCF8CA21-852C-41B9-9101-6990E545509D','A1CE55FE-6CF4-4F15-9BC2-8E1F8DF68561',
										'25994374-CB99-475B-8047-3CDB7474A083','9F691B87-4936-4C4C-A757-4B3E12F7E1D9', 'E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9', '0F111ADC-B7F6-46A4-81BC-72FFDEB305EB',
										'4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712','2D39B12D-8FBF-4D4E-B2A5-5E3FE57580DF'--,'Locked Sessions'
										)
GO
--[sec].[View]------------------------17001 to 18000------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7079BD63-BFE2-4519-9B1B-8158A2F3A12A','System Logs','System Logs',null,'BAAF681E-AB1C-4A64-9A35-3F3951398881',null,null,null,'{"$type":"Vanrise.Common.Business.MasterLogViewSettings, Vanrise.Common.Business","Items":[{"PermissionName":"VRCommon_System_Log: View General Logs","Directive":"vr-log-entry-search","Title":"General"},{"PermissionName":"VR_Integration_DataProcesses: View Logs","Directive":"vr-integration-log-search","Title":"Data Source"},{"PermissionName":"VR_Integration_DataProcesses: View Logs","Directive":"vr-integration-importedbatch-search","Title":"Imported Batch"},{"PermissionName":"VR_Integration_DataProcesses: View Logs", "Directive":"bp-instance-log-search","Title":"Business Process"},{"PermissionName":"VRCommon_System_Log: View Action Audit","Directive":"vr-common-actionaudit-search","Title":"Action Audit"}]}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',15),
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
('DAFC9E00-12C6-48E5-95DB-F2FA073D131A','Mediation Store','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataStoreSettings, Vanrise.GenericData.SQLDataStorage","ConfigId":"2aeec2de-ec44-4698-aaef-8e9dbf669d1e","ConnectionStringName":"Mediation_CDR_DBConnString","IsRemoteDataStore":false}')
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

--[genericdata].[DataRecordStorage]-----------201 to 300--------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('0B1837DF-C8CE-4B2A-B07E-1A9F75408741','Cooked CDR Record Storage','EAE69ABE-1E94-460C-8B6E-77DBC7DAB1D6','DAFC9E00-12C6-48E5-95DB-F2FA073D131A','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageSettings, Vanrise.GenericData.SQLDataStorage","TableName":"CookedCDR","TableSchema":"Mediation_Centrex","Columns":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CallId","SQLDataType":"nvarchar(500)","ValueExpression":"CallId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ConnectDateTime","SQLDataType":"datetime","ValueExpression":"ConnectDateTime"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DisconnectDateTime","SQLDataType":"datetime","ValueExpression":"DiconnectDateTime"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DisconnectReason","SQLDataType":"varchar(200)","ValueExpression":"DisconnectReason"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CallProgressState","SQLDataType":"varchar(200)","ValueExpression":"CallProgressState"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"Account","SQLDataType":"varchar(200)","ValueExpression":"Account"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginatorId","SQLDataType":"varchar(500)","ValueExpression":"OriginatorId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginatorNumber","SQLDataType":"varchar(500)","ValueExpression":"OriginatorNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginatorFromNumber","SQLDataType":"varchar(500)","ValueExpression":"OriginatorFromNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginalDialedNumber","SQLDataType":"varchar(500)","ValueExpression":"OriginalDialedNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TerminatorId","SQLDataType":"varchar(500)","ValueExpression":"TerminatorId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TerminatorNumber","SQLDataType":"varchar(500)","ValueExpression":"TerminatorNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"IncomingGwId","SQLDataType":"varchar(200)","ValueExpression":"IncomingGwId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OutgoingGwId","SQLDataType":"varchar(200)","ValueExpression":"OutgoingGwId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TransferredCallId","SQLDataType":"varchar(200)","ValueExpression":"TransferredCallId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DurationInSeconds","SQLDataType":"decimal(20,4)","ValueExpression":"DurationInSeconds"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"AttemptDateTime","SQLDataType":"datetime","ValueExpression":"AttemptDateTime"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"SendCallType","SQLDataType":"tinyint","ValueExpression":"SendCallType"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ReveiveCallType","SQLDataType":"tinyint","ValueExpression":"ReceiveCallType"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"FileName","SQLDataType":"nvarchar(255)","ValueExpression":"FileName"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ReplacedCallId","SQLDataType":"nvarchar(255)","ValueExpression":"ReplacedCallId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginatorExtension","SQLDataType":"varchar(20)","ValueExpression":"OriginatorExtension"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TerminatorExtension","SQLDataType":"varchar(20)","ValueExpression":"TerminatorExtension"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginatorIp","SQLDataType":"varchar(100)","ValueExpression":"OriginatorIp"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TerminatorIp","SQLDataType":"varchar(100)","ValueExpression":"TerminatorIp"}]},"NullableFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.NullableField, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[]},"IncludeQueueItemId":false,"DateTimeField":"ConnectDateTime","RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}',null)
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
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('6309e517-3006-47bd-8eb4-8741feac673b','Mediation Staging Records Queue Activator', '{ "QueueActivatorConfigId": "6309e517-3006-47bd-8eb4-8741feac673b" , "Name": " Mediation Staging Records Queue Activator" ,"Title" : " Mediation Staging Records Queue Activator", "Editor" :"mediation-generic-queueactivator-storestagingrecords"}')
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

Delete from [common].[Setting] where ID = '4047054E-1CF4-4BE6-A005-6D4706757AD3'--,'Session Lock'
--[common].[Setting]---------------------------501 to 600-------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('509E467B-4562-4CA6-A32E-E50473B74D2C','Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Mediation","VersionNumber":"version #VersionNumber# ~ #VersionDate#"}}',1)
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


--[common].[Type]--------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------
declare @MediationRecordType varchar(250) = 'Mediation.Generic.Entities.MediationRecord, Mediation.Generic.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'

INSERT INTO [common].[Type] ([Type])
SELECT @MediationRecordType WHERE NOT EXISTS (SELECT NULL FROM [common].[Type] WHERE [Type] = @MediationRecordType)

