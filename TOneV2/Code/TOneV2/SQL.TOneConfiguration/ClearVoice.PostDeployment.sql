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
(1,'Fact Test Calls Count','Test Calls Count',1,'{"ColumnName":"[Measures].[Fact Test Calls Count]","Exepression":"","Unit":"Nb"}',null),
(2,'Dim Call Test Result','Call Test Result',0,'{"ColumnID":"[Dim Call Test Result].[Pk Call Test Result Id]","ColumnName":"[Dim Call Test Result].[Call Test Result]"}',3),
(3,'Dim Call Test Status','Call Test Status',0,'{"ColumnID":"[Dim Call Test Status].[Pk Call Test Status Id]","ColumnName":"[Dim Call Test Status].[Call Test Status]"}',null),
(4,'Dim Countries','Countries',0,'{"ColumnID":"[Dim Countries].[Pk Country Id]","ColumnName":"[Dim Countries].[Name]"}',null),
(5,'Dim Schedules','Schedules',0,'{"ColumnID":"[Dim Schedules].[Pk Schedule Id]","ColumnName":"[Dim Schedules].[Name]"}',null),
(6,'Dim Suppliers','Suppliers',0,'{"ColumnID":"[Dim Suppliers].[Pk Supplier Id]","ColumnName":"[Dim Suppliers].[Name]"}',null),
(7,'Dim Users','Users',0,'{"ColumnID":"[Dim Users].[Pk User Id]","ColumnName":"[Dim Users].[Name]"}',null),
(8,'Dim Zones','Zones',0,'{"ColumnID":"[Dim Zones].[Pk Zone Id]","ColumnName":"[Dim Zones].[Name]"}',null),
(9,'Dim Call Test Date','Dim Call Test Date',2,'{"Date":"[Dim Time].[Date Instance]","Year":"[Dim Time].[Year]","MonthOfYear":"[Dim Time].[Month Of Year]","WeekOfMonth":"[Dim Time].[Week Of Month]","DayOfMonth":"[Dim Time].[Day Of Month]","Hour":"[Dim Time].[Hour]","IsDefault":"True"}',null)
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

--[sec].[Module]------------------------------1001 to 1100------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1001,'Quality Measurement','Quality Measurement',null,'/images/menu-icons/CLITester.png',12,0),
(1002,'Dynamic Management','Dynamic Management',1,null,13,0)
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

GO--delete 'My Scheduler Service' since it is replace with 'Schedule Test Calls'
delete from [sec].[View] where [Id]=5002
GO
--[sec].[View]-----------------------------10001 to 11000------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(10001,'Profile','Profile','#/view/QM_CLITester/Views/Profile/ProfileManagement',1001,'QM_CLITester/Profile/GetFilteredProfiles',null,null,null,0,10),
(10002,'Test call','Test call','#/view/QM_CLITester/Views/TestPage/Test',1001,'QM_CLITester/TestCall/GetFilteredTestCalls & QM_CLITester/TestCall/GetUpdated & QM_CLITester/TestCall/GetBeforeId',null,null,null,0,11),
(10004,'History','Calls History','#/view/QM_CLITester/Views/HistoryTestCall/HistoryTestCallManagement',1001,'QM_CLITester/TestCall/GetFilteredTestCalls',null,null,null,0,12),
(10003,'Schedule Test Calls','Schedule Test Calls','#/viewwithparams/Runtime/Views/SchedulerTaskManagement/{"myTasks":"1"}',1001,'VR_Runtime/SchedulerTask/GetFilteredMyTasks',null,null,null,0,12),
(10005,'Zone','Zone','#/view/QM_BusinessEntity/Views/Zone/ZoneManagement',101,'QM_BE/Zone/GetFilteredZones',null,null,null,0,20),
(10006,'Supplier','Supplier','#/view/QM_BusinessEntity/Views/Supplier/SupplierManagement',101,'QM_BE/Supplier/GetFilteredSuppliers',null,null,null,0,21)
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

--[sec].[BusinessEntityModule]-------------1001 to 1100---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------

--[sec].[BusinessEntity]------------------2701 to 3000----------------------------------------------------------
----------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(2701,'QM_CLITester_Profile','Profile',1,0,'["View", "Edit"]'),
(2702,'QM_CLITester_TestCall','Test Call',1,0,'["View", "Test Call"]'),
(2703,'QM_BE_Supplier','Supplier',1,0,'["View", "Add", "Edit", "Download Template", "Upload"]'),
(2704,'QM_BE_Zone','Zone',1,0,'["View"]')
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

--[common].[TemplateConfig]----------30001 to 40000---------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[TemplateConfig] on;
;with cte_data([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(30001,'TOne V1 Suppliers','QM_BE_SourceSupplierReader','qm-be-sourcesupplierreader-tonev1',null,null),
(30002,'TOne V2 Suppliers','QM_BE_SourceSupplierReader','qm-be-sourcesupplierreader-tonev2',null,null),
(30003,'Connector Profiles','QM_CLITester_SourceProfileReader','qm-clitester-sourceprofilereader-itest',null,null),
(30004,'TOne V2 Zones','QM_BE_SourceZoneReader','qm-be-sourcezonereader-tonev2',null,null),
(30005,'Initiate Test - Connector','QM_CLITester_ConnectorInitiateTest','qm-clitester-testconnector-initiatetest-itest',null,null),
(30006,'Test Progress - Connector','QM_CLITester_ConnectorTestProgress','qm-clitester-testconnector-testprogress-itest',null,null),
(30007,'TOne V2 Countries','VRCommon_SourceCountryReader','vr-common-sourcecountryreader-tonev2',null,null),
(30008,'Connector Countries','VRCommon_SourceCountryReader','qm-clitester-sourcecountryreader-itest',null,null),
(30009,'Connector Zones','QM_BE_SourceZoneReader','qm-clitester-sourcezonereader-itest',null,null),
(30010,'TOne V1 Zones','QM_BE_SourceZoneReader','qm-be-sourcezonereader-tonev1',null,null),
(30011,'TOne V1 Countries','VRCommon_SourceCountryReader','vr-common-sourcecountryreader-tonev1',null,null)
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

--[common].[Setting]---------------------------301 to 400-------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[Setting] on;
;with cte_data([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(301,'Keep history in Test Calls Page','QM_CLITester_LastTestCall_Settings','General','{"Editor":"vr-qm-clitester-lasttestcall-settings-editor"}','{"$type":"QM.CLITester.Entities.LastTestCallsSettingsData, QM.CLITester.Entities","LastTestCall":60}',0),
(302,'Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Clear Voice","VersionNumber":"version 0.9"}}',0)
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