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
	(N'ZAINIQ', N'1'),
	(N'VAS', N'1'),
	(N'INV', N'1'),
	(N'INT', N'2'),
	(N'KOREKTEL', N'0'),
	(N'ASIACELL', N'0')
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
	( N'1', N'outgoing Voice'),
	( N'2', N'Incoming Voice Call'),
	( N'29', N'call Forward'),
	( N'30', N'Incoming Sms'),
	( N'31', N'Outgoing Sms'),
	( N'26', N'Roaming call forward')
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
	( N'1',N'Open'),
	( N'2',N'Pending'),
	( N'3',N'Closed: Fraud'),
	( N'4',N'Closed: White List'),
	( N'5',N'Cancelled')
) 
AS Source ([Id], [Name])
ON Target.[Name] = Source.[Name] 
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Id], [Name])
VALUES ([Id], [Name])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;


MERGE INTO FraudAnalysis.[Period] AS Target 
USING (VALUES 
	( N'Hourly'),
	( N'Daily')
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
	( N'Prepaid'),
	( N'Postpaid')
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



MERGE INTO FraudAnalysis.[SuspicionLevel] AS Target 
USING (VALUES 
	( N'Clean'),
	( N'Suspicious'),
	( N'Highly Suspicious'),
	( N'Fraud')
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




