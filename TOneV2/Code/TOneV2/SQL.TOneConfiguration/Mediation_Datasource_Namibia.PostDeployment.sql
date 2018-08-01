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

--[runtime].[ScheduleTask]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('03867363-a2a2-4ae3-ae9f-68745566a435','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":0,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":false},"StartEffDate":"2017-05-17T17:00:37.719"}',1)
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

	
--[integration].[DataSource]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('C33F9DAE-CBB6-4B0B-B8FF-65A9132B5762','Huawei WHS - Sample Datasource','396A4933-DF4F-49CD-9799-BF605B9F4597','null','03867363-A2A2-4AE3-AE9F-68745566A435','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".dat","Mask":"","Directory":"/Mediation/HuaweiNamibia","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","ActionAfterImport":0,"FileCheckCriteria":0,"CompressedFiles":false,"CompressionType":0,"FileCompletenessCheckInterval":5,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\nVanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, dataSourceId, new Guid(\"e2a77834-86da-42ba-9501-c3eb81f5f60b\"), (parsedBatch) =>\n{\n    switch (parsedBatch.RecordType)\n    {\n        case \"WHS_CDR\":\n            foreach (dynamic record in parsedBatch.Records)\n            {\n                DateTime? connectDateTime = record.GetFieldValue(\"ConnectDateTime\");\n                if (!connectDateTime.HasValue || connectDateTime.Value == DateTime.MinValue)\n                {\n                    int duration = record.GetFieldValue(\"DurationInSeconds\");\n                    if(duration == 0)\n                    {\n                        DateTime? disconnectDateTime = record.GetFieldValue(\"DisconnectDateTime\");\n                        record.SetFieldValue(\"ConnectDateTime\", disconnectDateTime);\n                    }\n                }\n            }\n\n            Vanrise.Integration.Entities.MappedBatchItem cdrBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n            mappedBatches.Add(\"CDR_TransformationStep\", cdrBatch);\n            break;\n\n        case \"SMS\":\n            Vanrise.Integration.Entities.MappedBatchItem smsBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n            mappedBatches.Add(\"SMS_TransformationStep\", smsBatch);\n            break;\n    }\n});\n\nVanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\nresult.Result = Vanrise.Integration.Entities.MappingResult.Valid;\nresult.Message = string.Format(\"Finished importing File {0}\", ImportedData.Name);\nLogVerbose(\"Finished\");\nreturn result;","ExecutionFlowId":"0a3850ab-4fc5-4cc0-8889-0e7159eedfe2"}')
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
