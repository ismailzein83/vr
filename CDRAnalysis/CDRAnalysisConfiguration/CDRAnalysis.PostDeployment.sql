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




MERGE INTO sec.[Module] AS Target 
USING (VALUES 
	( N'Administration', N'NULL', N'NULL', N'NULL', N'glyphicon-flash', N'NULL', N'True'),
	( N'Fraud Analysis', N'NULL', N'NULL', N'NULL', N'glyphicon-flash', N'NULL', N'True'),
	( N'Business Process', N'NULL', N'NULL', N'1', N'NULL', N'NULL', N'True')
) 
AS Source ([Name]  ,[Title]  ,[Url]  ,[ParentId]  ,[Icon]  ,[Rank]  ,[AllowDynamic])
ON Target.[Name] = Source.[Name] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[Title] = Source.[Title],
			[Url]  = Source.[Url], 
			[ParentId]  = Source.[ParentId], 
			[Icon]  = Source.[Icon], 
			[Rank]  = Source.[Rank], 
			[AllowDynamic]  = Source.[AllowDynamic] 
			
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Name]  ,[Title]  ,[Url]  ,[ParentId]  ,[Icon]  ,[Rank]  ,[AllowDynamic])
VALUES ([Name]  ,[Title]  ,[Url]  ,[ParentId]  ,[Icon]  ,[Rank]  ,[AllowDynamic])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;




MERGE INTO sec.[View] AS Target 
USING (VALUES 
	(	N'Users',	N'Users',	N'#/view/Security/Views/UserManagement',	N'1',	N'NULL'	,N'NULL',	N'NULL',	N'0'	,N'NULL'),
	(	N'Groups',	N'Groups',	N'#/view/Security/Views/RoleManagement',	N'1',	N'NULL',	N'NULL',	N'NULL',	N'0',	N'NULL'),
	(	N'System Entites',	N'System Entites',	N'#/view/Security/Views/BusinessEntityManagement',	N'1',	N'NULL',	N'NULL',	N'NULL',	N'0',	N'NULL'),
	(	N'Strategy Management',	N'Strategy Management',	N'#/view/FraudAnalysis/Views/Strategy/StrategyManagement',	N'2'	,N'NULL',	N'NULL'	,N'NULL',	N'0',	N'NULL'),
	(	N'Suspicion Analysis',	N'Suspicion Analysis',	N'#/view/FraudAnalysis/Views/SuspiciousAnalysis/SuspicionAnalysis',	N'2',	N'NULL',	N'NULL',	N'NULL',	N'0',	N'NULL'),
	(	N'History',	N'Business Process History'	,N'#view/BusinessProcess/Views/BPManagement',	N'3',	N'NULL',	N'NULL',	N'NULL',	N'0'	,N'NULL'),
	(	N'Management',	N'Business Process Management',	N'#view/BusinessProcess/Views/BPDefinitionManagement',	N'3',	N'NULL',	N'NULL',	N'NULL',	N'0',	N'NULL'),
	(	N'Scheduler Service',	N'Scheduler Service',	N'#/view/Runtime/Views/SchedulerTaskManagement',	N'1',	N'NULL',	N'NULL'	,N'NULL',	N'0',	N'NULL'),
	(	N'Data Sources'	,N'Data Sources'	,N'#/view/Integration/Views/DataSourceManagement',	N'1',	N'NULL'	,N'NULL',	N'NULL',	N'0',	N'NULL'),
	(	N'Dashboard'	,N'Dashboard',	N'#/view/FraudAnalysis/Views/Output/Dashboard',	N'2',	N'NULL',	N'NULL',	N'NULL',	N'0'	,N'NULL')
) 
AS Source ([Name]  ,[Title]  ,[Url]  ,[Module]  ,[RequiredPermissions]  ,[Audience]  ,[Content]  ,[Type]    ,[Rank])
ON Target.[Name] = Source.[Name] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[Title] = Source.[Title],
			[Url] = Source.[Url],
			[Module]  = Source.[Module],
			[RequiredPermissions]  = Source.[RequiredPermissions],
			[Audience]  = Source.[Audience],
			[Content]  = Source.[Content],
			[Type]  = Source.[Type],
			[Rank]  = Source.[Rank]
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Name]  ,[Title]  ,[Url]  ,[Module]  ,[RequiredPermissions]  ,[Audience]  ,[Content]  ,[Type]    ,[Rank])
VALUES ([Name]  ,[Title]  ,[Url]  ,[Module]  ,[RequiredPermissions]  ,[Audience]  ,[Content]  ,[Type]    ,[Rank])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;






