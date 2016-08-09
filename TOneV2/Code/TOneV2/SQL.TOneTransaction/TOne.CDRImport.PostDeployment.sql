/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
The target of this script is to define default data source for CDR import processing				
--------------------------------------------------------------------------------------
*/
--check if Datasource already defined
if exists (select 1 from [queue].[ExecutionFlowDefinition] where ID=-100)
BEGIN
 select 'Data source already defined, you may loose updated mapping and connection string. if you need to override please remove this condition.'
 RETURN
 END
--[queue].[ExecutionFlowDefinition]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [queue].[ExecutionFlowDefinition] on;
;with cte_data([ID],[Name],[Title],[ExecutionTree],[Stages])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-100,'Import CDR','Import CDR',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"CDR Storage Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":42,"BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataRecordStorageId":44,"ConfigId":1}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"CDR Billing Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":42,"BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.TransformBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataTransformationDefinitionId":30,"SourceRecordName":"CDRs","NextStagesRecords":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.QueueActivators.TransformBatchNextStageRecord, Vanrise.GenericData.QueueActivators]], mscorlib","$values":[{"$type":"Vanrise.GenericData.QueueActivators.TransformBatchNextStageRecord, Vanrise.GenericData.QueueActivators","RecordName":"MainCDRs","NextStages":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Main CDR Storage Stage","Billing Stats Daily Generation Stage"]}},{"$type":"Vanrise.GenericData.QueueActivators.TransformBatchNextStageRecord, Vanrise.GenericData.QueueActivators","RecordName":"InvalidCDRs","NextStages":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Invalid CDR Storage Stage"]}},{"$type":"Vanrise.GenericData.QueueActivators.TransformBatchNextStageRecord, Vanrise.GenericData.QueueActivators","RecordName":"FailedCDRs","NextStages":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Failed CDR Storage Stage"]}},{"$type":"Vanrise.GenericData.QueueActivators.TransformBatchNextStageRecord, Vanrise.GenericData.QueueActivators","RecordName":"AllBillingCDRs","NextStages":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Traffic Stats XMin Generation Stage","Traffic Stats Daily Generation Stage"]}}]},"ConfigId":2},"SourceStages":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["CDR Storage Stage"]}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Main CDR Storage Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":43,"BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataRecordStorageId":45,"ConfigId":1}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Invalid CDR Storage Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":43,"BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataRecordStorageId":46,"ConfigId":1}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Failed CDR Storage Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":43,"BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataRecordStorageId":48,"ConfigId":1}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Traffic Stats XMin Update Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.GenericSummaryRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":44,"BatchDescription":"#RECORDSCOUNT# of Traffic Stats"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.UpdateSummaryQueueActivator, Vanrise.GenericData.QueueActivators","SummaryTransformationDefinitionId":1,"ConfigId":4}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Traffic Stats XMin Generation Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":43,"BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.GenerateSummaryQueueActivator, Vanrise.GenericData.QueueActivators","SummaryTransformationDefinitionId":1,"NextStageName":"Traffic Stats XMin Update Stage","ConfigId":5}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Traffic Stats Daily Update Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.GenericSummaryRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":44,"BatchDescription":"#RECORDSCOUNT# of Traffic Stats"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.UpdateSummaryQueueActivator, Vanrise.GenericData.QueueActivators","SummaryTransformationDefinitionId":3,"ConfigId":4}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Traffic Stats Daily Generation Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":43,"BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.GenerateSummaryQueueActivator, Vanrise.GenericData.QueueActivators","SummaryTransformationDefinitionId":3,"NextStageName":"Traffic Stats Daily Update Stage","ConfigId":5}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Billing Stats Daily Update Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.GenericSummaryRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":45,"BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.UpdateSummaryQueueActivator, Vanrise.GenericData.QueueActivators","SummaryTransformationDefinitionId":2,"ConfigId":4}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Billing Stats Daily Generation Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":43,"BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.GenerateSummaryQueueActivator, Vanrise.GenericData.QueueActivators","SummaryTransformationDefinitionId":2,"NextStageName":"Billing Stats Daily Update Stage","ConfigId":5}}]}')
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
set identity_insert [queue].[ExecutionFlowDefinition] off;

--[queue].[ExecutionFlow]---------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [queue].[ExecutionFlow] on;
;with cte_data([ID],[Name],[ExecutionFlowDefinitionID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-100,'Import CDR',-100)
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
set identity_insert [queue].[ExecutionFlow] off;

--[runtime].[ScheduleTask]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [runtime].[ScheduleTask] on;
;with cte_data([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////)
(-100,'CDR Import Data Source Task',0,0,1,1,'{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"},"StartEffDate":"2016-05-26T13:53:00+03:00"}',1)
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
set identity_insert [runtime].[ScheduleTask] off;

--[integration].[DataSource]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [integration].[DataSource] on;
;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-100,'Import CDR From DB (ToneDevTest01)',3,'{"$type":"Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterState, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments","LastImportedId":0}',-100,'{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterArgument, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments","ConnectionString":"Server=192.168.110.185;Database=ToneDevTest01;User ID=Development;Password=dev!123","Query":"SELECT #TopRows# [CDRID]\n      ,[SwitchID]\n      ,[IDonSwitch]\n      ,[Tag]\n      ,[AttemptDateTime]\n      ,[AlertDateTime]\n      ,[ConnectDateTime]\n      ,[DisconnectDateTime]\n      ,[DurationInSeconds]\n      ,[IN_TRUNK]\n      ,[IN_CIRCUIT]\n      ,[IN_CARRIER]\n      ,[IN_IP]\n      ,[OUT_TRUNK]\n      ,[OUT_CIRCUIT]\n      ,[OUT_CARRIER]\n      ,[OUT_IP]\n      ,[CGPN]\n      ,[CDPN]\n      ,[CAUSE_FROM_RELEASE_CODE]\n      ,[CAUSE_FROM]\n      ,[CAUSE_TO_RELEASE_CODE]\n      ,[CAUSE_TO]\n      ,[Extra_Fields]\n      ,[IsRerouted]\n      ,[CDPNOut]\n      ,[SIP]\n  FROM [dbo].[CDR]\nWhere  \n(CDRID > ISNULL(@RangeStart, 0))\nAND (CDRID <= ISNULL(@RangeEnd, 9999999999999999))\nORDER BY CDRID","IdentifierColumnName":"CDRID","NumberOfParallelReader":3,"NumberOffSet":1000000,"MaxParallelRuntimeInstances":3},"MapperCustomCode":"LogVerbose(\"Started\");\n\n            var cdrs = new List<dynamic>();\n            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();\n            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(\"CDR\");\n\n            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));\n\n            IDataReader reader = importedData.Reader;\n\n            int rowCount = 0;\n            while (reader.Read())\n            {\n                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;\n                cdr.SwitchId = 5;\n                cdr.IDonSwitch = Utils.GetReaderValue<long>(reader, \"IDonSwitch\");\n                cdr.Tag = reader[\"Tag\"] as string;\n                cdr.AttemptDateTime = (DateTime)reader[\"AttemptDateTime\"];\n                cdr.AlertDateTime = Utils.GetReaderValue<DateTime>(reader, \"AlertDateTime\");\n                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, \"ConnectDateTime\");\n                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime>(reader, \"DisconnectDateTime\");\n                cdr.DurationInSeconds = Utils.GetReaderValue<Decimal>(reader, \"DurationInSeconds\");\n                cdr.InTrunk = reader[\"IN_TRUNK\"] as string;\n                cdr.InCircuit = reader[\"IN_CIRCUIT\"] != DBNull.Value ? Convert.ToInt64(reader[\"IN_CIRCUIT\"]) : default(Int64);\n                cdr.InCarrier = reader[\"IN_CARRIER\"] as string;\n                cdr.InIP = reader[\"IN_IP\"] as string;\n                cdr.OutTrunk = reader[\"OUT_TRUNK\"] as string;\n                cdr.OutCircuit = reader[\"OUT_CIRCUIT\"] != DBNull.Value ? Convert.ToInt64(reader[\"OUT_CIRCUIT\"]) : default(Int64); \n                cdr.OutCarrier = reader[\"OUT_CARRIER\"] as string;\n                cdr.OutIP = reader[\"OUT_IP\"] as string;\n\n                cdr.CGPN = reader[\"CGPN\"] as string;\n                cdr.CDPN = reader[\"CDPN\"] as string;\n                cdr.CauseFromReleaseCode = reader[\"CAUSE_FROM_RELEASE_CODE\"] as string;\n                cdr.CauseFrom = reader[\"CAUSE_FROM\"] as string;\n                cdr.CauseToReleaseCode = reader[\"CAUSE_TO_RELEASE_CODE\"] as string;\n                cdr.CauseTo = reader[\"CAUSE_TO\"] as string;\n                cdr.IsRerouted = reader[\"IsRerouted\"] != DBNull.Value ? ((reader[\"IsRerouted\"] as string) == \"Y\") : false;\n                cdr.CDPNOut = reader[\"CDPNOut\"] as string;\n                cdr.SIP = reader[\"SIP\"] as string;\n\n                cdrs.Add(cdr);\n                importedData.LastImportedId = reader[\"CDRID\"];\n                rowCount++;\n                if (rowCount == 50000)\n                    break;\n\n            }\n            if (cdrs.Count > 0)\n            {\n                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, \"#RECORDSCOUNT# of Raw CDRs\");\n                mappedBatches.Add(\"CDR Storage Stage\", batch);\n            }\n            else\n                importedData.IsEmpty = true;\n\n            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":-100}')
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
set identity_insert [integration].[DataSource] off;

