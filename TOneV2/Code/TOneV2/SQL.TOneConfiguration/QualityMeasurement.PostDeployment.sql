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
--[sec].[WidgetDefinition]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
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
(1,'Fact Test Calls Count','Test Calls Count',1,'{"ColumnName":"[Measures].[Fact Test Calls Count]","Exepression":"","Unit":"Nb"}',null),
(2,'Dim Call Test Result','Call Test Result',0,'{"ColumnID":"[Dim Call Test Result].[Pk Call Test Result Id]","ColumnName":"[Dim Call Test Result].[Call Test Result]"}',3),
(3,'Dim Call Test Status','Call Test Status',0,'{"ColumnID":"[Dim Call Test Status].[Pk Call Test Status Id]","ColumnName":"[Dim Call Test Status].[Call Test Status]"}',null),
(4,'Dim Countries','Countries',0,'{"ColumnID":"[Dim Countries].[Pk Country Id]","ColumnName":"[Dim Countries].[Name]"}',null),
(5,'Dim Schedules','Schedules',0,'{"ColumnID":"[Dim Schedules].[Pk Schedule Id]","ColumnName":"[Dim Schedules].[Name]"}',null),
(6,'Dim Suppliers','Suppliers',0,'{"ColumnID":"[Dim Suppliers].[Pk Supplier Id]","ColumnName":"[Dim Suppliers].[Name]"}',null),
(7,'Dim Users','Users',0,'{"ColumnID":"[Dim Users].[Pk User Id]","ColumnName":"[Dim Users].[Name]"}',null),
(8,'Dim Zones','Zones',0,'{"ColumnID":"[Dim Zones].[Pk Zone Id]","ColumnName":"[Dim Zones].[Name]"}',null)
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

--[sec].[Module]------------------------------701 to 800------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Administration','Administration','Administration',null,'/images/menu-icons/Administration.png',10,0),
(2,'CLI Tester','CLI Tester','CLI Tester',null,'/images/menu-icons/CLITester.png',12,0),
(3,'Business Entities','Business Entities','Business Entities',null,'/images/menu-icons/Business Entities.png',11,0),
(4,'Dynamic Management','Dynamic Management','Dynamic Management',1,null,13,0),
(5,'Business Intelligence','Business Intelligence','BI',null,'/images/menu-icons/busines intel.png',null,1)
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

--[sec].[View]-----------------------------7001 to 8000--------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Users','Users','#/view/Security/Views/User/UserManagement',1,'VR_Sec/Users/GetFilteredUsers',null,null,null,0,10),
(2,'Groups','Groups','#/view/Security/Views/Group/GroupManagement',1,'VR_Sec/Group/GetFilteredGroups',null,null,null,0,11),
(3,'System Entities','System Entities','#/view/Security/Views/Permission/BusinessEntityManagement',1,'VR_Sec/BusinessEntityNode/GetEntityNodes &amp; VR_Sec/Permission/GetFilteredEntityPermissions',null,null,null,0,12),
(4,'Ranking Pages','Ranking Pages','#/view/Security/Views/Pages/RankingPageManagement',1,'VR_Sec/View/UpdateViewsRank',null,null,null,0,14),
(5,'Test call','Test call','#/view/QM_CLITester/Views/TestPage/Test',2,'QM_CLITester/TestCall/GetFilteredTestCalls &amp; QM_CLITester/TestCall/GetUpdated &amp; QM_CLITester/TestCall/GetBeforeId',null,null,null,0,11),
(6,'Supplier','Supplier','#/view/QM_BusinessEntity/Views/Supplier/SupplierManagement',3,'QM_BE/Supplier/GetFilteredSuppliers',null,null,null,0,10),
(7,'Scheduler Service','Scheduler Service','#/view/Runtime/Views/SchedulerTaskManagement',1,'VR_Runtime/SchedulerTask/GetFilteredTasks',null,null,null,0,13),
(8,'History','Calls History','#/view/QM_CLITester/Views/HistoryTestCall/HistoryTestCallManagement',2,'QM_CLITester/TestCall/GetFilteredTestCalls',null,null,null,0,12),
(9,'Profile','Profile','#/view/QM_CLITester/Views/Profile/ProfileManagement',2,'QM_CLITester/Profile/GetFilteredProfiles',null,null,null,0,10),
(10,'Zone','Zone','#/view/QM_BusinessEntity/Views/Zone/ZoneManagement',3,'QM_BE/Zone/GetFilteredZones',null,null,null,0,10),
(11,'Country','Country','#/view/Common/Views/Country/CountryManagement',2,'VRCommon/Country/GetFilteredCountries',null,null,null,0,14),
(12,'Schedule Test Calls','Schedule Test Calls','#/view/QM_CLITester/Views/ScheduleTestCall/ScheduleTestCallManagement',2,'VR_Runtime/SchedulerTask/GetFilteredTasks',null,null,null,0,16),
(13,'Pages','Dynamic Pages Management','#/view/Security/Views/DynamicPages/DynamicPageManagement',4,null,null,null,null,0,11),
(14,'Widgets','Widgets Management','#/view/Security/Views/WidgetsPages/WidgetManagement',4,null,null,null,null,0,10)
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

--[sec].[BusinessEntity]------------------2101 to 2400----------------------------------------------------------
----------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(2101,'QM_CLITester_Profile','Profile',1,0,'["View", "Edit"]'),
(2102,'QM_CLITester_TestCall','Test Call',1,0,'["View", "Test Call"]'),
(2103,'QM_BE_Supplier','Supplier',1,0,'["View", "Add", "Edit", "Download Template", "Upload"]'),
(2104,'QM_BE_Zone','Zone',1,0,'["View"]')
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

--[common].[TemplateConfig]----------40001 to 50000---------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[TemplateConfig] on;
;with cte_data([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(40001,'TOne V1 Suppliers','QM_BE_SourceSupplierReader','qm-be-sourcesupplierreader-tonev1',null,null),
(40002,'TOne V2 Suppliers','QM_BE_SourceSupplierReader','qm-be-sourcesupplierreader-tonev2',null,null),
(40003,'iTest Profiles','QM_CLITester_SourceProfileReader','qm-clitester-sourceprofilereader-itest',null,null),
(40004,'TOne V2 Zones','QM_BE_SourceZoneReader','qm-be-sourcezonereader-tonev2',null,null),
(40005,'Initiate Test - ITest','QM_CLITester_ConnectorInitiateTest','qm-clitester-testconnector-initiatetest-itest',null,null),
(40006,'Test Progress - ITest','QM_CLITester_ConnectorTestProgress','qm-clitester-testconnector-testprogress-itest',null,null),
(40007,'TOne V2 Countries','VRCommon_SourceCountryReader','vr-common-sourcecountryreader-tonev2',null,null),
(40008,'I-Test Countries','VRCommon_SourceCountryReader','qm-clitester-sourcecountryreader-itest',null,null),
(40009,'I-Test Zones','QM_BE_SourceZoneReader','qm-clitester-sourcezonereader-itest',null,null),
(40010,'TOne V1 Zones','QM_BE_SourceZoneReader','qm-be-sourcezonereader-tonev1',null,null),
(40011,'TOne V1 Countries','VRCommon_SourceCountryReader','vr-common-sourcecountryreader-tonev1',null,null)
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
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('QM_CLITester/TestCall/AddNewTestCall','QM_CLITester_TestCall:Test Call'),
('QM_CLITester/TestCall/GetUpdated','QM_CLITester_TestCall: View'),
('QM_CLITester/TestCall/GetTotalCallsByUserId',null),
('QM_CLITester/TestCall/GetBeforeId','QM_CLITester_TestCall: View'),
('QM_CLITester/TestCall/GetFilteredTestCalls','QM_CLITester_TestCall: View'),
('QM_CLITester/TestCall/GetInitiateTestTemplates',null),
('QM_CLITester/TestCall/GetTestProgressTemplates',null),
('QM_CLITester/Profile/GetFilteredProfiles','QM_CLITester_Profile: View'),
('QM_CLITester/Profile/GetProfile',null),
('QM_CLITester/Profile/GetProfilesInfo',null),
('QM_CLITester/Profile/UpdateProfile','QM_CLITester_Profile: Edit'),
('QM_CLITester/Profile/GetProfileSourceTemplates',null),
('QM_BE/Zone/GetZoneSourceTemplates',null),
('QM_BE/Zone/GetZonesInfo',null),
('QM_BE/Zone/GetFilteredZones','QM_BE_Zone: View'),
('QM_BE/Supplier/GetFilteredSuppliers','QM_BE_Supplier: View'),
('QM_BE/Supplier/GetSupplier',null),
('QM_BE/Supplier/GetSuppliersInfo',null),
('QM_BE/Supplier/AddSupplier','QM_BE_Supplier: Add'),
('QM_BE/Supplier/UpdateSupplier','QM_BE_Supplier: Edit'),
('QM_BE/Supplier/GetSupplierSourceTemplates',null),
('QM_BE/Supplier/DownloadImportSupplierTemplate','QM_BE_Supplier: Download Template'),
('QM_BE/Supplier/UploadSuppliersList','QM_BE_Supplier: Upload')
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