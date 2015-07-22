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



MERGE INTO bp.[BPDefinition] AS Target 
USING (VALUES 
	(N'Vanrise.Fzero.CDRImport.BP.Arguments.CDRImportProcessInput', N'CDR Import Process', N'Vanrise.Fzero.CDRImport.BP.CDRImportProcess, Vanrise.Fzero.CDRImport.BP', N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/CDRImportProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/CDRImportProcessInput_Scheduled.html"}'),
	(N'Vanrise.Fzero.CDRImport.BP.Arguments.SaveCDRToDBProcessInput', N'Save CDR To DB Process', N'Vanrise.Fzero.CDRImport.BP.SaveCDRToDBProcess, Vanrise.Fzero.CDRImport.BP', N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/SaveCDRToDBProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/SaveCDRToDBProcessInput_Scheduled.html"}'),
	(N'Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput', N'Execute Strategy Process', N'Vanrise.Fzero.FraudAnalysis.BP.ExecuteStrategyProcess, Vanrise.Fzero.FraudAnalysis.BP', N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/ExecuteStrategyProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/ExecuteStrategyProcessInput_Scheduled.html"}')

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
