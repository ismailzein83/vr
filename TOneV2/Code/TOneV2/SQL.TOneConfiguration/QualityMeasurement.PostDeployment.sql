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
--sec.Module----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Administration','Administration','Administration',null,'/images/menu-icons/Administration.png',10,0),
(2,'CLI Tester','CLI Tester','CLI Tester',null,'/images/menu-icons/CLITester.png',12,0),
(3,'Business Entities','Business Entities','Business Entities',null,'/images/menu-icons/Business Entities.png',11,0)
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

--[sec].[View]--------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[RequiredPermissions],[Audience],[Content],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Users','Users','#/view/Security/Views/UserManagement',1,'Root/Administration Module/Users:View',null,null,0,10),
(2,'Groups','Groups','#/view/Security/Views/GroupManagement',1,'Root/Administration Module/Groups:View',null,null,0,11),
(3,'System Entities','System Entities','#/view/Security/Views/BusinessEntityManagement',1,'Root/Administration Module/System Entities:View',null,null,0,12),
(4,'Ranking Pages','Ranking Pages','#/view/Security/Views/Pages/RankingPageManagement',1,'Root/Administration Module:View',null,null,0,14),
(5,'Test call','Test call','#/view/QM_CLITester/Views/TestPage/Test',2,null,null,null,0,11),
(6,'Supplier','Supplier','#/view/QM_BusinessEntity/Views/Supplier/SupplierManagement',3,null,null,null,0,10),
(7,'Scheduler Service','Scheduler Service','#/view/Runtime/Views/SchedulerTaskManagement',1,null,null,null,0,13),
(8,'History','Calls History','#/view/QM_CLITester/Views/HistoryTestCall/HistoryTestCallManagement',2,null,null,null,0,12),
(9,'Profile','Profile','#/view/QM_CLITester/Views/Profile/ProfileManagement',2,null,null,null,0,10)
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
(6,'Business Process Module',1,0,'["View"]'),
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

--[sec].[Permission]--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(0,'1',0,'1','[{"Name":"Full Control","Value":1}]'),
(0,'1',1,'3','[{"Name":"View","Value":1}, {"Name":"Assign Permissions","Value":1}]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags]))
merge	[sec].[Permission] as t
using	cte_data as s
on		1=1 and t.[HolderType] = s.[HolderType] and t.[HolderId] = s.[HolderId] and t.[EntityType] = s.[EntityType] and t.[EntityId] = s.[EntityId]
when matched then
	update set
	[PermissionFlags] = s.[PermissionFlags]
when not matched by target then
	insert([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
	values(s.[HolderType],s.[HolderId],s.[EntityType],s.[EntityId],s.[PermissionFlags])
when not matched by source then
	delete;






