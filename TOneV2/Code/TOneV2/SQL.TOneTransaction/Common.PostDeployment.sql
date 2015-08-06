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
	(0, N'New', 1),
	(10, N'Running', 1),
	(20, N'Process Failed', 1),
	(50, N'Completed', 0),
	(60, N'Aborted', 0),
	(70, N'Suspended', 0),
	(80, N'Terminated', 0)

) 
AS Source ([ID], [Description], [IsOpened])
ON Target.[ID] = Source.[ID] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET [Description] = Source.[Description] ,
			[IsOpened] = Source.[IsOpened]
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([ID], [Description], [IsOpened])
VALUES ([ID], [Description], [IsOpened]) 
-- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;



MERGE INTO runtime.[SchedulerTaskTriggerType] AS Target 
USING (VALUES 
	(1, N'Timer', N'{"URL":"/Client/Modules/Runtime/Views/TriggerTemplates/TimerTriggerTemplate.html"}')
) 
AS Source ([ID], [Name], [TriggerTypeInfo])
ON Target.[ID] = Source.[ID] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[ID] = Source.[ID],
			[Name] = Source.[Name],
			[TriggerTypeInfo]  = Source.[TriggerTypeInfo]
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([ID], [Name], [TriggerTypeInfo])
VALUES ([ID], [Name], [TriggerTypeInfo])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;

MERGE INTO runtime.[SchedulerTaskActionType] AS Target 
USING (VALUES 
	(1, N'Workflow', N'{"URL":"/Client/Modules/Runtime/Views/ActionTemplates/WFActionTemplate.html", "SystemType":false}'),
	(2, N'Data Source', N'{"URL":"", "SystemType":true}')
) 
AS Source ([ID], [Name], [ActionTypeInfo])
ON Target.[ID] = Source.[ID] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[ID] = Source.[ID],
			[Name] = Source.[Name],
			[ActionTypeInfo]  = Source.[ActionTypeInfo]
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([ID], [Name], [ActionTypeInfo])
VALUES ([ID], [Name], [ActionTypeInfo])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;

MERGE INTO integration.[AdapterType] AS Target 
USING (VALUES 
	(1, N'File Receive Adapter', N'{"AdapterTemplateURL": "/Client/Modules/Integration/Views/AdapterTemplates/FileReceiveAdapterTemplate.html", "FQTN": "Vanrise.Integration.Adapters.FileReceiveAdapter.FileReceiveAdapter, Vanrise.Integration.Adapters.FileReceiveAdapter"}'),
	(2, N'FTP Receive Adapter', N'{"AdapterTemplateURL": "/Client/Modules/Integration/Views/AdapterTemplates/FTPReceiveAdapterTemplate.html","FQTN": "Vanrise.Integration.Adapters.FTPReceiveAdapter.FTPReceiveAdapter, Vanrise.Integration.Adapters.FTPReceiveAdapter"}'),
	(3, N'SQL Receive Adapter', N'{"AdapterTemplateURL": "/Client/Modules/Integration/Views/AdapterTemplates/DBReceiveAdapterTemplate.html", "FQTN": "Vanrise.Integration.Adapters.SQLReceiveAdapter.SQLReceiveAdapter, Vanrise.Integration.Adapters.SQLReceiveAdapter"}')
) 
AS Source ([ID], [Name], [Info])
ON Target.[ID] = Source.[ID] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[ID] = Source.[ID],
			[Name] = Source.[Name],
			[Info]  = Source.[Info]
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([ID], [Name], [Info])
VALUES ([ID], [Name], [Info])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;

