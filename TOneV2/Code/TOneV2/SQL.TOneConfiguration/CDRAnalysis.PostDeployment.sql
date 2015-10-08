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
--[BI].[SchemaConfiguration]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [BI].[SchemaConfiguration] on;
;with cte_data([ID],[Name],[DisplayName],[Type],[Configuration],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'CaseStatus','Case Status',0,'{"ColumnID":"[Dim Case Status].[Pk Case Status Id]","ColumnName":"[Dim Case Status].[Name]"}',null),
(2,'CaseCount','Case Count',1,'{"ColumnName":"[Measures].[Fact Cases Count]","Exepression":"","Unit":""}',null),
(4,'CaseId-Fact Cases','Case Id -  Fact Cases',1,'{"ColumnName":"[Measures].[MS Case Id - Fact Cases]","Exepression":"","Unit":""}',null),
(6,'CaseCDRId','Case CDR Id',1,'{"ColumnName":"[Measures].[MS CDR Id - Fact Cases]","Exepression":"","Unit":""}',null),
(8,'CaseMSDuration','Case MS Duration',1,'{"ColumnName":"[Measures].[MS Duration - Fact Cases]","Exepression":"","Unit":""}',null),
(9,'CallMSDuration','Call MS Duration',1,'{"ColumnName":"[Measures].[MS Duration]","Exepression":"","Unit":""}',null),
(10,'CallMSCDRId','Call MS CDR Id',1,'{"ColumnName":"[Measures].[MS CDR Id]","Exepression":"","Unit":""}',null),
(11,'CallCount','Call Count',1,'{"ColumnName":"[Measures].[Fact Calls Count]","Exepression":"","Unit":""}',null),
(12,'CallMSCaseId','Call MS Case Id',1,'{"ColumnName":"[Measures].[MS Case Id]","Exepression":"","Unit":""}',null),
(13,'CaseUser','Case User',0,'{"ColumnID":"[Case User].[Pk User Id]","ColumnName":"[Case User].[Name]"}',null),
(14,'BTS','BTS',0,'{"ColumnID":"[Dim BTS].[Pk BTS Id]","ColumnName":"[Dim BTS].[Name]"}',null),
(15,'CallClass','Call Class',0,'{"ColumnID":"[Dim Call Class].[Pk Call Class Id]","ColumnName":"[Dim Call Class].[Name]"}',null),
(16,'CallType','Call Type',0,'{"ColumnID":"[Dim Call Type].[Pk Call Type Id]","ColumnName":"[Dim Call Type].[Name]"}',null),
(17,'Network Type','Network Type',0,'{"ColumnID":"[Dim Network Type].[Pk Net Type Id]","ColumnName":"[Dim Network Type].[Name]"}',null),
(18,'Period','Period',0,'{"ColumnID":"[Dim Period].[Pk Period Id]","ColumnName":"[Dim Period].[Name]"}',null),
(19,'Strategy','Strategy',0,'{"ColumnID":"[Dim Strategy].[Pk Strategy Id]","ColumnName":"[Dim Strategy].[Name]"}',null),
(20,'Strategy Kind','Strategy Kind',0,'{"ColumnID":"[Dim Strategy Kind].[PK Kind Id]","ColumnName":"[Dim Strategy Kind].[Name]"}',null),
(21,'Subscriber Type','Subscriber Type',0,'{"ColumnID":"[Dim Subscriber Type].[Pk Subscriber Type Id]","ColumnName":"[Dim Subscriber Type].[Name]"}',null),
(22,'Suspicion Level','Suspicion Level',0,'{"ColumnID":"[Dim Suspicion Level].[Pk Suspicion Level Id]","ColumnName":"[Dim Suspicion Level].[Name]"}',null),
(23,'Strategy User','Strategy User',0,'{"ColumnID":"[Strategy User].[Pk User Id]","ColumnName":"[Strategy User].[Name]"}',null),
(24,'MSISDNDistinctCountCases',' MSISDN Distinct Count Cases',1,'{"ColumnName":"[Measures].[MS MSISDN Distinct Count Cases]","Exepression":"","Unit":""}',null)
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

--[sec].[Module]------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Administration','Administration','Administration',null,'/images/menu-icons/Administration.png',10,0),
(2,'Fraud Analysis','Fraud Analysis','Fraud Analysis',null,'/images/menu-icons/other.png',11,0),
(3,'Workflow','Workflow','Workflow',null,'/images/menu-icons/Business Entities.png',12,0),
(4,'DataSources','DataSources','DataSources',null,'/images/menu-icons/plug.png',13,0),
(5,'Reports','Reports','Reports',null,'/images/menu-icons/busines intel.png',14,0),
(6,'Business Intelligence','Business Intelligence','BI',null,'/images/menu-icons/busines intel.png',15,1),
(7,'Dynamic Management','Dynamic Management','Dynamic Management',1,null,15,0),
(8,'Network Infrastructure','Network Infrastructure','Network Infrastructure',1,null,14,0)
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

--[sec].[View]--------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[RequiredPermissions],[Audience],[Content],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Users','Users','#/view/Security/Views/UserManagement',1,'Root/Administration Module/Users:View',null,null,0,10),
(2,'Groups','Groups','#/view/Security/Views/GroupManagement',1,'Root/Administration Module/Groups:View',null,null,0,11),
(3,'System Entities','System Entities','#/view/Security/Views/BusinessEntityManagement',1,'Root/Administration Module/System Entities:View',null,null,0,12),
(4,'Suspicious Numbers','Suspicious Numbers','#/view/FraudAnalysis/Views/SuspiciousAnalysis/SuspicionAnalysis',2,'Root/Suspicion Analysis Module:View',null,null,0,11),
(5,'Log','Log History','#/view/BusinessProcess/Views/BPHistory',3,'Root/Business Process Module/History:View',null,null,0,12),
(6,'Scheduler Service','Scheduler Service','#/view/Runtime/Views/SchedulerTaskManagement',3,'Root/Business Process Module/Management:View',null,null,0,10),
(7,'Management','Business Process Management','#/view/BusinessProcess/Views/BPDefinitionManagement',3,'Root/Business Process Module/Management:View',null,null,0,11),
(23,'Normalization Rules','Normalization Rules','#/view/PSTN_BusinessEntity/Views/NormalizationRule/NormalizationRuleManagement',3,'Root/PSTN_BusinessEntity Module:View',null,null,0,13),
(9,'Strategies','Strategies','#/view/FraudAnalysis/Views/Strategy/StrategyManagement',2,'Root/Strategy Module:View',null,null,0,10),
(10,'Management','DataSources Management','#/view/Integration/Views/DataSourceManagement',4,'Root/Integration Module:View',null,null,0,10),
(11,'Log','Log History','#/view/Integration/Views/DataSourceLogManagement',4,'Root/Integration Module:View',null,null,0,12),
(12,'Imported Batches','Imported Batches','#/view/Integration/Views/DataSourceImportedBatchManagement',4,'Root/Integration Module:View',null,null,0,11),
(13,'Cases Productivity','Cases Productivity','#/view/FraudAnalysis/Views/Reports/CasesProductivity',5,'Root/Reporting Module:View',null,null,0,12),
(14,'Blocked Lines','Blocked Lines','#/view/FraudAnalysis/Views/Reports/BlockedLines',5,'Root/Reporting Module:View',null,null,0,10),
(15,'Lines Detected','Lines Detected','#/view/FraudAnalysis/Views/Reports/LinesDetected',5,'Root/Reporting Module:View',null,null,0,11),
(16,'Cancel Cases','Cancel Cases','#/view/FraudAnalysis/Views/SuspiciousAnalysis/CancelCases',2,'Root/Suspicion Analysis Module:View',null,null,0,12),
(17,'Widgets','Widgets Management','#/view/Security/Views/WidgetsPages/WidgetManagement',7,'Root/Administration Module/Dynamic Pages:View',null,null,0,10),
(18,'Pages','Dynamic Pages Management','#/view/Security/Views/DynamicPages/DynamicPageManagement',7,'Root/Administration Module/Dynamic Pages:View',null,null,0,11),
(19,'Switches','Switches','#/view/PSTN_BusinessEntity/Views/Switch/SwitchManagement',8,'Root/PSTN_BusinessEntity Module:View',null,null,0,12),
(20,'Trunks','Trunks','#/view/PSTN_BusinessEntity/Views/Trunk/SwitchTrunkManagement',8,'Root/PSTN_BusinessEntity Module:View',null,null,0,10),
(21,'Switch Types','Switch Types','#/view/PSTN_BusinessEntity/Views/Type/SwitchTypeManagement',8,'Root/PSTN_BusinessEntity Module:View',null,null,0,11),
(22,'Ranking Pages','Ranking Pages','#/view/Security/Views/Pages/RankingPageManagement',1,'Root/Administration Module:View',null,null,0,13)
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

--[sec].[BusinessEntityModule]-----
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Root',null,0,'["View","Full Control"]'),
(2,'Strategy Module',1,0,'["View","Add","Edit", "Add System built in", "Full Control"]'),
(3,'Suspicious Cases Module',1,0,'["View"]'),
(4,'Reporting Module',1,0,'["View"]'),
(5,'Dashboard Module',1,0,'["View"]'),
(6,'Business Process Module',1,0,'["View"]'),
(7,'Integration Module',1,0,'["View"]'),
(8,'Administration Module',1,0,'["View","Add","Edit", "Delete", "Full Control"]')
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
(1,'Users',8,0,'["View", "Add", "Edit", "Reset Password"]'),
(2,'Groups',8,0,'["View", "Add", "Edit", "Delete"]'),
(3,'System Entities',8,1,'["View", "Assign Permissions"]'),
(4,'History',6,0,'["View"]'),
(5,'Managment',6,0,'["View"]')
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






