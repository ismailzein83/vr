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
--BI.SchemaConfiguration----
set nocount on;
set identity_insert [BI].[SchemaConfiguration] on;
;with cte_data([ID],[Name],[DisplayName],[Type],[Configuration],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'ACD','ACD',1,'{"ColumnName":"[Measures].[ACD]","Expression":"","RequiredPermissions":"TOne/Business Intelligence Module/Billing Module/Billing Statistics:View"}',null),
(2,'COST','COST',1,'{"ColumnName":"[Measures].[Cost Net]","Expression":""}',null),
(3,'SALE','SALE',1,'{"ColumnName":"[Measures].[Sale Net]","Expression":""}',null),
(4,'DURATION_IN_MINUTES','DURATION IN MINUTES',1,'{"ColumnName":"[Measures].[Duration In Minutes]","Expression":""}',null),
(5,'PROFIT','PROFIT',1,'{"ColumnName":"[Measures].[Profit_CALC]","Expression":"MEMBER [Measures].[Profit_CALC]  AS ([Measures].[Sale Net] - [Measures].[Cost Net])"}',null),
(6,'SUCCESSFUL_ATTEMPTS','SUCCESSFUL ATTEMPTS',1,'{"ColumnName":"[Measures].[SuccessfulAttempts]","Expression":""}',null),
(7,'PDD','PDD',1,'{"ColumnName":"[Measures].[PDD]","Exepression":""}',null),
(8,'Supplier','Supplier',0,'{"ColumnID":"[SupplierAccounts].[Carrier Account ID]","ColumnName":"[SupplierAccounts].[Profile Name]","BehaviorFQTN":"TOne.BusinessEntity.Business.SupplierBehavior, TOne.BusinessEntity.Business"}',3),
(9,'Customer','Customer',0,'{"ColumnID":"[CustomerAccounts].[Carrier Account ID]","ColumnName":"[CustomerAccounts].[Profile Name]","BehaviorFQTN":"TOne.BusinessEntity.Business.CustomerBehavior, TOne.BusinessEntity.Business"}',2),
(14,'SaleZone','SaleZone',0,'{"ColumnID":"[SaleZones].[Zone ID]","ColumnName":"[SaleZones].[Z One Name]","Expression":""}',null),
(16,'BWASR','BWASR',1,'{"ColumnName":"[Measures].[BWASR]","Expression":""}',null),
(19,'Facttable Count','Facttable Count',1,'{"ColumnName":"[Measures].[Facttable Count]","Expression":""}',null),
(20,'Duration In Minutes','Duration In Minutes',1,'{"ColumnName":"[Measures].[Duration In Minutes]","Expression":""}',null),
(21,'ICABR','ICABR',1,'{"ColumnName":"[Measures].[ICABR]","Expression":""}',null),
(22,'ICASR','ICASR',1,'{"ColumnName":"[Measures].[ICASR]","Expression":""}',null),
(23,'ICCCR','ICCCR',1,'{"ColumnName":"[Measures].[ICCCR]","Expression":""}',null),
(27,'ICNER','ICNER',1,'{"ColumnName":"[Measures].[ICNER]","Expression":""}',null),
(28,'IIR','IIR',1,'{"ColumnName":"[Measures].[IIR]","Expression":""}',null),
(29,'MHT','MHT',1,'{"ColumnName":"[Measures].[MHT]","Expression":""}',null),
(30,'MHT-Per-Call','MHT-Per-Call',1,'{"ColumnName":"[Measures].[MHT-Per-Call]","Expression":""}',null),
(31,'OGABR','OGABR',1,'{"ColumnName":"[Measures].[OGABR]","Expression":""}',null),
(32,'OGASR','OGASR',1,'{"ColumnName":"[Measures].[OGASR]","Expression":""}',null),
(33,'OGCCR','OGCCR',1,'{"ColumnName":"[Measures].[OGCCR]","Expression":""}',null),
(34,'OGNER','OGNER',1,'{"ColumnName":"[Measures].[OGNER]","Expression":""}',null),
(35,'OIR','OIR',1,'{"ColumnName":"[Measures].[OIR]","Expression":""}',null),
(36,'PDD-Per-Call','PDD-Per-Call',1,'{"ColumnName":"[Measures].[PDD-Per-Call]","Expression":""}',null),
(37,'Seizures','Seizures',1,'{"ColumnName":"[Measures].[Seizures]","Expression":""}',null),
(41,'Switch','Switch',0,'{"ColumnID":"[Switch].[Switch ID]","ColumnName":"[Switch].[Name]","Expression":""}',null),
(42,'SaleRate','SaleRate',0,'{"ColumnID":"[SaleRate].[Rate ID]","ColumnName":"[SaleRate].[Rate]","Expression":""}',null),
(43,'CostZones','CostZones',0,'{"ColumnID":"[CostZones].[ID]","ColumnName":"[CostZones].[Z One Name]","Expression":""}',null)
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
(1,'Routing','Routing','Routing',null,'glyphicon-certificate',7,0),
(2,'Business Intelligence','Business Intelligence','BI',null,'glyphicon-certificate',11,1),
(3,'Business Entities','Business Entities','Business Entities',null,'glyphicon-cog',2,0),
(4,'NOC','NOC','NOC',null,'glyphicon-flash',9,1),
(5,'Administration','Administration','Administration',null,'glyphicon-flash',1,0),
(6,'Others','Others','Others',null,'glyphicon-pencil',13,0),
(10,'Business Process','Business Process','Business Process',5,null,null,0),
(11,'Billing','Billing','Billing',null,'glyphicon-pencil',6,0),
(12,'Reports','Reports','Reports',11,null,null,1),
(14,'Plugins','Plugins','Plugins',null,'glyphicon-cog',12,0),
(15,'Bilateral Agreement','Bilateral Agreement','Bilateral Agreement',14,null,3,0),
(16,'BQR','BQR','BQR',14,null,2,0),
(17,'Code Preparation','Code Preparation','Code Preparation',14,null,1,0),
(18,'Purchase Area','Purchase Area','Purchase Area',null,'glyphicon-flash',3,0),
(19,'Sale Area','Sale Area','Sale Area',null,'glyphicon-pencil',4,0),
(20,'Prepaid-Postpaid','Prepaid-Postpaid','Prepaid-Postpaid',null,'glyphicon-pencil',10,0),
(21,'Management','Management','Management',null,'glyphicon-flash',5,0),
(22,'Account Manager','Account Manager','Account Manager',null,'glyphicon-pencil',8,0)
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
(1,'Rule Management','Rule Management','#/view/Routing/Views/RoutingRules/RoutingRulesManagement',1,null,null,null,0,2),
(2,'Rate Plan','Rate Plan','#/view/Routing/Views/Management/RatePlanning',1,null,null,null,0,3),
(3,'Routing Manager','Routing Manager','#/view/Routing/Views/RoutingManagement/RoutingManagement',1,null,null,null,0,1),
(8,'Entity Report','Entity Report','#/view/BI/Views/Reports/EntityReport',6,null,null,null,0,5),
(9,'Zone Monitor','Zone Monitor','#/view/Analytics/Views/Traffic Statistics/ZoneMonitor',4,null,null,null,0,1),
(10,'Variation','Variation','#/view/Analytics/Views/Billing Statistics/Variation Reports/VariationReports',12,null,null,null,0,1),
(11,'Billing','Billing','#/view/Analytics/Views/Billing Reports/BillingReports',12,null,null,null,0,2),
(12,'CDR Log','CDR Log','#/view/Analytics/Views/CDR/CDRLog',4,null,null,null,0,2),
(13,'Users','Users','#/view/Security/Views/UserManagement',5,'TOne/Administration Module/Users:View',null,null,0,1),
(14,'Groups','Groups','#/view/Security/Views/GroupManagement',5,'TOne/Administration Module/Groups:View',null,null,0,2),
(15,'DumySwitchs','DumySwitchs','#/view/BusinessEntity/Views/Switchs',3,null,null,null,0,3),
(16,'Switches','Switches Managments','#/view/BusinessEntity/Views/SwitchManagment',5,null,null,null,0,5),
(17,'Carrier Account','CarrierAccount Managments','#/view/BusinessEntity/Views/CarrierAccountManagement',3,null,null,null,0,1),
(18,'Default','Default','#/Default',6,null,null,null,0,8),
(19,'Test View','Test View','#/TestView',6,null,null,null,0,9),
(20,'Strategy Management','Strategy Management','#/Strategy',6,null,null,null,0,10),
(21,'ZingChart','ZingChart','#/ZingChart',6,null,null,null,0,11),
(22,'HighChart','HighChart','#/HighChart',6,null,null,null,0,12),
(23,'HighChartSparkline','HighChartSparkline','#/HighChartSparkline',6,null,null,null,0,13),
(24,'FusionChart','FusionChart','#/FusionChart',6,null,null,null,0,14),
(25,'CanvasJSChart','CanvasJSChart','#/CanvasJSChart',6,null,null,null,0,15),
(26,'AMChart','AMChart','#/AMChart',6,null,null,null,0,16),
(27,'Tree','Tree','#/Tree',6,null,null,null,0,17),
(35,'System Entities','System Entities','#/view/Security/Views/BusinessEntityManagement',5,'TOne/Administration Module/System Entities:View',null,null,0,4),
(38,'Test','Test','#/view/BI/Views/Test',6,null,null,null,0,6),
(39,'Dynamic Pages Management','Dynamic Pages Management','#/view/Security/Views/DynamicPages/DynamicPageManagement',5,'TOne/Administration Module/Dynamic Pages:View',null,null,0,8),
(41,'History','History','#/view/BusinessProcess/Views/BPManagement',10,null,null,null,0,1),
(42,'Management','Management','#/view/BusinessProcess/Views/BPDefinitionManagement',10,null,null,null,0,2),
(43,'Scheduler Service','Scheduler Service','#/view/Runtime/Views/SchedulerTaskManagement',5,null,null,null,0,6),
(69,'Widgets Management','Widgets Management','#/view/Security/Views/WidgetsPages/WidgetManagement',5,'TOne/Administration Module/Dynamic Pages:View',null,null,0,7),
(129,'Test Page','Test Page','#/view/BusinessEntity/Views/TestPage',6,null,null,null,0,7),
(134,'Organizational Charts','Organizational Charts','#/view/Security/Views/OrgChartManagement',5,null,null,null,0,3),
(141,'Volume','Volume','#/view/Analytics/Views/Billing Statistics/Volume Reports/VolumeReports',12,null,null,null,0,3),
(173,'AccountManager','AccountManager Management','#/view/BusinessEntity/Views/AccountManager/AccountManagerManagement',3,null,null,null,0,3),
(200,'Carrier Profile','CarrierProfile Management','#/view/BusinessEntity/Views/CarrierProfileManagement',3,null,null,null,0,2),
(201,'Default','Default','#/Default',18,null,null,null,0,8),
(202,'Default','Default','#/Default',19,null,null,null,0,8),
(203,'Default','Default','#/Default',20,null,null,null,0,8),
(204,'Default','Default','#/Default',21,null,null,null,0,8),
(205,'Default','Default','#/Default',22,null,null,null,0,8),
(206,'Default','Default','#/Default',15,null,null,null,0,8),
(207,'Default','Default','#/Default',16,null,null,null,0,8),
(208,'Default','Default','#/Default',17,null,null,null,0,8),
(224,'First Dynamic Page','First Dynamic Page','#/viewwithparams/Security/Views/DynamicPages/DynamicPagePreview',2,null,null,'{"SummaryContents":[{"WidgetId":95,"NumberOfColumns":"2","SectionTitle":"Widget_test","DefaultGrouping":3,"DefaultPeriod":6}],"BodyContents":[{"WidgetId":120,"NumberOfColumns":"12","SectionTitle":"Test Widget","DefaultGrouping":3,"DefaultPeriod":0},{"WidgetId":103,"NumberOfColumns":"12","SectionTitle":"Top Sale","DefaultGrouping":3,"DefaultPeriod":0},{"WidgetId":96,"NumberOfColumns":"6","SectionTitle":"Current Month-Top Supplier"}],"DefaultGrouping":3,"DefaultPeriod":6}',1,null),
(225,'Carrier Group','Carrier Group','#/view/BusinessEntity/Views/CarrierGroupManagement',3,null,null,null,0,3)
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