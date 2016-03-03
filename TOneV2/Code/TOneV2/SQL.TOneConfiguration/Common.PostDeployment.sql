--[sec].[User]------
set nocount on;
set identity_insert [sec].[User] on;
;with cte_data([ID],[Name],[Password],[Email],[Status])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Administrator','9se8222byLvgU9Bzln+oPVZAblIhczMtIT8hLVNhMXQ=','admin@vanrise.com',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Password],[Email],[Status]))
merge	[sec].[User] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Password] = s.[Password],[Email] = s.[Email],[Status] = s.[Status]
when not matched by target then
	insert([ID],[Name],[Password],[Email],[Status])
	values(s.[ID],s.[Name],s.[Password],s.[Email],s.[Status])
when not matched by source then
	delete;
set identity_insert [sec].[User] off;

--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Runtime/SchedulerTask/GetFilteredTasks','VR_Runtime_SchedulerTask: View'),
('VR_Runtime/SchedulerTask/GetTask',null),
('VR_Runtime/SchedulerTask/GetSchedulesInfo',null),
('VR_Runtime/SchedulerTask/GetSchedulerTaskTriggerTypes',null),
('VR_Runtime/SchedulerTask/GetSchedulerTaskActionTypes',null),
('VR_Runtime/SchedulerTask/AddTask','VR_Runtime_SchedulerTask: Add'),
('VR_Runtime/SchedulerTask/UpdateTask','VR_Runtime_SchedulerTask: Edit'),
('VR_Runtime/SchedulerTask/DeleteTask',null),
('VRCommon/Country/GetFilteredCountries','VRCommon_Country: View'),
('VRCommon/Country/GetCountriesInfo',null),
('VRCommon/Country/GetCountry',null),
('VRCommon/Country/AddCountry','VRCommon_Country: Add'),
('VRCommon/Country/UpdateCountry','VRCommon_Country: Edit'),
('VRCommon/Country/GetCountrySourceTemplates',null),
('VRCommon/Country/DownloadCountriesTemplate','VRCommon_Country: Download Template'),
('VRCommon/Country/UploadCountries','VRCommon_Country: Upload'),
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
('VR_Sec/View/AddView',null),
('VR_Sec/View/UpdateView',null),
('VR_Sec/View/GetView',null),
('VR_Sec/View/DeleteView',null),
('VR_Sec/View/GetFilteredDynamicViews',null),
('VR_Sec/View/UpdateViewsRank','VR_Sec_View: Edit'),
('VR_Sec/Menu/GetMenuItems',null),
('VR_Sec/Menu/GetAllMenuItems',null),
('VR_Sec/BusinessEntityNode/GetEntityNodes','VR_Sec_Permission: View'),
('VR_Sec/BusinessEntityNode/ToggleBreakInheritance','VR_Sec_Permission: AllowInheritance'),
('VRCommon/City/GetFilteredCities',null),
('VRCommon/City/GetCitiesInfo',null),
('VRCommon/City/GetCity',null),
('VRCommon/City/AddCity','VRCommon_Country: Add City'),
('VRCommon/City/UpdateCity',null),
('VR_Sec/Permission/GetFilteredEntityPermissions','VR_Sec_Permission: View'),
('VR_Sec/Permission/GetHolderPermissions',null),
('VR_Sec/Permission/GetEffectivePermissions',null),
('VR_Sec/Permission/UpdatePermissions','VR_Sec_Permission: Edit'),
('VR_Sec/Permission/DeletePermissions','VR_Sec_Permission: Delete')
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

--[sec].[BusinessEntityModule]----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[Title],[ParentId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Root','Root',null,0,'["View","Full Control"]'),
(2,'Administration Module','Administration Module',1,0,'["View","Full Control"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ParentId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[Title],[ParentId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[Title],s.[ParentId],s.[BreakInheritance],s.[PermissionOptions]);
set identity_insert [sec].[BusinessEntityModule] off;

--[sec].[BusinessEntity]----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'VR_Sec_Users','Users',2,0,'["View", "Add", "Edit", "Reset Password"]'),
(2,'VR_Sec_Group','Groups',2,0,'["View", "Add", "Edit"]'),
(3,'VR_Sec_View','Ranking Page',2,0,'["Edit"]'),
(4,'VR_Sec_Permission','Permission',2,0,'["View", "Edit", "Delete", "AllowInheritance"]'),
(5,'VR_Runtime_SchedulerTask','Scheduler Task',2,0,'["View", "Add", "Edit"]'),
(6,'VRCommon_Country','Country',2,0,'["View", "Add", "Edit", "Download Template", "Upload", "Add City"]')
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
