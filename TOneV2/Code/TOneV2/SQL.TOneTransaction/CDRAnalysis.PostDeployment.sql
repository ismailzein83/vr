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
set nocount on;
set identity_insert [bp].[BPDefinition] on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config],[CreatedTime])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(7,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput','Execute Strategy Process','Vanrise.Fzero.FraudAnalysis.BP.ExecuteStrategyProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/ExecuteStrategyProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/ExecuteStrategyProcessInput_Scheduled.html"}','2015-06-25 13:00:22.830'),
(9,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput','Number Profiling Process','Vanrise.Fzero.FraudAnalysis.BP.NumberProfilingProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/NumberProfilingProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/NumberProfilingProcessInput_Scheduled.html"}','2015-06-25 13:00:22.830'),
(10,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyCasesProcessInput','Assign Strategy Cases Process','Vanrise.Fzero.FraudAnalysis.BP.AssignStrategyCasesProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/AssignStrategyCasesProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/AssignStrategyCasesProcessInput_Scheduled.html"}','2015-06-25 13:00:22.830'),
(12,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.FindRelatedNumbersProcessInput','Find Related Numbers Process','Vanrise.Fzero.FraudAnalysis.BP.FindRelatedNumbersProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/FindRelatedNumbersProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/FindRelatedNumbersProcessInput_Scheduled.html"}','2015-06-25 13:00:22.830')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[FQTN],[Config],[CreatedTime]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config],[CreatedTime] = s.[CreatedTime]
when not matched by target then
	insert([ID],[Name],[Title],[FQTN],[Config],[CreatedTime])
	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config],s.[CreatedTime])
when not matched by source then
	delete;
set identity_insert [bp].[BPDefinition] off;

--[queue].[ExecutionFlowDefinition]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [queue].[ExecutionFlowDefinition] on;
;with cte_data([ID],[Name],[Title],[ExecutionTree])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'CDRImportDefintion','CDR Import Definition','{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowTree, Vanrise.Queueing.Entities","Activities":{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.BaseExecutionActivity, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueStageExecutionActivity, Vanrise.Queueing.Entities","StageName":"CDR Import","QueueName":"CDRQueue","QueueSettings":{"$type":"Vanrise.Queueing.Entities.QueueSettings, Vanrise.Queueing.Entities","SingleConcurrentReader":false,"QueueActivatorFQTN":"Vanrise.Fzero.CDRImport.Business.CDRImportActivator, Vanrise.Fzero.CDRImport.Business, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"},"QueueTypeFQTN":"Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch, Vanrise.Fzero.CDRImport.Entities, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"}]}}')
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

--[queue].[ExecutionFlow]---------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [queue].[ExecutionFlow] on;
;with cte_data([ID],[Name],[ExecutionFlowDefinitionID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'CDR Import Execution Flow',1)
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


