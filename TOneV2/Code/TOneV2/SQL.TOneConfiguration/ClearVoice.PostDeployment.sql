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
--[common].[ExtensionConfiguration]---------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D6381DFC-F4D8-499A-9D4D-0D0EAAAC5D32','TOne V2 Suppliers','TOne V2 Suppliers','QM_BE_SourceSupplierReader','{"Editor":"qm-be-sourcesupplierreader-tonev2"}'),
('71FAA2F0-E8B5-4E32-9EDF-53C31DB8AB6A','TOne V2 Zones','TOne V2 Zones','QM_BE_SourceZoneReader','{"Editor":"qm-be-sourcezonereader-tonev2"}'),
('15FBC0B8-806C-4642-8DA8-580311E16576','TOne V1 Suppliers','TOne V1 Suppliers','QM_BE_SourceSupplierReader','{"Editor":"qm-be-sourcesupplierreader-tonev1"}'),
('3B68FAE1-C18F-4D15-8C56-74E12A56FD47','TOne V1 Zones','TOne V1 Zones','QM_BE_SourceZoneReader','{"Editor":"qm-be-sourcezonereader-tonev1"}'),

('098E574D-F4F4-4CEB-A03D-5B387A0BE8CB','Connector Zone Info','Connector Zone Info','QM_BE_ConnectorZoneReader','{"Editor":"qm-be-connectorzone-itest"}'),
('BB371F00-B036-4745-BDBC-65637F253054','Connector Countries','Connector Countries','VRCommon_SourceCountryReader','{"Editor":"qm-clitester-sourcecountryreader-itest"}'),
('F7559C74-4CE9-4F2A-BBB4-6E92F60F0643','Connector Zones','Connector Zones','QM_BE_SourceZoneReader','{"Editor":"qm-clitester-sourcezonereader-itest"}'),
('8CEA6378-48FB-4D91-A9B5-9E0C3209321E','Connector Profiles','Connector Profiles','QM_CLITester_SourceProfileReader','{"Editor":"qm-clitester-sourceprofilereader-itest"}'),
('37CF73DD-DDF8-4AB5-BD4D-BB13DED9FE7F','Connector VI','Connector VI','QM_CLITester_ConnectorVI','{"Editor":"qm-clitester-testconnector-connectorvi"}'),

('9F336216-BF7A-4E5A-A327-CC19DE2362D5','Initiate Test - Connector','Initiate Test - Connector','QM_CLITester_ConnectorInitiateTest','{"Editor":"qm-clitester-testconnector-initiatetest-itest"}'),

('613180C1-DBE9-4382-8531-C43FA14DE858','VR_Qm_Clitester_TestCallDetailObjectType','Test Call Detail Information','VR_Common_ObjectType','{"Editor":"vr-qm-clitestertestcalldetailobjecttype", "PropertyEvaluatorExtensionType": "VR_Qm_Clitester_TestCallDetailPropertyEvaluator"}'),
('3C2D781D-5089-4D1E-9061-FA5A895AE9A2','VR_Qm_Clitester_TestCallDetailPropertyEvaluator','Test Call Detail Property','VR_Qm_Clitester_TestCallDetailPropertyEvaluator','{"Editor":"vr-qm-clitester-testcalldetailpropertyevaluator"}'),

('DE87FF03-4013-4EF4-9B7D-06376EC59FBE','VR_Qm_Clitester_ScheduleTestCallObjectType','Schedule Test Call Information','VR_Common_ObjectType','{"Editor":"vr-qm-clitesterscheduletestcallobjecttype", "PropertyEvaluatorExtensionType": "VR_Qm_Clitester_ScheduleTestCallPropertyEvaluator"}'),
('AE69ADBA-FA08-48C7-896C-961C75A0A011','VR_Qm_Clitester_ScheduleTestCallPropertyEvaluator','Schedule Test Call Property','VR_Qm_Clitester_ScheduleTestCallPropertyEvaluator','{"Editor":"vr-qm-clitester-scheduletestcallpropertyevaluator"}')
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

----[BI].[SchemaConfiguration]------------------------------------------------------------------------
--begin
--set nocount on;
--set identity_insert [BI].[SchemaConfiguration] on;
--;with cte_data([ID],[Name],[DisplayName],[Type],[Configuration],[Rank])
--as (select * from (values
----//////////////////////////////////////////////////////////////////////////////////////////////////
--(1,'Fact Test Calls Count','Test Calls Count',1,'{"ColumnName":"[Measures].[Fact Test Calls Count]","Exepression":"","Unit":"Nb"}',null),
--(2,'Dim Call Test Result','Call Test Result',0,'{"ColumnID":"[Dim Call Test Result].[Pk Call Test Result Id]","ColumnName":"[Dim Call Test Result].[Call Test Result]"}',3),
--(3,'Dim Call Test Status','Call Test Status',0,'{"ColumnID":"[Dim Call Test Status].[Pk Call Test Status Id]","ColumnName":"[Dim Call Test Status].[Call Test Status]"}',null),
--(4,'Dim Countries','Countries',0,'{"ColumnID":"[Dim Countries].[Pk Country Id]","ColumnName":"[Dim Countries].[Name]"}',null),
--(5,'Dim Schedules','Schedules',0,'{"ColumnID":"[Dim Schedules].[Pk Schedule Id]","ColumnName":"[Dim Schedules].[Name]"}',null),
--(6,'Dim Suppliers','Suppliers',0,'{"ColumnID":"[Dim Suppliers].[Pk Supplier Id]","ColumnName":"[Dim Suppliers].[Name]"}',null),
--(7,'Dim Users','Users',0,'{"ColumnID":"[Dim Users].[Pk User Id]","ColumnName":"[Dim Users].[Name]"}',null),
--(8,'Dim Zones','Zones',0,'{"ColumnID":"[Dim Zones].[Pk Zone Id]","ColumnName":"[Dim Zones].[Name]"}',null),
--(9,'Dim Call Test Date','Dim Call Test Date',2,'{"Date":"[Dim Time].[Date Instance]","Year":"[Dim Time].[Year]","MonthOfYear":"[Dim Time].[Month Of Year]","WeekOfMonth":"[Dim Time].[Week Of Month]","DayOfMonth":"[Dim Time].[Day Of Month]","Hour":"[Dim Time].[Hour]","IsDefault":"True"}',null)
----\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
--)c([ID],[Name],[DisplayName],[Type],[Configuration],[Rank]))
--merge	[BI].[SchemaConfiguration] as t
--using	cte_data as s
--on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[DisplayName] = s.[DisplayName],[Type] = s.[Type],[Configuration] = s.[Configuration],[Rank] = s.[Rank]
--when not matched by target then
--	insert([ID],[Name],[DisplayName],[Type],[Configuration],[Rank])
--	values(s.[ID],s.[Name],s.[DisplayName],s.[Type],s.[Configuration],s.[Rank]);
--set identity_insert [BI].[SchemaConfiguration] off;
------------------------------------------------------------------------------------------------------
--end

--[sec].[Module]------------------------------1001 to 1100------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('21e542bb-6bfb-4dbb-b359-ea50e3b42c57','Quality Measurement','Quality Measurement',null,'/images/menu-icons/CLITester.png',25,0)
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
end

GO--delete useless views from ClearVoice product such 'My Scheduler Service' since it is replace with 'Schedule Test Calls', 'Style Definitions', 'Organizational Charts', Lookups without countries, etc...
delete from [sec].[View] where [Id] in ('C65ED28A-36D0-4047-BEC5-030D35B02308',--'My Scheduler Service'
										'66DE2441-8A96-41E7-94EA-9F8AF38A3515',--'Style Definitions'
										'DCF8CA21-852C-41B9-9101-6990E545509D',--'Organizational Charts'
										--'604B2CB5-B839-4E51-8D13-3C1C84D05DEE',--'Countries'
										'52C580DE-C91F-45E2-8E3A-46E0BA9E7EFD',--'Component Types'
										'8AC4B99E-01A0-41D1-AE54-09E679309086',--'Status Definitions'
										'A1CE55FE-6CF4-4F15-9BC2-8E1F8DF68561',--'Regions'
										'25994374-CB99-475B-8047-3CDB7474A083',--'Cities'
										'9F691B87-4936-4C4C-A757-4B3E12F7E1D9', --'Currencies'
										'E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9', --'Exchange Rates'
										'0F111ADC-B7F6-46A4-81BC-72FFDEB305EB', --'Time Zone'
										'4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712',--'Rate Types'
										'2D39B12D-8FBF-4D4E-B2A5-5E3FE57580DF'--,'Locked Sessions'
										)
GO
--------------------------------------------------------------------------------------------------------------
--[sec].[View]-----------------------------10001 to 11000------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('b5b041a1-f037-42fc-9750-7802b9610aab','Profile','Profile','#/view/QM_CLITester/Views/Profile/ProfileManagement'											,'21e542bb-6bfb-4dbb-b359-ea50e3b42c57','QM_CLITester/Profile/GetFilteredProfiles',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',10),
('7ce99d52-af55-4111-b59b-e6de6995b84f','Test call','Test call','#/view/QM_CLITester/Views/TestPage/Test'													,'21e542bb-6bfb-4dbb-b359-ea50e3b42c57','QM_CLITester/TestCall/GetFilteredTestCalls & QM_CLITester/TestCall/GetUpdated & QM_CLITester/TestCall/GetBeforeId',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',11),
('d845620f-f6c1-49c6-a521-0629ea8d2c66','History','Calls History','#/view/QM_CLITester/Views/HistoryTestCall/HistoryTestCallManagement'						,'21e542bb-6bfb-4dbb-b359-ea50e3b42c57','QM_CLITester/TestCall/GetFilteredTestCalls',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',12),
('c65ed28a-36d0-4047-bec5-030d35b02308','Schedule Test Calls','Schedule Test Calls','#/viewwithparams/Runtime/Views/SchedulerTaskManagement/{"myTasks":"1"}','21e542bb-6bfb-4dbb-b359-ea50e3b42c57',null,null,null,'{"$type":"Vanrise.Runtime.Business.UserSchedulerServiceViewSettings, Vanrise.Runtime.Business"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',12),
('7e71942c-9569-43e0-a3e0-5f24b45de666','Zone','Zone','#/view/QM_BusinessEntity/Views/Zone/ZoneManagement'													,'E73C4ABA-FD03-4137-B047-F3FB4F7EED03','QM_BE/Zone/GetFilteredZones',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',20),
('0abbf4d1-1050-42ce-bf29-9af1d5ca4d4e','Supplier','Supplier','#/view/QM_BusinessEntity/Views/Supplier/SupplierManagement'									,'E73C4ABA-FD03-4137-B047-F3FB4F7EED03','QM_BE/Supplier/GetFilteredSuppliers',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',21)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
---------------------------------------------------------------------------------------------------------------
end

DELETE FROM [sec].[BusinessEntityModule] WHERE [ID] IN ('16419FE1-ED56-49BA-B609-284A5E21FC07',--'Traffic'
														'520558FA-CF2F-440B-9B58-09C23B6A2E9B',--'Billing'
														'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',--'Business Entities'
														'9BBD7C00-011D-4AC9-8B25-36D3E2A8F7CF',--'Rules'
														'B6B8F582-4759-43FB-9220-AA7662C366EA')--'System Processes'

--[sec].[BusinessEntityModule]----------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1D503EF4-1682-4BC3-8A78-721DE7398FA5','Quality Measurement'				,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([ID],[Name],[ParentId],[BreakInheritance])
	values(s.[ID],s.[Name],s.[ParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
end

DELETE FROM [sec].[BusinessEntity] WHERE [ID] IN (	--'9A285D4E-D4A6-4ABA-A5DA-22E7E237E808',--'City'
													--'92EA996E-C5E9-4937-9157-7CD36EF0DA37',--'Currency'
													'8BE95D10-688E-40F3-99C1-86397A51AE9B',--'Rate Type'
													'A1DBA375-456A-4769-AD55-CC12C61C721F')--'Time Zone'
--[sec].[BusinessEntity]------------------2701 to 3000----------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('4B982296-2BFD-410B-BB2A-0F9447B09CD2','QM_CLITester_Profile','Profile'	,'1D503EF4-1682-4BC3-8A78-721DE7398FA5',0,'["View", "Edit"]'),
('5B0C448E-A9D2-4F36-A9BC-54DE4E941259','QM_CLITester_TestCall','Test Call'	,'1D503EF4-1682-4BC3-8A78-721DE7398FA5',0,'["View","Test Call","View All Users"]'),
('69E0AFB3-4B55-4E01-86DD-05FED04F18B4','QM_BE_Supplier','Supplier'			,'1D503EF4-1682-4BC3-8A78-721DE7398FA5',0,'["View", "Add", "Edit", "Download Template", "Upload"]'),
('194C1913-460D-4335-A3F9-DD0E5798F781','QM_BE_Zone','Zone'					,'1D503EF4-1682-4BC3-8A78-721DE7398FA5',0,'["View"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([ID],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[ID],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
----------------------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]------------------------------------------------------------------------------
begin
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('QM_CLITester/TestCall/GetTotalCallsByUserId',null),

('QM_CLITester/TestCall/AddNewTestCall'			,'QM_CLITester_TestCall: Test Call'),
('QM_CLITester/TestCall/GetUpdated'				,'QM_CLITester_TestCall: View'),
('QM_CLITester/TestCall/GetBeforeId'			,'QM_CLITester_TestCall: View'),
('QM_CLITester/TestCall/GetFilteredTestCalls'	,'QM_CLITester_TestCall: View'),
('QM_CLITester/TestCall/ViewAllUsers'			,'QM_CLITester_TestCall: View All Users'),

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
----------------------------------------------------------------------------------------------------
end

--Delete from [common].[Setting] where [ID] in (	'1CB20F2C-A835-4320-AEC7-E034C5A756E9',--'Bank Details'
--												'1C833B2D-8C97-4CDD-A1C1-C1B4D9D299DE',--'System Currency'
--												'81F62AC3-BAE4-4A2F-A60D-A655494625EA' )--'Company Setting'
Delete from [common].[Setting] where ID = '4047054E-1CF4-4BE6-A005-6D4706757AD3'--,'Session Lock'
--[common].[Setting]--------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3830C9C4-E055-40EA-8D91-3DAA72729CBD','Keep history in Test Calls Page','QM_CLITester_LastTestCall_Settings','General','{"Editor":"vr-qm-clitester-lasttestcall-settings-editor"}','{"$type":"QM.CLITester.Entities.LastTestCallsSettingsData, QM.CLITester.Entities","LastTestCall":60}',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
----------------------------------------------------------------------------------------------------
end

--[common].[VRObjectTypeDefinition]-----------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7B6B09AF-D2F8-47E0-B4FD-187A8075D4BB','TestCall','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultObjectType, QM.CLITester.Entities","ConfigId":"613180c1-dbe9-4382-8531-c43fa14de858"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Supplier Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Supplier Name","Description":"Supplier Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":0}},"User Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"User Name","Description":"User Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":1}},"Country Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Country Name","Description":"Country Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":2}},"Zone Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Zone Name","Description":"Zone Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":3}},"Call Test Status Description":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Call Test Status Description","Description":"Call Test Status Description","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":4}},"Call Test Result Description":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Call Test Result Description","Description":"Call Test Result Description","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":5}},"Schedule Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Schedule Name","Description":"Schedule Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":6}},"Pdd":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Pdd","Description":"Pdd","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":7}},"Mos":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Mos","Description":"Mos","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":8}},"Creation Date":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Creation Date","Description":"Creation Date","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":9}},"Source":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Source","Description":"Source","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":10}},"Destination":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Destination","Description":"Destination","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":11}},"Received Cli":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Received Cli","Description":"Received Cli","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":12}},"Ring Duration":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Ring Duration","Description":"Ring Duration","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":13}},"Release Code":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Release Code","Description":"Release Code","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":15}},"To Mail":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"To Mail","Description":"To Mail","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":16}},"Start":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Start","Description":"Start","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":17}}}}'),
('ADAB472E-799F-44C4-B632-D85362671056','Schedule Test Call','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"QM.CLITester.Entities.VRObjectTypes.ScheduleTestResultObjectType, QM.CLITester.Entities","ConfigId":"de87ff03-4013-4ef4-9b7d-06376ec59fbe"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Task Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Task Name","Description":"Task Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.ScheduleTestCallPropertyEvaluator, QM.CLITester.Entities","ConfigId":"ae69adba-fa08-48c7-896c-961c75a0a011","ScheduleTestCallField":0}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[VRObjectTypeDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);
----------------------------------------------------------------------------------------------------
END

--[common].[MailMessageType]------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D87DD73B-B143-486B-AFE9-303F8D4C90AC','Test Call','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","SendTestCallResultObjList":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"SendTestCallResultObjList","VRObjectTypeDefinitionId":"7b6b09af-d2f8-47e0-b4fd-187a8075d4bb"},"Product":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Product","VRObjectTypeDefinitionId":"62b9a4da-0018-4514-bcfd-8268a58f53a2"}}}'),
('134646B8-5336-4438-9E54-AA9B3D6B027D','Scheduler Test Call','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Product":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Product","VRObjectTypeDefinitionId":"62b9a4da-0018-4514-bcfd-8268a58f53a2"},"SendScheduleTestCallResultObjList":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"SendScheduleTestCallResultObjList","VRObjectTypeDefinitionId":"adab472e-799f-44c4-b632-d85362671056"}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[MailMessageType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);
----------------------------------------------------------------------------------------------------
end

--[common].[MailMessageTemplate]--------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[MessageTypeID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('019A4F17-CDDC-47FC-B0E0-4F65473E173D','Test Call','D87DD73B-B143-486B-AFE9-303F8D4C90AC','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"SendTestCallResultObjList\",\"To Mail\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): Test call summary for supplier: @Model.GetVal(\"SendTestCallResultObjList\",\"Supplier Name\") on Country: @Model.GetVal(\"SendTestCallResultObjList\",\"Country Name\")"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Dears,\n<div>Kindly find below Test call summary</div>\n<br />Supplier: @Model.GetVal(\"SendTestCallResultObjList\",\"Supplier Name\")\n<br />Country: @Model.GetVal(\"SendTestCallResultObjList\",\"Country Name\")\n<br />Zone: @Model.GetVal(\"SendTestCallResultObjList\",\"Zone Name\")\n<br />Test Date: @Model.GetVal(\"SendTestCallResultObjList\",\"Creation Date\")\n<br /><br />\n<table style=\"border-collapse: collapse;\" border=\"1\" cellpadding=\"10\">\n<tr>\n    <th>Source</th>\n    <th>Destination </th>\n    <th>Received Cli </th>\n    <th>Ring Duration </th>\n    <th>Release Code </th>\n\t<th>Start </th>\n    <th>Pdd </th>\n    <th>Mos </th>\n    <th>Call Result </th>\n</tr>\n<!--repeated section-->\n@if (Model.GetVal(\"SendTestCallResultObjList\",\"Source\") != null)\n{\nvar  source= Model.GetVal(\"SendTestCallResultObjList\",\"Source\").ToString();\nvar sourceSplit= source.Split('';'');\n\nvar destination= Model.GetVal(\"SendTestCallResultObjList\",\"Destination\").ToString();\nstring[] destinationSplit= destination.Split('';'');\n\nvar receivedCli= Model.GetVal(\"SendTestCallResultObjList\",\"Received Cli\").ToString();\nstring[] receivedCliSplit= receivedCli.Split('';'');\n\nvar ringDuration= Model.GetVal(\"SendTestCallResultObjList\",\"Ring Duration\").ToString();\nstring[] ringDurationSplit= ringDuration.Split('';'');\n\nvar releaseCode= Model.GetVal(\"SendTestCallResultObjList\",\"Release Code\").ToString();\nstring[] releaseCodeSplit= releaseCode.Split('';'');\n\nvar start= Model.GetVal(\"SendTestCallResultObjList\",\"Start\").ToString();\nstring[] startSplit= start.Split('';'');\n\nvar Pdd= Model.GetVal(\"SendTestCallResultObjList\",\"Pdd\").ToString();\nstring[] pddSplit= Pdd.Split('';'');\n\n\nvar Mos= Model.GetVal(\"SendTestCallResultObjList\",\"Mos\").ToString();\nstring[] mosSplit= Mos.Split('';'');\n\nvar testResult= Model.GetVal(\"SendTestCallResultObjList\",\"Call Test Result Description\").ToString();\nstring[] testResultSplit= testResult.Split('';'');\n\n\nint i = 0;\nforeach (var s in sourceSplit) {\nif (s != \"\"){\n\n<tr>\n    <td>@s </td>\n    <td>@destinationSplit[i] </td>\n    <td>@receivedCliSplit[i] </td>\n    <td>@ringDurationSplit[i] </td>\n    <td>@releaseCodeSplit[i] </td>\n\t<td>@startSplit[i] </td>\n    <td>@pddSplit[i] </td>\n    <td>@mosSplit[i] </td>\n    <td>@testResultSplit[i] </td>\n</tr>\n\n} i++;}\n\n}\n<!--end repeated section-->\n</table>\n\n<br />\n<div>Regards;</div>\n<div>@Model.GetVal(\"SendTestCallResultObjList\",\"User Name\")</div>"}}'),
('0538B964-C1A8-4185-88A6-D88AB1509E4F','Scheduler Test Call','134646B8-5336-4438-9E54-AA9B3D6B027D','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"emails from predefined scheduler"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): Scheduler Test call: @Model.GetVal(\"SendScheduleTestCallResultObjList\",\"Task Name\")"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"<div>Dears,</div><div>Kindly find attached your scheduler task details.</div>\n<div>For more information, don''t hesitate to contact us.</div>"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[MessageTypeID],[Settings]))
merge	[common].[MailMessageTemplate] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[MessageTypeID] = s.[MessageTypeID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[MessageTypeID],[Settings])
	values(s.[ID],s.[Name],s.[MessageTypeID],s.[Settings]);
----------------------------------------------------------------------------------------------------
end

Delete from [runtime].[SchedulerTaskActionType] where Id in ('0A15BC35-A3A7-4ED3-B09B-1B41A7A9DDC9','7A35F562-319B-47B3-8258-EC1A704A82EB') --Exchange Rate, workflow

--[runtime].[SchedulerTaskActionType]---------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[ActionTypeInfo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A6918DD7-90F2-48EB-A221-F8721F169926','Create Test Call','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/TestCallTemplate.html","SystemType":false,"Editor":"qm-clitester-testcall","FQTN":"QM.CLITester.Business.TestCallTaskAction, QM.CLITester.Business","IsUserTask":true}'),

('D834DF4F-FA44-413F-BC5F-3B9D5CA758F9','Download Test Result','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/TestProgressTemplate.html","SystemType":false,"Editor":"qm-clitester-testprogress","FQTN":"QM.CLITester.Business.TestProgressTaskAction, QM.CLITester.Business","Security":{"$type":"Vanrise.Runtime.Entities.ActionTypeInfoSecurity, Vanrise.Runtime.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["View"]}]},"ConfigurePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Configure"]}]},"RunPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Run"]}]}}}'),
('DB26E91D-9E36-4398-822C-4A41D868BA50','Initiate Test','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/InitiateTestTemplate.html","SystemType":false,"Editor":"qm-clitester-initiatetest","FQTN":"QM.CLITester.Business.InitiateTestTaskAction, QM.CLITester.Business","Security":{"$type":"Vanrise.Runtime.Entities.ActionTypeInfoSecurity, Vanrise.Runtime.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["View"]}]},"ConfigurePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Configure"]}]},"RunPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Run"]}]}}}'),
('81212549-83AE-495B-8F34-5D7559165791','Zone Synchronize','{"URL":"/Client/Modules/QM_BusinessEntity/Views/Zone/SchedulerTaskAction/ZoneSynchronizeTemplate.html","SystemType":false,"Editor":"qm-be-sourcezonereader","FQTN":"QM.BusinessEntity.Business.ZoneSyncTaskAction, QM.BusinessEntity.Business","Security":{"$type":"Vanrise.Runtime.Entities.ActionTypeInfoSecurity, Vanrise.Runtime.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["View"]}]},"ConfigurePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Configure"]}]},"RunPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Run"]}]}}}'),
('7E491110-062D-4AA1-87D9-79656A2880E7','Supplier Synchronize','{"URL":"/Client/Modules/QM_BusinessEntity/Views/Supplier/SchedulerTaskAction/SupplierSynchronizeTemplate.html","SystemType":false,"Editor":"qm-be-sourcesupplierreader","FQTN":"QM.BusinessEntity.Business.SupplierSyncTaskAction, QM.BusinessEntity.Business","Security":{"$type":"Vanrise.Runtime.Entities.ActionTypeInfoSecurity, Vanrise.Runtime.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["View"]}]},"ConfigurePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Configure"]}]},"RunPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Run"]}]}}}'),
('92D77B93-B9DC-4A28-8FB4-E9C17BCCFB06','Profile Synchronize','{"URL":"/Client/Modules/QM_CLITester/Views/Profile/SchedulerTaskAction/ProfileSynchronizeTemplate.html","SystemType":false,"Editor":"vr-qm-clitester-sourceprofilereader","FQTN":"QM.CLITester.Business.ProfileSyncTaskAction, QM.CLITester.Business","Security":{"$type":"Vanrise.Runtime.Entities.ActionTypeInfoSecurity, Vanrise.Runtime.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["View"]}]},"ConfigurePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Configure"]}]},"RunPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Run"]}]}}}'),
('6AF4B89D-7809-4221-BB92-FAC2F620C502','Connector Zones Codes','{"URL": "/Client/Modules/QM_BusinessEntity/Views/ConnectorZoneInfo/ConnectorZoneInfoTemplate.html", "SystemType": false, "Editor": "qm-be-connectorzoneinfo", "FQTN": "QM.CLITester.iTestIntegration.VIConnectorSyncTaskAction, QM.CLITester.iTestIntegration", "Security":{"$type":"Vanrise.Runtime.Entities.ActionTypeInfoSecurity, Vanrise.Runtime.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["View"]}]},"ConfigurePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Configure"]}]},"RunPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Run"]}]}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ActionTypeInfo]))
merge	[runtime].[SchedulerTaskActionType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ActionTypeInfo] = s.[ActionTypeInfo]
when not matched by target then
	insert([ID],[Name],[ActionTypeInfo])
	values(s.[ID],s.[Name],s.[ActionTypeInfo]);
----------------------------------------------------------------------------------------------------
END



--[runtime].[RuntimeNodeConfiguration]--------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('DE964189-1EC2-44F7-9AD5-692BF47E47EC','Default','{"$type":"Vanrise.Runtime.Entities.RuntimeNodeConfigurationSettings, Vanrise.Runtime.Entities","Processes":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities]], mscorlib","1fa8c599-fa3f-4b67-9934-fcb7b9afe44e":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Scheduler Service Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":1,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","c5205fb9-51c8-45e7-a6d8-d69cd097b3c4":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Scheduler Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.Runtime.SchedulerService, Vanrise.Runtime","ServiceTypeUniqueName":"Vanrise.Runtime.SchedulerService, Vanrise.Runtime, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","Interval":"00:00:01"}}}}}}}}',null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy]))
merge	[runtime].[RuntimeNodeConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[Settings] = s.[Settings],[CreatedBy] = s.[CreatedBy],[LastModifiedBy] = s.[LastModifiedBy]
when not matched by target then
	insert([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
	values(s.[ID],s.[Name],s.[Settings],s.[CreatedBy],s.[LastModifiedBy]);



--[runtime].[RuntimeNode]---------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('EF1E6CC9-ED0B-47FC-85AB-EFC7A8C505DB','DE964189-1EC2-44F7-9AD5-692BF47E47EC','Node 1',null,null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy]))
merge	[runtime].[RuntimeNode] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[RuntimeNodeConfigurationID] = s.[RuntimeNodeConfigurationID],[Name] = s.[Name],[Settings] = s.[Settings],[CreatedBy] = s.[CreatedBy],[LastModifiedBy] = s.[LastModifiedBy]
when not matched by target then
	insert([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
	values(s.[ID],s.[RuntimeNodeConfigurationID],s.[Name],s.[Settings],s.[CreatedBy],s.[LastModifiedBy]);
