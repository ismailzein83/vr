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
--[sec].[Module]---------------------------801 to 900---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(801,'Pricelist Management',null,null,'/images/menu-icons/Purchase Area.png',10,0)
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

--[sec].[View]-----------------------------8001 to 9000-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(8001,'Customers','Customers Management','#/view/CP_SupplierPricelist/Views/Customer/CustomerManagement',1,'CP_SupPriceList/Customer/GetFilteredCustomers',null,null,null,0,15),
(8002,'Upload Pricelist','Upload Pricelist','#/view/CP_SupplierPricelist/Views/SupplierPriceList/SupplierPriceListManagement',801,'CP_SupPriceList/PriceList/GetUpdated',null,null,null,0,10),
(8003,'Supplier Mapping','Supplier Mapping','#/view/CP_SupplierPricelist/Views/SupplierMapping/SupplierMappingManagement',801,'CP_SupPriceList/SupplierMapping/GetFilteredCustomerSupplierMappings',null,null,null,0,11),
(8004,'Pricelists History','Pricelists History','#/view/CP_SupplierPricelist/Views/SupplierPriceList/PriceLists',801,'CP_SupPriceList/PriceList/GetFilteredPriceLists',null,null,null,0,12)
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

--[sec].[BusinessEntityModule]-------------801 to 900---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(801,'Business Process Module',1,0)
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

--[sec].[BusinessEntity]-------------------2101 to 2400-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(2101,'CP_SupPriceList_Customer','Customer',1,0,'["View", "Add","Edit", "Assign/Unassign User"]'),
(2102,'CP_SupPriceList_SupplierMapping','Supplier Mapping',1,0,'["View", "Add/Edit"]'),
(2103,'CP_SupPriceList_PriceList','PriceList',1,0,'["View", "Upload Pricelist","Search"]')
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

--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('CP_SupPriceList/PriceList/GetUpdated','CP_SupPriceList_PriceList:View'),
('CP_SupPriceList/PriceList/ImportPriceList','CP_SupPriceList_PriceList:Upload Pricelist'),
('CP_SupPriceList/PriceList/GetFilteredPriceLists','CP_SupPriceList_PriceList :Search'),

('CP_SupPriceList/CustomerUser/AddCustomerUser','CP_SupPriceList_Customer:Assign/Unassign User'),
('CP_SupPriceList/CustomerUser/DeleteCustomerUser','CP_SupPriceList_Customer:Assign/Unassign User'),
('CP_SupPriceList/CustomerUser/IsCurrentUserCustomer',null),

('CP_SupPriceList/SupplierMapping/GetFilteredCustomerSupplierMappings','CP_SupPriceList_SupplierMapping:View'),
('CP_SupPriceList/SupplierMapping/AddCustomerSupplierMapping','CP_SupPriceList_SupplierMapping:Add/Edit'),
('CP_SupPriceList/SupplierMapping/UpdateCustomerSupplierMapping','CP_SupPriceList_SupplierMapping:Add/Edit'),
('CP_SupPriceList/SupplierMapping/GetCustomerSuppliers',null),
('CP_SupPriceList/SupplierMapping/GetCustomerSupplierMapping','CP_SupPriceList_SupplierMapping:View'),
('CP_SupPriceList/SupplierMapping/DeleteCustomerSupplierMapping',null),

('CP_SupPriceList/Customer/GetFilteredCustomers','CP_SupPriceList_Customer:View'),
('CP_SupPriceList/Customer/GetCustomer','CP_SupPriceList_Customer:View'),
('CP_SupPriceList/Customer/GetCustomerTemplates',null),
('CP_SupPriceList/Customer/GetCustomerInfos',null),
('CP_SupPriceList/Customer/AddCustomer','CP_SupPriceList_Customer:Add'),
('CP_SupPriceList/Customer/UpdateCustomer','CP_SupPriceList_Customer:Edit')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);

--[common].[TemplateConfig]----------10001 to 20000---------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[TemplateConfig] on;
;with cte_data([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(10001,'TOne V1','CP_SupplierPriceList_CustomerConnector','vr-cp-supplierpricelist-tonev1integration-customerconnector',null,null),
(10002,'Customer Group','VR_Sec_GroupSettings','vr-cp-group-customer',null,null),
(10003,'Supplier Group','VR_Sec_GroupSettings','vr-cp-group-supplier',null,null)
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

--[common].[Setting]---------------------------801 to 900-------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[Setting] on;
;with cte_data([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(801,'Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Carrier Portal","VersionNumber":"version 0.9"}}',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[Id],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
set identity_insert [common].[Setting] off;