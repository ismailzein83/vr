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
('6ED6E42E-5DB0-4E90-BA94-ABD02500A01A','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":false},"StartEffDate":"2017-04-12T12:02:07.812"}',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId]))
merge	[runtime].[ScheduleTask] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[IsEnabled] = s.[IsEnabled],[TaskType] = s.[TaskType],[TriggerTypeId] = s.[TriggerTypeId],[ActionTypeId] = s.[ActionTypeId],[TaskSettings] = s.[TaskSettings],[OwnerId] = s.[OwnerId]
when not matched by target then
	insert([ID],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
	values(s.[ID],s.[Name],s.[IsEnabled],s.[TaskType],s.[TriggerTypeId],s.[ActionTypeId],s.[TaskSettings],s.[OwnerId]);



--[integration].[DataSource]----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;

;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('87ED078D-4F13-4528-BED2-2D81AB1CD49D','File - Teles - Sample Datasource','396A4933-DF4F-49CD-9799-BF605B9F4597','null','6ED6E42E-5DB0-4E90-BA94-ABD02500A01A','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments","Extension":".txt","Mask":"","Directory":"/MultiNet/CDRs_New","ServerIP":"192.168.110.185","UserName":"devftpuser","Password":"P@ssw0rd","ActionAfterImport":0,"BasedOnLastModifiedTime":false,"MaxParallelRuntimeInstances":1},"MapperCustomCode":"Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));\n            var cdrs = new List<dynamic>();\n            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();\n            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(\"ParsedCDR\");\n            try\n            {\n                System.IO.StreamReader sr = ImportedData.StreamReader;\n                while (!sr.EndOfStream)\n                {\n                    string currentLine = sr.ReadLine();\n                    if (string.IsNullOrEmpty(currentLine))\n                        continue;\n                    string[] rowData = currentLine.Split('','');\n                    dynamic cdr = Activator.CreateInstance(mediationCDRRuntimeType) as dynamic;\n                    cdr.TC_VERSIONID = rowData[0].Trim(''\"'');\n                    cdr.TC_CALLID = rowData[13].Trim(''\"'');\n                    cdr.TC_LOGTYPE = rowData[1].Trim(''\"'');\n                    cdr.TC_TIMESTAMP = DateTime.ParseExact(rowData[3].Trim(''\"''), \"yyyyMMddHHmmss:fff\", System.Globalization.CultureInfo.InvariantCulture);\n\n                    cdr.TC_DISCONNECTREASON = rowData[4].Trim(''\"'');\n                    cdr.TC_CALLPROGRESSSTATE = rowData[5].Trim(''\"'');\n                    cdr.TC_ACCOUNT = rowData[6].Trim(''\"'');\n                    cdr.TC_ORIGINATORID = rowData[7].Trim(''\"'');\n                    cdr.TC_ORIGINATORNUMBER = rowData[7].Trim(''\"'');\n                    cdr.TC_ORIGINALFROMNUMBER = rowData[9].Trim(''\"'');\n                    cdr.TC_ORIGINALDIALEDNUMBER = rowData[10].Trim(''\"'');\n                    cdr.TC_TERMINATORID = rowData[11].Trim(''\"'');\n                    cdr.TC_TERMINATORNUMBER = rowData[12].Trim(''\"'');\n                    cdr.TC_INCOMINGGWID = rowData[15].Trim(''\"'');\n                    cdr.TC_OUTGOINGGWID = rowData[16].Trim(''\"'');\n                    cdr.TC_TRANSFERREDCALLID = rowData[20].Trim(''\"'');\n                    cdr.TC_TERMINATORIP = rowData[33].Trim(''\"'');\n                    cdr.TC_ORIGINATORIP = rowData[32].Trim(''\"'');\n                    cdr.TC_REPLACECALLID = rowData[18].Trim(''\"'');\n                    cdr.TC_CALLINDICATOR = rowData[14].Trim(''\"''); \ncdr.FileName = ImportedData.Name;\nDateTime? attemptDateTime = default(DateTime?);\nif(!string.IsNullOrEmpty(rowData[36].Trim(''\"'')))\nattemptDateTime  = (DateTime?)(DateTime.ParseExact(rowData[36].Trim(''\"''), \"yyyyMMddHHmmss:fff\", System.Globalization.CultureInfo.InvariantCulture));\n                    cdr.TC_SESSIONINITIATIONTIME = attemptDateTime;\n                    cdr.TC_SEQUENCENUMBER = rowData[2].Trim(''\"'');\n                    cdrs.Add(cdr);\n\n                }\n            }\n            catch (Exception ex)\n            {\n                throw ex;\n            }\n\n            MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, \"#RECORDSCOUNT# of Raw CDRs\", \"ParsedCDR\");\n            mappedBatches.Add(\"Teles Mediation Stage\", batch);\n            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            return result;","ExecutionFlowId":"359ed26c-61a5-4853-88ae-9102a58db882"}')
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