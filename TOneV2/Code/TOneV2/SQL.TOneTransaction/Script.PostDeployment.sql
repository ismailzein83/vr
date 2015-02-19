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
MERGE INTO bp.LKUP_ExecutionStatus AS Target 
USING (VALUES 
	(0, N'New'),
	(10, N'Running'),
	(20, N'Process Failed'),
	(50, N'Completed'),
	(60, N'Aborted'),
	(70, N'Suspended'),
	(80, N'Terminated')

) 
AS Source ([ID], [Description])
ON Target.[ID] = Source.[ID] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET [Description] = Source.[Description] 
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([ID], [Description])
VALUES ([ID], [Description]) 
-- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;


MERGE INTO bp.[BPDefinition] AS Target 
USING (VALUES 
	(N'DailyRepricingProcess', N'Daily Repricing Process', N'TOne.CDRProcess.DailyRepricingProcess, TOne.CDRProcess', N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":2,"RetryOnProcessFailed":false}'),
	(N'RoutingProcess', N'Routing Process', N'TOne.LCRProcess.RoutingProcess, TOne.LCRProcess', N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}')


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
--WHEN NOT MATCHED BY SOURCE THEN 
--DELETE
;

