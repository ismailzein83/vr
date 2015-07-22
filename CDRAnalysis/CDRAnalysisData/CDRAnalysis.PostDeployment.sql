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




MERGE INTO FraudAnalysis.[CallClass] AS Target 
USING (VALUES 
	(N'1', N'Administration', N'NULL', N'NULL', N'NULL', N'glyphicon-flash', N'NULL', N'True'),
	(N'2', N'Fraud Analysis', N'NULL', N'NULL', N'NULL', N'glyphicon-flash', N'NULL', N'True'),
	(N'3', N'Business Process', N'NULL', N'NULL', N'1', N'NULL', N'NULL', N'True')
) 
AS Source ([Description]  ,[NetType])
ON Target.[Description] = Source.[Description] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[NetType] = Source.[NetType]
			
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Description]  ,[NetType])
VALUES ([Description]  ,[NetType])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;



MERGE INTO FraudAnalysis.[CallType] AS Target 
USING (VALUES 
	(N'1', N'1', N'outgoing Voice'),
	(N'2', N'2', N'Incoming Voice Call'),
	(N'3', N'29', N'call Forward'),
	(N'4', N'30', N'Incoming Sms'),
	(N'5', N'31', N'Outgoing Sms'),
	(N'6', N'26', N'Roaming call forward'),
) 
AS Source ([Code] ,[Description])
ON Target.[Code] = Source.[Code] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[Description]  = Source.[Description] 
			
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Code] ,[Description])
VALUES ([Code] ,[Description])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;


MERGE INTO FraudAnalysis.[CaseStatus] AS Target 
USING (VALUES 
	(N'1', N'Pending'),
	(N'2', N'Ignored'),
	(N'3', N'Confirmed'),
	(N'4', N'White List')
) 
AS Source ([Name])
ON Target.[Name] = Source.[Name] 
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Name])
VALUES ([Name])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;


MERGE INTO FraudAnalysis.[Period] AS Target 
USING (VALUES 
	(N'1', N'Hour'),
	(N'2', N'Day')
) 
AS Source ([Description])
ON Target.[Description] = Source.[Description] 
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Description])
VALUES ([Description])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;



MERGE INTO FraudAnalysis.[SubType] AS Target 
USING (VALUES 
	(N'1', N'Prepaid'),
	(N'2', N'Postpaid')
) 
AS Source ([Description])
ON Target.[Description] = Source.[Description] 
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Description])
VALUES ([Description])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;



MERGE INTO FraudAnalysis.[Suspicion_Level] AS Target 
USING (VALUES 
	(N'1', N'Clean'),
	(N'2', N'Suspicious'),
	(N'3', N'Highly Suspicious'),
	(N'4', N'Fraud')
) 
AS Source ([Name])
ON Target.[Name] = Source.[Name] 
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Name])
VALUES ([Name])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;




