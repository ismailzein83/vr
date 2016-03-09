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
--[sec].[Module]---------------------------601 to 700---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Routing','Routing','Routing',null,'/images/menu-icons/Routing.png',16,0),
(2,'Business Intelligence','Business Intelligence','BI',null,'/images/menu-icons/busines intel.png',20,1),
(3,'Business Entities','Business Entities','Business Entities',null,'/images/menu-icons/Business Entities.png',11,0),
(4,'Traffic Analysis','Traffic Analysis','NOC',null,'/images/menu-icons/NOC.png',18,1),
(5,'Administration','Administration','Administration',null,'/images/menu-icons/Administration.png',10,0),
(10,'Business Process','Business Process','Business Process',5,null,12,0),
(11,'Billing','Billing','Billing',null,'/images/menu-icons/billing.png',16,0),
(14,'Plugins','Plugins','Plugins',null,'/images/menu-icons/plug.png',22,0),
(18,'Purchase Area','Purchase Area','Purchase Area',null,'/images/menu-icons/Purchase Area.png',13,0),
(19,'Sale Area','Sale Area','Sale Area',null,'/images/menu-icons/Sale Area.png',14,0),
(20,'Prepaid-Postpaid','Prepaid-Postpaid','Prepaid-Postpaid',null,'/images/menu-icons/post paid - pre paid.png',21,0),
(21,'Management','Management','Management',null,'/images/menu-icons/Management.png',15,0),
(22,'Account Manager','Account Manager','Account Manager',null,'/images/menu-icons/Account.png',17,0),
(23,'Dynamic Management','Dynamic Management','Dynamic Management',5,null,13,0),
(24,'CDR Process','CDR Process','CDR Process',null,null,12,0),
(25,'Data Sources','Data Sources','Data Sources',5,null,11,0),
(26,'Entities','Entities',null,18,null,10,null),
(27,'Entities','Entities',null,19,null,10,null),
(28,'Security','Security',null,5,null,10,null),
(29,'Carriers','Carriers',null,3,null,10,null),
(30,'Lookups','Lookups',null,3,null,11,null),
(31,'Rules','Rules',null,24,null,10,null),
(32,'Rules','Rules',null,1,null,10,null),
(33,'Rules','Rules',null,18,null,11,null),
(34,'Rules','Rules',null,19,null,11,null),
(35,'Queueing','Queueing',null,5,null,15,null),
(36,'Generic Data','Generic Data',null,5,null,14,null)
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

--[sec].[View]-----------------------------6001 to 7000-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(9,'Traffic Monitor','Traffic Monitor','#/view/WhS_Analytics/Views/TrafficMonitor',4,null,null,null,0,10),
(12,'CDR Log','CDR Log','#/view/WhS_Analytics/Views/CDR/CDRLog',4,null,null,null,0,12),
(13,'Users','Users','#/view/Security/Views/User/UserManagement',28,null,null,null,0,10),
(14,'Groups','Groups','#/view/Security/Views/Group/GroupManagement',28,null,null,null,0,11),
(17,'Carrier Accounts','Carrier Accounts','#/view/WhS_BusinessEntity/Views/CarrierAccount/CarrierAccountManagement',29,null,null,null,0,11),
(35,'System Entities','System Entities','#/view/Security/Views/Permission/BusinessEntityManagement',28,null,null,null,0,12),
(39,'Pages','Dynamic Pages Management','#/view/Security/Views/DynamicPages/DynamicPageManagement',23,null,null,null,0,11),
(41,'Log','Log History','#/view/BusinessProcess/Views/BPHistory',10,null,null,null,0,11),
(42,'Management','Management','#/view/BusinessProcess/Views/BPDefinitionManagement',10,null,null,null,0,10),
(43,'Scheduler Service','Scheduler Service','#/view/Runtime/Views/SchedulerTaskManagement',5,null,null,null,0,16),
(69,'Widgets','Widgets Management','#/view/Security/Views/WidgetsPages/WidgetManagement',23,null,null,null,0,10),
(134,'Organizational Charts','Organizational Charts','#/view/Security/Views/OrgChart/OrgChartManagement',5,null,null,null,0,15),
(173,'Account Manager','Account Manager','#/view/WhS_BusinessEntity/Views/AccountManager/AccountManagerManagement',22,null,null,null,0,10),
(200,'Carrier Profiles','Carrier Profiles','#/view/WhS_BusinessEntity/Views/CarrierAccount/CarrierProfileManagement',29,null,null,null,0,10),
(236,'Ranking Pages','Ranking Pages','#/view/Security/Views/Pages/RankingPageManagement',5,'Root/Administration Module:View',null,null,0,17),
(253,'Hourly Report','Hourly Report','#/view/WhS_Analytics/Views/HourlyReport',4,null,null,null,0,11),
(254,'Daily Report','Daily Report','#/view/WhS_Analytics/Views/DailyReport',21,null,null,null,0,11),
(257,'Carrier Summary',' Carrier Summary','#/view/WhS_Analytics/Views/CarrierSummary',21,null,null,null,0,10),
(258,'Raw CDR Log','Raw CDR Log','#/view/WhS_Analytics/Views/RawCDR/RawCDRLog',4,null,null,null,0,13),
(323,'Routing Products','Routing Products','#/view/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductManagement',1,null,null,null,0,11),
(334,'Route Rules','Route Rules','#/view/WhS_Routing/Views/RouteRule/RouteRuleManagement',32,null,null,null,0,10),
(337,'Selling Products','Selling Products','#/view/WhS_BusinessEntity/Views/SellingProduct/SellingProductManagement',27,null,null,null,0,11),
(338,'Customer Selling Product','Customer Selling Product','#/view/WhS_BusinessEntity/Views/SellingProduct/CustomerSellingProductManagement',27,null,null,null,0,12),
(341,'Sale Pricing Rules','Sale Pricing Rules','#/view/WhS_BusinessEntity/Views/PricingRule/SalePricingRuleManagement',34,null,null,null,0,10),
(342,'Purchase Pricing Rules','Purchase Pricing Rules','#/view/WhS_BusinessEntity/Views/PricingRule/PurchasePricingRuleManagement',33,null,null,null,0,10),
(351,'Rate Plan','Rate Plan','#/view/Whs_Sales/Views/RatePlan',19,null,null,null,0,12),
(352,'Customer Identification Rules','Customer Identification Rules','#/view/WhS_CDRProcessing/Views/CustomerRule/CustomerIdentificationRuleManagement',31,null,null,null,0,10),
(353,'Supplier Identification Rules','Supplier Identification Rules','#/view/WhS_CDRProcessing/Views/SupplierRule/SupplierIdentificationRuleManagement',31,null,null,null,0,11),
(354,'Code Groups','Code Groups','#/view/WhS_BusinessEntity/Views/CodeGroup/CodeGroupManagement',30,null,null,null,0,14),
(355,'Normalization Rules','Normalization Rules','#/view/WhS_CDRProcessing/Views/NormalizationRule/NormalizationRuleManagement',31,null,null,null,0,13),
(356,'Management','Data Sources Management','#/view/Integration/Views/DataSourceManagement',25,'Root/Integration Module:View',null,null,0,10),
(357,'Log','Log History','#/view/Integration/Views/DataSourceLogManagement',25,'Root/Integration Module:View',null,null,0,12),
(358,'Imported Batches','Imported Batches','#/view/Integration/Views/DataSourceImportedBatchManagement',25,'Root/Integration Module:View',null,null,0,11),
(360,'Currencies','Currencies','#/view/Common/Views/Currency/CurrencyManagement',30,null,null,null,0,12),
(363,'Currency Exchange Rates','Currency Exchange Rates','#/view/Common/Views/CurrencyExchangeRate/CurrencyExchangeRateManagement',30,null,null,null,0,13),
(364,'Rate Types','Rate Types','#/view/WhS_BusinessEntity/Views/RateType/RateTypeManagement',30,null,null,null,0,15),
(365,'Zone Service Config','Zone Service Config','#/view/WhS_BusinessEntity/Views/ZoneServiceConfig/ZoneServiceConfigManagement',30,null,null,null,0,16),
(366,'Countries','Countries','#/view/Common/Views/Country/CountryManagement',30,null,null,null,0,10),
(367,'Sale Zones','Sale Zones','#/view/WhS_BusinessEntity/Views/SaleZone/SaleZoneManagement',27,null,null,null,0,13),
(368,'Cities','Cities','#/view/Common/Views/City/CityManagement',30,null,null,null,0,11),
(369,'Supplier Zones','Supplier Zones','#/view/WhS_BusinessEntity/Views/SupplierZone/SupplierZoneManagement',26,null,null,null,0,11),
(371,'Selling Number Plans','Selling Number Plans','#/view/WhS_BusinessEntity/Views/SellingNumberPlan/SellingNumberPlanManagement',27,null,null,null,0,10),
(374,'Customer Routes','Customer Routes','#/view/WhS_Routing/Views/CustomerRoute/CustomerRouteManagement',1,null,null,null,0,13),
(375,'Sale Rates','Sale Rates','#/view/WhS_BusinessEntity/Views/SaleRate/SaleRateManagement',27,null,null,null,0,15),
(376,'Supplier Rates','Supplier Rates','#/view/WhS_BusinessEntity/Views/SupplierRate/SupplierRateManagement',26,null,null,null,0,13),
(377,'Supplier Codes','Supplier Codes','#/view/WhS_BusinessEntity/Views/SupplierCode/SupplierCodeManagement',26,null,null,null,0,12),
(378,'Sale Codes','Sale Codes','#/view/WhS_BusinessEntity/Views/SaleCode/SaleCodeManagement',27,null,null,null,0,14),
(381,'Product Routes','Product Routes','#/view/WhS_Routing/Views/RPRoute/RPRouteManagement',1,null,null,null,0,12),
(387,'Switches','Switches','#/view/WhS_BusinessEntity/Views/Switch/SwitchManagement',3,null,null,null,0,12),
(403,'Code Preparation','Code Preparation','#/view/WhS_CodePreparation/Views/CodePreparationManagement',19,null,null,null,0,13),
(404,'Switch Identification Rules','Switch Identification Rules','#/view/WhS_CDRProcessing/Views/SwitchRule/SwitchIdentificationRuleManagement',31,null,null,null,0,12),
(406,'Supplier Price Lists','Supplier Price Lists','#/view/WhS_BusinessEntity/Views/SupplierPricelist/SupplierPricelist',26,null,null,null,0,10),
(412,'Data Record Types','Data Record Types','#/view/Common/Views/GenericDataRecord/DataRecordTypeManagement',5,null,null,null,0,14),
(415,'Import Supplier Price List','Import Supplier Price List','#/view/WhS_SupplierPriceList/Views/SupplierPriceList',18,null,null,null,0,12),
(429,'Sale Price Lists','Sale Price Lists','#/view/Whs_BusinessEntity/views/SalePriceList/SalePriceList',27,null,null,null,null,0,16),
(431,'Selling Rules','Selling Rules','#/view/WhS_Sales/Views/SellingRule/SellingRuleManagement',34,null,null,null,null,0,11),
(435,'Execution Flows','Execution Flows','#/view/Queueing/Views/ExecutionFlow/ExecutionFlowManagement',35,null,null,null,null,0,11),
(437,'Data Transformation Definition','Data Transformation Definition','#/view/VR_GenericData/Views/DataTransformationDefinition/DataTransformationDefinitionManagement',36,null,null,null,null,0,12),
(440,'Generic Rule Definition','Generic Rule Definition','#/view/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionManagement',36,null,null,null,null,0,11),
(441,'Queues','Queues','#/view/Queueing/Views/QueueInstance/QueueInstanceManagement',35,null,null,null,null,0,12),
(465,'Data Store','Data Store','#/view/VR_GenericData/Views/DataStore/DataStoreManagement',36,null,null,null,null,0,13),
(466,'Data Record Storage','Data Record Storage','#/view/VR_GenericData/Views/DataRecordStorage/DataRecordStorageManagement',36,null,null,null,null,0,14),
(470,'Queue Items','Queue Items','#/view/Queueing/Views/QueueItemHeader/QueueItemHeaderManagement',35,null,null,null,null,0,13),
(475,'Execution Flow Definition','Execution Flow Definition','#/view/Queueing/Views/ExecutionFlowDefinition/ExecutionFlowDefinitionManagement',35,null,null,null,null,0,10)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Type],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Type],s.[Rank]);
set identity_insert [sec].[View] off;

--[sec].[BusinessEntityModule]-------------601 to 700---------------------------------------------------------
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
----------------------------------------------------------------------------------------------------
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