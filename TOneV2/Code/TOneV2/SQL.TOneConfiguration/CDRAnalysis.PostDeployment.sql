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
--[sec].[Module]----
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Administration',null,null,null,'/images/menu-icons/Administration.png',null,1),
(2,'Fraud Analysis',null,null,null,'/images/menu-icons/other.png',null,1),
(3,'Workflow Management',null,null,null,'/images/menu-icons/Business Entities.png',null,1),
(4,'Data Sources',null,null,null,'/images/menu-icons/plug.png',null,1),
(5,'Reports',null,null,null,'/images/menu-icons/busines intel.png',null,1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic])
when not matched by source then
	delete;
set identity_insert [sec].[Module] off;

--[sec].[View]----
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[RequiredPermissions],[Audience],[Content],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(3,'Users','Users','#/view/Security/Views/UserManagement',1,'Root/Administration Module/Users:View',null,null,0,null),
(4,'Groups','Groups','#/view/Security/Views/GroupManagement',1,'Root/Administration Module/Groups:View',null,null,0,null),
(5,'System Entities','System Entities','#/view/Security/Views/BusinessEntityManagement',1,'Root/Administration Module/System Entities:View',null,null,0,null),
(8,'Suspicious Accounts','Suspicious Accounts','#/view/FraudAnalysis/Views/SuspiciousAnalysis/SuspicionAnalysis',2,'Root/Suspicion Analysis Module:View',null,null,0,2),
(11,'History','Business Process History','#view/BusinessProcess/Views/BPHistory',3,'Root/Business Process Module/History:View',null,null,0,null),
(13,'Scheduler Service','Scheduler Service','#/view/Runtime/Views/SchedulerTaskManagement',3,'Root/Business Process Module/Management:View',null,null,0,null),
(17,'Management','Business Process Management','#view/BusinessProcess/Views/BPDefinitionManagement',3,'Root/Business Process Module/Management:View',null,null,0,null),
(18,'Dashboard','Dashboard','#/view/FraudAnalysis/Views/Output/Dashboard',5,'Root/Dashboard Module:View',null,null,0,3),
(25,'Strategy Management','Strategy Management','#/view/FraudAnalysis/Views/Strategy/StrategyManagement',2,'Root/Strategy Module:View',null,null,0,1),
(26,'Datasource Management','Datasource Management','#/view/Integration/Views/DataSourceManagement',4,'Root/Integration Module:View',null,null,0,null),
(27,'Logs','Logs','#/view/Integration/Views/DataSourceLogManagement',4,'Root/Integration Module:View',null,null,0,null),
(28,'Imported Batches','Imported Batches','#/view/Integration/Views/DataSourceImportedBatchManagement',4,'Root/Integration Module:View',null,null,0,null),
(29,'Cases Productivity','Cases Productivity','#/view/FraudAnalysis/Views/Reports/CasesProductivity',5,'Root/Reporting Module:View',null,null,0,1),
(30,'Blocked Lines','Blocked Lines','#/view/FraudAnalysis/Views/Reports/BlockedLines',5,'Root/Reporting Module:View',null,null,0,2),
(31,'Lines Detected','Lines Detected','#/view/FraudAnalysis/Views/Reports/LinesDetected',5,'Root/Reporting Module:View',null,null,0,3),
(34,'Cancel Cases','Cancel Cases','#/view/FraudAnalysis/Views/SuspiciousAnalysis/CancelCases',2,'Root/Suspicion Analysis Module:View',null,null,0,3)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[RequiredPermissions],[Audience],[Content],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[RequiredPermissions] = s.[RequiredPermissions],[Audience] = s.[Audience],[Content] = s.[Content],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[RequiredPermissions],[Audience],[Content],[Type],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[RequiredPermissions],s.[Audience],s.[Content],s.[Type],s.[Rank])
when not matched by source then
	delete;
set identity_insert [sec].[View] off;


--[sec].[BusinessEntityModule]-----
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Root',null,0,'["View","Full Control"]'),
(2,'Strategy Module',1,0,'["View","Add","Edit", "Add System built in", "Full Control"]'),
(3,'Suspicious Cases Module',1,0,'["View"]'),
(4,'Reporting Module',1,0,'["View"]'),
(5,'Dashboard Module',1,0,'["View"]'),
(6,'Business Process Module',1,0,'["View"]'),
(7,'Integration Module',1,0,'["View"]'),
(8,'Administration Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(9,'Reporting Module',1,0,'["View"]')
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
(3,'System Entities',8,1,'["View", "Assign Permissions"]'),
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






