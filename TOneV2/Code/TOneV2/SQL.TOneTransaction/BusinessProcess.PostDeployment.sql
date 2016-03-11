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
VALUES ([ID], [Description], [IsOpened]);
