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
	( N'Administration', NULL, NULL, NULL, N'glyphicon-flash', NULL, N'True'),
	( N'Fraud Analysis', NULL, NULL, NULL, N'glyphicon-flash', NULL, N'True'),
	( N'Business Process', NULL, NULL, N'1', NULL, NULL, N'True')
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
	(	N'Users',	N'Users',	N'#/view/Security/Views/UserManagement',	N'1',	NULL	,NULL,	NULL,	N'0'	,NULL),
	(	N'Groups',	N'Groups',	N'#/view/Security/Views/RoleManagement',	N'1',	NULL,	NULL,	NULL,	N'0',	NULL),
	(	N'System Entites',	N'System Entites',	N'#/view/Security/Views/BusinessEntityManagement',	N'1',	NULL,	NULL,	NULL,	N'0',	NULL),
	(	N'Strategy Management',	N'Strategy Management',	N'#/view/FraudAnalysis/Views/Strategy/StrategyManagement',	N'2'	,NULL,	NULL	,NULL,	N'0',	NULL),
	(	N'Suspicion Analysis',	N'Suspicion Analysis',	N'#/view/FraudAnalysis/Views/SuspiciousAnalysis/SuspicionAnalysis',	N'2',	NULL,	NULL,	NULL,	N'0',	NULL),
	(	N'History',	N'Business Process History'	,N'#view/BusinessProcess/Views/BPManagement',	N'3',	NULL,	NULL,	NULL,	N'0'	,NULL),
	(	N'Management',	N'Business Process Management',	N'#view/BusinessProcess/Views/BPDefinitionManagement',	N'3',	NULL,	NULL,	NULL,	N'0',	NULL),
	(	N'Scheduler Service',	N'Scheduler Service',	N'#/view/Runtime/Views/SchedulerTaskManagement',	N'1',	NULL,	NULL	,NULL,	N'0',	NULL),
	(	N'Data Sources'	,N'Data Sources'	,N'#/view/Integration/Views/DataSourceManagement',	N'1',	NULL	,NULL,	NULL,	N'0',	NULL),
	(	N'Dashboard'	,N'Dashboard',	N'#/view/FraudAnalysis/Views/Output/Dashboard',	N'2',	NULL,	NULL,	NULL,	N'0'	, NULL)
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






