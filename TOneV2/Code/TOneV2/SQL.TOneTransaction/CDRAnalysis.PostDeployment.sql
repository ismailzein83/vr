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
;with cte_data([ID],[Name],[Title],[Stages])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1, N'NormalCDRImportDefinition', N'Normal CDR Import Definition', N'{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Map Normal CDRs","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.Fzero.CDRImport.QueueActivators.NormalCDRFlowStageItemType, Vanrise.Fzero.CDRImport.QueueActivators"},"QueueActivator":{"$type":"Vanrise.Queueing.Entities.CustomQueueActivator, Vanrise.Queueing.Entities","FQTN":"Vanrise.Fzero.CDRImport.QueueActivators.NormalizeCDRActivator, Vanrise.Fzero.CDRImport.QueueActivators","ConfigId":3}},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Save Normal CDRs","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.Fzero.CDRImport.QueueActivators.NormalCDRFlowStageItemType, Vanrise.Fzero.CDRImport.QueueActivators"},"QueueActivator":{"$type":"Vanrise.Queueing.Entities.CustomQueueActivator, Vanrise.Queueing.Entities","FQTN":"Vanrise.Fzero.CDRImport.QueueActivators.SaveCDRActivator, Vanrise.Fzero.CDRImport.QueueActivators","ConfigId":3},"SourceStages":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Map Normal CDRs"]}}]}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Stages]))
merge	[queue].[ExecutionFlowDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Stages] = s.[Stages]
when not matched by target then
	insert([ID],[Name],[Title],[Stages])
	values(s.[ID],s.[Name],s.[Title],s.[Stages])
when not matched by source then
	delete;
set identity_insert [queue].[ExecutionFlowDefinition] off;




