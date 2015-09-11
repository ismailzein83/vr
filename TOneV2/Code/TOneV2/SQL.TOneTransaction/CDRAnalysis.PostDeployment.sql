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
(10,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyCasesProcessInput','Assign Strategy Cases Process','Vanrise.Fzero.FraudAnalysis.BP.AssignStrategyCasesProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/AssignStrategyCasesProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/AssignStrategyCasesProcessInput_Scheduled.html"}','2015-06-25 13:00:22.830')
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


