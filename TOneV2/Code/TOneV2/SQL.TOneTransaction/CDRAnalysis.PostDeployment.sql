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

--[bp].[BPDefinition]-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------


MERGE INTO bp.[BPDefinition] AS Target 
USING (VALUES 
('Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput','Execute Strategy Process','Vanrise.Fzero.FraudAnalysis.BP.ExecuteStrategyProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ScheduledExecEditor":"vr-cdr-fraudanalysis-executestrategy","ManualExecEditor":"vr-cdr-fraudanalysis-executestrategy-manual","RetryOnProcessFailed":false, "HasChildProcesses":true}'),
('Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyForNumberRangeProcessInput','Execute Strategy Process for Number Range','Vanrise.Fzero.FraudAnalysis.BP.ExecuteStrategyForNumberRangeProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
('Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput','Number Profiling Process','Vanrise.Fzero.FraudAnalysis.BP.NumberProfilingProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ScheduledExecEditor":"vr-cdr-fraudanalysis-numberprofiling","ManualExecEditor":"vr-cdr-fraudanalysis-numberprofiling-manual","RetryOnProcessFailed":false, "HasChildProcesses":true}'),
('Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingForNumberRangeProcessInput','Number Profiling Process for Number Range','Vanrise.Fzero.FraudAnalysis.BP.NumberProfilingForNumberRangeProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
('Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyCasesProcessInput','Assign Strategy Cases Process','Vanrise.Fzero.FraudAnalysis.BP.AssignStrategyCasesProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ScheduledExecEditor":"vr-cdr-fraudanalysis-assignstrategy","ManualExecEditor":"vr-cdr-fraudanalysis-assignstrategy-manual","RetryOnProcessFailed":false}'),
('Vanrise.Fzero.FraudAnalysis.BP.Arguments.FindRelatedNumbersProcessInput','Find Related Numbers Process','Vanrise.Fzero.FraudAnalysis.BP.FindRelatedNumbersProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ScheduledExecEditor":"vr-cdr-fraudanalysis-findrelatednumbers","ManualExecEditor":"vr-cdr-fraudanalysis-findrelatednumbers-manual","RetryOnProcessFailed":false}'),
('Vanrise.Fzero.CDRImport.BP.Arguments.StagingtoCDRProcessInput','Staging to CDR Process','Vanrise.Fzero.CDRImport.BP.StagingtoCDRProcess, Vanrise.Fzero.CDRImport.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ScheduledExecEditor":"vr-cdr-pstnbe-stagingtocdr","ManualExecEditor":"vr-cdr-pstnbe-stagingtocdr-manual","RetryOnProcessFailed":false}'),
('Vanrise.Fzero.FraudAnalysis.BP.Arguments.FillDataWarehouseProcessInput','Fill Data Warehouse Process','Vanrise.Fzero.FraudAnalysis.BP.FillDataWarehouseProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ScheduledExecEditor":"vr-cdr-fraudanalysis-filldatawarehouse","ManualExecEditor":"vr-cdr-fraudanalysis-filldatawarehouse-manual","RetryOnProcessFailed":false}'),
('Vanrise.Fzero.FraudAnalysis.BP.Arguments.CancelStrategyExecutionProcessInput','Cancel Strategy Execution Process','Vanrise.Fzero.FraudAnalysis.BP.CancelStrategyExecutionProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ScheduledExecEditor":"","ManualExecEditor":"","RetryOnProcessFailed":false}')
) 

AS Source ([Name], [Title], [FQTN], [Config])
ON Target.[Name] = Source.[Name] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[Title] = Source.[Title],
			[FQTN] = Source.[FQTN],
			[Config]  = Source.[Config]
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Name], [Title], [FQTN], [Config])
VALUES ([Name], [Title], [FQTN], [Config])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;


--[queue].[ExecutionFlowDefinition]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [queue].[ExecutionFlowDefinition] on;
;with cte_data([ID],[Name],[Title],[ExecutionTree])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'CDRImportDefintion','CDR Import Defintion','{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowTree, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Normalize CDRs","QueueName":"NormalizeCDRQueue","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"Vanrise.Fzero.CDRImport.QueueActivators.NormalizeCDRActivator, Vanrise.Fzero.CDRImport.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch, Vanrise.Fzero.CDRImport.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"CDR Import","QueueName":"CDRQueue","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"Vanrise.Fzero.CDRImport.QueueActivators.SaveCDRActivator, Vanrise.Fzero.CDRImport.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch, Vanrise.Fzero.CDRImport.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"}]}}'),
(2,'StagingCDRImportDefintion','Staging CDR Import Defintion','{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowTree, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.SequenceExecutionActivity, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Normalize CDRs","QueueName":"NormalizeCDRQueue","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"Vanrise.Fzero.CDRImport.QueueActivators.NormalizeStagingCDRActivator, Vanrise.Fzero.CDRImport.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch, Vanrise.Fzero.CDRImport.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"Save CDRs","QueueName":"StoreCDRQueue","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"Vanrise.Fzero.CDRImport.QueueActivators.SaveStagingCDRActivator, Vanrise.Fzero.CDRImport.QueueActivators, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch, Vanrise.Fzero.CDRImport.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"}]}}]}}')
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




