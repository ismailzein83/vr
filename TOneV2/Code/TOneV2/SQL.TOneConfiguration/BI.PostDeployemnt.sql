--sec.WidgetDefinition--------
set nocount on;
set identity_insert [sec].[WidgetDefinition] on;
;with cte_data([ID],[Name],[DirectiveName],[Setting])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Report                                            ','vr-bi-datagrid                                    ','{"DirectiveTemplateURL":"/Client/Modules/BI/Directives/Templates/vr-bi-datagrid-directive-template.html","Sections":[1]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                '),
(2,'Chart                                             ','vr-bi-chart                                       ','{"DirectiveTemplateURL":"/Client/Modules/BI/Directives/Templates/vr-bi-chart-directive-template.html","Sections":[1]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   '),
(3,'Summary                                           ','vr-bi-summary                                     ','{"DirectiveTemplateURL":"/Client/Modules/BI/Directives/Templates/vr-bi-summary-directive-template.html","Sections":[0]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 ')
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
(1,'ACD','ACD',1,'{"ColumnName":"[Measures].[ACD]","Expression":"","RequiredPermissions":"Root/Business Intelligence Module/Billing Module/Billing Statistics:View","Unit":"min/call"}',null),
(2,'COST','COST',1,'{"ColumnName":"[Measures].[Cost Net]","Expression":"","Unit":"Currency"}',null),
(3,'SALE','SALE',1,'{"ColumnName":"[Measures].[Sale Net]","Expression":"","Unit":"Currency"}',null),
(5,'PROFIT','PROFIT',1,'{"ColumnName":"[Measures].[Profit_CALC]","Expression":"MEMBER [Measures].[Profit_CALC]  AS ([Measures].[Sale Net] - [Measures].[Cost Net])","Unit":"Currency"}',null),
(6,'SUCCESSFUL_ATTEMPTS','ATTEMPTS',1,'{"ColumnName":"[Measures].[SuccessfulAttempts]","Expression":"","Unit":"#Calls"}',null),
(7,'PDD','PDD',1,'{"ColumnName":"[Measures].[PDD]","Exepression":"","Unit":"sec"}',null),
(8,'Supplier','Supplier',0,'{"ColumnID":"[SupplierAccounts].[Carrier Account ID]","ColumnName":"[SupplierAccounts].[Carrier Name]","BehaviorFQTN":"TOne.BusinessEntity.Business.SupplierBehavior, TOne.BusinessEntity.Business"}',3),
(9,'Customer','Customer',0,'{"ColumnID":"[CustomerAccounts].[Carrier Account ID]","ColumnName":"[CustomerAccounts].[Carrier Name]","BehaviorFQTN":"TOne.BusinessEntity.Business.CustomerBehavior, TOne.BusinessEntity.Business"}',2),
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
(2,'Business Intelligence','Business Intelligence','BI',null,'/images/menu-icons/busines intel.png',19,1),
(5,'Administration','Administration','Administration',null,'/images/menu-icons/Administration.png',10,0),
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
(13,'Users','Users','#/view/Security/Views/UserManagement',5,'Root/Administration Module/Users:View',null,null,0,1),
(14,'Groups','Groups','#/view/Security/Views/GroupManagement',5,'Root/Administration Module/Groups:View',null,null,0,2),
(35,'System Entities','System Entities','#/view/Security/Views/BusinessEntityManagement',5,'Root/Administration Module/System Entities:View',null,null,0,4),
(134,'Organizational Charts','Organizational Charts','#/view/Security/Views/OrgChartManagement',5,null,null,null,0,3),
(173,'AccountManager','AccountManager Management','#/view/BusinessEntity/Views/AccountManager/AccountManagerManagement',22,null,null,null,0,3),
(39,'Pages','Dynamic Pages Management','#/view/Security/Views/DynamicPages/DynamicPageManagement',23,'Root/Administration Module/Dynamic Pages:View',null,null,0,11),
(69,'Widgets','Widgets Management','#/view/Security/Views/WidgetsPages/WidgetManagement',23,'Root/Administration Module/Dynamic Pages:View',null,null,0,10),
(236,'Ranking Pages','Ranking Pages','#/view/Security/Views/Pages/RankingPageManagement',5,'Root/Administration Module:View',null,null,0,15)
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