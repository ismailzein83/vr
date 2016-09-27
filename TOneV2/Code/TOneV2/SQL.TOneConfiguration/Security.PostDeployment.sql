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

--to be removed in next stage
Update [sec].[User] set TenantID = 1
--[sec].[User]--------------------------------------------------------------------------------------
begin
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
------------------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]------------------1 to 300----------------------------------------------------------
begin
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
------------------------------------------------------------------------------------------------------------
end

--[sec].[Module]------------------------------1 to 100------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D018C0CD-F15F-486D-80C3-F9B87C3F47B8','System Configuration','System Configuration',null,'/images/menu-icons/Administration.png',1,0),
('50624672-CD25-44FD-8580-0E3AC8E34C71','Administration','Administration',null,'/Client/images/menu-icons/admin.png',5,0),
('BAAF681E-AB1C-4A64-9A35-3F3951398881','System',null,'50624672-CD25-44FD-8580-0E3AC8E34C71',null,1,0),
('9B73765C-BDD7-487B-8D32-E386288DB79B','Security',null,'50624672-CD25-44FD-8580-0E3AC8E34C71',null,5,0)
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

--[sec].[viewtype]---------------------------------0 to 100-----------------------------------------
begin
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
----------------------------------------------------------------------------------------------------
end

--[sec].[View]-----------------------------1 to 1000---------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('17B7D7F7-0A1E-4AC1-B2A9-CC71755E3216','Users','Users','#/view/Security/Views/User/UserManagement','9B73765C-BDD7-487B-8D32-E386288DB79B','VR_Sec/Users/GetFilteredUsers',null,null,null,0,1),
('6D39DDC2-E5EB-46C3-A926-37D59A9FD1AD','Groups','Groups','#/view/Security/Views/Group/GroupManagement','9B73765C-BDD7-487B-8D32-E386288DB79B','VR_Sec/Group/GetFilteredGroups',null,null,null,0,5),
('147C1862-9B0C-4285-B415-00C9210FC691','System Entities','System Entities','#/view/Security/Views/Permission/BusinessEntityManagement','9B73765C-BDD7-487B-8D32-E386288DB79B','VR_Sec/BusinessEntityNode/GetEntityNodes & VR_Sec/Permission/GetFilteredEntityPermissions',null,null,null,0,10),
('7AFED44F-6470-47C7-B6E8-B1652D17EB6D','System Entities Definition','System Entities Definition','#/view/Security/Views/BusinessEntity/BusinessEntityDefinitionManagement','9B73765C-BDD7-487B-8D32-E386288DB79B','VR_Sec/BusinessEntityNode/GetEntityModules',null,null,null,0,15),

('241540BF-51E5-4D63-9806-EB212DADC3B3','Tenants','Tenants','#/view/Security/Views/Tenant/TenantManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VR_Sec/Tenants/GetFilteredTenants',null,null,null,0,50),

('5B11405B-F0E1-408C-BCF5-0AB328955B12','Views','Views','#/view/Security/Views/View/ViewManagement','BAAF681E-AB1C-4A64-9A35-3F3951398881','VR_Sec/View/GetFilteredViews',null,null,null,0,1),
('E4101095-F599-414F-8E8C-4790E9FF00FA','Menus','Menus','#/view/Security/Views/Menu/MenuManagement','BAAF681E-AB1C-4A64-9A35-3F3951398881','VR_Sec/View/UpdateViewsRank',null,null,null,0,5),
('DCF8CA21-852C-41B9-9101-6990E545509D','Organizational Charts','Organizational Charts','#/view/Security/Views/OrgChart/OrgChartManagement','BAAF681E-AB1C-4A64-9A35-3F3951398881','VR_Sec/Users/GetFilteredUsers',null,null,null,0,25)

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
-------------------------------------------------------------------------------------------------------------
end

--[sec].[Permission]--------------------------------------------------------------------------------
begin
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

--[common].[MailMessageTemplate]--------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[MessageTypeID],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('D9B56FC2-EB3E-4340-8918-159A281B95BC','New User','E14BE7AF-9D4C-490B-AD3B-122229A660C2','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"User\",\"Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): New Password"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Dear  @Model.GetVal(\"User\",\"Name\"),</div>\n<div>This is your login password: @Model.GetVal(\"Password\",\"Value\")</div>\n<div>@Model.GetVal(\"Product\",\"Product Name\")</div>"}}'),('E21CD125-61F0-4091-A03E-200CFE33F6E3','Forgot Password','62671C45-8598-4BA2-9E96-8927B07FCB4D','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"User\",\"Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): Forgot Password"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Dear  @Model.GetVal(\"User\",\"Name\"),</div>\n<div>Please find your new password after forgot: @Model.GetVal(\"Password\",\"Value\")</div>\n<div>@Model.GetVal(\"Product\",\"Product Name\")</div>"}}'),('10264FE7-99D5-4F6A-8E8C-44A0702F392E','Reset Password','716A6E2D-7AC5-4A55-AABA-F2A4CFEB46A3','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"User\",\"Email\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): Reset Password"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Dear  @Model.GetVal(\"User\",\"Name\"),</div>\n<div>Please find your new password after reset: @Model.GetVal(\"Password\",\"Value\")</div>\n<div>@Model.GetVal(\"Product\",\"Product Name\")</div>"}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[MessageTypeID],[Settings]))merge	[common].[MailMessageTemplate] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[MessageTypeID] = s.[MessageTypeID],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[MessageTypeID],[Settings])	values(s.[ID],s.[Name],s.[MessageTypeID],s.[Settings]);----------------------------------------------------------------------------------------------------\end--common.[extensionconfiguration]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('45BB8E6B-D8A1-47E2-BB29-123B994F781A','VR_Security_VRObjectTypes_User','User','VR_Common_ObjectType','{"Editor":"vr-sec-userobjecttype", "PropertyEvaluatorExtensionType": "VR_Security_UserProfile_PropertyEvaluator"}'),('2A2F21E2-1B3E-456D-9D91-B0898B3F6D49','VR_Security_VRObjectTypes_UserProfilePropertyEvaluator','Profile Property','VR_Security_UserProfile_PropertyEvaluator','{"Editor":"vr-sec-userprofilepropertyevaluator"}'),('BE6619AE-687F-45E3-BD7B-90D1DB4626B6','Static Group','Static Group','VR_Sec_GroupSettings','{"Editor":"vr-sec-group-static"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[extensionconfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end