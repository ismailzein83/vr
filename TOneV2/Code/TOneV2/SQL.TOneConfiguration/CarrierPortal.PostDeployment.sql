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
--[sec].[Module]---------------------------701 to 800---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Administration','Administration','Administration',null,'/images/menu-icons/Administration.png',10,0),
(2,'Pricelist Management','Pricelist Management','Pricelist Management',null,'/images/menu-icons/Purchase Area.png',11,0)
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
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
set identity_insert [sec].[Module] off;

--[sec].[View]-----------------------------7001 to 8000-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Users','Users','#/view/Security/Views/User/UserManagement',1,'VR_Sec/Users/GetFilteredUsers',null,null,null,0,10),
(2,'Groups','Groups','#/view/Security/Views/Group/GroupManagement',1,'VR_Sec/Group/GetFilteredGroups',null,null,null,0,11),
(3,'System Entities','System Entities','#/view/Security/Views/Permission/BusinessEntityManagement',1,'VR_Sec/BusinessEntityNode/GetEntityNodes & VR_Sec/Permission/GetFilteredEntityPermissions',null,null,null,0,12),
(5,'Ranking Pages','Ranking Pages','#/view/Security/Views/Pages/RankingPageManagement',1,'VR_Sec/View/UpdateViewsRank',null,null,null,0,13),
(6,'Upload Pricelist','Upload Pricelist','#/view/CP_SupplierPricelist/Views/SupplierPriceList/SupplierPriceListManagement',2,'CP_SupPriceList/PriceList/GetUpdated',null,null,null,0,10),
(7,'Scheduler Service','Scheduler Service','#/view/Runtime/Views/SchedulerTaskManagement',1,'VR_Runtime/SchedulerTask/GetFilteredTasks',null,null,null,0,14),
(8,'Supplier Mapping','Supplier Mapping','#/view/CP_SupplierPricelist/Views/SupplierMapping/SupplierMappingManagement',2,'CP_SupPriceList/SupplierMapping/GetFilteredCustomerSupplierMappings',null,null,null,0,11),
(9,'Customers','Customers Management','#/view/CP_SupplierPricelist/Views/Customer/CustomerManagement',1,'CP_SupPriceList/Customer/GetFilteredCustomers',null,null,null,0,15)
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

--[sec].[BusinessEntityModule]-------------701 to 800---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[Title],[ParentId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(101,'Business Process Module','Business Process Module',1,0,'["View"]')
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


--[sec].[BusinessEntity]-------------------2101 to 2400-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(300,'CP_SupPriceList_Customer','Customer',1,0,'["View", "Add","Edit", "Assign/Unassign User"]'),
(301,'CP_SupPriceList_SupplierMapping','Supplier Mapping',1,0,'["View", "Add/Edit"]'),
(302,'CP_SupPriceList_PriceList','PriceList',1,0,'["View", "Import PriceList"]')
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

--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('CP_SupPriceList/Customer/GetFilteredCustomers','CP_SupPriceList_Customer:View'),
('CP_SupPriceList/PriceList/GetUpdated','CP_SupPriceList_PriceList:View'),
('CP_SupPriceList/SupplierMapping/GetFilteredCustomerSupplierMappings','CP_SupPriceList_SupplierMapping:View'),
('CP_SupPriceList/PriceList/ImportPriceList','CP_SupPriceList_PriceList:Import PriceList'),
('CP_SupPriceList/CustomerUser/AddCustomerUser','CP_SupPriceList_Customer:Assign/Unassign User'),
('CP_SupPriceList/CustomerUser/DeleteCustomerUser','CP_SupPriceList_Customer:Assign/Unassign User'),
('CP_SupPriceList/SupplierMapping/AddCustomerSupplierMapping','CP_SupPriceList_SupplierMapping:Add/Edit'),
('CP_SupPriceList/SupplierMapping/UpdateCustomerSupplierMapping','CP_SupPriceList_SupplierMapping:Add/Edit'),
('CP_SupPriceList/SupplierMapping/GetCustomerSuppliers',null),
('CP_SupPriceList/SupplierMapping/GetCustomerSupplierMapping','CP_SupPriceList_SupplierMapping:View'),
('CP_SupPriceList/Customer/GetCustomer','CP_SupPriceList_Customer:View'),
('CP_SupPriceList/Customer/GetCustomerTemplates',null),
('CP_SupPriceList/Customer/GetCustomerInfos',null),
('CP_SupPriceList/Customer/AddCustomer','CP_SupPriceList_Customer:Add'),
('CP_SupPriceList/Customer/UpdateCustomer','CP_SupPriceList_Customer:Edit'),
('CP_SupPriceList/CustomerUser/IsCurrentUserCustomer',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);

--[common].[TemplateConfig]----------20001 to 30000---------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[TemplateConfig] on;
;with cte_data([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(20001,'TOne V1','CP_SupplierPriceList_CustomerConnector','vr-cp-supplierpricelist-tonev1integration-customerconnector',null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings]))
merge	[common].[TemplateConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ConfigType] = s.[ConfigType],[Editor] = s.[Editor],[BehaviorFQTN] = s.[BehaviorFQTN],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
	values(s.[ID],s.[Name],s.[ConfigType],s.[Editor],s.[BehaviorFQTN],s.[Settings]);
set identity_insert [common].[TemplateConfig] off;