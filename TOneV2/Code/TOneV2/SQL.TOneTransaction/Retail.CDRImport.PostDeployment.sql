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
--check if Datasource already defined
if exists (select 1 from [queue].[ExecutionFlowDefinition] where ID=-101)
BEGIN
 select 'Data source already defined, you may loose updated mapping and connection string. if you need to override please remove this condition.'
 RETURN
 END


 --queue.ExecutionFlowDefinition---------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [queue].[ExecutionFlowDefinition] on;
;with cte_data([ID],[Name],[Title],[ExecutionTree],[Stages])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-101,'EDRImport','EDR Import',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Voice EDR Storage Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":1,"BatchDescription":"#RECORDSCOUNT# of EDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataRecordStorageId":1,"ConfigId":1}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Message EDR Storage Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":2,"BatchDescription":"#RECORDSCOUNT# of EDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataRecordStorageId":2,"ConfigId":1}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Gprs EDR Storage Stage","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.GenericData.QueueActivators.DataRecordBatchQueueItemType, Vanrise.GenericData.QueueActivators","DataRecordTypeId":3,"BatchDescription":"#RECORDSCOUNT# of EDRs"},"QueueActivator":{"$type":"Vanrise.GenericData.QueueActivators.StoreBatchQueueActivator, Vanrise.GenericData.QueueActivators","DataRecordStorageId":3,"ConfigId":1}}]}')
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



--queue.ExecutionFlow-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [queue].[ExecutionFlow] on;
;with cte_data([ID],[Name],[ExecutionFlowDefinitionID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-101,'EDR Import',-101)
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


--runtime.ScheduleTask------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [runtime].[ScheduleTask] on;
;with cte_data([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-101,'Voice Data Source Task',1,0,1,1,'{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":0,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"},"StartEffDate":"2016-06-13T12:54:00+03:00"}',1),
(-102,'Sms Data Source Task',1,0,1,1,'{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":0,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"},"StartEffDate":"2016-06-13T12:54:00+03:00"}',1),
(-103,'Gprs Data Source Task',1,0,1,1,'{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":0,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"},"StartEffDate":"2016-06-13T12:54:00+03:00"}',1),
(-104,'Mms Data Source Task',1,0,1,1,'{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":0,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"},"StartEffDate":"2016-06-13T12:54:00+03:00"}',1)
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


--integration.DataSource----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [integration].[DataSource] on;
;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-101,'Voice Data Source',2,'null',-101,'{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".csv","Directory":"/Retail/Voice","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","DirectorytoMoveFile":"/Retail/Voice_Processed","ActionAfterImport":2,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"LogVerbose(\"Started\");\n            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\n            var voiceEDRs = new List<dynamic>;();\n\n            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();\n\n            Type voiceEDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(\"VoiceEDR\");\n\n            var currentItemCount = 27;\n            var headerText = \"H\";\n\n            DateTime creationDate = default(DateTime);\n            System.IO.StreamReader sr = ImportedData.StreamReader;\n            while (!sr.EndOfStream)\n            {\n                string currentLine = sr.ReadLine();\n                if (string.IsNullOrEmpty(currentLine))\n                    continue;\n\n                string[] rowData = currentLine.Split('';'');\n\n                if (rowData.Length == 2 &amp;&amp; rowData[0] == headerText)\n                {\n                    creationDate = DateTime.ParseExact(rowData[1], \"yyyyMMddHHmmss\", System.Globalization.CultureInfo.InvariantCulture);\n                    continue;\n                }\n\n                else if (rowData.Length != currentItemCount)\n                    continue;\n\n                dynamic edr = Activator.CreateInstance(voiceEDRRuntimeType) as dynamic;\n                edr.IdCDR = long.Parse(rowData[0]);\n                edr.StartDate = DateTime.ParseExact(rowData[2], \"dd/MM/yyyy HH:mm:ss\", System.Globalization.CultureInfo.InvariantCulture);\n\n                string parentIdCDR = rowData[23];\n                edr.ParentIdCDR = !string.IsNullOrEmpty(parentIdCDR) ? long.Parse(parentIdCDR) : default(long?);\n\n                edr.TrafficType = rowData[3];\n                edr.TypeCalled = rowData[4];\n                edr.DirectionTraffic = rowData[5];\n                edr.Calling = rowData[6];\n                edr.Called = rowData[7];\n                edr.RedirectingNumber = rowData[8];\n                edr.TypeNet = rowData[9];\n                edr.SourceOperator = rowData[10];\n                edr.DestinationOperator = rowData[11];\n                edr.SourceArea = rowData[12];\n                edr.DestinationArea = rowData[13];\n\n                string duration = rowData[14];\n                edr.Duration = !string.IsNullOrEmpty(duration) ? decimal.Parse(duration) : default(decimal?);\n\n                edr.DurationUnit = rowData[15];\n\n                string balance = rowData[17];\n                edr.Balance = !string.IsNullOrEmpty(balance) ? decimal.Parse(balance) : default(decimal?);\n\n                string amount = rowData[19];\n                edr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal?);\n\n                edr.TypeConsumed = rowData[20];\n                edr.Bag = rowData[18];\n                edr.PricePlan = rowData[21];\n                edr.Promotion = rowData[22];\n                edr.FileName = rowData[25];\n\n                edr.FileDate = DateTime.ParseExact(rowData[26], \"dd/MM/yyyy HH:mm:ss\", System.Globalization.CultureInfo.InvariantCulture);\n                edr.CreationDate = creationDate;\n\n                voiceEDRs.Add(edr);\n            }\n\n            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(voiceEDRs, \"#RECORDSCOUNT# of Raw EDRs\");\n            mappedBatches.Add(\"Voice EDR Storage Stage\", batch);\n\n            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":-101}'),
(-102,'Sms Data Source',2,'null',-102,'{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".csv","Directory":"/Retail/SMS","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","DirectorytoMoveFile":"/Retail/SMS_Processed","ActionAfterImport":2,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"LogVerbose(\"Started\");\n            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\n            var smsEDRs = new List<dynamic>;();\n\n            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();\n            Type messageEDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(\"MessageEDR\");\n\n\n            var currentItemCount = 26;\n            var headerText = \"H\";\n\n            DateTime creationDate = default(DateTime);\n            System.IO.StreamReader sr = ImportedData.StreamReader;\n            while (!sr.EndOfStream)\n            {\n                string currentLine = sr.ReadLine();\n                if (string.IsNullOrEmpty(currentLine))\n                    continue;\n\n                string[] rowData = currentLine.Split('';'');\n\n                if (rowData.Length == 2 &amp;&amp; rowData[0] == headerText)\n                {\n                    creationDate = DateTime.ParseExact(rowData[1], \"yyyyMMddHHmmss\", System.Globalization.CultureInfo.InvariantCulture);\n                    continue;\n                }\n\n                else if (rowData.Length != currentItemCount)\n                    continue;\n\n                dynamic edr = Activator.CreateInstance(messageEDRRuntimeType) as dynamic;\n                edr.IdCDR = long.Parse(rowData[0]);\n                edr.StartDate = DateTime.ParseExact(rowData[2], \"dd/MM/yyyy HH:mm:ss\", System.Globalization.CultureInfo.InvariantCulture);\n\n                string parentIdCDR = rowData[22];\n                edr.ParentIdCDR = !string.IsNullOrEmpty(parentIdCDR) ? long.Parse(parentIdCDR) : default(long?);\n\n                edr.TrafficType = rowData[3];\n                edr.TypeMessage = rowData[4];\n                edr.DirectionTraffic = rowData[5];\n                edr.Calling = rowData[6];\n                edr.Called = rowData[7];\n                edr.TypeNet = rowData[8];\n                edr.SourceOperator = rowData[9];\n                edr.DestinationOperator = rowData[10];\n                edr.SourceArea = rowData[11];\n                edr.DestinationArea = rowData[12];\n\n                string bill = rowData[13];\n                edr.Bill = !string.IsNullOrEmpty(bill) ? int.Parse(bill) : default(int?);\n\n                string credit = rowData[15];\n                edr.Credit = !string.IsNullOrEmpty(credit) ? decimal.Parse(credit) : default(decimal?);\n\n                edr.Unit = rowData[14];\n\n                string balance = rowData[16];\n                edr.Balance = !string.IsNullOrEmpty(balance) ? decimal.Parse(balance) : default(decimal?);\n\n                edr.Bag = rowData[17];\n\n                string amount = rowData[18];\n                edr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal?);\n\n                edr.TypeConsumed = rowData[19];\n                edr.PricePlan = rowData[20];\n                edr.Promotion = rowData[21];\n\n                edr.FileName = rowData[24];\n                edr.FileDate = DateTime.ParseExact(rowData[25], \"dd/MM/yyyy HH:mm:ss\", System.Globalization.CultureInfo.InvariantCulture);\n                edr.CreationDate = creationDate;\n\n                smsEDRs.Add(edr);\n            }\n\n            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(smsEDRs, \"#RECORDSCOUNT# of Raw EDRs\");\n            mappedBatches.Add(\"Message EDR Storage Stage\", batch);\n\n            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":-101}'),
(-103,'Gprs Data Source',2,'null',-103,'{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".csv","Directory":"/Retail/GPRS","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","DirectorytoMoveFile":"/Retail/GPRS_Processed","ActionAfterImport":2,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"LogVerbose(\"Started\");\n            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\n\n            var dataEDRs = new List<dynamic>;();\n\n            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();\n            Type gprsEDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(\"GprsEDR\");\n\n            var currentItemCount = 21;\n            var headerText = \"H\";\n\n            DateTime creationDate = default(DateTime);\n            System.IO.StreamReader sr = ImportedData.StreamReader;\n            while (!sr.EndOfStream)\n            {\n                string currentLine = sr.ReadLine();\n                if (string.IsNullOrEmpty(currentLine))\n                    continue;\n\n                string[] rowData = currentLine.Split('';'');\n\n                if (rowData.Length == 2 &amp;&amp; rowData[0] == headerText)\n                {\n                    creationDate = DateTime.ParseExact(rowData[1], \"yyyyMMddHHmmss\", System.Globalization.CultureInfo.InvariantCulture);\n                    continue;\n                }\n\n                else if (rowData.Length != currentItemCount)\n                    continue;\n\n                dynamic edr = Activator.CreateInstance(gprsEDRRuntimeType) as dynamic;\n                edr.StartDate = DateTime.ParseExact(rowData[2], \"dd/MM/yyyy HH:mm:ss\", System.Globalization.CultureInfo.InvariantCulture);\n                edr.TypeGprs = rowData[4];\n                edr.Calling = rowData[5];\n                edr.Zone = rowData[6];\n\n                string bill = rowData[7];\n                edr.Bill = !string.IsNullOrEmpty(bill) ? int.Parse(bill) : default(int?);\n\n                string credit = rowData[9];\n                edr.Credit = !string.IsNullOrEmpty(credit) ? decimal.Parse(credit) : default(decimal?);\n\n                edr.TrafficType = rowData[3];\n                edr.Unit = rowData[8];\n\n                string balance = rowData[10];\n                edr.Balance = !string.IsNullOrEmpty(balance) ? decimal.Parse(balance) : default(decimal?);\n\n                edr.Bag = rowData[11];\n\n                string amount = rowData[12];\n                edr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal?);\n\n                edr.TypeConsumed = rowData[13];\n                edr.PricePlan = rowData[14];\n                edr.Promotion = rowData[15];\n                edr.AccessPointName = rowData[16];\n                //cdr.ParentIdCDR\n                edr.IdCDR = long.Parse(rowData[0]);\n\n                string idCdrGprs = rowData[17];\n                edr.IdCdrGprs = !string.IsNullOrEmpty(idCdrGprs) ? long.Parse(idCdrGprs) : default(long?);\n\n\n                edr.FileName = rowData[19];\n                edr.FileDate = DateTime.ParseExact(rowData[20], \"dd/MM/yyyy HH:mm:ss\", System.Globalization.CultureInfo.InvariantCulture);\n                edr.CreationDate = creationDate;\n\n\n                dataEDRs.Add(edr);\n            }\n\n            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(dataEDRs, \"#RECORDSCOUNT# of Raw EDRs\");\n            mappedBatches.Add(\"Gprs EDR Storage Stage\", batch);\n\n            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":-101}'),
(-104,'Mms Data Source',2,'null',-104,'{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".csv","Directory":"/Retail/MMS","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","DirectorytoMoveFile":"/Retail/MMS_Processed","ActionAfterImport":2,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"LogVerbose(\"Started\");\n            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\n            var mmsEDRs = new List<dynamic>;();\n\n            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();\n            Type messageEDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(\"MessageEDR\");\n\n\n            var currentItemCount = 27;\n            var headerText = \"H\";\n\n            DateTime creationDate = default(DateTime);\n            System.IO.StreamReader sr = ImportedData.StreamReader;\n            while (!sr.EndOfStream)\n            {\n                string currentLine = sr.ReadLine();\n                if (string.IsNullOrEmpty(currentLine))\n                    continue;\n\n                string[] rowData = currentLine.Split('';'');\n\n                if (rowData.Length == 2 &amp;&amp; rowData[0] == headerText)\n                {\n                    creationDate = DateTime.ParseExact(rowData[1], \"yyyyMMddHHmmss\", System.Globalization.CultureInfo.InvariantCulture);\n                    continue;\n                }\n\n                else if (rowData.Length != currentItemCount)\n                    continue;\n\n                dynamic edr = Activator.CreateInstance(messageEDRRuntimeType) as dynamic;\n                edr.IdCDR = long.Parse(rowData[0]);\n                edr.StartDate = DateTime.ParseExact(rowData[2], \"dd/MM/yyyy HH:mm:ss\", System.Globalization.CultureInfo.InvariantCulture);\n\n                \n\n                edr.TrafficType = rowData[3];\n                edr.TypeMessage = rowData[4];\n                edr.DirectionTraffic = rowData[5];\n                edr.Calling = rowData[6];\n                edr.Called = rowData[7];\n                edr.TypeNet = rowData[8];\n                edr.SourceOperator = rowData[9];\n                edr.DestinationOperator = rowData[10];\n                edr.SourceArea = rowData[11];\n                edr.DestinationArea = rowData[12];\n\n                string bill = rowData[13];\n                edr.Bill = !string.IsNullOrEmpty(bill) ? int.Parse(bill) : default(int?);\n\n                edr.Unit = rowData[14];\n\n                string credit = rowData[15];\n                edr.Credit = !string.IsNullOrEmpty(credit) ? decimal.Parse(credit) : default(decimal?);\n\n                string balance = rowData[16];\n                edr.Balance = !string.IsNullOrEmpty(balance) ? decimal.Parse(balance) : default(decimal?);\n\n                edr.Bag = rowData[17];\n\n                string amount = rowData[18];\n                edr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal?);\n\n                edr.TypeConsumed = rowData[19];\n                edr.PricePlan = rowData[20];\n                edr.Promotion = rowData[21];\n\n                string parentIdCDR = rowData[24];\n                edr.ParentIdCDR = !string.IsNullOrEmpty(parentIdCDR) ? long.Parse(parentIdCDR) : default(long?);\n\n                edr.FileName = rowData[25];\n                edr.FileDate = DateTime.ParseExact(rowData[26], \"dd/MM/yyyy HH:mm:ss\", System.Globalization.CultureInfo.InvariantCulture);\n                edr.CreationDate = creationDate;\n\n                mmsEDRs.Add(edr);\n            }\n\n            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(mmsEDRs, \"#RECORDSCOUNT# of Raw EDRs\");\n            mappedBatches.Add(\"Message EDR Storage Stage\", batch);\n\n            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":-101}')
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