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

--[genericdata].[DataRecordType]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('C7583FF3-8870-4E1C-AB6E-EEC9BB5121E0','ICX_Alcatel_CDR',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Id","Title":"Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":2,"IsNullable":false,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DateOfCall","Title":"Date Of Call","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":2,"IsNullable":true,"ViewerEditor":"vr-genericdata-fieldtype-datetime-viewereditor","DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false,"CanRoundValue":false}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TimeOfCall","Title":"Time Of Call","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","DataType":1,"IsNullable":true,"ViewerEditor":"vr-genericdata-fieldtype-datetime-viewereditor","DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false,"CanRoundValue":false}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"DurationInSeconds","Title":"Duration In Seconds","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":1,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ANumber","Title":"ANumber","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","OrderType":1,"ViewerEditor":"vr-genericdata-fieldtype-text-viewereditor","DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"BNumber","Title":"BNumber","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","OrderType":1,"ViewerEditor":"vr-genericdata-fieldtype-text-viewereditor","DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false}},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"FileName","Title":"File Name","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","OrderType":1,"ViewerEditor":"vr-genericdata-fieldtype-text-viewereditor","DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false}}]}',null,'{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities","DateTimeField":"DateOfCall","IdField":"Id"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings]))
merge	[genericdata].[DataRecordType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentID] = s.[ParentID],[Fields] = s.[Fields],[ExtraFieldsEvaluator] = s.[ExtraFieldsEvaluator],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
	values(s.[ID],s.[Name],s.[ParentID],s.[Fields],s.[ExtraFieldsEvaluator],s.[Settings]);


--[genericdata].[DataRecordStorage]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('E0598BE2-B025-4772-89EF-61CC55006DFF','Alcatel CDR Record Storage','C7583FF3-8870-4E1C-AB6E-EEC9BB5121E0','DAFC9E00-12C6-48E5-95DB-F2FA073D131A','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageSettings, Vanrise.GenericData.SQLDataStorage","TableName":"AlcatelCDR","TableSchema":"Mediation_ICX","Columns":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"Id","SQLDataType":"bigint","ValueExpression":"Id","IsUnique":true},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DateOfCall","SQLDataType":"date","ValueExpression":"DateOfCall","IsUnique":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TimeOfCall","SQLDataType":"time(3)","ValueExpression":"TimeOfCall","IsUnique":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DurationInSeconds","SQLDataType":"int","ValueExpression":"DurationInSeconds","IsUnique":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ANumber","SQLDataType":"varchar(50)","ValueExpression":"ANumber","IsUnique":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"BNumber","SQLDataType":"varchar(50)","ValueExpression":"BNumber","IsUnique":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"FileName","SQLDataType":"varchar(255)","ValueExpression":"FileName","IsUnique":false}]},"NullableFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.NullableField, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[]},"IncludeQueueItemId":false,"DateTimeField":"DateOfCall","EnableUseCaching":false,"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}',null),
('41F39A89-BEB4-40FF-B580-3400B4EDB113','Alcatel Bad CDR Record Storage','C7583FF3-8870-4E1C-AB6E-EEC9BB5121E0','DAFC9E00-12C6-48E5-95DB-F2FA073D131A','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageSettings, Vanrise.GenericData.SQLDataStorage","TableName":"AlcatelBadCDR","TableSchema":"Mediation_ICX","Columns":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"Id","SQLDataType":"bigint","ValueExpression":"Id","IsUnique":true},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DateOfCall","SQLDataType":"date","ValueExpression":"DateOfCall","IsUnique":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"TimeOfCall","SQLDataType":"time(3)","ValueExpression":"TimeOfCall","IsUnique":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"DurationInSeconds","SQLDataType":"int","ValueExpression":"DurationInSeconds","IsUnique":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ANumber","SQLDataType":"varchar(50)","ValueExpression":"ANumber","IsUnique":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"BNumber","SQLDataType":"varchar(50)","ValueExpression":"BNumber","IsUnique":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"FileName","SQLDataType":"varchar(255)","ValueExpression":"FileName","IsUnique":false}]},"NullableFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.NullableField, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[]},"IncludeQueueItemId":false,"DateTimeField":"DateOfCall","EnableUseCaching":false,"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}',null)
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


--[queue].[ExecutionFlowDefinition]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[Title],[ExecutionTree],[Stages])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A4620B4F-BEC7-44CB-BFB8-3C336CC01631','Alcatel_Execution_Flow','Alcatel Execution Flow',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"AlcatelStorageStage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":"c7583ff3-8870-4e1c-ab6e-eec9bb5121e0","BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataRecordStorageId":"e0598be2-b025-4772-89ef-61cc55006dff","NbOfMaxConcurrentActivators":10,"ConfigId":"f6ced9a6-86c4-4fb9-b706-6e91d8f02908"},"IsSequential":false},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"AlcatelBadStorageStage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":"c7583ff3-8870-4e1c-ab6e-eec9bb5121e0","BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataRecordStorageId":"41f39a89-beb4-40ff-b580-3400b4edb113","NbOfMaxConcurrentActivators":10,"ConfigId":"f6ced9a6-86c4-4fb9-b706-6e91d8f02908"},"IsSequential":false},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"CDRTransformationStage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":"c7583ff3-8870-4e1c-ab6e-eec9bb5121e0","BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.TransformBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataTransformationDefinitionId":"46e3d96b-1189-4aef-8356-e1ec2eb48f1e","SourceRecordName":"CDRs","NextStagesRecords":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.QueueActivators.TransformBatchNextStageRecord, Vanrise.GenericData.QueueActivators]], mscorlib","$values":[{"$type":"Vanrise.GenericData.QueueActivators.TransformBatchNextStageRecord, Vanrise.GenericData.QueueActivators","RecordName":"CookedCDRs","NextStages":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["AlcatelStorageStage"]}},{"$type":"Vanrise.GenericData.QueueActivators.TransformBatchNextStageRecord, Vanrise.GenericData.QueueActivators","RecordName":"BadCDRs","NextStages":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["AlcatelBadStorageStage"]}}]},"NbOfMaxConcurrentActivators":10,"ConfigId":"65db9db0-61ca-47da-af9e-7e9e0adb11d9"},"IsSequential":false}]}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ExecutionTree],[Stages]))
merge	[queue].[ExecutionFlowDefinition] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ExecutionTree] = s.[ExecutionTree],[Stages] = s.[Stages]
when not matched by target then
	insert([Id],[Name],[Title],[ExecutionTree],[Stages])
	values(s.[Id],s.[Name],s.[Title],s.[ExecutionTree],s.[Stages]);


--[queue].[ExecutionFlow]---------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[ExecutionFlowDefinitionID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('B56D4AD5-9856-4894-A365-EB1B3F50E5D0','Alcatel Execution Flow','A4620B4F-BEC7-44CB-BFB8-3C336CC01631')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ExecutionFlowDefinitionID]))
merge	[queue].[ExecutionFlow] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ExecutionFlowDefinitionID] = s.[ExecutionFlowDefinitionID]
when not matched by target then
	insert([Id],[Name],[ExecutionFlowDefinitionID])
	values(s.[Id],s.[Name],s.[ExecutionFlowDefinitionID]);

--[integration].[DataSource]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('D139C7A9-C689-4E1F-B0A4-013B9C5237F9','ICX Alcatel - Sample Datasource','396A4933-DF4F-49CD-9799-BF605B9F4597','null','BF80C3C3-1A6B-4AFC-9F01-DBDDB6ADDC5F','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".dat","Mask":"","Directory":"/Mediation/AlcatelOgero","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","ActionAfterImport":0,"BasedOnLastModifiedTime":false,"CompressedFiles":false,"CompressionType":0,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = true };\n            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\n            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, new Guid(\"30553F01-CE03-4D29-9BF5-80D0D06DFA34\"), options, (parsedBatch) =>\n            {\n                Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, \"#RECORDSCOUNT# of Alcatel Parsed CDRs\", parsedBatch.RecordType);\n                switch (parsedBatch.RecordType)\n                {\n                    case \"ICX_Alcatel_CDR\":\n                        mappedBatches.Add(\"CDRTransformationStage\", batch);\n                        break;\n                     default:break;\n                }\n\n            });\n            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":"b56d4ad5-9856-4894-a365-eb1b3f50e5d0"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings]))merge	[integration].[DataSource] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[AdapterID] = s.[AdapterID],[AdapterState] = s.[AdapterState],[TaskId] = s.[TaskId],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])	values(s.[ID],s.[Name],s.[AdapterID],s.[AdapterState],s.[TaskId],s.[Settings]);--[runtime].[ScheduleTask]------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('BF80C3C3-1A6B-4AFC-9F01-DBDDB6ADDC5F','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":0,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":false},"StartEffDate":"2018-02-26T15:23:05.223"}', -1)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId]))merge	[runtime].[ScheduleTask] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]--when matched then--	update set--	[Name] = s.[Name],[IsEnabled] = s.[IsEnabled],[TaskType] = s.[TaskType],[TriggerTypeId] = s.[TriggerTypeId],[ActionTypeId] = s.[ActionTypeId],[TaskSettings] = s.[TaskSettings],[OwnerId] = s.[OwnerId]when not matched by target then	insert([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])	values(s.[Id],s.[Name],s.[IsEnabled],s.[TaskType],s.[TriggerTypeId],s.[ActionTypeId],s.[TaskSettings],s.[OwnerId]);--[genericdata].[DataTransformationDefinition]------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('46E3D96B-1189-4AEF-8356-E1EC2EB48F1E','AlcatelCDRTransformation','Alcatel CDR Transformation','{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationDefinition, Vanrise.GenericData.Transformation.Entities","DataTransformationDefinitionId":"46e3d96b-1189-4aef-8356-e1ec2eb48f1e","Name":"AlcatelCDRTransformation","Title":"Alcatel CDR Transformation","RecordTypes":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities","RecordName":"CDRs","DataRecordTypeId":"c7583ff3-8870-4e1c-ab6e-eec9bb5121e0","IsArray":true},{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities","RecordName":"CookedCDRs","DataRecordTypeId":"c7583ff3-8870-4e1c-ab6e-eec9bb5121e0","IsArray":true},{"$type":"Vanrise.GenericData.Transformation.Entities.DataTransformationRecordType, Vanrise.GenericData.Transformation.Entities","RecordName":"BadCDRs","DataRecordTypeId":"c7583ff3-8870-4e1c-ab6e-eec9bb5121e0","IsArray":true}]},"MappingSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.ForLoopStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"d7ce9698-2721-467e-9820-1ed44b446d0d","Steps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.IfElseStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"9cf3c165-1921-4f83-990d-03b82a04aa5a","Condition":"!String.IsNullOrEmpty(cdr.BNumber)","ThenSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AddItemToArrayStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"9c158fa5-8516-4af7-aedd-1bc69d026afc","ArrayVariableName":"CookedCDRs","VariableName":"cdr"}]},"ElseSteps":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Transformation.Entities.MappingStep, Vanrise.GenericData.Transformation.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AddItemToArrayStep, Vanrise.GenericData.Transformation.MainExtensions","ConfigId":"9c158fa5-8516-4af7-aedd-1bc69d026afc","ArrayVariableName":"BadCDRs","VariableName":"cdr"}]}}]},"ArrayVariableName":"CDRs","IterationVariableName":"cdr"}]}}')
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
	values(s.[ID],s.[Name],s.[Title],s.[Details]);--[dataparser].[ParserType]-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('30553F01-CE03-4D29-9BF5-80D0D06DFA34','ICX Alcatel Parser', '{"$type":"Vanrise.DataParser.Entities.ParserTypeSettings, Vanrise.DataParser.Entities","UseRecordType":false,"ExtendedSettings":{"$type":"Vanrise.DataParser.Business.BinaryParserType, Vanrise.DataParser.Business","ConfigId":"92ca6f6f-8901-4c86-9540-efa2941d25e2","RecordParser":{"$type":"Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers.SkipRecordParser, Vanrise.DataParser.MainExtensions","ConfigId":"03e65d50-978c-4641-b2fc-08dad18849eb","RecordParser":{"$type":"Vanrise.DataParser.Entities.BinaryRecordParser, Vanrise.DataParser.Entities","Settings":{"$type":"Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers.PositionedBlockRecordParser, Vanrise.DataParser.MainExtensions","ConfigId":"a7804af5-8a20-4b15-a768-6df1be5e9742","BlockSize":2064,"RecordParser":{"$type":"Vanrise.DataParser.Entities.BinaryRecordParser, Vanrise.DataParser.Entities","Settings":{"$type":"Vanrise.DataParser.MainExtensions.BinaryParsers.AlcatelParsers.RecordParsers.HeaderRecordParser, Vanrise.DataParser.MainExtensions","ConfigId":"bb5ee31f-db10-4480-9a25-9919b71ccb57","HeaderByteLength":32,"RecordStartingTag":"c6c4","RecordParser":{"$type":"Vanrise.DataParser.Entities.BinaryRecordParser, Vanrise.DataParser.Entities","Settings":{"$type":"Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers.PositionedBlockRecordParser, Vanrise.DataParser.MainExtensions","ConfigId":"a7804af5-8a20-4b15-a768-6df1be5e9742","BlockSize":42,"RecordParser":{"$type":"Vanrise.DataParser.Entities.BinaryRecordParser, Vanrise.DataParser.Entities","Settings":{"$type":"Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers.PositionedFieldsRecordParser, Vanrise.DataParser.MainExtensions","ConfigId":"301b945e-765f-4d90-952e-d86da4ae4040","RecordType":"ICX_Alcatel_CDR","FieldParsers":{"$type":"System.Collections.Generic.List`1[[Vanrise.DataParser.Entities.PositionedFieldParser, Vanrise.DataParser.Entities]], mscorlib","$values":[{"$type":"Vanrise.DataParser.Entities.PositionedFieldParser, Vanrise.DataParser.Entities","Position":3,"Length":2,"FieldParser":{"$type":"Vanrise.DataParser.Entities.BinaryFieldParser, Vanrise.DataParser.Entities","Settings":{"$type":"Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers.BCDNumberParser, Vanrise.DataParser.MainExtensions","ConfigId":"55fd305b-707f-4b98-a5da-2caec314fc85","FieldName":"DayNumber","AIsZero":false,"RemoveHexa":false,"Reverse":false}}},{"$type":"Vanrise.DataParser.Entities.PositionedFieldParser, Vanrise.DataParser.Entities","Position":5,"Length":3,"FieldParser":{"$type":"Vanrise.DataParser.Entities.BinaryFieldParser, Vanrise.DataParser.Entities","Settings":{"$type":"Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers.TimeFieldParser, Vanrise.DataParser.MainExtensions","ConfigId":"bae38d63-10cd-487c-9d74-4c2125f18b30","FieldName":"TimeOfCall"}}},{"$type":"Vanrise.DataParser.Entities.PositionedFieldParser, Vanrise.DataParser.Entities","Position":8,"Length":4,"FieldParser":{"$type":"Vanrise.DataParser.Entities.BinaryFieldParser, Vanrise.DataParser.Entities","Settings":{"$type":"Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers.BCDNumberParser, Vanrise.DataParser.MainExtensions","ConfigId":"55fd305b-707f-4b98-a5da-2caec314fc85","FieldName":"ANumber","AIsZero":false,"RemoveHexa":true,"Reverse":false}}},{"$type":"Vanrise.DataParser.Entities.PositionedFieldParser, Vanrise.DataParser.Entities","Position":18,"Length":4,"FieldParser":{"$type":"Vanrise.DataParser.Entities.BinaryFieldParser, Vanrise.DataParser.Entities","Settings":{"$type":"Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers.BCDNumberParser, Vanrise.DataParser.MainExtensions","ConfigId":"55fd305b-707f-4b98-a5da-2caec314fc85","FieldName":"BNumber","AIsZero":false,"RemoveHexa":true,"Reverse":false}}},{"$type":"Vanrise.DataParser.Entities.PositionedFieldParser, Vanrise.DataParser.Entities","Position":30,"Length":2,"FieldParser":{"$type":"Vanrise.DataParser.Entities.BinaryFieldParser, Vanrise.DataParser.Entities","Settings":{"$type":"Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers.NumberFieldParser, Vanrise.DataParser.MainExtensions","ConfigId":"11fce310-6bff-43bd-acd8-f229c8f4ed8a","FieldName":"DurationInSeconds","NumberType":1,"Reverse":false,"FieldIndex":0,"FieldBytesLength":2}}}]},"CompositeFieldsParsers":{"$type":"System.Collections.Generic.List`1[[Vanrise.DataParser.Entities.CompositeFieldsParser, Vanrise.DataParser.Entities]], mscorlib","$values":[{"$type":"Vanrise.DataParser.MainExtensions.CompositeFieldParsers.FileNameCompositeParser, Vanrise.DataParser.MainExtensions","ConfigId":"5de5361e-ae73-4613-a9c6-ebe8e71b9145","FieldName":"FileName"},{"$type":"Vanrise.DataParser.MainExtensions.CompositeFieldParsers.DateFromDayNumberCompositeParser, Vanrise.DataParser.MainExtensions","ConfigId":"d0251ee2-1fb9-4b7c-99e9-cae8c2a90705","FieldName":"DateOfCall","YearFieldName":"Year","DayNumberFieldName":"DayNumber"}]},"ZeroBytesBlockAction":1}}}},"TagsToIgnore":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["a5110002","a5010002"]}}}}},"RecordStartingTag":"c6c4"}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[dataparser].[ParserType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings]) 
	values(s.[ID],s.[Name],s.[Settings]);