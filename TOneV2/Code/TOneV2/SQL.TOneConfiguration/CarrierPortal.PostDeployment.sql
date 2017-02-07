﻿/*
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
begin
set nocount on;
--set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('0d24e501-99c1-47ce-a002-282a86826107','Pricelist Management',null,null,'/images/menu-icons/Purchase Area.png',10,0)
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
--set identity_insert [sec].[Module] off;
--------------------------------------------------------------------------------------------------------------
end

GO--delete useless views from ClearVoice product such 'My Scheduler Service' since it is replace with 'Schedule Test Calls', 'Style Definitions', 'Organizational Charts','Countries'
delete from [sec].[View] where [Id] in ('C65ED28A-36D0-4047-BEC5-030D35B02308','66DE2441-8A96-41E7-94EA-9F8AF38A3515','DCF8CA21-852C-41B9-9101-6990E545509D','25994374-CB99-475B-8047-3CDB7474A083','9F691B87-4936-4C4C-A757-4B3E12F7E1D9', 'E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9', '0F111ADC-B7F6-46A4-81BC-72FFDEB305EB', '4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712','604B2CB5-B839-4E51-8D13-3C1C84D05DEE')
GO
--[sec].[View]-----------------------------8001 to 9000-------------------------------------------------------
begin

set nocount on;
--set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('ade187cf-146c-45db-882a-2c943d38dd70','Customers','Customers Management','#/view/CP_SupplierPricelist/Views/Customer/CustomerManagement','50624672-cd25-44fd-8580-0e3ac8e34c71','CP_SupPriceList/Customer/GetFilteredCustomers',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',15),
('e2ed2857-dea3-42fe-8041-b2b06eb1e20e','Upload Pricelist','Upload Pricelist','#/view/CP_SupplierPricelist/Views/SupplierPriceList/SupplierPriceListManagement','0d24e501-99c1-47ce-a002-282a86826107','CP_SupPriceList/PriceList/GetUpdated',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',10),
('ea38345e-2354-4e67-9fac-d5612fae62c3','Supplier Mapping','Supplier Mapping','#/view/CP_SupplierPricelist/Views/SupplierMapping/SupplierMappingManagement','0d24e501-99c1-47ce-a002-282a86826107','CP_SupPriceList/SupplierMapping/GetFilteredCustomerSupplierMappings',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',11),
('a33a27e2-16e9-4475-b75c-e4e1320ce31f','Pricelists History','Pricelists History','#/view/CP_SupplierPricelist/Views/SupplierPriceList/PriceLists','0d24e501-99c1-47ce-a002-282a86826107','CP_SupPriceList/PriceList/GetFilteredPriceLists',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',12)
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
--set identity_insert [sec].[View] off;
--------------------------------------------------------------------------------------------------------------

end



--[sec].[BusinessEntityModule]-------------801 to 900---------------------------------------------------------
begin
set nocount on;
--set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('AE487324-A151-478C-B171-67C79E9BA2DA','Business Process Module','5A9E78AE-229E-41B9-9DBF-492997B42B61',0)
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
--set identity_insert [sec].[BusinessEntityModule] off;
--------------------------------------------------------------------------------------------------------------

end


--[sec].[BusinessEntity]-------------------2101 to 2400-------------------------------------------------------
begin

set nocount on;
--set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('F9AB8A79-7863-44B2-95CD-6587C3939FA1','CP_SupPriceList_Customer','Customer','5A9E78AE-229E-41B9-9DBF-492997B42B61',0,'["View", "Add","Edit", "Assign/Unassign User"]'),
('0F757375-F0CC-4011-96FF-7F4A8341AC8D','CP_SupPriceList_SupplierMapping','Supplier Mapping','5A9E78AE-229E-41B9-9DBF-492997B42B61',0,'["View", "Add/Edit"]'),
('2796A68C-4A6B-44B2-B2CE-9FBFC2072E24','CP_SupPriceList_PriceList','PriceList','5A9E78AE-229E-41B9-9DBF-492997B42B61',0,'["View", "Upload Pricelist","Search"]')
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
--set identity_insert [sec].[BusinessEntity] off;
--------------------------------------------------------------------------------------------------------------

end


--[sec].[SystemAction]------------------------------------------------------------------------------
begin

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
----------------------------------------------------------------------------------------------------
end

GO--delete useless [Setting] from ClearVoice product such 'System Currency', etc...
delete from [common].[Setting] where [Id] in ('1C833B2D-8C97-4CDD-A1C1-C1B4D9D299DE')
GO
--[common].[Setting]---------------------------801 to 900-------------------------------------------
begin
set nocount on;
--set identity_insert [common].[Setting] on;
;with cte_data([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('509E467B-4562-4CA6-A32E-E50473B74D2C','Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Carrier Portal","VersionNumber":"version 0.9"}}',1)
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
--set identity_insert [common].[Setting] off;

----------------------------------------------------------------------------------------------------

end


--[common].[ExtensionConfiguration]---------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('9A1C1152-B9DC-4E4C-B2FF-181325CD6D9B','Customer Group','Customer Group','VR_Sec_GroupSettings','{"Editor":"vr-cp-group-customer"}'),
('6E558C88-31F8-4EB6-A362-58DB06AB9CC7','Supplier Group','Supplier Group','VR_Sec_GroupSettings','{"Editor":"vr-cp-group-supplier"}'),
('FCE278EC-410E-4C92-85E0-A9F2E4BB27A8','Carrier Portal Connector','Carrier Portal Connector','CP_SupplierPriceList_ConnectorUploadPriceList','{"Editor":"vr-cp-supplierpricelist-connector"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))
merge	[common].[ExtensionConfiguration] as t
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

Delete from [runtime].[SchedulerTaskActionType] where Id in ('0A15BC35-A3A7-4ED3-B09B-1B41A7A9DDC9','7A35F562-319B-47B3-8258-EC1A704A82EB') --Exchange Rate, workflow
