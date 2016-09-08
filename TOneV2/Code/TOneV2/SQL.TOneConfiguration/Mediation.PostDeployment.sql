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

--[sec].[View]------------------------17001 to 18000------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(17001,'Mediation Definitions', 'Mediation Definitions', '#/view/Mediation_Generic/Views/MediationDefinition/MediationDefinitionManagement', 3, 'Mediation_Generic/MediationSettingDefinition/GetFilteredMediationSettingDefinitions', NULL, NULL, NULL, 0, 200)

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

--[sec].[BusinessEntity]-------------------5101 to 5400-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(5101,'BusinessProcess_BP_Mediation','Mediation',602,0,'["View", "StartInstance", "ScheduleTask"]')
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

--[bp].[BPDefinition]----------------------4001 to 5000------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [bp].[BPDefinition] on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(4001,'Mediation.Generic.BP.Arguments.MediationProcessInput','Mediation','Mediation.Generic.BP.MediationProcess, Mediation.Generic.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"mediation-generic-mediationprocess-manual","ScheduledExecEditor":"mediation-generic-mediationprocess","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":5101,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":5101,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":5101,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}')
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
set identity_insert [bp].[BPDefinition] off;

--[genericdata].[DataStore]---------------------201 to 300------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [genericdata].[DataStore] on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(201,'Mediation Store', '{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataStoreSettings, Vanrise.GenericData.SQLDataStorage","ConnectionStringName":"Mediation_GenericRecord_DBConnString","ConfigId":1}')
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
set identity_insert [genericdata].[DataStore] off;



--[genericdata].[DataRecordType]----------------201 to 300------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [genericdata].[DataRecordType] on;
;with cte_data([ID],[Name],[ParentID],[Fields])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(201,'ParsedCDR', NULL, '{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_VERSIONID","Title":"TC_VERSIONID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_LOGTYPE","Title":"TC_LOGTYPE","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_CALLID","Title":"TC_CALLID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_TIMESTAMP","Title":"TC_TIMESTAMP","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","DataType":0,"IsNullable":false,"ConfigId":3,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_DISCONNECTREASON","Title":"TC_DISCONNECTREASON","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_CALLPROGRESSSTATE","Title":"TC_CALLPROGRESSSTATE","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_ACCOUNT","Title":"TC_ACCOUNT","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_ORIGINATORID","Title":"TC_ORIGINATORID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_ORIGINATORNUMBER","Title":"TC_ORIGINATORNUMBER","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_ORIGINALFROMNUMBER","Title":"TC_ORIGINALFROMNUMBER","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_ORIGINALDIALEDNUMBER","Title":"TC_ORIGINALDIALEDNUMBER","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_TERMINATORID","Title":"TC_TERMINATORID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_TERMINATORNUMBER","Title":"TC_TERMINATORNUMBER","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_INCOMINGGWID","Title":"TC_INCOMINGGWID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_OUTGOINGGWID","Title":"TC_OUTGOINGGWID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TC_TRANSFERREDCALLID","Title":"TC_TRANSFERREDCALLID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}}]}'),
(202,'CookedCDR', NULL, '{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CallId","Title":"Call Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ConnectDateTime","Title":"Connect DateTime","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","DataType":0,"IsNullable":true,"ConfigId":3,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DiconnectDateTime","Title":"Disconnect DateTime","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","DataType":0,"IsNullable":true,"ConfigId":3,"OrderType":0}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DisconnectReason","Title":"Disconnect Reason","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CallProgressState","Title":"Call Progress State","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Account","Title":"Account","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OriginatorId","Title":"Orginator Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OriginatorNumber","Title":"Originator Number","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OriginatorFromNumber","Title":"Originator From Number","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OriginalDialedNumber","Title":"Original Dialed Number","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TerminatorId","Title":"Terminator Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TerminatorNumber","Title":"Terminator Number","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"IncomingGwId","Title":"Incoming Gw Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"OutgoingGwId","Title":"Outgoing Gw Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TransferredCallId","Title":"Transferred Call Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","OrderType":1,"ConfigId":1}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DurationInSeconds","Title":"Duration In Seconds","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","DataPrecision":0,"DataType":0,"IsNullable":false,"ConfigId":2,"OrderType":0}}]}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentID],[Fields]))
merge	[genericdata].[DataRecordType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID] and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentID] = s.[ParentID],[Fields] = s.[Fields]
when not matched by target then
	insert([ID],[Name],[ParentID],[Fields])
	values(s.[ID],s.[Name],s.[ParentID],s.[Fields]);
set identity_insert [genericdata].[DataRecordType] off;


--[genericdata].[DataRecordStorage]-----------201 to 300--------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [genericdata].[DataRecordStorage] on;
;with cte_data([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(201,'Cooked CDR Record Storage', 202, 201, '{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageSettings, Vanrise.GenericData.SQLDataStorage","TableName":"CookedCDR","TableSchema":"Mediation_Generic","Columns":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CallId","SQLDataType":"nvarchar(100)","ValueExpression":"CallId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ConnectDateTime","SQLDataType":"datetime","ValueExpression":"ConnectDateTime"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DisconnectDateTime","SQLDataType":"datetime","ValueExpression":"DiconnectDateTime"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DisconnectReason","SQLDataType":"nvarchar(100)","ValueExpression":"DisconnectReason"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"CallProgressState","SQLDataType":"nvarchar(100)","ValueExpression":"CallProgressState"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"Account","SQLDataType":"nvarchar(100)","ValueExpression":"Account"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginatorId","SQLDataType":"nvarchar(100)","ValueExpression":"OriginatorId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginatorNumber","SQLDataType":"nvarchar(100)","ValueExpression":"OriginatorNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginatorFromNumber","SQLDataType":"nvarchar(100)","ValueExpression":"OriginatorFromNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OriginalDialedNumber","SQLDataType":"nvarchar(100)","ValueExpression":"OriginalDialedNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TerminatorId","SQLDataType":"nvarchar(100)","ValueExpression":"TerminatorId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TerminatorNumber","SQLDataType":"nvarchar(100)","ValueExpression":"TerminatorNumber"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"IncomingGwId","SQLDataType":"nvarchar(100)","ValueExpression":"IncomingGwId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"OutgoingGwId","SQLDataType":"nvarchar(100)","ValueExpression":"OutgoingGwId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TransferredCallId","SQLDataType":"nvarchar(100)","ValueExpression":"TransferredCallId"},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DurationInSeconds","SQLDataType":"decimal(10,4)","ValueExpression":"DurationInSeconds"}]},"DateTimeField":"ConnectDateTime"}', NULL)

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
set identity_insert [genericdata].[DataRecordStorage] off;

--[genericdata].[DataTransformationDefinition]-------1001 to 2000-----------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [genericdata].[DataTransformationDefinition] on;
;with cte_data([ID],[Name],[Title],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1001,'ParsedToCookedTransformation','Parsed To Cooked Transformation','{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationDefinition, Vanrise.GenericData.Transformation.Entities","DataTransformationDefinitionId":2,"Name":"ParsedToCookedTransformation","Title":"Parsed To Cooked Transformation","RecordTypes":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities","RecordName":"parsedCDRs","DataRecordTypeId":201,"IsArray":true},{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities","RecordName":"cookedCDR","DataRecordTypeId":202,"IsArray":false}]},"MappingSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.ForLoopStep, Vanrise.GenericData.Transformation.MainExtensions","Steps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.IfElseStep, Vanrise.GenericData.Transformation.MainExtensions","Condition":"parsedCDRItem.TC_LOGTYPE ==  \"START\"","ThenSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_TIMESTAMP","Target":"cookedCDR.ConnectDateTime","ConfigId":1}]},"ElseSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.IfElseStep, Vanrise.GenericData.Transformation.MainExtensions","Condition":"parsedCDRItem.TC_LOGTYPE == \"STOP\"","ThenSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_TIMESTAMP","Target":"cookedCDR.DiconnectDateTime","ConfigId":1}]},"ElseSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[]},"ConfigId":9}]},"ConfigId":9},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_DISCONNECTREASON","Target":"cookedCDR.DisconnectReason","ConfigId":1},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_ORIGINATORID","Target":"cookedCDR.OriginatorId","ConfigId":1},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_ORIGINATORNUMBER","Target":"cookedCDR.OriginatorNumber","ConfigId":1},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_ORIGINALFROMNUMBER","Target":"cookedCDR.OriginatorFromNumber","ConfigId":1},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_TERMINATORID","Target":"cookedCDR.TerminatorId","ConfigId":1},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_TERMINATORNUMBER","Target":"cookedCDR.TerminatorNumber","ConfigId":1},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_ACCOUNT","Target":"cookedCDR.Account","ConfigId":1},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_INCOMINGGWID","Target":"cookedCDR.IncomingGwId","ConfigId":1},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_TRANSFERREDCALLID","Target":"cookedCDR.TransferredCallId","ConfigId":1},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"parsedCDRItem.TC_CALLID","Target":"cookedCDR.CallId","ConfigId":1}]},"ArrayVariableName":"parsedCDRs","IterationVariableName":"parsedCDRItem","ConfigId":6},{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.IfElseStep, Vanrise.GenericData.Transformation.MainExtensions","Condition":"cookedCDR.ConnectDateTime.HasValue && cookedCDR.DiconnectDateTime.HasValue","ThenSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"(decimal)(cookedCDR.DiconnectDateTime.Value - cookedCDR.ConnectDateTime.Value).TotalSeconds","Target":"cookedCDR.DurationInSeconds","ConfigId":1}]},"ElseSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions","Source":"0","Target":"cookedCDR.DurationInSeconds","ConfigId":1}]},"ConfigId":9}]}}')
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
set identity_insert [genericdata].[DataTransformationDefinition] off;

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
set identity_insert [common].[Setting] on;
;with cte_data([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(501,'Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Mediation","VersionNumber":"version 0.9"}}',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[Id],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
set identity_insert [common].[Setting] off;