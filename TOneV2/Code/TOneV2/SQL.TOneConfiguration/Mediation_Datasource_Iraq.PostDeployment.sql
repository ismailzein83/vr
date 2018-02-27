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

--Mediation_Mobile.Ericsson.PostDeployment
--[runtime].[ScheduleTask]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D666497F-E89E-487C-934A-A26716AAA142','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":false},"StartEffDate":"2017-06-07T09:30:45.072"}',-1),
('06210623-D740-498F-9B66-BCEDFCDFAC80','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":false},"StartEffDate":"2017-08-16T17:29:12.628"}',-1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId]))
merge	[runtime].[ScheduleTask] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
--when matched then
--	update set
--	[Name] = s.[Name],[IsEnabled] = s.[IsEnabled],[TaskType] = s.[TaskType],[TriggerTypeId] = s.[TriggerTypeId],[ActionTypeId] = s.[ActionTypeId],[TaskSettings] = s.[TaskSettings],[OwnerId] = s.[OwnerId]
when not matched by target then
	insert([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
	values(s.[Id],s.[Name],s.[IsEnabled],s.[TaskType],s.[TriggerTypeId],s.[ActionTypeId],s.[TaskSettings],s.[OwnerId]);


--[integration].[DataSource]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7143695F-031D-4136-9D0F-316A7B552333','Ericsson Iraq - Sample Datasource','396A4933-DF4F-49CD-9799-BF605B9F4597','null','D666497F-E89E-487C-934A-A26716AAA142','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".dat","Mask":"","Directory":"/Mediation/ERIC","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","ActionAfterImport":0,"BasedOnLastModifiedTime":false,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\nVanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream,ImportedData.Name, new Guid(\"BA810002-0B4D-4563-9A0D-EE228D69A1A6\"), (parsedBatch) =>\n            {                  \n                switch (parsedBatch.RecordType)\n                {\n                    case \"MobileCDR\":\n                        List<dynamic> multiLegRecords = new List<dynamic>();\n                        List<dynamic> normalRecords = new List<dynamic>();\n                        foreach (dynamic record in parsedBatch.Records)\n                        {\n                            if (record.IntermediateRecordNumber != null)\n                               {\n                                record.SessionId= record.GlobalCallReference + \"_\" + record.RecordType;\n                                multiLegRecords.Add(record);\n                                }\n                            else\n                                normalRecords.Add(record);\n                        }\n                        Vanrise.Integration.Entities.MappedBatchItem normalBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(normalRecords, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n                        mappedBatches.Add(\"MobileTransformationStage\", normalBatch);\n\n                        if (multiLegRecords.Count > 0)\n                        {\n                            Vanrise.Integration.Entities.MappedBatchItem multiLegBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(multiLegRecords, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n                            mappedBatches.Add(\"EricssonMediationStage\", multiLegBatch);\n                        }\n                        break;\n                    case \"SMS\":\n                        Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n\n                        mappedBatches.Add(\"SMSTransformationStage\", batch);\n                        break;\n                }                       \n\n            });\n           Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":"89affe5b-7e04-415b-87b5-3308c4cd582c"}'),
('E356E717-6077-49E2-A23D-F700E92D34CB','Ericsson GPRS - Sample Datasource','396A4933-DF4F-49CD-9799-BF605B9F4597','null','06210623-D740-498F-9B66-BCEDFCDFAC80','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".dat","Mask":"","Directory":"/Mediation/ERIC/GPRS","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","ActionAfterImport":0,"BasedOnLastModifiedTime":false,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\nVanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream,ImportedData.Name, new Guid(\"B9648105-8914-4C70-8550-F63D946F5B0C\"), (parsedBatch) =>\n            {                   \n                    Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n\n     \n                                      mappedBatches.Add(\"GPRSStoreStage\", batch);  \n\n            });\n           Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":"89affe5b-7e04-415b-87b5-3308c4cd582c"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings]))
merge	[integration].[DataSource] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[AdapterID] = s.[AdapterID],[AdapterState] = s.[AdapterState],[TaskId] = s.[TaskId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
	values(s.[ID],s.[Name],s.[AdapterID],s.[AdapterState],s.[TaskId],s.[Settings]);

--Mediation_Mobile.Huawei.PostDeployment
--[runtime].[ScheduleTask]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('2D72E5DC-5E65-492A-A4F1-BF4F2A04F911','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":false},"StartEffDate":"2017-06-07T09:30:45.072"}',-1),
('77D60DEC-57B5-4A12-9253-F50FC042F9F5','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":false},"StartEffDate":"2017-06-07T15:27:48.363"}',-1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId]))
merge	[runtime].[ScheduleTask] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
--when matched then
--	update set
--	[Name] = s.[Name],[IsEnabled] = s.[IsEnabled],[TaskType] = s.[TaskType],[TriggerTypeId] = s.[TriggerTypeId],[ActionTypeId] = s.[ActionTypeId],[TaskSettings] = s.[TaskSettings],[OwnerId] = s.[OwnerId]
when not matched by target then
	insert([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
	values(s.[Id],s.[Name],s.[IsEnabled],s.[TaskType],s.[TriggerTypeId],s.[ActionTypeId],s.[TaskSettings],s.[OwnerId]);


--[integration].[DataSource]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('9A015F50-498F-4663-A5C1-151D63553D30','Huawei GPRS - Sample Datasource','396A4933-DF4F-49CD-9799-BF605B9F4597','null','77D60DEC-57B5-4A12-9253-F50FC042F9F5','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".dat","Mask":"","Directory":"/Mediation/Huawei/GPRS_NEW","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","ActionAfterImport":0,"BasedOnLastModifiedTime":false,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\nVanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream,ImportedData.Name, new Guid(\"16b6af8d-6a15-46a1-9c19-ccfac1ebbdde\"), (parsedBatch) =>\n            {                   \n                    Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n\n     \n                                      mappedBatches.Add(\"GPRSStoreStage\", batch);  \n\n            });\n           Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":"89affe5b-7e04-415b-87b5-3308c4cd582c"}'),
('7CF5DBD9-80AF-4F27-A07E-88B15D6479DF','Huawei EDR - Sample Datasource','396A4933-DF4F-49CD-9799-BF605B9F4597','null','2D72E5DC-5E65-492A-A4F1-BF4F2A04F911','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".gz","Mask":"","Directory":"/Mediation/Huawei/Iraq","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","ActionAfterImport":0,"BasedOnLastModifiedTime":false,"CompressedFiles":true,"CompressionType":0,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\nVanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream,ImportedData.Name, new Guid(\"3B0C2ED7-CC17-46C0-8F96-697BD185B273\"), (parsedBatch) =>\n            {                  \n                switch (parsedBatch.RecordType)\n                {\n                    case \"MobileCDR\":\n                        List<dynamic> multiLegRecords = new List<dynamic>();\n                        List<dynamic> normalRecords = new List<dynamic>();\n                        foreach (dynamic record in parsedBatch.Records)\n                        {\n                            if (record.SequenceNumber == null || record.SequenceNumber >0)\n                               {\n                                record.SessionId= record.GlobalCallReference + \"_\" + record.RecordType;\n                                multiLegRecords.Add(record);\n                                }\n                            else\n                                normalRecords.Add(record);\n                        }\n                        Vanrise.Integration.Entities.MappedBatchItem normalBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(normalRecords, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n                        mappedBatches.Add(\"MobileTransformationStage\", normalBatch);\n\n                        if (multiLegRecords.Count > 0)\n                        {\n                            Vanrise.Integration.Entities.MappedBatchItem multiLegBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(multiLegRecords, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n                            mappedBatches.Add(\"HuaweiMediationStage\", multiLegBatch);\n                        }\n                        break;\n                    case \"SMS\":\n                        Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n\n                        mappedBatches.Add(\"SMSTransformationStage\", batch);\n                        break;\n                }                       \n\n            });\n           Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":"89affe5b-7e04-415b-87b5-3308c4cd582c"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings]))
merge	[integration].[DataSource] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[AdapterID] = s.[AdapterID],[AdapterState] = s.[AdapterState],[TaskId] = s.[TaskId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
	values(s.[ID],s.[Name],s.[AdapterID],s.[AdapterState],s.[TaskId],s.[Settings]);


--Mediation_Mobile.Nokia.PostDeployment
--[runtime].[ScheduleTask]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7761FC9F-BBD2-4E68-A6AB-A256EE7E295B','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":false},"StartEffDate":"2017-06-07T09:30:45.072"}',-1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId]))
merge	[runtime].[ScheduleTask] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
--when matched then
--	update set
--	[Name] = s.[Name],[IsEnabled] = s.[IsEnabled],[TaskType] = s.[TaskType],[TriggerTypeId] = s.[TriggerTypeId],[ActionTypeId] = s.[ActionTypeId],[TaskSettings] = s.[TaskSettings],[OwnerId] = s.[OwnerId]
when not matched by target then
	insert([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
	values(s.[Id],s.[Name],s.[IsEnabled],s.[TaskType],s.[TriggerTypeId],s.[ActionTypeId],s.[TaskSettings],s.[OwnerId]);


--[integration].[DataSource]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D62D71E1-3B77-43D0-B2EA-BA8FF8E7B08B','Nokia EDR - Sample Datasource','396A4933-DF4F-49CD-9799-BF605B9F4597','null','7761FC9F-BBD2-4E68-A6AB-A256EE7E295B','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".dat","Mask":"","Directory":"/Mediation/Nok","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","ActionAfterImport":0,"BasedOnLastModifiedTime":false,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\nVanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream,ImportedData.Name, new Guid(\"230bedb5-a3ee-4cbe-802c-dfdaa2a2d438\"), (parsedBatch) =>\n            {                  \n                switch (parsedBatch.RecordType)\n                {\n                    case \"MobileCDR\":\n                        List<dynamic> multiLegRecords = new List<dynamic>();\n                        List<dynamic> normalRecords = new List<dynamic>();\n                        foreach (dynamic record in parsedBatch.Records)\n                        {\n                            if (record.IntermediateChargingIndicator != 0)\n                               {\n                                record.SessionId= record.GlobalCallReference + \"_\" + record.RecordType;\n                                multiLegRecords.Add(record);\n                                }\n                            else\n                                normalRecords.Add(record);\n                        }\n                        Vanrise.Integration.Entities.MappedBatchItem normalBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(normalRecords, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n                        mappedBatches.Add(\"MobileTransformationStage\", normalBatch);\n\n                        if (multiLegRecords.Count > 0)\n                        {\n                            Vanrise.Integration.Entities.MappedBatchItem multiLegBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(multiLegRecords, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n                            mappedBatches.Add(\"NokiaMediationStage\", multiLegBatch);\n                        }\n                        break;\n                    case \"SMS\":\n                        Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, \"#RECORDSCOUNT# of Parsed CDRs\", parsedBatch.RecordType);\n\n                        mappedBatches.Add(\"SMSTransformationStage\", batch);\n                        break;\n                }                       \n\n            });\n           Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":"89affe5b-7e04-415b-87b5-3308c4cd582c"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings]))
merge	[integration].[DataSource] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[AdapterID] = s.[AdapterID],[AdapterState] = s.[AdapterState],[TaskId] = s.[TaskId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
	values(s.[ID],s.[Name],s.[AdapterID],s.[AdapterState],s.[TaskId],s.[Settings]);