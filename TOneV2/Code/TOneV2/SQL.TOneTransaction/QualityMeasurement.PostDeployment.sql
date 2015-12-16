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
MERGE INTO runtime.[SchedulerTaskActionType] AS Target 
USING (VALUES 
	(3, N'Supplier Synchronize', N'{"URL":"/Client/Modules/QM_BusinessEntity/Views/Supplier/SchedulerTaskAction/SupplierSynchronizeTemplate.html","SystemType":false,"FQTN":"QM.BusinessEntity.Business.SupplierSyncTaskAction, QM.BusinessEntity.Business"}'),
	(4, N'Profile Synchronize', N'{"URL":"/Client/Modules/QM_CLITester/Views/Profile/SchedulerTaskAction/ProfileSynchronizeTemplate.html","SystemType":false,"FQTN":"QM.CLITester.Business.ProfileSyncTaskAction, QM.CLITester.Business"}'),
	(5, N'Zone Synchronize', N'{"URL":"/Client/Modules/QM_BusinessEntity/Views/Zone/SchedulerTaskAction/ZoneSynchronizeTemplate.html","SystemType":false,"FQTN":"QM.BusinessEntity.Business.ZoneSyncTaskAction, QM.BusinessEntity.Business"}'),
	(6, N'CLI Tester - Initiate Test', N'{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/InitiateTestTemplate.html","SystemType":false,"FQTN":"QM.CLITester.Business.InitiateTestTaskAction, QM.CLITester.Business"}'),
	(7, N'CLI Tester - Download Test Result', N'{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/TestProgressTemplate.html","SystemType":false,"FQTN":"QM.CLITester.Business.TestProgressTaskAction, QM.CLITester.Business"}')


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
;
