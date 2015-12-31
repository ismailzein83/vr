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

--queue.ExecutionFlowDefinition---------------------------------------------------------------------
----------------------------------------------------------------------------------------------------




set nocount on;
set identity_insert [queue].[ExecutionFlowDefinition] on;
;with cte_data([ID],[Name],[Title],[ExecutionTree])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(3,'DefExecFlow','Default Exc Flow','{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowTree, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.ParallelExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Store Raw CDRs","QueueName":"RawCDRs","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.StoreRawCDRActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.CDRBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.SequenceExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Generate Billing CDRs","QueueName":"BillingCDRs","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.GenerateBillingCDRActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.CDRBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.SplitExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.SequenceExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Generate CDR Prices","QueueName":"CDRPrices","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.GenerateCDRPricesActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.CDRMainBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.ParallelExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Store Main CDRs","QueueName":"MainCDRs","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.StoreCDRMainActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.CDRMainBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.SequenceExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Generate BillingStats","QueueName":"BillingStats","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.GenerateBillingStatsActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.CDRMainBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Store BillingStats","QueueName":"StoreBillingStats","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.StoreBillingStatsActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.BillingStatisticBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"}]}}]}}]}},{"$type":"Vanrise.Queueing.Entities.ParallelExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.SequenceExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Generate Stats","QueueName":"Stats","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.GenerateStatsActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.CDRBillingBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.ParallelExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Store Stats","QueueName":"StoreStats","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.StoreStatsActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.TrafficStatisticByIntervalBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.SequenceExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Generate Daily Stats","QueueName":"DailyStats","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.GenerateDailyStatsActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.TrafficStatisticByIntervalBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Store Daily Stats","QueueName":"StoreDailyStats","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.StoreDailyStatsActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.TrafficStatisticDailyBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"}]}}]}}]}},{"$type":"Vanrise.Queueing.Entities.SequenceExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Generate StatsByCode","QueueName":"StatsByCode","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.GenerateStatsByCodeActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.CDRBillingBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Store StatsByCode","QueueName":"StoreStatsByCode","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.StoreStatsByCodeActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.TrafficStatisticByCodeBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"}]}}]}},{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Store Invalid CDRs","QueueName":"InvalidCDRs","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.StoreCDRInvalidActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.CDRInvalidBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Store Failed CDRs","QueueName":"FailedCDRs","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"TOne.WhS.CDRProcessing.QueueActivators.StoreCDRFailedActivator, TOne.WhS.CDRProcessing.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"TOne.WhS.CDRProcessing.Entities.CDRFailedBatch, TOne.WhS.CDRProcessing.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"}]}}]}}]}}]}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ExecutionTree]))
merge	[queue].[ExecutionFlowDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ExecutionTree] = s.[ExecutionTree]
when not matched by target then
	insert([ID],[Name],[Title],[ExecutionTree])
	values(s.[ID],s.[Name],s.[Title],s.[ExecutionTree])
when not matched by source then
	delete;
set identity_insert [queue].[ExecutionFlowDefinition] off;

--DELETE ALL QUEUES USED-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
DELETE FROM [queue].QueueInstance
DELETE FROM [queue].QueueItemType
DELETE FROM [queue].QueueSubscription

--queue.ExecutionFlow-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [queue].[ExecutionFlow] on;
;with cte_data([ID],[Name],[ExecutionFlowDefinitionID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Default Exx Flow',3)
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
	values(s.[ID],s.[Name],s.[ExecutionFlowDefinitionID])
when not matched by source then
	delete;
set identity_insert [queue].[ExecutionFlow] off;

--integration.DataSource----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [integration].[DataSource] on;
;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Def Data Source',3,'{"$type":"Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterState, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments","LastImportedId":"0"}',38,'{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterArgument, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments","ConnectionString":"Server=192.168.110.185;Database=TestCDRs;User ID=Development;Password=dev!123","Query":"select TOP (50000) *  from CDR Where CDRID>{LastImportedId}"},"MapperCustomCode":"LogVerbose(\"Started\");          \nTOne.WhS.CDRProcessing.Entities.CDRBatch batch = new TOne.WhS.CDRProcessing.Entities.CDRBatch(); \nbatch.CDRs = new List<TOne.WhS.CDRProcessing.Entities.CDR>();\nVanrise.Integration.Entities.DBReaderImportedData ImportedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));           \nIDataReader reader = ImportedData.Reader;        \nstring index = ImportedData.LastImportedId;              \nwhile (reader.Read())            {             \nTOne.WhS.CDRProcessing.Entities.CDR cdr = new TOne.WhS.CDRProcessing.Entities.CDR();                 \ncdr.Attempt= (DateTime) reader[\"AttemptDateTime\"];                \ncdr.ID=(long) reader[\"CDRID\"];  \ncdr.SwitchID= Convert.ToInt32(reader[\"SwitchID\"]);               \ncdr.InCarrier=reader[\"IN_CARRIER\"] as string;                 \ncdr.InTrunk=reader[\"IN_TRUNK\"] as string;                 \ncdr.CDPN=reader[\"CDPN\"] as string;                  \ncdr.OutTrunk=reader[\"OUT_TRUNK\"] as string;                 \ncdr.OutCarrier=reader[\"OUT_CARRIER\"] as string;                 \ncdr.DurationInSeconds= Convert.ToInt32(reader[\"DurationInSeconds\"]);         \ncdr.Alert= (DateTime) reader[\"AlertDateTime\"];                  \ncdr.Connect=(DateTime)reader[\"ConnectDateTime\"];                  \ncdr.Disconnect=(DateTime)reader[\"DisconnectDateTime\"];                  \ncdr.CGPN=reader[\"CDPN\"] as string;                  \ncdr.PortOut=reader[\"OUT_IP\"] as string;                  \ncdr.PortIn=reader[\"IN_IP\"] as string;                  \ncdr.ReleaseCode=reader[\"CAUSE_FROM_RELEASE_CODE\"] as string;                 \ncdr.ReleaseSource=reader[\"CAUSE_TO_RELEASE_CODE\"] as string;    \nindex = cdr.ID.ToString();                   \nbatch.CDRs.Add(cdr);             }               \nImportedData.LastImportedId = index;               \nmappedBatches.Add(\"Store Raw CDRs\", batch);              \nVanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();              \nresult.Result = Vanrise.Integration.Entities.MappingResult.Valid;             \nLogVerbose(\"Finished\");              \nreturn result;","ExecutionFlowId":1}')
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
	values(s.[ID],s.[Name],s.[AdapterID],s.[AdapterState],s.[TaskId],s.[Settings])
when not matched by source then
	delete;
set identity_insert [integration].[DataSource] off;

--runtime.ScheduleTask------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [runtime].[ScheduleTask] on;
;with cte_data([ID],[Name],[IsEnabled],[TaskType],[Status],[LastRunTime],[NextRunTime],[LockedByProcessID],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId],[ExecutionInfo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(36,'xs',1,1,2,'2015-09-18 11:01:41.163',null,null,1,1,'{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskActionArgument":{"$type":"Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.WFTaskActionArgument, Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments","BPDefinitionID":1,"ProcessInputArguments":{"$type":"TOne.CDRProcess.Arguments.DailyRepricingProcessInput, TOne.CDRProcess.Arguments","RepricingDay":"2015-09-12T15:36:00Z","GenerateTrafficStatistic":false,"DivideProcessIntoSubProcesses":true,"ProcessName":"TOne.CDRProcess.Arguments.DailyRepricingProcessInput"}},"TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":0.5,"IntervalType":0,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"},"StartEffDate":"2015-10-13T20:02:00Z","EndEffDate":"2015-09-18T02:51:00Z"}',null,null),
(38,'Data Source Task',0,0,0,null,null,null,1,2,'{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":0,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"},"StartEffDate":"2015-12-29T17:33:00Z"}',0,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[IsEnabled],[TaskType],[Status],[LastRunTime],[NextRunTime],[LockedByProcessID],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId],[ExecutionInfo]))
merge	[runtime].[ScheduleTask] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[IsEnabled] = s.[IsEnabled],[TaskType] = s.[TaskType],[Status] = s.[Status],[LastRunTime] = s.[LastRunTime],[NextRunTime] = s.[NextRunTime],[LockedByProcessID] = s.[LockedByProcessID],[TriggerTypeId] = s.[TriggerTypeId],[ActionTypeId] = s.[ActionTypeId],[TaskSettings] = s.[TaskSettings],[OwnerId] = s.[OwnerId],[ExecutionInfo] = s.[ExecutionInfo]
when not matched by target then
	insert([ID],[Name],[IsEnabled],[TaskType],[Status],[LastRunTime],[NextRunTime],[LockedByProcessID],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId],[ExecutionInfo])
	values(s.[ID],s.[Name],s.[IsEnabled],s.[TaskType],s.[Status],s.[LastRunTime],s.[NextRunTime],s.[LockedByProcessID],s.[TriggerTypeId],s.[ActionTypeId],s.[TaskSettings],s.[OwnerId],s.[ExecutionInfo])
when not matched by source then
	delete;
set identity_insert [runtime].[ScheduleTask] off;

