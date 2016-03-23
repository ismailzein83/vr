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
--sec.WidgetDefinition--------
set nocount on;
set identity_insert [sec].[WidgetDefinition] on;
;with cte_data([ID],[Name],[DirectiveName],[Setting])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Report                                            ','vr-bi-datagrid                                    ','{"DirectiveTemplateURL":"vr-bi-datagrid-template","Sections":[1]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                '),
(2,'Chart                                             ','vr-bi-chart                                       ','{"DirectiveTemplateURL":"vr-bi-chart-template","Sections":[1]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   '),
(3,'Summary                                           ','vr-bi-summary                                     ','{"DirectiveTemplateURL":"vr-bi-summary-template","Sections":[0]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 ')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DirectiveName],[Setting]))
merge	[sec].[WidgetDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DirectiveName] = s.[DirectiveName],[Setting] = s.[Setting]
when not matched by target then
	insert([ID],[Name],[DirectiveName],[Setting])
	values(s.[ID],s.[Name],s.[DirectiveName],s.[Setting])
when not matched by source then
	delete;
set identity_insert [sec].[WidgetDefinition] off; 
--BI.SchemaConfiguration----
set nocount on;
set identity_insert [BI].[SchemaConfiguration] on;
;with cte_data([ID],[Name],[DisplayName],[Type],[Configuration],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'ACD','ACD',1,'{"ColumnName":"[Measures].[ACD]","Expression":"","RequiredPermissions":"Root/Business Intelligence Module/Billing Module/Billing Statistics:View","Unit":"min/call"}',null),
(2,'COST','COST',1,'{"ColumnName":"[Measures].[Cost Net]","Expression":"","Unit":"Currency"}',null),
(3,'SALE','SALE',1,'{"ColumnName":"[Measures].[Sale Net]","Expression":"","Unit":"Currency"}',null),
(5,'PROFIT','PROFIT',1,'{"ColumnName":"[Measures].[Profit_CALC]","Expression":"MEMBER [Measures].[Profit_CALC]  AS ([Measures].[Sale Net] - [Measures].[Cost Net])","Unit":"Currency"}',null),
(6,'SUCCESSFUL_ATTEMPTS','ATTEMPTS',1,'{"ColumnName":"[Measures].[SuccessfulAttempts]","Expression":"","Unit":"#Calls"}',null),
(7,'PDD','PDD',1,'{"ColumnName":"[Measures].[PDD]","Exepression":"","Unit":"sec"}',null),
(8,'Supplier','Supplier',0,'{"ColumnID":"[SupplierAccounts].[Carrier Account ID]","ColumnName":"[SupplierAccounts].[Profile Name]","BehaviorFQTN":"TOne.BusinessEntity.Business.SupplierBehavior, TOne.BusinessEntity.Business"}',3),
(9,'Customer','Customer',0,'{"ColumnID":"[CustomerAccounts].[Carrier Account ID]","ColumnName":"[CustomerAccounts].[Profile Name]","BehaviorFQTN":"TOne.BusinessEntity.Business.CustomerBehavior, TOne.BusinessEntity.Business"}',2),
(14,'SaleZone','SaleZone',0,'{"ColumnID":"[SaleZones].[Zone ID]","ColumnName":"[SaleZones].[Z One Name]","Expression":""}',null),
(16,'BWASR','BWASR',1,'{"ColumnName":"[Measures].[BWASR]","Expression":"","Unit":"%"}',null),
(19,'Facttable Count','Facttable',1,'{"ColumnName":"[Measures].[Facttable Count]","Expression":"","Unit":"min/call"}',null),
(20,'Duration In Minutes','Duration',1,'{"ColumnName":"[Measures].[Duration In Minutes]","Expression":"","Unit":"min"}',null),
(21,'ICABR','ICABR',1,'{"ColumnName":"[Measures].[ICABR]","Expression":"","Unit":"%"}',null),
(22,'ICASR','ICASR',1,'{"ColumnName":"[Measures].[ICASR]","Expression":"","Unit":"%"}',null),
(23,'ICCCR','ICCCR',1,'{"ColumnName":"[Measures].[ICCCR]","Expression":"","Unit":"%"}',null),
(27,'ICNER','ICNER',1,'{"ColumnName":"[Measures].[ICNER]","Expression":"","Unit":"%"}',null),
(28,'IIR','IIR',1,'{"ColumnName":"[Measures].[IIR]","Expression":"","Unit":"%"}',null),
(29,'MHT','MHT',1,'{"ColumnName":"[Measures].[MHT]","Expression":"","Unit":"sec"}',null),
(30,'MHT-Per-Call','MHT-Per-Call',1,'{"ColumnName":"[Measures].[MHT-Per-Call]","Expression":"","Unit":"sec"}',null),
(31,'OGABR','OGABR',1,'{"ColumnName":"[Measures].[OGABR]","Expression":"","Unit":"%"}',null),
(32,'OGASR','OGASR',1,'{"ColumnName":"[Measures].[OGASR]","Expression":"","Unit":"%"}',null),
(33,'OGCCR','OGCCR',1,'{"ColumnName":"[Measures].[OGCCR]","Expression":"","Unit":"%"}',null),
(34,'OGNER','OGNER',1,'{"ColumnName":"[Measures].[OGNER]","Expression":"","Unit":"%"}',null),
(35,'OIR','OIR',1,'{"ColumnName":"[Measures].[OIR]","Expression":"","Unit":"%"}',null),
(36,'PDD-Per-Call','PDD-Per-Call',1,'{"ColumnName":"[Measures].[PDD-Per-Call]","Expression":"","Unit":"sec"}',null),
(37,'Seizures','Seizures',1,'{"ColumnName":"[Measures].[Seizures]","Expression":"","Unit":"calls"}',null),
(41,'Switch','Switch',0,'{"ColumnID":"[Switch].[Switch ID]","ColumnName":"[Switch].[Name]","Expression":""}',null),
(42,'SaleRate','SaleRate',0,'{"ColumnID":"[SaleRate].[Rate ID]","ColumnName":"[SaleRate].[Rate]","Expression":""}',null),
(43,'CostZones','CostZones',0,'{"ColumnID":"[CostZones].[ID]","ColumnName":"[CostZones].[Z One Name]","Expression":""}',null),
(48,'Customer Priced Duration In Minutes','Priced sale duration ',1,'{"ColumnName":"[Measures].[Customer Priced Duration In Minutes]","Expression":"","Unit":"min"}',null),
(50,'Supplier Priced Duration In Minutes','Priced cost duration',1,'{"ColumnName":"[Measures].[Supplier Priced Duration In Minutes]","Expression":"","Unit":"min"}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DisplayName],[Type],[Configuration],[Rank]))
merge	[BI].[SchemaConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DisplayName] = s.[DisplayName],[Type] = s.[Type],[Configuration] = s.[Configuration],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[DisplayName],[Type],[Configuration],[Rank])
	values(s.[ID],s.[Name],s.[DisplayName],s.[Type],s.[Configuration],s.[Rank]);
set identity_insert [BI].[SchemaConfiguration] off;
--[sec].[Module]---------------------------701 to 800---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(701,'Carriers','Carriers',null,101,null,2,10),
(702,'Account Manager','Account Manager','Account Manager',null,'/images/menu-icons/Account.png',20,0),
(703,'Management','Management','Management',null,'/images/menu-icons/Management.png',30,0),
(704,'Sale Area','Sale Area','Sale Area',null,'/images/menu-icons/Sale Area.png',40,0),
(705,'Sale Entities','Entities',null,704,null,1,0),
(706,'Sale Rules','Rules',null,704,null,1,0),
(707,'Purchase Area','Purchase Area','Purchase Area',null,'/images/menu-icons/Purchase Area.png',50,0),
(708,'Purchase Entities','Entities',null,707,null,1,0),
(709,'Purchase Rules','Rules',null,707,null,1,0),
(710,'Routing',null,null,null,null,10,0),
(711,'Routing Rules','Rules',null,710,null,1,0),
(712,'CDR Process','CDR Process','CDR Process',null,null,60,0),
(713,'CDR Process Rules','Rules',null,712,null,1,0),
(714,'Traffic Analysis','Traffic Analysis','NOC',null,'/images/menu-icons/NOC.png',70,1),
(715,'Billing','Billing','Billing',null,'/images/menu-icons/billing.png',80,0),
(716,'Prepaid-Postpaid','Prepaid-Postpaid','Prepaid-Postpaid',null,'/images/menu-icons/post paid - pre paid.png',90,0),
(717,'Plugins','Plugins','Plugins',null,'/images/menu-icons/plug.png',100,0),
(718,'Dynamic Management','Dynamic Management','Dynamic Management',1,null,110,0),
(719,'Business Intelligence','Business Intelligence','BI',null,'/images/menu-icons/busines intel.png',120,1)
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
(7002,'Switches','Switches','#/view/WhS_BusinessEntity/Views/Switch/SwitchManagement',101,null,null,null,null,0,5),
(7003,'Code Groups','Code Groups','#/view/WhS_BusinessEntity/Views/CodeGroup/CodeGroupManagement',102,null,null,null,null,0,7),
(7004,'Zone Service Config','Zone Service Config','#/view/WhS_BusinessEntity/Views/ZoneServiceConfig/ZoneServiceConfigManagement',102,null,null,null,null,0,8),
(7005,'Carrier Profiles','Carrier Profiles','#/view/WhS_BusinessEntity/Views/CarrierAccount/CarrierProfileManagement',701,null,null,null,null,0,2),
(7006,'Carrier Accounts','Carrier Accounts','#/view/WhS_BusinessEntity/Views/CarrierAccount/CarrierAccountManagement',701,null,null,null,null,0,3),
(7007,'Account Manager','Account Manager','#/view/WhS_BusinessEntity/Views/AccountManager/AccountManagerManagement',702,null,null,null,null,0,2),
(7008,'Carrier Summary',' Carrier Summary','#/view/WhS_Analytics/Views/CarrierSummary',703,null,null,null,null,0,2),
(7009,'Daily Report','Daily Report','#/view/WhS_Analytics/Views/DailyReport',703,null,null,null,null,0,3),
(7010,'Rate Plan','Rate Plan','#/view/Whs_Sales/Views/RatePlan',704,null,null,null,null,0,4),
(7011,'Code Preparation','Code Preparation','#/view/WhS_CodePreparation/Views/CodePreparationManagement',704,null,null,null,null,0,5),
(7012,'Sale Price Lists','Sale Price Lists','#/view/Whs_BusinessEntity/views/SalePriceList/SalePriceList',705,null,null,null,null,0,8),
(7013,'Sale Zones','Sale Zones','#/view/WhS_BusinessEntity/Views/SaleZone/SaleZoneManagement',705,null,null,null,null,0,5),
(7014,'Sale Codes','Sale Codes','#/view/WhS_BusinessEntity/Views/SaleCode/SaleCodeManagement',705,null,null,null,null,0,6),
(7015,'Sale Rates','Sale Rates','#/view/WhS_BusinessEntity/Views/SaleRate/SaleRateManagement',705,null,null,null,null,0,7),
(7016,'Selling Products','Selling Products','#/view/WhS_BusinessEntity/Views/SellingProduct/SellingProductManagement',705,null,null,null,null,0,3),
(7017,'Customer Selling Product','Customer Selling Product','#/view/WhS_BusinessEntity/Views/SellingProduct/CustomerSellingProductManagement',705,null,null,null,null,0,4),
(7018,'Selling Number Plans','Selling Number Plans','#/view/WhS_BusinessEntity/Views/SellingNumberPlan/SellingNumberPlanManagement',705,null,null,null,null,0,2),
(7019,'Sale Pricing Rules','Sale Pricing Rules','#/view/WhS_BusinessEntity/Views/PricingRule/SalePricingRuleManagement',706,null,null,null,null,0,2),
(7020,'Selling Rules','Selling Rules','#/view/WhS_Sales/Views/SellingRule/SellingRuleManagement',706,null,null,null,null,0,3),
(7021,'Import Supplier Price List','Import Supplier Price List','#/view/WhS_SupplierPriceList/Views/SupplierPriceList',707,null,null,null,null,0,4),
(7022,'Supplier Zones','Supplier Zones','#/view/WhS_BusinessEntity/Views/SupplierZone/SupplierZoneManagement',708,null,null,null,null,0,3),
(7023,'Supplier Codes','Supplier Codes','#/view/WhS_BusinessEntity/Views/SupplierCode/SupplierCodeManagement',708,null,null,null,null,0,4),
(7024,'Supplier Rates','Supplier Rates','#/view/WhS_BusinessEntity/Views/SupplierRate/SupplierRateManagement',708,null,null,null,null,0,5),
(7025,'Supplier Price Lists','Supplier Price Lists','#/view/WhS_BusinessEntity/Views/SupplierPricelist/SupplierPricelist',708,null,null,null,null,0,2),
(7026,'Purchase Pricing Rules','Purchase Pricing Rules','#/view/WhS_BusinessEntity/Views/PricingRule/PurchasePricingRuleManagement',709,null,null,null,null,0,10),
(7027,'Routing Products','Routing Products','#/view/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductManagement',710,null,null,null,null,0,4),
(7028,'Product Routes','Product Routes','#/view/WhS_Routing/Views/RPRoute/RPRouteManagement',710,null,null,null,null,0,5),
(7029,'Customer Routes','Customer Routes','#/view/WhS_Routing/Views/CustomerRoute/CustomerRouteManagement',710,null,null,null,null,0,3),
(7030,'Route Rules','Route Rules','#/view/WhS_Routing/Views/RouteRule/RouteRuleManagement',711,null,null,null,null,0,2),
(7031,'Customer Identification Rules','Customer Identification Rules','#/view/WhS_CDRProcessing/Views/CustomerRule/CustomerIdentificationRuleManagement',713,null,null,null,null,0,2),
(7032,'Supplier Identification Rules','Supplier Identification Rules','#/view/WhS_CDRProcessing/Views/SupplierRule/SupplierIdentificationRuleManagement',713,null,null,null,null,0,3),
(7033,'Switch Identification Rules','Switch Identification Rules','#/view/WhS_CDRProcessing/Views/SwitchRule/SwitchIdentificationRuleManagement',713,null,null,null,null,0,4),
(7034,'Normalization Rules','Normalization Rules','#/view/WhS_CDRProcessing/Views/NormalizationRule/NormalizationRuleManagement',713,null,null,null,null,0,5),
(7035,'Traffic Monitor','Traffic Monitor','#/view/WhS_Analytics/Views/TrafficMonitor',714,null,null,null,null,0,2),
(7036,'Hourly Report','Hourly Report','#/view/WhS_Analytics/Views/HourlyReport',714,null,null,null,null,0,3),
(7037,'CDR Log','CDR Log','#/view/WhS_Analytics/Views/CDR/CDRLog',714,null,null,null,null,0,4),
(7038,'Raw CDR Log','Raw CDR Log','#/view/WhS_Analytics/Views/RawCDR/RawCDRLog',714,null,null,null,null,0,5),
(7039,'Widgets','Widgets Management','#/view/Security/Views/WidgetsPages/WidgetManagement',718,null,null,null,null,0,2),
(7040,'Pages','Dynamic Pages Management','#/view/Security/Views/DynamicPages/DynamicPageManagement',718,null,null,null,null,0,3)
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
;with cte_data([Id],[Name],[ParentId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(2,'Routing Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(3,'Business Intelligence Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(4,'Sales Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(5,'Business Entity Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(7,'Billing Module',3,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(8,'Trafic Module',3,0,'["View","Add","Edit", "Delete", "Full Control"]')
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
	values(s.[Id],s.[Name],s.[ParentId],s.[BreakInheritance],s.[PermissionOptions]);
set identity_insert [sec].[BusinessEntityModule] off;


--[sec].[BusinessEntity]-------------------1801 to 2100-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(13,'Integration','Integration',2,0,'["View"]'),
(14,'Business Process','Business Process',2,0,'["View"]'),
(15,'Strategy','Strategy',4,0,'["View","Add","Edit", "Full Control"]'),
(16,'Network Infrastructure','Network Infrastructure',2,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(17,'Normalization Rule','Normalization Rule',2,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(18,'Case Management','Case Management',4,0,'["View","Edit"]'),
(19,'Number Prefixes','Number Prefixes',2,0,'["View","Edit"]'),
(20,'Strategy Execution','Strategy Execution',4,0,'["View"]'),
(21,'Related Numbers','Related Numbers',4,0,'["View"]'),
(22,'CDR','CDR',4,0,'["View"]'),
(24,'VR_Runtime_SchedulerTask','Schedule Services',3,0,'["View", "Add", "Edit", "ViewMyTask"]')
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

--[sec].[Permission]---
set nocount on;
;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(0,'1',0,'1','[{"Name":"Full Control","Value":1}]'),
(0,'1',1,'15','[{"Name":"View","Value":1}, {"Name":"Assign Permissions","Value":1}]'),
(0,'1',1,'17','[{"Name":"View","Value":1}, {"Name":"Edit","Value":1}, {"Name":"Delete","Value":1}, {"Name":"Add","Value":1}, {"Name":"Validate","Value":1}]')
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


--[common].[TemplateConfig]----------1 to 10000---------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[TemplateConfig] on;
;with cte_data([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(2,'Selective Sale Zones','WhS_BE_SaleZoneGroup','vr-whs-be-salezonegroup-selective','TOne.WhS.BusinessEntity.Business.SelectiveSaleZonesBehavior, TOne.WhS.BusinessEntity.Business',null),
(4,'Selective Suppliers','WhS_BE_SupplierGroup','vr-whs-be-suppliergroup-selective','TOne.WhS.BusinessEntity.Business.SelectiveSuppliersBehavior, TOne.WhS.BusinessEntity.Business',null),
(7,'All Except Sale Zones','WhS_BE_SaleZoneGroup','vr-whs-be-salezonegroup-allexcept','TOne.WhS.BusinessEntity.Business.AllExceptSaleZonesBehavior, TOne.WhS.BusinessEntity.Business',null),
(8,'Selective Customers','WhS_BE_CustomerGroup','vr-whs-be-customergroup-selective','TOne.WhS.BusinessEntity.Business.SelectiveCustomersBehavior, TOne.WhS.BusinessEntity.Business',null),
(9,'Selective Code Criteria','WhS_BE_CodeCriteriaGroup','vr-whs-be-codecriteriagroup-selective','TOne.WhS.BusinessEntity.Business.SelectiveCodeCriteriaBehavior, TOne.WhS.BusinessEntity.Business',null),
(10,'Days Of Week','WhS_BE_PricingRuleRateTypeSettings','vr-whs-be-pricingrulesettings-ratetype-daysofweek','TOne.WhS.BusinessEntity.MainExtensions.PricingRules.RateTypeSettings.DaysOfWeekRateTypeSettings, TOne.WhS.BusinessEntity.MainExtensions',null),
(11,'Regular Tariff','WhS_BE_PricingRuleTariffSettings','vr-whs-be-pricingrulesettings-tariff-regular','TOne.WhS.BusinessEntity.Business.PricingRules.RuleTypes.TariffRegularTariffBehavior, TOne.WhS.BusinessEntity.Business',null),
(12,'Fixed Extra Charge','WhS_BE_PricingRuleExtraChargeSettings','vr-whs-be-pricingrulesettings-extracharge-fixed','TOne.WhS.BusinessEntity.MainExtensions.PricingRules.ExtraChargeActions.FixedExtraChargeSettings, TOne.WhS.BusinessEntity.MainExtensions',null),
(13,'Percentage Extra Charge','WhS_BE_PricingRuleExtraChargeSettings','vr-whs-be-pricingrulesettings-extracharge-percentage','TOne.WhS.BusinessEntity.MainExtensions.PricingRules.ExtraChargeActions.PercentageExtraChargeSettings, TOne.WhS.BusinessEntity.MainExtensions',null),
(14,'Block','WhS_Routing_RouteRuleSettingsType','vr-whs-routing-routerulesettings-block',null,null),
(15,'Regular','WhS_Routing_RouteRuleSettingsType','vr-whs-routing-routerulesettings-regular',null,null),
(16,'Selective Suppliers With Zones','WhS_BE_SuppliersWithZonesGroupSettings','vr-whs-be-supplierswithzones-selective','TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups.SelectiveSuppliersWithZonesGroup, TOne.WhS.BusinessEntity.MainExtensions',null),
(17,'Selective Option','WhS_Routing_RouteOptionSettingsGroup','vr-whs-routing-routerulesettings-selective',null,null),
(18,'By Rate','WhS_Routing_RouteRuleOptionOrderSettings','vr-whs-routing-routerulesettings-order-byrate',null,null),
(19,'Remove Loss','WhS_Routing_RouteRuleOptionFilterSettings','vr-whs-routing-routerulesettings-filter-removeloss',null,null),
(20,'Fixed','WhS_Routing_RouteRuleOptionPercentageSettings','vr-whs-routing-routerulesettings-percentage-fixed',null,null),
(22,'AddPrefix','VR_Rules_NormalizeNumberAction','vr-rules-normalizationnumbersettings-addprefix',null,null),
(23,'ReplaceString','VR_Rules_NormalizeNumberAction','vr-rules-normalizationnumbersettings-replacestring',null,null),
(24,'Substring','VR_Rules_NormalizeNumberAction','vr-rules-normalizationnumbersettings-substring',null,null),
(25,'Minimum Profit','WhS_Routing_RouteRuleOptionFilterSettings','vr-whs-routing-routerulesettings-filter-minprofit',null,null),
(26,'Specific Day','WhS_BE_PricingRuleRateTypeSettings','vr-whs-be-pricingrulesettings-ratetype-specific','TOne.WhS.BusinessEntity.MainExtensions.PricingRules.RateTypeSettings.SpecificDayRateTypeSettings, TOne.WhS.BusinessEntity.MainExtensions',null),
(27,'Highest Rate','WhS_Routing_SupplierZoneToRPOptionType','vr-whs-routing-policy-rate-highest','TOne.WhS.Routing.Business.SupplierZoneToRPOptionHighestRatePolicy, TOne.WhS.Routing.Business',null),
(29,'Lowest Rate','WhS_Routing_SupplierZoneToRPOptionType','vr-whs-routing-policy-rate-lowest','TOne.WhS.Routing.Business.SupplierZoneToRPOptionLowestRatePolicy, TOne.WhS.Routing.Business',null),
(30,'Average Rate','WhS_Routing_SupplierZoneToRPOptionType','vr-whs-routing-policy-rate-average','TOne.WhS.Routing.Business.SupplierZoneToRPOptionAverageRatePolicy, TOne.WhS.Routing.Business',null),
(31,'Average','WhS_Sales_CostCalculationMethod','vr-whs-sales-avgcostcalculation',null,null),
(32,'Custom Percentage','WhS_Sales_CostCalculationMethod','vr-whs-sales-percentagecostcalculation',null,null),
(33,'Route Percentage','WhS_Sales_CostCalculationMethod','vr-whs-sales-routepercentagecostcalculation',null,null),
(34,'Text','VRGeneric_DataRecordFieldType','vr-genericdata-text','Vanrise.Common.MainExtensions.DataRecordFieldTextType, Vanrise.Common.MainExtensions',null),
(36,'Number','VRGeneric_DataRecordFieldType','vr-genericdata-number','Vanrise.Common.MainExtensions.DataRecordFieldNumberType, Vanrise.Common.MainExtensions',null),
(37,'DateTime','VRGeneric_DataRecordFieldType','vr-genericdata-datetime','Vanrise.Common.MainExtensions.DataRecordFieldDateTimeType,Vanrise.Common.MainExtensions',null),
(38,'Choices','VRGeneric_DataRecordFieldType','vr-genericdata-choices','Vanrise.Common.MainExtensions.DataRecordFieldChoicesType, Vanrise.Common.MainExtensions',null),
(39,'Boolean','VRGeneric_DataRecordFieldType','vr-genericdata-boolean','Vanrise.Common.MainExtensions.DataRecordFieldBooleanType, Vanrise.Common.MainExtensions',null),
(40,'Fixed','WhS_Sales_RateCalculationMethod','vr-whs-sales-fixedratecalculation',null,null),
(41,'Margin','WhS_Sales_RateCalculationMethod','vr-whs-sales-marginratecalculation',null,null),
(42,'Margin Percentage','WhS_Sales_RateCalculationMethod','vr-whs-sales-marginpercentageratecalculation',null,null),
(44,'Carrier Portal Connector','CP_SupplierPriceList_ConnectorUploadPriceList','vr-cp-supplierpricelist-connector',null,null),
(49,'Margin','WhS_Sales_SellingRuleSettingsType','vr-whs-sales-sellingrulesettings-margin',null,null),
(51,'Fixed','WhS_Sales_SellingRuleSettingsType','vr-whs-sales-sellingrulesettings-fixed',null,null),
(53,'Mapping','VR_GenericData_GenericRuleDefinitionSettings','vr-genericdata-genericruledefinitionsettings-mapping',null,null),
(55,'Days Of Week','VR_Rules_PricingRuleRateTypeSettings','vr-rules-pricingrulesettings-ratetype-daysofweek',null,null),
(57,'Specific','VR_Rules_PricingRuleRateTypeSettings','vr-rules-pricingrulesettings-ratetype-specific',null,null),
(58,'rules','VR_Rules_PricingRuleTariffSettings','vr-rules-pricingrulesettings-tariff-regular',null,null),
(59,'Fixed Extra Charge','VR_Rules_PricingRuleExtraChargeSettings','vr-rules-pricingrulesettings-extracharge-fixed',null,null),
(60,'Percentage Extra Charge','VR_Rules_PricingRuleExtraChargeSettings','vr-rules-pricingrulesettings-extracharge-percentage',null,null),
(61,'Fixed Rate Value','VR_Rules_PricingRuleRateValueSettings','vr-rules-pricingrulesettings-ratevalue-fixed',null,null)
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

--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('WhS_BE/RoutingProduct/GetFilteredRoutingProducts',null),
('WhS_BE/RoutingProduct/AddRoutingProduct',null),
('WhS_Routing/RouteRule/AddRule',null),
('WhS_BE/CarrierAccount/GetFilteredCarrierAccounts','Carrier:View'),
('WhS_BE/CarrierAccount/UpdateCarrierAccount','Carrier:Edit'),
('WhS_BE/CarrierAccount/AddCarrierAccount','Carrier:Add')
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

--[bp].[BPTaskType]-------------------------1 to 10000----------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [bp].[BPTaskType] on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'TOne.WhS.SupplierPriceList.BP.Arguments.Tasks.PreviewTaskData','{"$type":"Vanrise.BusinessProcess.Entities.BPTaskTypeSettings, Vanrise.BusinessProcess.Entities","Editor":"/Client/Modules/WhS_SupplierPriceList/Views/SupplierPriceListPreview.html"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[bp].[BPTaskType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);
set identity_insert [bp].[BPTaskType] off;

--[bp].[BPDefinition]----------------------1 to 1000------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [bp].[BPDefinition] on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'TOne.WhS.Routing.BP.Arguments.BuildRoutesByCodePrefixInput','Build Routes By Code Prefix','TOne.WhS.Routing.BP.BuildRoutesByCodePrefix, TOne.WhS.Routing.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ScheduledExecEditor":"","ManualExecEditor":"vr-whs-routing-buildbycodeprefix","RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/BuildRoutesByCodePrefix.html"}'),
(2,'TOne.WhS.Routing.BP.Arguments.RoutingProcessInput','Whole Sale Routing Process Process','TOne.WhS.Routing.BP.RoutingProcess, TOne.WhS.Routing.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ScheduledExecEditor":"","ManualExecEditor":"vr-whs-routing-buildrouteprocess","RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/BuildRouteProcess.html"}'),
(3,'TOne.WhS.SupplierPriceList.BP.Arguments.SupplierPriceListProcessInput','Import Supplier PriceList Process','TOne.WhS.SupplierPriceList.BP.ImportSupplierPriceList, TOne.WhS.SupplierPriceList.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}'),
(4,'TOne.WhS.Routing.BP.Arguments.RPRoutingProcessInput','Routing Product Routing Process','TOne.WhS.Routing.BP.RPRoutingProcess, TOne.WhS.Routing.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ScheduledExecEditor":"","ManualExecEditor":"vr-whs-routing-rpbuildproduct","RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/RPBuildProductRoutesProcess.html"}'),
(5,'TOne.WhS.Routing.BP.Arguments.RPBuildCodeMatchesByCodePrefixInput','Routing Product Build Code Matches By Code Prefix','TOne.WhS.Routing.BP.RPBuildCodeMatchesByCodePrefix, TOne.WhS.Routing.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false}'),
(6,'TOne.WhS.Routing.BP.Arguments.RPBuildRoutingProductInput','Routing Product Build Process Input','TOne.WhS.Routing.BP.RPBuildRoutingProducts, TOne.WhS.Routing.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false}'),
(7,'TOne.WhS.CodePreparation.BP.Arguments.CodePreparationInput','Code Preparation Process','TOne.WhS.CodePreparation.BP.CodePreparation, TOne.WhS.CodePreparation.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([ID],[Name],[Title],[FQTN],[Config])
	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
set identity_insert [bp].[BPDefinition] off;