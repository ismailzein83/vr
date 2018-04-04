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

--common.[extensionconfiguration]-------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('372ED3CB-4B7B-4464-9ABF-59CD7B08BD23','VR_Sec_Default','Default','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Security/Views/Menu/ViewEditor.html","EnableAdd":false}'),

('45BB8E6B-D8A1-47E2-BB29-123B994F781A','VR_Security_VRObjectTypes_User','User','VR_Common_ObjectType','{"Editor":"vr-sec-userobjecttype", "PropertyEvaluatorExtensionType": "VR_Security_UserProfile_PropertyEvaluator"}'),
('2A2F21E2-1B3E-456D-9D91-B0898B3F6D49','VR_Security_VRObjectTypes_UserProfilePropertyEvaluator','Profile Property','VR_Security_UserProfile_PropertyEvaluator','{"Editor":"vr-sec-userprofilepropertyevaluator"}'),
('BE6619AE-687F-45E3-BD7B-90D1DB4626B6','Static Group','Static Group','VR_Sec_GroupSettings','{"Editor":"vr-sec-group-static"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))
merge	[common].[extensionconfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[ConfigType],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);
----------------------------------------------------------------------------------------------------
end

--[sec].[User]--------------------------------------------------------------------------------------
begin
set nocount on;
set identity_insert [sec].[User] on;
;with cte_data([ID],[Name],[Password],[Email],[TenantId],[Description],[IsSystemUser])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-999,'System Account',null,'System Account',1,'this User is used as System account for System tasks', 1),
(1,'Administrator','9se8222byLvgU9Bzln+oPVZAblIhczMtIT8hLVNhMXQ=','admin@vanrise.com',1,null, null),
(-1,'Support','FY5vaAn+kWWvh6cgCgfysC0R+27O0Do=','support@vanrise.com',1,null, null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Password],[Email],[TenantId],[Description],[IsSystemUser]))
merge	[sec].[User] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[Password] = s.[Password],[Email] = s.[Email]
when not matched by target then
	insert([ID],[Name],[Password],[Email],[TenantId],[Description],[IsSystemUser])
	values(s.[ID],s.[Name],s.[Password],s.[Email],s.[TenantId],s.[Description],s.[IsSystemUser]);
set identity_insert [sec].[User] off;
----------------------------------------------------------------------------------------------------
end

UPDATE	[sec].[Group] set [Name] = [Name] + ' - To be deleted group' WHERE	[PSIdentifier] is null and [Name] = 'Administrators'

--[sec].[Group]--------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([PSIdentifier],[Name],[Description],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Administrators','Administrators',null,'{"$type":"Vanrise.Security.Business.StaticGroup, Vanrise.Security.Business","ConfigId":"be6619ae-687f-45e3-bd7b-90d1db4626b6","MemberIds":{"$type":"System.Collections.Generic.List`1[[System.Int32, mscorlib]], mscorlib","$values":[1,-1]}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([PSIdentifier],[Name],[Description],[Settings]))
merge	[sec].[Group] as t
using	cte_data as s
on		1=1 and t.[PSIdentifier] = s.[PSIdentifier]
--when matched then
--	update set
--	[Name] = s.[Name],[Description] = s.[Description],[Settings] = s.[Settings]
when not matched by target then
	insert([PSIdentifier],[Name],[Description],[Settings])
	values(s.[PSIdentifier],s.[Name],s.[Description],s.[Settings]);

END

DECLARE @VR_AdministratorsGroupId int = (SELECT ID FROM sec.[Group] where [PSIdentifier] = 'VR_Administrators')

--[sec].[Permission]--------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,@VR_AdministratorsGroupId,0,'5A9E78AE-229E-41B9-9DBF-492997B42B61','[{"Name":"Full Control","Value":1}]'),
(1,@VR_AdministratorsGroupId,1,'0fc55a70-ca73-426c-afb1-6f5a72004926','[{"Name":"View","Value":1},{"Name":"Edit","Value":1}]'),

(0,'-1',0,'5A9E78AE-229E-41B9-9DBF-492997B42B61','[{"Name":"Full Control","Value":1}]'),
(0,'-1',0,'7913acd9-38c5-43b3-9612-beff66606f22','[{"Name":"Full Control","Value":1}]'),
(0,'-1',1,'0fc55a70-ca73-426c-afb1-6f5a72004926','[{"Name":"View","Value":1},{"Name":"Edit","Value":1},{"Name":"AllowInheritance","Value":1}]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags]))
merge	[sec].[Permission] as t
using	cte_data as s
on		1=1 and t.[HolderType] = s.[HolderType] and t.[HolderId] = s.[HolderId] and t.[EntityType] = s.[EntityType] and t.[EntityId] = s.[EntityId]
--when matched then
--	update set
--	[PermissionFlags] = s.[PermissionFlags]
when not matched by target then
	insert([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
	values(s.[HolderType],s.[HolderId],s.[EntityType],s.[EntityId],s.[PermissionFlags]);
----------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]------------------------------------------------------------------------------
begin
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Sec/Users/GetFilteredUsers','VR_Sec_Users: View'),
('VR_Sec/Users/GetUsersInfo',null),
('VR_Sec/Users/GetUserbyId','VR_Sec_Users: View'),
('VR_Sec/Users/UpdateUser','VR_Sec_Users: Edit'),
('VR_Sec/Users/UpdateUserExpiration','VR_Sec_Users: Edit'),
('VR_Sec/Users/AddUser','VR_Sec_Users: Add'),
('VR_Sec/Users/CheckUserName',null),
('VR_Sec/Users/ResetPassword','VR_Sec_Users: Reset Password'),
('VR_Sec/Users/LoadLoggedInUserProfile',null),
('VR_Sec/Users/EditUserProfile',null),
('VR_Sec/Users/GetUserDetailsByEmail','VR_Sec_Users: View'),
('VR_Sec/Users/GetUserDetailsById','VR_Sec_Users: View'),

('VR_Sec/Group/GetFilteredGroups','VR_Sec_Group: View'),
('VR_Sec/Group/GetGroupInfo',null),
('VR_Sec/Group/GetGroup','VR_Sec_Group: View'),
('VR_Sec/Group/AddGroup','VR_Sec_Group: Add'),
('VR_Sec/Group/UpdateGroup','VR_Sec_Group: Edit'),
('VR_Sec/Group/AssignUserToGroup','VR_Sec_Group: Edit'),

('VR_Sec/View/AddView','VR_Sec_View: Add'),
('VR_Sec/View/UpdateView','VR_Sec_View: Edit'),
('VR_Sec/View/GetView',null),
('VR_Sec/View/DeleteView','VR_Sec_View: Delete'),
('VR_Sec/View/GetFilteredViews','VR_Sec_View: View'),
('VR_Sec/View/GetFilteredDynamicViews','VR_Sec_View: View'),
('VR_Sec/View/UpdateViewsRank','VR_Sec_View: Edit'),

('VR_Sec/Menu/GetMenuItems',null),
('VR_Sec/Menu/GetAllMenuItems','VR_Sec_View: Edit'),

('VR_Sec/Module/AddModule','VR_Sec_View: Add'),
('VR_Sec/Module/UpdateModule','VR_Sec_View: Edit'),
('VR_Sec/Module/GetModule',null),
('VR_Sec/Module/GetModules',null),

('VR_Sec/BusinessEntity/UpdateBusinessEntity','VR_Sec_SystemEntitiesDefinition: Edit'),
('VR_Sec/BusinessEntity/AddBusinessEntity','VR_Sec_SystemEntitiesDefinition: Add'),
('VR_Sec/BusinessEntity/GetFilteredBusinessEntities',null),
('VR_Sec/BusinessEntity/GetBusinessEntity',null),

('VR_Sec/BusinessEntityModule/UpdateBusinessEntityModule','VR_Sec_SystemEntitiesDefinition: Edit'),
('VR_Sec/BusinessEntityModule/AddBusinessEntityModule','VR_Sec_SystemEntitiesDefinition: Add'),
('VR_Sec/BusinessEntityModule/GetBusinessEntityModuleById','VR_Sec_SystemEntitiesDefinition: View'),

('VR_Sec/BusinessEntityNode/UpdateEntityNodesRank','VR_Sec_SystemEntitiesDefinition: Edit'),
('VR_Sec/BusinessEntityNode/GetEntityNodes','VR_Sec_Permission: View'),
('VR_Sec/BusinessEntityNode/ToggleBreakInheritance','VR_Sec_Permission: AllowInheritance'),
('VR_Sec/BusinessEntityNode/GetEntityModules','VR_Sec_SystemEntitiesDefinition: View'),


('VR_Sec/Permission/GetFilteredEntityPermissions','VR_Sec_Permission: View'),
('VR_Sec/Permission/GetHolderPermissions',null),
('VR_Sec/Permission/GetEffectivePermissions',null),
('VR_Sec/Permission/UpdatePermissions','VR_Sec_Permission: Edit'),
('VR_Sec/Permission/DeletePermissions','VR_Sec_Permission: Edit')

--('VR_Sec/Tenants/GetFilteredTenants','VR_Sec_Tenants:View'),
--('VR_Sec/Tenants/GetTenantsInfo',null),
--('VR_Sec/Tenants/GetTenantbyId',null),
--('VR_Sec/Tenants/UpdateTenant','VR_Sec_Tenants:Edit'),
--('VR_Sec/Tenants/AddTenant','VR_Sec_Tenants:Add')


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

--[sec].[BusinessEntity]------------------1 to 300----------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('B4158657-230E-40BF-B88C-F2B2CA8835DE','VR_Sec_Users','Users'											,'5B13C15E-C118-41DC-A2D4-437C9E93F13B',0,'["View", "Add", "Edit", "Reset Password"]'),
('720069F9-753C-45E3-BF31-EC3C5AA1DD33','VR_Sec_Group','Groups'											,'5B13C15E-C118-41DC-A2D4-437C9E93F13B',0,'["View", "Add", "Edit"]'),
('0FC55A70-CA73-426C-AFB1-6F5A72004926','VR_Sec_Permission','Permission'								,'5B13C15E-C118-41DC-A2D4-437C9E93F13B',1,'["View", "Edit", "AllowInheritance"]'),

('102B6C02-7D5D-4768-B65B-87BB141EAADC','VR_Sec_View','Menus and Views'									,'7913ACD9-38C5-43B3-9612-BEFF66606F22',0,'["View", "Add", "Edit"]'),
('92E7AFD2-02F5-4C55-91A6-18D4051C4F6C','VR_Sec_SystemEntitiesDefinition','System Entities Definition'	,'7913ACD9-38C5-43B3-9612-BEFF66606F22',0,'["View","Add","Edit"]')
--('CBF75F7A-DD92-46C2-B4B2-422B309E4B68','VR_Sec_Tenants','Tenants'									,'7913ACD9-38C5-43B3-9612-BEFF66606F22',0,'["View", "Add", "Edit"]')
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
------------------------------------------------------------------------------------------------------------
end

--[sec].[View]-----------------------------1 to 1000---------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],IsDeleted)
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('6D39DDC2-E5EB-46C3-A926-37D59A9FD1AD','Groups','Groups','#/view/Security/Views/Group/GroupManagement'																		,'9B73765C-BDD7-487B-8D32-E386288DB79B','VR_Sec/Group/GetFilteredGroups',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',1,0),
('17B7D7F7-0A1E-4AC1-B2A9-CC71755E3216','Users','Users','#/view/Security/Views/User/UserManagement'																			,'9B73765C-BDD7-487B-8D32-E386288DB79B','VR_Sec/Users/GetFilteredUsers',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5,0),
('147C1862-9B0C-4285-B415-00C9210FC691','Permissions','Permissions','#/view/Security/Views/Permission/BusinessEntityManagement'												,'9B73765C-BDD7-487B-8D32-E386288DB79B','VR_Sec/BusinessEntityNode/GetEntityNodes & VR_Sec/Permission/GetFilteredEntityPermissions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',10,0),

('241540BF-51E5-4D63-9806-EB212DADC3B3','Tenants','Tenants','#/view/Security/Views/Tenant/TenantManagement'																	,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VR_Sec/Tenants/GetFilteredTenants',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',50,1),

('5B11405B-F0E1-408C-BCF5-0AB328955B12','Views','Views','#/view/Security/Views/View/ViewManagement'																			,'A28351BA-A5D7-4651-913C-6C9E09B92AC1','VR_Sec/View/GetFilteredViews',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',1,0),
('E4101095-F599-414F-8E8C-4790E9FF00FA','Menus','Menus','#/view/Security/Views/Menu/MenuManagement'																			,'A28351BA-A5D7-4651-913C-6C9E09B92AC1','VR_Sec/View/UpdateViewsRank',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5,0),
('7AFED44F-6470-47C7-B6E8-B1652D17EB6D','Permissions Definition','Permissions Definition','#/view/Security/Views/BusinessEntity/BusinessEntityDefinitionManagement'			,'A28351BA-A5D7-4651-913C-6C9E09B92AC1','VR_Sec/BusinessEntityNode/GetEntityModules',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',15,0),

('DCF8CA21-852C-41B9-9101-6990E545509D','Organizational Charts','Organizational Charts','#/view/Security/Views/OrgChart/OrgChartManagement'									,'BAAF681E-AB1C-4A64-9A35-3F3951398881','VR_Sec/Users/GetFilteredUsers',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',115,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],IsDeleted))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],
	[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank], [IsDeleted] = s.[IsDeleted]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank], [IsDeleted])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank],s.[IsDeleted]);
-------------------------------------------------------------------------------------------------------------
end

--common.[VRObjectTypeDefinition]-------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('E3887CC9-1FBB-44D1-B1E3-7A0922400550','User','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.Security.MainExtensions.VRObjectTypes.UserObjectType, Vanrise.Security.MainExtensions","ConfigId":"45bb8e6b-d8a1-47e2-bb29-123b994f781a"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Email":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Email","Description":"Email of the User","PropertyEvaluator":{"$type":"Vanrise.Security.MainExtensions.VRObjectTypes.UserProfilePropertyEvaluator, Vanrise.Security.MainExtensions","ConfigId":"2a2f21e2-1b3e-456d-9d91-b0898b3f6d49","UserField":0}},"Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Name","Description":"Name of the User","PropertyEvaluator":{"$type":"Vanrise.Security.MainExtensions.VRObjectTypes.UserProfilePropertyEvaluator, Vanrise.Security.MainExtensions","ConfigId":"2a2f21e2-1b3e-456d-9d91-b0898b3f6d49","UserField":1}},"ExtendedSettings":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"ExtendedSettings","Description":"Extended Settings","PropertyEvaluator":{"$type":"Vanrise.Security.MainExtensions.VRObjectTypes.UserProfilePropertyEvaluator, Vanrise.Security.MainExtensions","ConfigId":"2a2f21e2-1b3e-456d-9d91-b0898b3f6d49","UserField":4}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[VRObjectTypeDefinition] as t
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

--[common].[MailMessageTemplate]--------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[MessageTypeID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D9B56FC2-EB3E-4340-8918-159A281B95BC','New User','E14BE7AF-9D4C-490B-AD3B-122229A660C2','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"User\",\"Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"BCC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): New Password"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Dear  @Model.GetVal(\"User\",\"Name\"),</div>\n<div>This is your login password: @Model.GetVal(\"Password\",\"Value\")</div>\n<div>@Model.GetVal(\"Product\",\"Product Name\")</div>"}}'),
('E21CD125-61F0-4091-A03E-200CFE33F6E3','Forgot Password','62671C45-8598-4BA2-9E96-8927B07FCB4D','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"User\",\"Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"BCC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): Forgot Password"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Dear  @Model.GetVal(\"User\",\"Name\"),</div>\n<div>Please find your new password after forgot: @Model.GetVal(\"Password\",\"Value\")</div>\n<div>@Model.GetVal(\"Product\",\"Product Name\")</div>"}}'),
('10264FE7-99D5-4F6A-8E8C-44A0702F392E','Reset Password','716A6E2D-7AC5-4A55-AABA-F2A4CFEB46A3','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"User\",\"Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"BCC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): Reset Password"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Dear  @Model.GetVal(\"User\",\"Name\"),</div>\n<div>Please find your new password after reset: @Model.GetVal(\"Password\",\"Value\")</div>\n<div>@Model.GetVal(\"Product\",\"Product Name\")</div>"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[MessageTypeID],[Settings]))
merge	[common].[MailMessageTemplate] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[MessageTypeID] = s.[MessageTypeID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[MessageTypeID],[Settings])
	values(s.[ID],s.[Name],s.[MessageTypeID],s.[Settings]);
----------------------------------------------------------------------------------------------------
end

--[common].[Setting]------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D799DC5E-8D52-44B5-8E92-5EA83BAF4963','Security','VR_Sec_Settings','General','{"Editor" : "vr-sec-settings-editor"}','{"$type":"Vanrise.Security.Entities.SecuritySettings, Vanrise.Security.Entities","MailMessageTemplateSettings":{"$type":"Vanrise.Security.Entities.MailMessageTemplateSettings, Vanrise.Security.Entities","NewUserId":"d9b56fc2-eb3e-4340-8918-159a281b95bc","ResetPasswordId":"10264fe7-99d5-4f6a-8e8c-44a0702f392e","ForgotPasswordId":"e21cd125-61f0-4091-a03e-200cfe33f6e3"},"PasswordSettings":{"$type":"Vanrise.Security.Entities.PasswordSettings, Vanrise.Security.Entities","PasswordLength":6,"MaxPasswordLength":15,"MaximumUserLoginTries":10,"UserPasswordHistoryCount":3,"MinutesToLock":5,"FailedInterval":"10:00:00","PasswordComplexity":2},"SendEmailNewUser":false,"SendEmailOnResetPasswordByAdmin":false,"SessionExpirationInMinutes":30}',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
----------------------------------------------------------------------------------------------------
end

--[logging].[LoggableEntity]------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[UniqueName],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('79860CB9-49F5-4492-A7E2-DE2B47997CB4','VR_Security_OrgChart','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Security_OrgChart_ViewHistoryItem"}'),

('ED80ECDC-D751-4E48-B728-5069A78DB7DA','VR_Security_View','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Security_View_ViewHistoryItem"}'),
('DFF05265-22BD-49D5-BE5F-6A97F6624284','VR_Security_User','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Security_User_ViewHistoryItem"}'),
('11CC2EF2-6874-40A9-A6A1-C9D86798EEBA','VR_Security_Group','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Security_Group_ViewHistoryItem"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[UniqueName],[Settings]))
merge	[logging].[LoggableEntity] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[UniqueName],[Settings])
	values(s.[ID],s.[UniqueName],s.[Settings]);
----------------------------------------------------------------------------------------------------
end


--[genericdata].[BusinessEntityDefinition]------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('217a8f71-1dd6-4613-8ae2-540a510f5ff5','VR_Security_User','User'	,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-sec-user-selector","ManagerFQTN":"Vanrise.Security.Business.UserManager,Vanrise.Security.Business", "IdType": "System.Int32"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);
----------------------------------------------------------------------------------------------------
end