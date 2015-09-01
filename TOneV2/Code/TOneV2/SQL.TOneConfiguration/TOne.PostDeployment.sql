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
(44,'DURATION_IN_MINUTES','DURATION',1,'{"ColumnName":"[Measures].[Duration In Minutes]","Expression":"","Unit":"min"}',null)
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
	values(s.[ID],s.[Name],s.[DisplayName],s.[Type],s.[Configuration],s.[Rank])
when not matched by source then
	delete;
set identity_insert [BI].[SchemaConfiguration] off;
--sec.Module-----
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Routing','Routing','Routing',null,'/images/menu-icons/Routing.png',16,0),
(2,'Business Intelligence','Business Intelligence','BI',null,'/images/menu-icons/busines intel.png',19,1),
(3,'Business Entities','Business Entities','Business Entities',null,'/images/menu-icons/Business Entities.png',11,0),
(4,'NOC','NOC','NOC',null,'/images/menu-icons/NOC.png',18,1),
(5,'Administration','Administration','Administration',null,'/images/menu-icons/Administration.png',10,0),
(6,'Others','Others','Others',null,'/images/menu-icons/other.png',22,0),
(10,'Business Process','Business Process','Business Process',5,null,17,0),
(11,'Billing','Billing','Billing',null,'/images/menu-icons/billing.png',15,0),
(12,'Reports','Reports','Reports',11,null,10,1),
(14,'Plugins','Plugins','Plugins',null,'/images/menu-icons/plug.png',21,0),
(15,'Bilateral Agreement','Bilateral Agreement','Bilateral Agreement',14,null,10,0),
(16,'BQR','BQR','BQR',14,null,12,0),
(17,'Code Preparation','Code Preparation','Code Preparation',14,null,11,0),
(18,'Purchase Area','Purchase Area','Purchase Area',null,'/images/menu-icons/Purchase Area.png',12,0),
(19,'Sale Area','Sale Area','Sale Area',null,'/images/menu-icons/Sale Area.png',13,0),
(20,'Prepaid-Postpaid','Prepaid-Postpaid','Prepaid-Postpaid',null,'/images/menu-icons/post paid - pre paid.png',20,0),
(21,'Management','Management','Management',null,'/images/menu-icons/Management.png',14,0),
(22,'Account Manager','Account Manager','Account Manager',null,'/images/menu-icons/Account.png',17,0),
(23,'Dynamic Management','Dynamic Management','Dynamic Management',5,null,16,0)
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
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic])
when not matched by source then
	delete;
set identity_insert [sec].[Module] off;

--sec.[View]-----
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[RequiredPermissions],[Audience],[Content],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Rule Management','Rule Management','#/view/Routing/Views/RoutingRules/RoutingRulesManagement',1,null,null,null,0,10),
(2,'Rate Plan','Rate Plan','#/view/Routing/Views/Management/RatePlanning',1,null,null,null,0,12),
(3,'Routing Manager','Routing Manager','#/view/Routing/Views/RoutingManagement/RoutingManagement',1,null,null,null,0,11),
(8,'Entity Report','Entity Report','#/view/BI/Views/Reports/EntityReport',6,null,null,null,0,10),
(9,'Traffic Monitor','Traffic Monitor','#/view/Analytics/Views/Traffic Statistics/TrafficMonitor',4,null,null,null,0,10),
(10,'Variation','Variation','#/view/Analytics/Views/Billing Statistics/Variation Reports/VariationReports',12,null,null,null,0,11),
(11,'Billing','Billing','#/view/Analytics/Views/Billing Reports/BillingReports',12,null,null,null,0,10),
(12,'CDR Log','CDR Log','#/view/Analytics/Views/CDR/CDRLog',4,null,null,null,0,16),
(13,'Users','Users','#/view/Security/Views/UserManagement',5,'Root/Administration Module/Users:View',null,null,0,10),
(14,'Groups','Groups','#/view/Security/Views/GroupManagement',5,'Root/Administration Module/Groups:View',null,null,0,11),
(15,'DumySwitchs','DumySwitchs','#/view/BusinessEntity/Views/Switchs',3,null,null,null,0,14),
(16,'Switches','Switches Managments','#/view/BusinessEntity/Views/SwitchManagment',5,null,null,null,0,13),
(17,'Carrier Account','CarrierAccount Managments','#/view/BusinessEntity/Views/CarrierAccountManagement',3,null,null,null,0,11),
(18,'Default','Default','#/Default',6,null,null,null,0,13),
(19,'Test View','Test View','#/TestView',6,null,null,null,0,14),
(20,'Strategy Management','Strategy Management','#/Strategy',6,null,null,null,0,15),
(21,'ZingChart','ZingChart','#/ZingChart',6,null,null,null,0,16),
(22,'HighChart','HighChart','#/HighChart',6,null,null,null,0,17),
(23,'HighChartSparkline','HighChartSparkline','#/HighChartSparkline',6,null,null,null,0,18),
(24,'FusionChart','FusionChart','#/FusionChart',6,null,null,null,0,19),
(25,'CanvasJSChart','CanvasJSChart','#/CanvasJSChart',6,null,null,null,0,20),
(26,'AMChart','AMChart','#/AMChart',6,null,null,null,0,21),
(35,'System Entities','System Entities','#/view/Security/Views/BusinessEntityManagement',5,'Root/Administration Module/System Entities:View',null,null,0,12),
(39,'Pages','Dynamic Pages Management','#/view/Security/Views/DynamicPages/DynamicPageManagement',23,'Root/Administration Module/Dynamic Pages:View',null,null,0,11),
(41,'History','History','#/view/BusinessProcess/Views/BPHistory',10,null,null,null,0,11),
(42,'Management','Management','#/view/BusinessProcess/Views/BPDefinitionManagement',10,null,null,null,0,10),
(43,'Scheduler Service','Scheduler Service','#/view/Runtime/Views/SchedulerTaskManagement',5,null,null,null,0,14),
(69,'Widgets','Widgets Management','#/view/Security/Views/WidgetsPages/WidgetManagement',23,'Root/Administration Module/Dynamic Pages:View',null,null,0,10),
(134,'Organizational Charts','Organizational Charts','#/view/Security/Views/OrgChartManagement',5,null,null,null,0,12),
(141,'Volume','Volume','#/view/Analytics/Views/Billing Statistics/Volume Reports/VolumeReports',12,null,null,null,0,12),
(173,'Management','Account Manager Management','#/view/BusinessEntity/Views/AccountManager/AccountManagerManagement',22,null,null,null,0,10),
(200,'Carrier Profile','CarrierProfile Management','#/view/BusinessEntity/Views/CarrierProfileManagement',3,null,null,null,0,10),
(203,'Default','Default','#/Default',20,null,null,null,0,10),
(206,'Default','Default','#/Default',15,null,null,null,0,10),
(207,'Default','Default','#/Default',16,null,null,null,0,10),
(208,'Default','Default','#/Default',17,null,null,null,0,10),
(225,'Carrier Group','Carrier Group','#/view/BusinessEntity/Views/CarrierGroupManagement',3,null,null,null,0,12),
(234,'Blocked Attempts','Blocked Attempts','#/view/Analytics/Views/BlockedAttempts',4,null,null,null,0,14),
(236,'Ranking Pages','Ranking Pages','#/view/Security/Views/Pages/RankingPageManagement',5,'Root/Administration Module:View',null,null,0,15),
(239,'Repeated Numbers','Repeated Numbers','#/view/Analytics/Views/RepeatedNumbers',4,null,null,null,0,13),
(241,'Release Code Statistics','Release Code Statistics','#/view/Analytics/Views/Traffic Statistics/ReleaseCodeStatistics',4,null,null,null,0,15),
(248,'Carrier Mask','Carrier Mask','#/view/BusinessEntity/Views/CarrierMaskManagement',3,null,null,null,0,13),
(249,'New dashboard','New dashboard','#/viewwithparams/Security/Views/DynamicPages/DynamicPagePreview',2,null,'{"Users":[1],"Groups":[68]}','{"SummaryContents":[{"WidgetId":124,"NumberOfColumns":"6","SectionTitle":"Current Month-test-vr-circle"},{"WidgetId":95,"NumberOfColumns":"6","SectionTitle":"Current Month-Widget_test"}],"BodyContents":[{"WidgetId":102,"NumberOfColumns":"6","SectionTitle":"Current Month-Top Cost"},{"WidgetId":97,"NumberOfColumns":"6","SectionTitle":"Current Month-Top Customer"},{"WidgetId":115,"NumberOfColumns":"12","SectionTitle":"Current Month-Suppliers Report"},{"WidgetId":119,"NumberOfColumns":"12","SectionTitle":"Current Month-Timely Financial"}],"DefaultGrouping":3,"DefaultPeriod":6}',1,11),
(253,'Hourly Report','Hourly Report','#/view/Analytics/Views/Traffic Statistics/HourlyReport',4,null,null,null,0,12),
(254,'Daily Report','Daily Report','#/view/Analytics/Views/DailyReport',21,null,null,null,0,11),
(257,'Carrier Summary',' Carrier Summary','#/view/Analytics/Views/CarrierSummaryStats',21,null,null,null,0,10),
(258,'Raw CDR Log','Raw CDR Log','#/view/Analytics/Views/RawCDRLog',4,null,null,null,0,11),
(265,'Customer TOD','Customer TOD','#/view/BusinessEntity/Views/SalesArea/CustomerTODManagement',19,null,null,null,0,12),
(266,'Supplier Tariff','Supplier Tariff','#/view/BusinessEntity/Views/PurchaseArea/SupplierTariffManagement',18,null,null,null,0,10),
(268,'Customer Pricelists','Customer Pricelists','#/view/BusinessEntity/Views/SalesArea/Customer Pricelists/CustomerPricelists',19,null,null,null,0,10),
(274,'Supplier Pricelists','Supplier Pricelists','#/view/BusinessEntity/Views/PurchaseArea/Supplier Pricelists/SupplierPricelists',18,null,null,null,0,null),
(278,'Customer Tariff','Customer Tariff','#/view/BusinessEntity/Views/SalesArea/CustomerTariffManagement',19,null,null,null,0,11),
(279,'Rate Analysis','Rate Analysis','#/view/BusinessEntity/Views/Rate Analysis/RateAnalysis',3,null,null,null,0,null),
(287,'Supplier TOD','Supplier TOD','#/view/BusinessEntity/Views/PurchaseArea/SupplierTODManagement',18,null,null,null,0,11),
(288,'Supplier Invoice','Supplier Invoice','#/view/Billing/Views/SupplierInvoiceManagement',11,null,null,null,0,10)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[RequiredPermissions],[Audience],[Content],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[RequiredPermissions] = s.[RequiredPermissions],[Audience] = s.[Audience],[Content] = s.[Content],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[RequiredPermissions],[Audience],[Content],[Type],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[RequiredPermissions],s.[Audience],s.[Content],s.[Type],s.[Rank])
when not matched by source then
	delete;
set identity_insert [sec].[View] off;

--[sec].[BusinessEntityModule]----
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Root',null,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(2,'Routing Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(3,'Business Intelligence Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(4,'Sales Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(5,'Business Entity Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(6,'Administration Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]'),
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
	values(s.[Id],s.[Name],s.[ParentId],s.[BreakInheritance],s.[PermissionOptions])
when not matched by source then
	delete;
set identity_insert [sec].[BusinessEntityModule] off;


--[sec].[BusinessEntity]-----
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Routes',2,0,'["View"]'),
(2,'Route Rules',2,0,'["View","Add Block Route","Edit Block Route", "Add Override Route", "Edit Override Route"]'),
(3,'Billing Statistics',7,0,'["View"]'),
(4,'CDRs',7,0,'["View"]'),
(6,'Traffic Statistics',8,0,'["View"]'),
(7,'CDRs',8,0,'["View"]'),
(8,'Rates',4,0,'["View", "Edit"]'),
(9,'LCR',4,0,'["View"]'),
(10,'Carrier',5,0,'["View", "Edit", "Delete"]'),
(11,'Code',5,0,'["View", "Edit"]'),
(12,'Zone',5,0,'["View", "Edit"]'),
(13,'Users',6,0,'["View", "Add", "Edit", "Reset Password"]'),
(14,'Groups',6,0,'["View", "Add", "Edit", "Delete"]'),
(15,'System Entities',6,1,'["View", "Assign Permissions"]'),
(17,'Dynamic Pages',6,1,'["View", "Add", "Edit", "Delete","Validate"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions])
when not matched by source then
	delete;
set identity_insert [sec].[BusinessEntity] off;