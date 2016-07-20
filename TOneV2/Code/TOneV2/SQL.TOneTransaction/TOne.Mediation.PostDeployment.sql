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
--check if Datasource already defined
if exists (select 1 from [queue].[ExecutionFlowDefinition] where ID=-102)
BEGIN
 select 'Data source already defined, you may loose updated mapping and connection string. if you need to override please remove this condition.'
 RETURN
 END

--[queue].[ExecutionFlowDefinition]---------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [queue].[ExecutionFlowDefinition] on;
;with cte_data([ID],[Name],[Title],[ExecutionTree],[Stages])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-102,'MediationExecutionFlow', 'Mediation Execution Flow', NULL, '{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Mediation Store Batch","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":201,"BatchDescription":"#RECORDSCOUNT# of CDRs"},"QueueActivator":{"$type":"Mediation.Generic.QueueActivators.StoreStagingRecordsQueueActivator, Mediation.Generic.QueueActivators","MediationDefinitionId":1,"ConfigId":501}}]}')
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


--[queue].[ExecutionFlow]-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [queue].[ExecutionFlow] on;
;with cte_data([ID],[Name],[ExecutionFlowDefinitionID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-102,'Mediation Flow',-102)
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


--[runtime].[ScheduleTask]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [runtime].[ScheduleTask] on;
;with cte_data([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-105,'Mediation Data Source Task', 0, 0, 1, 1, '{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"},"StartEffDate":"2016-06-12T10:35:00+03:00"}', 1)
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


--[integration].[DataSource]----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [integration].[DataSource] on;
;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-105,'File Data Source - MultiNet', 1, '{"$type":"Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterState, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments","LastImportedId":0}', -105, '{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".txt","Directory":"/MultiNet/CDRs","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","DirectorytoMoveFile":"/MultiNet/Processed","ActionAfterImport":2,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\n            var cdrs = new List<dynamic>();\n            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();\n            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(\"ParsedCDR\");\n            try\n            {\n                System.IO.StreamReader sr = ImportedData.StreamReader;\n                while (!sr.EndOfStream)\n                {\n                    string currentLine = sr.ReadLine();\n                    if (string.IsNullOrEmpty(currentLine))\n                        continue;\n                    string[] rowData = currentLine.Split('','');\n                    dynamic cdr = Activator.CreateInstance(mediationCDRRuntimeType) as dynamic;\n                    cdr.TC_VERSIONID = rowData[0].Trim(''\"'');\n                    cdr.TC_CALLID = rowData[13].Trim(''\"'');\n                    cdr.TC_LOGTYPE = rowData[1].Trim(''\"'');\n                    cdr.TC_TIMESTAMP = DateTime.ParseExact(rowData[3].Trim(''\"''), \"yyyyMMddHHmmss:fff\", System.Globalization.CultureInfo.InvariantCulture);\n\n                    cdr.TC_DISCONNECTREASON = rowData[4].Trim(''\"'');\n                    cdr.TC_CALLPROGRESSSTATE = rowData[5].Trim(''\"'');\n                    cdr.TC_ACCOUNT = rowData[6].Trim(''\"'');\n                    cdr.TC_ORIGINATORID = rowData[7].Trim(''\"'');\n                    cdr.TC_ORIGINATORNUMBER = rowData[8].Trim(''\"'');\n                    cdr.TC_ORIGINALFROMNUMBER = rowData[9].Trim(''\"'');\n                    cdr.TC_ORIGINALDIALEDNUMBER = rowData[10].Trim(''\"'');\n                    cdr.TC_TERMINATORID = rowData[11].Trim(''\"'');\n                    cdr.TC_TERMINATORNUMBER = rowData[12].Trim(''\"'');\n                    cdr.TC_INCOMINGGWID = rowData[15].Trim(''\"'');\n                    cdr.TC_OUTGOINGGWID = rowData[16].Trim(''\"'');\n                    cdr.TC_TRANSFERREDCALLID = rowData[20].Trim(''\"'');\n                    cdrs.Add(cdr);\n\n                }\n            }\n            catch (Exception ex)\n            {                \n                throw ex;\n            }\n\n            MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, \"#RECORDSCOUNT# of Raw CDRs\");\n            mappedBatches.Add(\"Mediation Store Batch\", batch);\n            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            return result;","ExecutionFlowId":-102}')
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