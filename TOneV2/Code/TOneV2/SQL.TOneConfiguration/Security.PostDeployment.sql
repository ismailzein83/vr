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
begin
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
----------------------------------------------------------------------------------------------------
end

--common.[extensionconfiguration]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('372ED3CB-4B7B-4464-9ABF-59CD7B08BD23','VR_Sec_Default','Default','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Security/Views/Menu/ViewEditor.html","EnableAdd":false}'),('45BB8E6B-D8A1-47E2-BB29-123B994F781A','VR_Security_VRObjectTypes_User','User','VR_Common_ObjectType','{"Editor":"vr-sec-userobjecttype", "PropertyEvaluatorExtensionType": "VR_Security_UserProfile_PropertyEvaluator"}'),
('2A2F21E2-1B3E-456D-9D91-B0898B3F6D49','VR_Security_VRObjectTypes_UserProfilePropertyEvaluator','Profile Property','VR_Security_UserProfile_PropertyEvaluator','{"Editor":"vr-sec-userprofilepropertyevaluator"}'),
('BE6619AE-687F-45E3-BD7B-90D1DB4626B6','Static Group','Static Group','VR_Sec_GroupSettings','{"Editor":"vr-sec-group-static"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[extensionconfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end

--[sec].[User]--------------------------------------------------------------------------------------
begin
set nocount on;
set identity_insert [sec].[User] on;
;with cte_data([ID],[Name],[Password],[Email],[TenantId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Administrator','9se8222byLvgU9Bzln+oPVZAblIhczMtIT8hLVNhMXQ=','admin@vanrise.com',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Password],[Email],[TenantId]))
merge	[sec].[User] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[Password] = s.[Password],[Email] = s.[Email]
when not matched by target then
	insert([ID],[Name],[Password],[Email],[TenantId])
	values(s.[ID],s.[Name],s.[Password],s.[Email],s.[TenantId]);
set identity_insert [sec].[User] off;
----------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]------------------------------------------------------------------------------
begin
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
----------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntityModule]------------------------1 to 100----------------------------------------------
begin
set nocount on;;with cte_data([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('5A9E78AE-229E-41B9-9DBF-492997B42B61',1,'Root',null,null,0),('7913ACD9-38C5-43B3-9612-BEFF66606F22',-1,'Configuration'	,'5A9E78AE-229E-41B9-9DBF-492997B42B61',1,0),('61451603-E7B9-40C6-AE27-6CBA974E1B3B',2,'Administration'	,'5A9E78AE-229E-41B9-9DBF-492997B42B61',1,0),('5B13C15E-C118-41DC-A2D4-437C9E93F13B',3,'Security'		,'61451603-E7B9-40C6-AE27-6CBA974E1B3B',2,0),('4C9719E3-F818-454D-9977-01A9668E7ABA',null,'System'		,'7913ACD9-38C5-43B3-9612-BEFF66606F22',-1,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[ParentId] = s.[ParentId],[OldParentId] = s.[OldParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])	values(s.[ID],s.[OldId],s.[Name],s.[ParentId],s.[OldParentId],s.[BreakInheritance]);
------------------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]------------------1 to 300----------------------------------------------------------
begin
set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('B4158657-230E-40BF-B88C-F2B2CA8835DE',1,'VR_Sec_Users','Users'					,'5B13C15E-C118-41DC-A2D4-437C9E93F13B',3,0,'["View", "Add", "Edit", "Reset Password"]'),('720069F9-753C-45E3-BF31-EC3C5AA1DD33',2,'VR_Sec_Group','Groups'					,'5B13C15E-C118-41DC-A2D4-437C9E93F13B',3,0,'["View", "Add", "Edit"]'),('102B6C02-7D5D-4768-B65B-87BB141EAADC',3,'VR_Sec_View','Menus and Views'				,'4C9719E3-F818-454D-9977-01A9668E7ABA',3,0,'["View", "Add", "Edit"]'),('92E7AFD2-02F5-4C55-91A6-18D4051C4F6C',5,'VR_Sec_BusinessEntity','Business Entity'		,'4C9719E3-F818-454D-9977-01A9668E7ABA',3,0,'["View","Ranking","AddEntity","AddModule","EditEntity","EditModule"]'),('0FC55A70-CA73-426C-AFB1-6F5A72004926',4,'VR_Sec_Permission','Permission'			,'5B13C15E-C118-41DC-A2D4-437C9E93F13B',3,0,'["View", "Edit", "Delete", "AllowInheritance"]'),('CBF75F7A-DD92-46C2-B4B2-422B309E4B68',6,'VR_Sec_Tenants','Tenants'				,'7913ACD9-38C5-43B3-9612-BEFF66606F22',-1,0,'["View", "Add", "Edit"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);
------------------------------------------------------------------------------------------------------------
end

--[sec].[Module]------------------------------1 to 100------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D018C0CD-F15F-486D-80C3-F9B87C3F47B8','Configuration'	,null,null,'/Client/Images/menu-icons/Administration.png',1,0),
('50624672-CD25-44FD-8580-0E3AC8E34C71','Administration',null	,null,'/Client/Images/menu-icons/admin.png',5,0),
('BAAF681E-AB1C-4A64-9A35-3F3951398881','System',null			,'50624672-CD25-44FD-8580-0E3AC8E34C71',null,1,0),
('9B73765C-BDD7-487B-8D32-E386288DB79B','Security',null			,'50624672-CD25-44FD-8580-0E3AC8E34C71',null,5,0),

('A28351BA-A5D7-4651-913C-6C9E09B92AC1','System',null			,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,1,0)
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

------------------------------------------------------------------------------------------------------------
end

--[sec].[View]-----------------------------1 to 1000---------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('17B7D7F7-0A1E-4AC1-B2A9-CC71755E3216','Users','Users','#/view/Security/Views/User/UserManagement'																			,'9B73765C-BDD7-487B-8D32-E386288DB79B','VR_Sec/Users/GetFilteredUsers',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,1),
('6D39DDC2-E5EB-46C3-A926-37D59A9FD1AD','Groups','Groups','#/view/Security/Views/Group/GroupManagement'																		,'9B73765C-BDD7-487B-8D32-E386288DB79B','VR_Sec/Group/GetFilteredGroups',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,5),
('147C1862-9B0C-4285-B415-00C9210FC691','System Entities','System Entities','#/view/Security/Views/Permission/BusinessEntityManagement'										,'9B73765C-BDD7-487B-8D32-E386288DB79B','VR_Sec/BusinessEntityNode/GetEntityNodes & VR_Sec/Permission/GetFilteredEntityPermissions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,10),

('241540BF-51E5-4D63-9806-EB212DADC3B3','Tenants','Tenants','#/view/Security/Views/Tenant/TenantManagement'																	,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VR_Sec/Tenants/GetFilteredTenants',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,50),

('5B11405B-F0E1-408C-BCF5-0AB328955B12','Views','Views','#/view/Security/Views/View/ViewManagement'																			,'A28351BA-A5D7-4651-913C-6C9E09B92AC1','VR_Sec/View/GetFilteredViews',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,1),
('E4101095-F599-414F-8E8C-4790E9FF00FA','Menus','Menus','#/view/Security/Views/Menu/MenuManagement'																			,'A28351BA-A5D7-4651-913C-6C9E09B92AC1','VR_Sec/View/UpdateViewsRank',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,5),
('7AFED44F-6470-47C7-B6E8-B1652D17EB6D','Entities Definition','System Entities Definition','#/view/Security/Views/BusinessEntity/BusinessEntityDefinitionManagement'		,'A28351BA-A5D7-4651-913C-6C9E09B92AC1','VR_Sec/BusinessEntityNode/GetEntityModules',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,15),

('DCF8CA21-852C-41B9-9101-6990E545509D','Organizational Charts','Organizational Charts','#/view/Security/Views/OrgChart/OrgChartManagement'									,'BAAF681E-AB1C-4A64-9A35-3F3951398881','VR_Sec/Users/GetFilteredUsers',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,25)

--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);
-------------------------------------------------------------------------------------------------------------
end

--[sec].[Permission]--------------------------------------------------------------------------------
begin
set nocount on;;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(0,'1',0,'5A9E78AE-229E-41B9-9DBF-492997B42B61','[{"Name":"Full Control","Value":1}]')--(0,'1',1,'102B6C02-7D5D-4768-B65B-87BB141EAADC','[{"Name":"View","Value":1}, {"Name":"Assign Permissions","Value":1}]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags]))merge	[sec].[Permission] as tusing	cte_data as son		1=1 and t.[HolderType] = s.[HolderType] and t.[HolderId] = s.[HolderId] and t.[EntityType] = s.[EntityType] and t.[EntityId] = s.[EntityId]when matched then	update set	[PermissionFlags] = s.[PermissionFlags]when not matched by target then	insert([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])	values(s.[HolderType],s.[HolderId],s.[EntityType],s.[EntityId],s.[PermissionFlags]);
----------------------------------------------------------------------------------------------------
end

--common.[VRObjectTypeDefinition]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('E3887CC9-1FBB-44D1-B1E3-7A0922400550','User','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.Security.MainExtensions.VRObjectTypes.UserObjectType, Vanrise.Security.MainExtensions","ConfigId":"45bb8e6b-d8a1-47e2-bb29-123b994f781a"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Email":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Email","Description":"Email of the User","PropertyEvaluator":{"$type":"Vanrise.Security.MainExtensions.VRObjectTypes.UserProfilePropertyEvaluator, Vanrise.Security.MainExtensions","UserField":0,"ConfigId":"2a2f21e2-1b3e-456d-9d91-b0898b3f6d49"}},"Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Name","Description":"Name of the User","PropertyEvaluator":{"$type":"Vanrise.Security.MainExtensions.VRObjectTypes.UserProfilePropertyEvaluator, Vanrise.Security.MainExtensions","UserField":1,"ConfigId":"2a2f21e2-1b3e-456d-9d91-b0898b3f6d49"}}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[VRObjectTypeDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);----------------------------------------------------------------------------------------------------end
--[common].[MailMessageType]------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Settings],[CreatedTime])
as (select * from (values
('E14BE7AF-9D4C-490B-AD3B-122229A660C2','New User','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","User":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"User","VRObjectTypeDefinitionId":"e3887cc9-1fbb-44d1-b1e3-7a0922400550"},"Password":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Password","VRObjectTypeDefinitionId":"1c93042e-939b-4022-9f13-43c3718ef644"},"Product":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Product","VRObjectTypeDefinitionId":"62b9a4da-0018-4514-bcfd-8268a58f53a2"}}}','2016-08-25 16:19:36.283'),
('62671C45-8598-4BA2-9E96-8927B07FCB4D','Forgot Password','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","User":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"User","VRObjectTypeDefinitionId":"e3887cc9-1fbb-44d1-b1e3-7a0922400550"},"Password":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Password","VRObjectTypeDefinitionId":"1c93042e-939b-4022-9f13-43c3718ef644"},"Product":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Product","VRObjectTypeDefinitionId":"62b9a4da-0018-4514-bcfd-8268a58f53a2"}}}','2016-08-25 11:38:30.343'),
('716A6E2D-7AC5-4A55-AABA-F2A4CFEB46A3','Reset Password','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","User":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"User","VRObjectTypeDefinitionId":"e3887cc9-1fbb-44d1-b1e3-7a0922400550"},"Password":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Password","VRObjectTypeDefinitionId":"1c93042e-939b-4022-9f13-43c3718ef644"},"Product":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Product","VRObjectTypeDefinitionId":"62b9a4da-0018-4514-bcfd-8268a58f53a2"}}}','2016-08-25 17:10:56.517')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings],[CreatedTime]))
merge	[common].[MailMessageType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);
----------------------------------------------------------------------------------------------------
end

--[common].[MailMessageTemplate]--------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[MessageTypeID],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('D9B56FC2-EB3E-4340-8918-159A281B95BC','New User','E14BE7AF-9D4C-490B-AD3B-122229A660C2','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"User\",\"Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): New Password"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Dear  @Model.GetVal(\"User\",\"Name\"),</div>\n<div>This is your login password: @Model.GetVal(\"Password\",\"Value\")</div>\n<div>@Model.GetVal(\"Product\",\"Product Name\")</div>"}}'),('E21CD125-61F0-4091-A03E-200CFE33F6E3','Forgot Password','62671C45-8598-4BA2-9E96-8927B07FCB4D','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"User\",\"Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): Forgot Password"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Dear  @Model.GetVal(\"User\",\"Name\"),</div>\n<div>Please find your new password after forgot: @Model.GetVal(\"Password\",\"Value\")</div>\n<div>@Model.GetVal(\"Product\",\"Product Name\")</div>"}}'),('10264FE7-99D5-4F6A-8E8C-44A0702F392E','Reset Password','716A6E2D-7AC5-4A55-AABA-F2A4CFEB46A3','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"User\",\"Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): Reset Password"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Dear  @Model.GetVal(\"User\",\"Name\"),</div>\n<div>Please find your new password after reset: @Model.GetVal(\"Password\",\"Value\")</div>\n<div>@Model.GetVal(\"Product\",\"Product Name\")</div>"}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[MessageTypeID],[Settings]))merge	[common].[MailMessageTemplate] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[MessageTypeID] = s.[MessageTypeID],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[MessageTypeID],[Settings])	values(s.[ID],s.[Name],s.[MessageTypeID],s.[Settings]);----------------------------------------------------------------------------------------------------\end--[common].[Setting]------------------------------------------------------------------------
begin
set nocount on;;with cte_data([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('D799DC5E-8D52-44B5-8E92-5EA83BAF4963',3,'Security','VR_Sec_Settings','General','{"Editor" : "vr-sec-settings-editor"}','{"$type":"Vanrise.Security.Entities.SecuritySettings, Vanrise.Security.Entities","MailMessageTemplateSettings":{"$type":"Vanrise.Security.Entities.MailMessageTemplateSettings, Vanrise.Security.Entities","NewUserId":"d9b56fc2-eb3e-4340-8918-159a281b95bc","ResetPasswordId":"10264fe7-99d5-4f6a-8e8c-44a0702f392e","ForgotPasswordId":"e21cd125-61f0-4091-a03e-200cfe33f6e3"},"SendEmailNewUser":false}',1)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))merge	[common].[Setting] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]when not matched by target then	insert([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])	values(s.[ID],s.[OldId],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
----------------------------------------------------------------------------------------------------
end