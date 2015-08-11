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
	( N'Administration',      NULL, NULL, NULL, N'glyphicon-flash',        NULL, N'True'),
	( N'Fraud Analysis',      NULL, NULL, NULL, N'glyphicon-indent-right', NULL, N'True'),
	( N'Workflow Managment',  NULL, NULL, NULL, N'glyphicon-qrcode',       NULL, N'True'),
	( N'Data Sources',        NULL, NULL, NULL, N'glyphicon-tasks',        NULL, N'True')
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
	(	N'Users',	N'Users',	N'#/view/Security/Views/UserManagement',	N'1',	N'Root/Administration Module/Users:View'	,NULL,	NULL,	N'0'	,NULL),
	(	N'Groups',	N'Groups',	N'#/view/Security/Views/GroupManagement',	N'1',	N'Root/Administration Module/Groups:View',	NULL,	NULL,	N'0',	NULL),
	(	N'System Entites',	N'System Entites',	N'#/view/Security/Views/BusinessEntityManagement',	N'1',	N'Root/Administration Module/System Entities:View',	NULL,	NULL,	N'0',	NULL),
	(	N'Suspicion Analysis',	N'Suspicion Analysis',	N'#/view/FraudAnalysis/Views/SuspiciousAnalysis/SuspicionAnalysist',	N'2'	,N'Root/Suspicion Analysis Module:View',	NULL	,NULL,	N'0',	2),
	(	N'History',	N'Business Process History',	N'#view/BusinessProcess/Views/BPHistory',	N'3',	N'Root/Business Process Module/History:View',	NULL,	NULL,	N'0',	NULL),
	(	N'Scheduler Service',	N'Scheduler Service'	,N'#/view/Runtime/Views/SchedulerTaskManagement',	N'3',	N'Root/Business Process Module/Management:View',	NULL,	NULL,	N'0'	,NULL),
	(	N'Management',	N'Business Process Management',	N'#view/BusinessProcess/Views/BPDefinitionManagement',	N'3',	N'Root/Business Process Module/Management:View',	NULL,	NULL,	N'0',	NULL),
	(	N'Dashboard',	N'Dashboard',	N'#/view/FraudAnalysis/Views/Output/Dashboard',	N'2',	N'Root/Dashboard Module:View',	NULL	,NULL,	N'0',	3),
	(	N'Detection Strategies'	,N'Detection Strategies'	,N'#/view/FraudAnalysis/Views/Strategy/StrategyManagement',	N'2',	N'Root/Strategy Module:View'	,NULL,	NULL,	N'0',	1),
	(	N'Datasource Management'	,N'Management',	N'#/view/Integration/Views/DataSourceManagement',	N'4',	N'Root/Integration Module:View',	NULL,	NULL,	N'0'	, NULL),
	(	N'Logs'	,N'Logs',	N'#/view/Integration/Views/DataSourceLogManagement',	N'4',	N'Root/Integration Module:View',	NULL,	NULL,	N'0'	, NULL),
	(	N'Imported Batches'	,N'Imported Batches',	N'#/view/Integration/Views/DataSourceImportedBatchManagement',	N'4',	N'Root/Integration Module:View',	NULL,	NULL,	N'0'	, NULL)
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


--[sec].[BusinessEntityModule]----
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Root',null,0,'["View","Full Control"]'),
(2,'Strategy Module',1,0,'["View","Add","Edit", "Full Control"]'),
(3,'Suspicion Analysis Module',1,0,'["View"]'),
(4,'Reporting Module',1,0,'["View"]'),
(5,'Dashboard Module',1,0,'["View"]'),
(6,'Business Process Module',1,0,'["View"]'),
(7,'Integration Module',1,0,'["View"]'),
(8,'Administration Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ParentId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[ParentId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[ParentId],s.[BreakInheritance],s.[PermissionOptions])
when not matched by source then
	delete;
set identity_insert [sec].[BusinessEntityModule] off;


--[sec].[BusinessEntity]-----
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Users',8,0,'["View", "Add", "Edit", "Reset Password"]'),
(2,'Groups',8,0,'["View", "Add", "Edit", "Delete"]'),
(3,'System Entities',8,0,'["View", "Assign Permissions"]'),
(4,'History',6,0,'["View"]'),
(5,'Managment',6,0,'["View"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions])
when not matched by source then
	delete;
set identity_insert [sec].[BusinessEntity] off;






