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
--[BI].[SchemaConfiguration]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
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

--[sec].[Module]---------------------------1301 to 1400---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1201,'Carriers',null,101,null,2,10),
(1202,'Account Manager','Account Manager',null,'/images/menu-icons/Account.png',20,0),
(1203,'Management','Management',null,'/images/menu-icons/Management.png',30,0),
(1204,'Sale Area','Sale Area',null,'/images/menu-icons/Sale Area.png',40,0),
(1205,'Sale Entities',null,1204,null,1,0),
(1206,'Sale Rules',null,1204,null,1,0),
(1207,'Purchase Area',null,null,'/images/menu-icons/Purchase Area.png',50,0),
(1208,'Purchase Entities',null,1207,null,1,0),
(1209,'Purchase Rules',null,1207,null,1,0),
(1210,'Routing',null,null,null,10,0),
(1211,'Routing Rules',null,1210,null,1,0),
(1212,'CDR Process',null,null,null,60,0),
(1213,'CDR Process Rules',null,1212,null,1,0),
(1214,'Traffic Analysis',null,null,'/images/menu-icons/NOC.png',70,1),
(1215,'Billing',null,null,'/images/menu-icons/billing.png',80,0),
(1216,'Prepaid-Postpaid',null,null,'/images/menu-icons/post paid - pre paid.png',90,0),
(1217,'Plugins',null,null,'/images/menu-icons/plug.png',100,0)
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

--[sec].[View]-----------------------------12001 to 13000-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(12001,'Switches','Switches','#/view/WhS_BusinessEntity/Views/Switch/SwitchManagement',101,'WhS_BE/Switch/GetFilteredSwitches',null,null,null,0,5),
(12002,'Code Groups','Code Groups','#/view/WhS_BusinessEntity/Views/CodeGroup/CodeGroupManagement',102,'WhS_BE/CodeGroup/GetFilteredCodeGroups',null,null,null,0,12),
(12003,'Zone Service Config','Zone Service Config','#/view/WhS_BusinessEntity/Views/ZoneServiceConfig/ZoneServiceConfigManagement',102,'WhS_BE/ZoneServiceConfig/GetFilteredZoneServiceConfigs',null,null,null,0,24),
(12004,'Carrier Profiles','Carrier Profiles','#/view/WhS_BusinessEntity/Views/CarrierAccount/CarrierProfileManagement',1201,'WhS_BE/CarrierProfile/GetFilteredCarrierProfiles',null,null,null,0,2),
(12005,'Carrier Accounts','Carrier Accounts','#/view/WhS_BusinessEntity/Views/CarrierAccount/CarrierAccountManagement',1201,'WhS_BE/CarrierAccount/GetFilteredCarrierAccounts',null,null,null,0,3),
(12006,'Account Manager','Account Manager','#/view/WhS_BusinessEntity/Views/AccountManager/AccountManagerManagement',1202,'WhS_BE/AccountManager/GetFilteredAssignedCarriers',null,null,null,0,2),
(12007,'Carrier Summary',' Carrier Summary','#/view/WhS_Analytics/Views/CarrierSummary',1203,null,null,null,null,0,2),
(12008,'Daily Report','Daily Report','#/view/WhS_Analytics/Views/DailyReport',1203,null,null,null,null,0,3),
(12009,'Rate Plan','Rate Plan','#/view/Whs_Sales/Views/RatePlan',1204,null,null,null,null,0,4),
(12010,'Numbering Plan','Numbering Plan Management','#/view/WhS_CodePreparation/Views/CodePreparationManagement',1204,null,null,null,null,0,5),
(12011,'Sale Price Lists','Sale Price Lists','#/view/Whs_BusinessEntity/views/SalePriceList/SalePriceList',1205,'WhS_BE/SalePricelist/GetFilteredSalePriceLists',null,null,null,0,8),
(12012,'Sale Zones','Sale Zones','#/view/WhS_BusinessEntity/Views/SaleZone/SaleZoneManagement',1205,'WhS_BE/SaleZone/GetFilteredSaleZones',null,null,null,0,5),
(12013,'Sale Codes','Sale Codes','#/view/WhS_BusinessEntity/Views/SaleCode/SaleCodeManagement',1205,'WhS_BE/SaleCode/GetFilteredSaleCodes',null,null,null,0,6),
(12014,'Sale Rates','Sale Rates','#/view/WhS_BusinessEntity/Views/SaleRate/SaleRateManagement',1205,'WhS_BE/SaleRate/GetFilteredSaleRate',null,null,null,0,7),
(12015,'Selling Products','Selling Products','#/view/WhS_BusinessEntity/Views/SellingProduct/SellingProductManagement',1205,'WhS_BE/SellingProduct/GetFilteredSellingProducts',null,null,null,0,3),
(12016,'Customer Selling Product','Customer Selling Product','#/view/WhS_BusinessEntity/Views/SellingProduct/CustomerSellingProductManagement',1205,'WhS_BE/CustomerSellingProduct/GetFilteredCustomerSellingProducts',null,null,null,0,4),
(12017,'Selling Number Plans','Selling Number Plans','#/view/WhS_BusinessEntity/Views/SellingNumberPlan/SellingNumberPlanManagement',1205,'WhS_BE/SellingNumberPlan/GetFilteredSellingNumberPlans',null,null,null,0,2),
(12018,'Sale Pricing Rules','Sale Pricing Rules','#/view/WhS_BusinessEntity/Views/PricingRule/SalePricingRuleManagement',1206,null,null,null,null,0,2),
(12019,'Selling Rules','Selling Rules','#/view/WhS_Sales/Views/SellingRule/SellingRuleManagement',1206,'WhS_Sales/SellingRule/GetFilteredSellingRules',null,null,null,0,3),
(12020,'Import Supplier Price List','Import Supplier Price List','#/view/WhS_SupplierPriceList/Views/SupplierPriceList',1207,null,null,null,null,0,4),
(12021,'Supplier Zones','Supplier Zones','#/view/WhS_BusinessEntity/Views/SupplierZone/SupplierZoneManagement',1208,'WhS_BE/SupplierZone/GetFilteredSupplierZones',null,null,null,0,3),
(12022,'Supplier Codes','Supplier Codes','#/view/WhS_BusinessEntity/Views/SupplierCode/SupplierCodeManagement',1208,'WhS_BE/SupplierCode/GetFilteredSupplierCodes',null,null,null,0,4),
(12023,'Supplier Rates','Supplier Rates','#/view/WhS_BusinessEntity/Views/SupplierRate/SupplierRateManagement',1208,'WhS_BE/SupplierRate/GetFilteredSupplierRates',null,null,null,0,5),
(12024,'Supplier Price Lists','Supplier Price Lists','#/view/WhS_BusinessEntity/Views/SupplierPricelist/SupplierPricelist',1208,'GetFilteredSupplierPricelist',null,null,null,0,6),
(12025,'Purchase Pricing Rules','Purchase Pricing Rules','#/view/WhS_BusinessEntity/Views/PricingRule/PurchasePricingRuleManagement',1209,null,null,null,null,0,10),
(12026,'Routing Products','Routing Products','#/view/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductManagement',1210,null,null,null,null,0,4),
(12027,'Product Routes','Product Routes','#/view/WhS_Routing/Views/RPRoute/RPRouteManagement',1210,'WhS_Routing/RPRoute/GetFilteredRPRoutes',null,null,null,0,5),
(12028,'Customer Routes','Customer Routes','#/view/WhS_Routing/Views/CustomerRoute/CustomerRouteManagement',1210,'WhS_Routing/CustomerRoute/GetFilteredCustomerRoutes',null,null,null,0,3),
(12029,'Route Rules','Route Rules','#/view/WhS_Routing/Views/RouteRule/RouteRuleManagement',1211,'WhS_Routing/RouteRule/GetFilteredRouteRules',null,null,null,0,2),
(12030,'Route Options Rules','Route Options Rules','#/view/WhS_Routing/Views/RouteOptionRule/RouteOptionRuleManagement',1211,'WhS_Routing/RouteOptionRule/GetFilteredRouteOptionRules',null,null,null,0,2),
(12031,'Customer Identification Rules','Customer Identification Rules','#/view/WhS_CDRProcessing/Views/CustomerRule/CustomerIdentificationRuleManagement',1213,null,null,null,null,0,2),
(12032,'Supplier Identification Rules','Supplier Identification Rules','#/view/WhS_CDRProcessing/Views/SupplierRule/SupplierIdentificationRuleManagement',1213,null,null,null,null,0,3),
(12033,'Switch Identification Rules','Switch Identification Rules','#/view/WhS_CDRProcessing/Views/SwitchRule/SwitchIdentificationRuleManagement',1213,null,null,null,null,0,4),
(12034,'Normalization Rules','Normalization Rules','#/view/WhS_CDRProcessing/Views/NormalizationRule/NormalizationRuleManagement',1213,null,null,null,null,0,5),
(12035,'Traffic Monitor','Traffic Monitor','#/view/WhS_Analytics/Views/TrafficMonitor',1214,null,null,null,null,0,2),
(12036,'Hourly Report','Hourly Report','#/view/WhS_Analytics/Views/HourlyReport',1214,null,null,null,null,0,3),
(12037,'CDR Log','CDR Log','#/view/WhS_Analytics/Views/CDR/CDRLog',1214,null,null,null,null,0,4),
(12038,'Raw CDR Log','Raw CDR Log','#/view/WhS_Analytics/Views/RawCDR/RawCDRLog',1214,null,null,null,null,0,5)
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

--[sec].[BusinessEntityModule]-------------1201 to 1300---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1201,'Purchase',1,0)
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


--[sec].[BusinessEntity]-------------------3301 to 3600-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(3301,'WhS_BE_SupplierPricelist','Supplier PriceList',1201,0,'["View"]')
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

--[common].[TemplateConfig]----------1 to 10000---------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[TemplateConfig] on;
;with cte_data([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Specific Sales Zones','WhS_BE_SaleZoneGroup','vr-whs-be-salezonegroup-selective','TOne.WhS.BusinessEntity.Business.SelectiveSaleZonesBehavior, TOne.WhS.BusinessEntity.Business',null),
(2,'Selective Suppliers','WhS_BE_SupplierGroup','vr-whs-be-suppliergroup-selective','TOne.WhS.BusinessEntity.Business.SelectiveSuppliersBehavior, TOne.WhS.BusinessEntity.Business',null),
(3,'All Sales Zones Except','WhS_BE_SaleZoneGroup','vr-whs-be-salezonegroup-allexcept','TOne.WhS.BusinessEntity.Business.AllExceptSaleZonesBehavior, TOne.WhS.BusinessEntity.Business',null),
(4,'Specific Customers','WhS_BE_CustomerGroup','vr-whs-be-customergroup-selective','TOne.WhS.BusinessEntity.Business.SelectiveCustomersBehavior, TOne.WhS.BusinessEntity.Business',null),
(5,'Selective Code Criteria','WhS_BE_CodeCriteriaGroup','vr-whs-be-codecriteriagroup-selective','TOne.WhS.BusinessEntity.Business.SelectiveCodeCriteriaBehavior, TOne.WhS.BusinessEntity.Business',null),
(6,'Days Of Week','WhS_BE_PricingRuleRateTypeSettings','vr-whs-be-pricingrulesettings-ratetype-daysofweek','TOne.WhS.BusinessEntity.MainExtensions.PricingRules.RateTypeSettings.DaysOfWeekRateTypeSettings, TOne.WhS.BusinessEntity.MainExtensions',null),
(7,'Regular Tariff','WhS_BE_PricingRuleTariffSettings','vr-whs-be-pricingrulesettings-tariff-regular','TOne.WhS.BusinessEntity.Business.PricingRules.RuleTypes.TariffRegularTariffBehavior, TOne.WhS.BusinessEntity.Business',null),
(8,'Fixed Extra Charge','WhS_BE_PricingRuleExtraChargeSettings','vr-whs-be-pricingrulesettings-extracharge-fixed','TOne.WhS.BusinessEntity.MainExtensions.PricingRules.ExtraChargeActions.FixedExtraChargeSettings, TOne.WhS.BusinessEntity.MainExtensions',null),
(9,'Percentage Extra Charge','WhS_BE_PricingRuleExtraChargeSettings','vr-whs-be-pricingrulesettings-extracharge-percentage','TOne.WhS.BusinessEntity.MainExtensions.PricingRules.ExtraChargeActions.PercentageExtraChargeSettings, TOne.WhS.BusinessEntity.MainExtensions',null),
(10,'Block','WhS_Routing_RouteRuleSettingsType','vr-whs-routing-routerulesettings-block',null,null),
(11,'Standard Rule','WhS_Routing_RouteRuleSettingsType','vr-whs-routing-routerulesettings-regular',null,null),
(12,'Selective Suppliers With Zones','WhS_BE_SuppliersWithZonesGroupSettings','vr-whs-be-supplierswithzones-selective','TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups.SelectiveSuppliersWithZonesGroup, TOne.WhS.BusinessEntity.MainExtensions',null),
(13,'Selective Option','WhS_Routing_RouteOptionSettingsGroup','vr-whs-routing-routerulesettings-selective',null,null),
(14,'By Rate','WhS_Routing_RouteRuleOptionOrderSettings','vr-whs-routing-routerulesettings-order-byrate',null,'{"$type":"TOne.WhS.Routing.Entities.OptionOrderTemplateConfigSettings, TOne.WhS.Routing.Entities","IsRequired":true}'),
(15,'By Rate','WhS_Routing_RouteRuleOptionFilterSettings','vr-whs-routing-routerulesettings-filter-rate',null,null),
(16,'Fixed','WhS_Routing_RouteRuleOptionPercentageSettings','vr-whs-routing-routerulesettings-percentage-fixed',null,null),
(17,'Specific Day','WhS_BE_PricingRuleRateTypeSettings','vr-whs-be-pricingrulesettings-ratetype-specific','TOne.WhS.BusinessEntity.MainExtensions.PricingRules.RateTypeSettings.SpecificDayRateTypeSettings, TOne.WhS.BusinessEntity.MainExtensions',null),
(18,'Highest Rate','WhS_Routing_SupplierZoneToRPOptionType','vr-whs-routing-policy-rate-highest','TOne.WhS.Routing.Business.SupplierZoneToRPOptionHighestRatePolicy, TOne.WhS.Routing.Business',null),
(19,'Lowest Rate','WhS_Routing_SupplierZoneToRPOptionType','vr-whs-routing-policy-rate-lowest','TOne.WhS.Routing.Business.SupplierZoneToRPOptionLowestRatePolicy, TOne.WhS.Routing.Business',null),
(20,'Average Rate','WhS_Routing_SupplierZoneToRPOptionType','vr-whs-routing-policy-rate-average','TOne.WhS.Routing.Business.SupplierZoneToRPOptionAverageRatePolicy, TOne.WhS.Routing.Business',null),
(21,'Average','WhS_Sales_CostCalculationMethod','vr-whs-sales-avgcostcalculation',null,null),
(22,'Custom Percentage','WhS_Sales_CostCalculationMethod','vr-whs-sales-percentagecostcalculation',null,null),
(23,'Route Percentage','WhS_Sales_CostCalculationMethod','vr-whs-sales-routepercentagecostcalculation',null,null),
(24,'Text','VRGeneric_DataRecordFieldType','vr-genericdata-text','Vanrise.Common.MainExtensions.DataRecordFieldTextType, Vanrise.Common.MainExtensions',null),
(25,'Number','VRGeneric_DataRecordFieldType','vr-genericdata-number','Vanrise.Common.MainExtensions.DataRecordFieldNumberType, Vanrise.Common.MainExtensions',null),
(26,'DateTime','VRGeneric_DataRecordFieldType','vr-genericdata-datetime','Vanrise.Common.MainExtensions.DataRecordFieldDateTimeType,Vanrise.Common.MainExtensions',null),
(27,'Choices','VRGeneric_DataRecordFieldType','vr-genericdata-choices','Vanrise.Common.MainExtensions.DataRecordFieldChoicesType, Vanrise.Common.MainExtensions',null),
(28,'Boolean','VRGeneric_DataRecordFieldType','vr-genericdata-boolean','Vanrise.Common.MainExtensions.DataRecordFieldBooleanType, Vanrise.Common.MainExtensions',null),
(29,'Fixed','WhS_Sales_RateCalculationMethod','vr-whs-sales-fixedratecalculation',null,null),
(30,'Margin','WhS_Sales_RateCalculationMethod','vr-whs-sales-marginratecalculation',null,null),
(31,'Margin Percentage','WhS_Sales_RateCalculationMethod','vr-whs-sales-marginpercentageratecalculation',null,null),
(32,'Carrier Portal Connector','CP_SupplierPriceList_ConnectorUploadPriceList','vr-cp-supplierpricelist-connector',null,null),
(33,'Margin','WhS_Sales_SellingRuleSettingsType','vr-whs-sales-sellingrulesettings-margin',null,null),
(34,'Fixed','WhS_Sales_SellingRuleSettingsType','vr-whs-sales-sellingrulesettings-fixed',null,null),
(35,'Block','WhS_Routing_RouteOptionRuleSettingsType','vr-whs-routing-routeoptionrulesettings-block',null,null),
(36,'By Quality','WhS_Routing_RouteRuleOptionOrderSettings','vr-whs-routing-routerulesettings-order-byquality',null,null),
(37,'Specific Selling Number Plans','WhS_BE_SaleZoneGroup','vr-whs-be-sellingnumberplan-selective',null,'{"$type":"TOne.WhS.BusinessEntity.MainExtensions.SellingNumberPlan.SelectiveSellingNumberPlanTemplateConfigSettings, TOne.WhS.BusinessEntity.MainExtensions","IsHiddenInRP":true}')
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
('WhS_BE/CarrierAccount/GetFilteredCarrierAccounts','Carrier:View'),
('WhS_BE/CarrierAccount/UpdateCarrierAccount','Carrier:Edit'),
('WhS_BE/CarrierAccount/AddCarrierAccount','Carrier:Add'),

('WhS_BE/CodeGroup/GetFilteredCodeGroups',null),
('WhS_BE/CodeGroup/GetAllCodeGroups',null),
('WhS_BE/CodeGroup/GetCodeGroup',null),
('WhS_BE/CodeGroup/UpdateCodeGroup',null),
('WhS_BE/CodeGroup/AddCodeGroup',null),
('WhS_BE/CodeGroup/DownloadCodeGroupListTemplate',null),
('WhS_BE/CodeGroup/UploadCodeGroupList',null),
('WhS_BE/CodeGroup/DownloadCodeGroupLog',null),

('WhS_BE/SellingNumberPlan/GetFilteredSellingNumberPlans',null),
('WhS_BE/SellingNumberPlan/GetSellingNumberPlans',null),
('WhS_BE/SellingNumberPlan/AddSellingNumberPlan',null),
('WhS_BE/SellingNumberPlan/UpdateSellingNumberPlan',null),
('WhS_BE/SellingNumberPlan/GetSellingNumberPlan',null),

('WhS_BE/SellingProduct/GetFilteredSellingProducts',null),
('WhS_BE/SellingProduct/AddSellingProduct',null),
('WhS_BE/SellingProduct/UpdateSellingProduct',null),
('WhS_BE/SellingProduct/GetSellingProduct',null),
('WhS_BE/SellingProduct/GetSellingProductsInfo',null),

('WhS_BE/CustomerSellingProduct/GetFilteredCustomerSellingProducts',null),
('WhS_BE/CustomerSellingProduct/AddCustomerSellingProduct',null),
('WhS_BE/CustomerSellingProduct/UpdateCustomerSellingProduct',null),
('WhS_BE/CustomerSellingProduct/GetCustomerSellingProduct',null),
('WhS_BE/CustomerSellingProduct/IsCustomerAssignedToSellingProduct',null),

('WhS_BE/CarrierAccount/GetCarrierAccountCurrency',null),
('WhS_BE/CarrierAccount/GetCarrierAccount',null),
('WhS_BE/CarrierAccount/GetCarrierAccountInfo',null),
('WhS_BE/CarrierAccount/GetSupplierGroupTemplates',null),
('WhS_BE/CarrierAccount/GetCustomerGroupTemplates',null),
('WhS_BE/CarrierAccount/GetSuppliersWithZonesGroupsTemplates',null),

('WhS_BE/CarrierProfile/GetFilteredCarrierProfiles',null),
('WhS_BE/CarrierProfile/GetCarrierProfile',null),
('WhS_BE/CarrierProfile/GetCarrierProfilesInfo',null),
('WhS_BE/CarrierProfile/UpdateCarrierProfile',null),
('WhS_BE/CarrierProfile/AddCarrierProfile',null),

('WhS_BE/AccountManager/GetLinkedOrgChartId',null),
('WhS_BE/AccountManager/GetFilteredAssignedCarriers',null),
('WhS_BE/AccountManager/GetAssignedCarrierDetails',null),
('WhS_BE/AccountManager/UpdateLinkedOrgChart',null),
('WhS_BE/AccountManager/AssignCarriers',null),

('WhS_BE/CustomerZone/GetCustomerZones',null),
('WhS_BE/CustomerZone/GetCountriesToSell',null),
('WhS_BE/CustomerZone/AddCustomerZones',null),

('WhS_BE/RoutingProduct/GetFilteredRoutingProducts',null),
('WhS_BE/RoutingProduct/AddRoutingProduct',null),
('WhS_BE/RoutingProduct/GetRoutingProductsInfoBySellingNumberPlan',null),
('WhS_BE/RoutingProduct/GetRoutingProductInfo',null),
('WhS_BE/RoutingProduct/GetRoutingProduct',null),
('WhS_BE/RoutingProduct/UpdateRoutingProduct',null),
('WhS_BE/RoutingProduct/DeleteRoutingProduct',null),

('WhS_BE/SaleCode/GetFilteredSaleCodes',null),

('WhS_BE/SalePricelist/GetFilteredSalePriceLists',null),

('WhS_BE/SaleRate/GetFilteredSaleRate',null),

('WhS_BE/SaleZone/GetFilteredSaleZones',null),
('WhS_BE/SaleZone/GetSaleZonesInfo',null),
('WhS_BE/SaleZone/GetSaleZonesInfoByIds',null),
('WhS_BE/SaleZone/GetSaleZone',null),
('WhS_BE/SaleZone/GetSellingNumberPlanIdBySaleZoneIds',null),
('WhS_BE/SaleZone/GetSaleZoneGroupTemplates',null),
('WhS_BE/SaleZone/GetSaleZonesByName',null),
('WhS_BE/SaleZone/GetSaleZoneInfoByCountryId',null),

('WhS_BE/SupplierCode/GetFilteredSupplierCodes',null),
('WhS_BE/SupplierRate/GetFilteredSupplierRates',null),
('WhS_BE/SupplierZone/GetFilteredSupplierZones',null),
('WhS_BE/SupplierZone/GetSupplierZoneInfo',null),
('WhS_BE/SupplierZone/GetSupplierZoneInfoByIds',null),

('WhS_BE/Switch/GetFilteredSwitches',null),
('WhS_BE/Switch/GetSwitch',null),
('WhS_BE/Switch/AddSwitch',null),
('WhS_BE/Switch/UpdateSwitch',null),
('WhS_BE/Switch/DeleteSwitch',null),
('WhS_BE/Switch/GetSwitchesInfo',null),

('WhS_BE/ZoneServiceConfig/GetFilteredZoneServiceConfigs',null),
('WhS_BE/ZoneServiceConfig/GetAllZoneServiceConfigs',null),
('WhS_BE/ZoneServiceConfig/GetZoneServiceConfig',null),
('WhS_BE/ZoneServiceConfig/UpdateZoneServiceConfig',null),
('WhS_BE/ZoneServiceConfig/AddZoneServiceConfig',null),

('WhS_CodePrep/CodePreparation/GetZoneItems',null),
('WhS_CodePrep/CodePreparation/CheckCodePreparationState',null),
('WhS_CodePrep/CodePreparation/CancelCodePreparationState',null),
('WhS_CodePrep/CodePreparation/GetCodeItems',null),
('WhS_CodePrep/CodePreparation/DownloadImportCodePreparationTemplate',null),
('WhS_CodePrep/CodePreparation/SaveChanges',null),
('WhS_CodePrep/CodePreparation/MoveCodes',null),
('WhS_CodePrep/CodePreparation/CloseCodes',null),
('WhS_CodePrep/CodePreparation/SaveNewZone',null),
('WhS_CodePrep/CodePreparation/SaveNewCode',null),
('WhS_CodePrep/CodePreparation/GetChanges',null),
('WhS_CodePrep/CodePreparation/CloseZone',null),
('WhS_CodePrep/CodePreparation/RenameZone',null),

('WhS_Routing/CustomerRoute/GetFilteredCustomerRoutes',null),

('WhS_Routing/RouteRule/AddRule',null),
('WhS_Routing/RouteRule/GetFilteredRouteRules',null),
('WhS_Routing/RouteRule/GetRule',null),
('WhS_Routing/RouteRule/UpdateRule',null),
('WhS_Routing/RouteRule/DeleteRule',null),
('WhS_Routing/RouteRule/GetCodeCriteriaGroupTemplates',null),
('WhS_Routing/RouteRule/GetRouteRuleSettingsTemplates',null),

('WhS_Routing/RouteRuleSettings/GetRouteOptionSettingsGroupTemplates',null),
('WhS_Routing/RouteRuleSettings/GetRouteOptionOrderSettingsTemplates',null),
('WhS_Routing/RouteRuleSettings/GetRouteOptionFilterSettingsTemplates',null),
('WhS_Routing/RouteRuleSettings/GetRouteOptionPercentageSettingsTemplates',null),

('WhS_Routing/RoutingDatabase/GetRoutingDatabaseInfo',null),

('WhS_Routing/RPRoute/GetFilteredRPRoutes',null),
('WhS_Routing/RPRoute/GetRPRouteOptionSupplier',null),
('WhS_Routing/RPRoute/GetPoliciesOptionTemplates',null),
('WhS_Routing/RPRoute/GetFilteredRPRouteOptions',null),

('WhS_Sales/RatePlan/ValidateCustomer',null),
('WhS_Sales/RatePlan/GetZoneLetters',null),
('WhS_Sales/RatePlan/GetZoneItems',null),
('WhS_Sales/RatePlan/GetZoneItem',null),
('WhS_Sales/RatePlan/GetDefaultItem',null),
('WhS_Sales/RatePlan/GetCostCalculationMethodTemplates',null),
('WhS_Sales/RatePlan/GetRateCalculationMethodTemplates',null),
('WhS_Sales/RatePlan/GetChangesSummary',null),
('WhS_Sales/RatePlan/GetFilteredZoneRateChanges',null),
('WhS_Sales/RatePlan/GetFilteredZoneRoutingProductChanges',null),
('WhS_Sales/RatePlan/SavePriceList',null),
('WhS_Sales/RatePlan/SaveChanges',null),
('WhS_Sales/RatePlan/ApplyCalculatedRates',null),
('WhS_Sales/RatePlan/CheckIfDraftExists',null),
('WhS_Sales/RatePlan/DeleteDraft',null),
('WhS_Sales/SellingRule/GetFilteredSellingRules',null),
('WhS_Sales/SellingRule/GetRule',null),
('WhS_Sales/SellingRule/AddRule',null),
('WhS_Sales/SellingRule/UpdateRule',null),
('WhS_Sales/SellingRule/DeleteRule',null),
('WhS_Sales/SellingRule/GetCodeCriteriaGroupTemplates',null),
('WhS_Sales/SellingRule/GetSellingRuleSettingsTemplates',null),

('WhS_BE/SupplierPricelist/GetFilteredSupplierPricelist','WhS_BE_SupplierPricelist: View'),

('WhS_SupPL/SupplierPriceList/DownloadSupplierPriceListTemplate',null),
('WhS_SupPL/SupplierPriceListPreview/GetFilteredZonePreview',null),
('WhS_SupPL/SupplierPriceListPreview/GetFilteredCodePreview',null),
('WhS_SupPL/SupplierPriceListPreview/GetFilteredRatePreview',null),

('WhS_Routing/RouteOptionRule/AddRule',null),
('WhS_Routing/RouteOptionRule/GetFilteredRouteOptionRules',null),
('WhS_Routing/RouteOptionRule/GetRule',null),
('WhS_Routing/RouteOptionRule/UpdateRule',null),
('WhS_Routing/RouteOptionRule/DeleteRule',null),
('WhS_Routing/RouteOptionRule/GetRouteOptionRuleSettingsTemplates',null)

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
(2,'TOne.WhS.Routing.BP.Arguments.RoutingProcessInput','Whole Sale Routing Process Process','TOne.WhS.Routing.BP.RoutingProcess, TOne.WhS.Routing.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ScheduledExecEditor":"","ManualExecEditor":"vr-whs-routing-buildrouteprocess","RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/BuildRouteProcess.html","HasChildProcesses":true}'),
(3,'TOne.WhS.SupplierPriceList.BP.Arguments.SupplierPriceListProcessInput','Import Supplier PriceList Process','TOne.WhS.SupplierPriceList.BP.ImportSupplierPriceList, TOne.WhS.SupplierPriceList.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false,"HasBusinessRules":true}'),
(4,'TOne.WhS.Routing.BP.Arguments.RPRoutingProcessInput','Routing Product Routing Process','TOne.WhS.Routing.BP.RPRoutingProcess, TOne.WhS.Routing.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ScheduledExecEditor":"","ManualExecEditor":"vr-whs-routing-rpbuildproduct","RetryOnProcessFailed":false,"Url":"/Client/Modules/WhS_Routing/Views/ProcessInputTemplates/RPBuildProductRoutesProcess.html","HasChildProcesses":true}'),
(5,'TOne.WhS.Routing.BP.Arguments.RPBuildCodeMatchesByCodePrefixInput','Routing Product Build Code Matches By Code Prefix','TOne.WhS.Routing.BP.RPBuildCodeMatchesByCodePrefix, TOne.WhS.Routing.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false}'),
(6,'TOne.WhS.Routing.BP.Arguments.RPBuildRoutingProductInput','Routing Product Build Process Input','TOne.WhS.Routing.BP.RPBuildRoutingProducts, TOne.WhS.Routing.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false}'),
(7,'TOne.WhS.CodePreparation.BP.Arguments.CodePreparationInput','Code Preparation Process','TOne.WhS.CodePreparation.BP.CodePreparation, TOne.WhS.CodePreparation.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false,"HasBusinessRules":true}')
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

--[bp].[BPBusinessRuleActionType]-----------------1 to 100------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [bp].[BPBusinessRuleActionType] on;
;with cte_data([ID],[Description],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Stop Execution Action','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionType, Vanrise.BusinessProcess.Entities","BPBusinessRuleActionTypeId":1, "Description":"Stop Execution Action", "Editor":"businessprocess-bp-business-rule-stop-execution-action"}'),
(2,'Exclude Item Action','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionType, Vanrise.BusinessProcess.Entities","BPBusinessRuleActionTypeId":2, "Description":"Exclude Item Action", "Editor":"businessprocess-bp-business-rule-exclude-item-action"}'),
(3,'Warning Item Action','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionType, Vanrise.BusinessProcess.Entities","BPBusinessRuleActionTypeId":3, "Description":"Warning Item Action", "Editor":"businessprocess-bp-business-rule-warning-item-action"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Description],[Settings]))
merge	[bp].[BPBusinessRuleActionType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Description] = s.[Description],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Description],[Settings])
	values(s.[ID],s.[Description],s.[Settings]);
set identity_insert [bp].[BPBusinessRuleActionType] off;

--[bp].[BPBusinessRuleAction]-----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([BusinessRuleDefinitionId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":1}}'),
(2,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.ExcludeItemAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":2}}'),
(3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":1}}'),
(4,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":1}}'),
(5,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":1}}'),
(6,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":1}}'),
(7,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":1}}'),
(8,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":1}}'),
(9,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":1}}'),
(10,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.WarningItemAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":3}}'),
(11,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.WarningItemAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":3}}'),
(12,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.WarningItemAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":3}}'),
(13,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.WarningItemAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":3}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([BusinessRuleDefinitionId],[Settings]))
merge	[bp].[BPBusinessRuleAction] as t
using	cte_data as s
on		1=1 and t.[BusinessRuleDefinitionId] = s.[BusinessRuleDefinitionId]
when matched then
	update set
	[Settings] = s.[Settings]
when not matched by target then
	insert([BusinessRuleDefinitionId],[Settings])
	values(s.[BusinessRuleDefinitionId],s.[Settings]);

--[bp].[BPBusinessRuleDefinition]------------------1 to 10000---------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [bp].[BPBusinessRuleDefinition] on;
;with cte_data([ID],[Name],[BPDefintionId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'ValidateCodesZones',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Code Group Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.CodeGroupCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[1,2]}'),
(2,'ValidateCodesZones',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Multiple Countries In Same Zone Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.MultipleCountryCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[1,2]}'),
(3,'ValidateCodesZones',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Missing Zones Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.MissingZonesCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[1,2]}'),
(4,'ValidateCodesZones',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Missing Codes Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.MissingCodesCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[1,2]}'),
(5,'ValidateCodesZones',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Missing Rates Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.MissingRatesCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[1,2]}'),
(6,'ValidateCodesZones',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Missing BED Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.MissingBEDCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[1,2]}'),
(7,'ValidateCodesZones',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Same Code In Same Zone Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.SameCodeInSameZoneCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[1,2]}'),
(8,'ValidateCodesZones',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Same Zone With Different Rates Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.SameZoneWithDifferentRatesCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[1,2]}'),
(9,'ValidateCountries',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Same Code With Different Zones Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.SameCodeWithDifferentZonesCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[1,2]}'),
(10,'ValidateAfterProcessing',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"BED Zones Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.ZoneBEDCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[3]}'),
(11,'ValidateAfterProcessing',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"BED Codes Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.CodeBEDCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[3]}'),
(12,'ValidateAfterProcessing',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"EED Codes Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.CodeEEDCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[3]}'),
(13,'ValidateAfterProcessing',3,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"EED Zone Rule","Condition": {"$type":"TOne.WhS.SupplierPriceList.Business.ZoneeEDCondition, TOne.WhS.SupplierPriceList.Business"},"ActionTypes":[3]}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[BPDefintionId],[Settings]))
merge	[bp].[BPBusinessRuleDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[BPDefintionId] = s.[BPDefintionId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[BPDefintionId],[Settings])
	values(s.[ID],s.[Name],s.[BPDefintionId],s.[Settings]);
set identity_insert [bp].[BPBusinessRuleDefinition] off;