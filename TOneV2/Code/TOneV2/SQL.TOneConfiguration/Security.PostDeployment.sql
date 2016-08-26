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
--[sec].[Tenant]------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Tenant] on;
;with cte_data([ID],[Name],[ParentTenantID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Root Tenant',null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentTenantID],[Settings]))
merge	[sec].[Tenant] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentTenantID] = s.[ParentTenantID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ParentTenantID],[Settings])
	values(s.[ID],s.[Name],s.[ParentTenantID],s.[Settings]);
set identity_insert [sec].[Tenant] off;

--to be removed in next stage
Update [sec].[User] set TenantID = 1
--[sec].[User]--------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[User] on;
;with cte_data([ID],[Name],[Password],[Email],[TenantId],[Status])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Administrator','9se8222byLvgU9Bzln+oPVZAblIhczMtIT8hLVNhMXQ=','admin@vanrise.com',1,1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Password],[Email],[TenantId],[Status]))
merge	[sec].[User] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Password] = s.[Password],[Email] = s.[Email],[TenantId] = s.[TenantId],[Status] = s.[Status]
when not matched by target then
	insert([ID],[Name],[Password],[Email],[TenantId],[Status])
	values(s.[ID],s.[Name],s.[Password],s.[Email],s.[TenantId],s.[Status]);
set identity_insert [sec].[User] off;

--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Sec/Users/GetFilteredUsers','VR_Sec_Users: View'),
('VR_Sec/Users/GetUsersInfo',null),
('VR_Sec/Users/GetMembers',null),
('VR_Sec/Users/GetUserbyId',null),
('VR_Sec/Users/UpdateUser','VR_Sec_Users: Edit'),
('VR_Sec/Users/AddUser','VR_Sec_Users: Add'),
('VR_Sec/Users/CheckUserName',null),
('VR_Sec/Users/ResetPassword','VR_Sec_Users: Reset Password'),
('VR_Sec/Users/LoadLoggedInUserProfile',null),
('VR_Sec/Users/EditUserProfile',null),

('VR_Sec/Group/GetFilteredGroups','VR_Sec_Group: View'),
('VR_Sec/Group/GetGroupInfo',null),
('VR_Sec/Group/GetGroup',null),
('VR_Sec/Group/AddGroup','VR_Sec_Group: Add'),
('VR_Sec/Group/UpdateGroup','VR_Sec_Group: Edit'),

('VR_Sec/View/AddView','VR_Sec_View: Add'),
('VR_Sec/View/UpdateView','VR_Sec_View: Edit'),
('VR_Sec/View/GetView',null),
('VR_Sec/View/DeleteView','VR_Sec_View: Delete'),
('VR_Sec/View/GetFilteredViews','VR_Sec_View: View'),
('VR_Sec/View/GetFilteredDynamicViews','VR_Sec_View: View'),
('VR_Sec/View/UpdateViewsRank','VR_Sec_View: Edit'),

('VR_Sec/Menu/GetMenuItems',null),
('VR_Sec/Menu/GetAllMenuItems','VR_Sec_View: Edit'),

('VR_Sec/Module/AddModule','VR_Sec_View: AddModule'),
('VR_Sec/Module/UpdateModule','VR_Sec_View: EditModule'),
('VR_Sec/Module/GetModule',null),
('VR_Sec/Module/GetModules',null),

('VR_Sec/BusinessEntity/UpdateBusinessEntity','VR_Sec_BusinessEntity: EditEntity'),
('VR_Sec/BusinessEntity/AddBusinessEntity','VR_Sec_BusinessEntity: AddEntity'),
('VR_Sec/BusinessEntity/GetFilteredBusinessEntities',null),
('VR_Sec/BusinessEntity/GetBusinessEntity',null),

('VR_Sec/BusinessEntityModule/UpdateBusinessEntityModule','VR_Sec_BusinessEntity: EditModule'),
('VR_Sec/BusinessEntityModule/AddBusinessEntityModule','VR_Sec_BusinessEntity: AddModule'),
('VR_Sec/BusinessEntityModule/GetBusinessEntityModuleById',null),

('VR_Sec/BusinessEntityNode/UpdateEntityNodesRank','VR_Sec_BusinessEntity: Ranking'),
('VR_Sec/BusinessEntityNode/GetEntityNodes','VR_Sec_Permission: View'),
('VR_Sec/BusinessEntityNode/ToggleBreakInheritance','VR_Sec_Permission: AllowInheritance'),
('VR_Sec/BusinessEntityNode/GetEntityModules','VR_Sec_BusinessEntity: View'),


('VR_Sec/Permission/GetFilteredEntityPermissions','VR_Sec_Permission: View'),
('VR_Sec/Permission/GetHolderPermissions',null),
('VR_Sec/Permission/GetEffectivePermissions',null),
('VR_Sec/Permission/UpdatePermissions','VR_Sec_Permission: Edit'),
('VR_Sec/Permission/DeletePermissions','VR_Sec_Permission: Delete'),

('VR_Sec/Tenants/GetFilteredTenants','VR_Sec_Tenants:View'),
('VR_Sec/Tenants/GetTenantsInfo',null),
('VR_Sec/Tenants/GetTenantbyId',null),
('VR_Sec/Tenants/UpdateTenant','VR_Sec_Tenants:Edit'),
('VR_Sec/Tenants/AddTenant','VR_Sec_Tenants:Add')


--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[Name] = s.[Name],[RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);

--[sec].[BusinessEntityModule]------------------------1 to 100----------------------------------------------
------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Root',null,0),
(2,'Administration',1,0),
(3,'Security',2,0),
(-1,'System Configuration',1,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([Id],[Name],[ParentId],[BreakInheritance])
	values(s.[Id],s.[Name],s.[ParentId],s.[BreakInheritance]);
set identity_insert [sec].[BusinessEntityModule] off;

--[sec].[BusinessEntity]------------------1 to 300----------------------------------------------------------
------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'VR_Sec_Users','Users',3,0,'["View", "Add", "Edit", "Reset Password"]'),
(2,'VR_Sec_Group','Groups',3,0,'["View", "Add", "Edit"]'),
(3,'VR_Sec_View','Ranking Page',3,0,'["View", "Add", "Edit"]'),
(4,'VR_Sec_Permission','Permission',3,0,'["View", "Edit", "Delete", "AllowInheritance"]'),
(5,'VR_Sec_BusinessEntity','Business Entity',3,0,'["View","Ranking","AddEntity","AddModule","EditEntity","EditModule"]'),
(6,'VR_Sec_Tenants','Tenants',3,0,'["View", "Add", "Edit"]')

--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
set identity_insert [sec].[BusinessEntity] off;

--[sec].[Module]------------------------------1 to 100------------------------------------------------------
------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Administration','Administration',null,'/Client/images/menu-icons/admin.png',1,0),
(2,'Security',null,1,null,2,0),
(3,'System',null,1,null,1,0),
(-100,'System Configuration','System Configuration',null,'/Client/images/menu-icons/Administration.png',0,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[Id],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
set identity_insert [sec].[Module] off;

--[sec].[viewtype]---------------------------------0 to 100-----------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(0,'VR_Sec_Default','Default','{"ViewTypeId":0,"Name":"VR_Sec_Default","Title":"Default","Editor":"/Client/Modules/Security/Views/Menu/ViewEditor.html","EnableAdd":false}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Details]))
merge	[sec].[viewtype] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Title],[Details])
	values(s.[ID],s.[Name],s.[Title],s.[Details]);

--[sec].[View]-----------------------------1 to 1000---------------------------------------------------------
-------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Users'					,'Users'				,'#/view/Security/Views/User/UserManagement'				,2,'VR_Sec/Users/GetFilteredUsers'	,null,null,null,0,1),
(2,'Groups'					,'Groups'				,'#/view/Security/Views/Group/GroupManagement'				,2,'VR_Sec/Group/GetFilteredGroups'	,null,null,null,0,2),
(3,'System Entities'		,'System Entities'		,'#/view/Security/Views/Permission/BusinessEntityManagement',2,'VR_Sec/BusinessEntityNode/GetEntityNodes & VR_Sec/Permission/GetFilteredEntityPermissions',null,null,null,0,3),
(4,'System Entities Definition','System Entities Definition','#/view/Security/Views/BusinessEntity/BusinessEntityDefinitionManagement',2,'VR_Sec/BusinessEntityNode/GetEntityModules',null,null,null,0,4),
(5,'Views','Views'			,'#/view/Security/Views/View/ViewManagement'										,3,'VR_Sec/View/GetFilteredViews'	,null,null,null,0,1),
(6,'Menus','Menus'			,'#/view/Security/Views/Menu/MenuManagement'										,3,'VR_Sec/View/UpdateViewsRank'	,null,null,null,0,2),
(7,'Organizational Charts'	,'Organizational Charts','#/view/Security/Views/OrgChart/OrgChartManagement'		,3,'VR_Sec/Users/GetFilteredUsers'	,null,null,null,0,25)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
set identity_insert [sec].[View] off;

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
	values(s.[HolderType],s.[HolderId],s.[EntityType],s.[EntityId],s.[PermissionFlags]);

--[common].[EmailTemplate]-----------------------1 to 1000------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[EmailTemplate] on;
;with cte_data([ID],[Name],[BodyTemplate],[SubjectTemplate],[Type])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'New Password','<span>Dear @Model.Name,</span><br/><span>Please find below your new password:</span><br/><span>@Model.Password</span><br/><span><strong>ProductX</strong></span>','ProductX: New Password for user @Model.Name','VR_Sec_NewPassword'),
(2,'Reset Password','<span>Dear @Model.Name,</span><br/><span>Please find below your new password after reset:</span><br/><span>@Model.Password</span><br/><span><strong>ProductX</strong></span>','ProductX: Reset Password for user @Model.Name','VR_Sec_ResetPassword'),
(3,'Forgot Password','<span>Dear @Model.Name,</span><br/><span>Please find below your new password after forgot:</span><br/><span>@Model.Password</span><br/><span><strong>ProductX</strong></span>','ProductX: Forgot Password for user @Model.Name','VR_Sec_ForgotPassword')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[BodyTemplate],[SubjectTemplate],[Type]))
merge	[common].[EmailTemplate] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[BodyTemplate] = s.[BodyTemplate],[SubjectTemplate] = s.[SubjectTemplate],[Type] = s.[Type]
when not matched by target then
	insert([ID],[Name],[BodyTemplate],[SubjectTemplate],[Type])
	values(s.[ID],s.[Name],s.[BodyTemplate],s.[SubjectTemplate],s.[Type]);
set identity_insert [common].[EmailTemplate] off;