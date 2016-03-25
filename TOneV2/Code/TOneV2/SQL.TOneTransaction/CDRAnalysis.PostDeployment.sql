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




