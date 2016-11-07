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
begin
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
----------------------------------------------------------------------------------------------------
end


--[sec].[Module]------------------------------1001 to 1100------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('21e542bb-6bfb-4dbb-b359-ea50e3b42c57','Quality Measurement','Quality Measurement',null,'/images/menu-icons/CLITester.png',12,0)
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
delete from [sec].[View] where [Id] in ('C65ED28A-36D0-4047-BEC5-030D35B02308','66DE2441-8A96-41E7-94EA-9F8AF38A3515','DCF8CA21-852C-41B9-9101-6990E545509D','25994374-CB99-475B-8047-3CDB7474A083','9F691B87-4936-4C4C-A757-4B3E12F7E1D9', 'E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9', '0F111ADC-B7F6-46A4-81BC-72FFDEB305EB', '4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712')
GO
--------------------------------------------------------------------------------------------------------------
--[sec].[View]-----------------------------10001 to 11000------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('b5b041a1-f037-42fc-9750-7802b9610aab','Profile','Profile','#/view/QM_CLITester/Views/Profile/ProfileManagement','21e542bb-6bfb-4dbb-b359-ea50e3b42c57','QM_CLITester/Profile/GetFilteredProfiles',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,10),
('7ce99d52-af55-4111-b59b-e6de6995b84f','Test call','Test call','#/view/QM_CLITester/Views/TestPage/Test','21e542bb-6bfb-4dbb-b359-ea50e3b42c57','QM_CLITester/TestCall/GetFilteredTestCalls & QM_CLITester/TestCall/GetUpdated & QM_CLITester/TestCall/GetBeforeId',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,11),
('d845620f-f6c1-49c6-a521-0629ea8d2c66','History','Calls History','#/view/QM_CLITester/Views/HistoryTestCall/HistoryTestCallManagement','21e542bb-6bfb-4dbb-b359-ea50e3b42c57','QM_CLITester/TestCall/GetFilteredTestCalls',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,12),
('c65ed28a-36d0-4047-bec5-030d35b02308','Schedule Test Calls','Schedule Test Calls','#/viewwithparams/Runtime/Views/SchedulerTaskManagement/{"myTasks":"1"}','21e542bb-6bfb-4dbb-b359-ea50e3b42c57','VR_Runtime/SchedulerTask/GetFilteredMyTasks',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,12),
('7e71942c-9569-43e0-a3e0-5f24b45de666','Zone','Zone','#/view/QM_BusinessEntity/Views/Zone/ZoneManagement','E73C4ABA-FD03-4137-B047-F3FB4F7EED03','QM_BE/Zone/GetFilteredZones',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,20),
('0abbf4d1-1050-42ce-bf29-9af1d5ca4d4e','Supplier','Supplier','#/view/QM_BusinessEntity/Views/Supplier/SupplierManagement','E73C4ABA-FD03-4137-B047-F3FB4F7EED03','QM_BE/Supplier/GetFilteredSuppliers',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,21)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType]=s.[OldType],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);
---------------------------------------------------------------------------------------------------------------

end


--[sec].[BusinessEntityModule]-------------1001 to 1100---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------

--[sec].[BusinessEntity]------------------2701 to 3000----------------------------------------------------------
begin
set nocount on;;with cte_data([ID],[OldId],[Name],[Title],[OleModuleId],[ModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('4B982296-2BFD-410B-BB2A-0F9447B09CD2',2701,'QM_CLITester_Profile','Profile',1,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0,'["View", "Edit"]'),('5B0C448E-A9D2-4F36-A9BC-54DE4E941259',2702,'QM_CLITester_TestCall','Test Call',1,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0,'["View","Test Call","View All Users"]'),('69E0AFB3-4B55-4E01-86DD-05FED04F18B4',2703,'QM_BE_Supplier','Supplier',1,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0,'["View", "Add", "Edit", "Download Template", "Upload"]'),('194C1913-460D-4335-A3F9-DD0E5798F781',2704,'QM_BE_Zone','Zone',1,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0,'["View"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[Title],[OleModuleId],[ModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[OleModuleId] = s.[OleModuleId],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([ID],[OldId],[Name],[Title],[OleModuleId],[ModuleId],[BreakInheritance],[PermissionOptions])	values(s.[ID],s.[OldId],s.[Name],s.[Title],s.[OleModuleId],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
----------------------------------------------------------------------------------------------------------------
end


--[sec].[SystemAction]------------------------------------------------------------------------------
begin
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('QM_CLITester/TestCall/GetTotalCallsByUserId',null),

('QM_CLITester/TestCall/AddNewTestCall'			,'QM_CLITester_TestCall:Test Call'),
('QM_CLITester/TestCall/GetUpdated'				,'QM_CLITester_TestCall: View'),
('QM_CLITester/TestCall/GetBeforeId'			,'QM_CLITester_TestCall: View'),
('QM_CLITester/TestCall/GetFilteredTestCalls'	,'QM_CLITester_TestCall: View'),
('QM_CLITester/TestCall/ViewAllUsers'			,'QM_CLITester_TestCall:View All Users'),

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


GO--delete useless [Setting] from ClearVoice product such 'System Currency', etc...
delete from [common].[Setting] where [Id] in ('1C833B2D-8C97-4CDD-A1C1-C1B4D9D299DE')
GO
--[common].[Setting]--------------------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('3830C9C4-E055-40EA-8D91-3DAA72729CBD',301,'Keep history in Test Calls Page','QM_CLITester_LastTestCall_Settings','General','{"Editor":"vr-qm-clitester-lasttestcall-settings-editor"}','{"$type":"QM.CLITester.Entities.LastTestCallsSettingsData, QM.CLITester.Entities","LastTestCall":60}',0),('AC45AC65-5CB4-417E-912C-B18200BF1451',302,'Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Clear Voice","VersionNumber":"version 0.9"}}',1)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))merge	[common].[Setting] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldID] = s.[OldID],[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]when not matched by target then	insert([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])	values(s.[ID],s.[OldID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);----------------------------------------------------------------------------------------------------end
--[common].[VRObjectTypeDefinition]---------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('7B6B09AF-D2F8-47E0-B4FD-187A8075D4BB','TestCall','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultObjectType, QM.CLITester.Entities","ConfigId":"613180c1-dbe9-4382-8531-c43fa14de858"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Supplier Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Supplier Name","Description":"Supplier Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":0}},"User Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"User Name","Description":"User Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":1}},"Country Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Country Name","Description":"Country Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":2}},"Zone Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Zone Name","Description":"Zone Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":3}},"Call Test Status Description":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Call Test Status Description","Description":"Call Test Status Description","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":4}},"Call Test Result Description":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Call Test Result Description","Description":"Call Test Result Description","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":5}},"Schedule Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Schedule Name","Description":"Schedule Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":6}},"Pdd":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Pdd","Description":"Pdd","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":7}},"Mos":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Mos","Description":"Mos","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":8}},"Creation Date":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Creation Date","Description":"Creation Date","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":9}},"Source":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Source","Description":"Source","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":10}},"Destination":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Destination","Description":"Destination","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":11}},"Received Cli":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Received Cli","Description":"Received Cli","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":12}},"Ring Duration":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Ring Duration","Description":"Ring Duration","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":13}},"Release Code":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Release Code","Description":"Release Code","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":15}},"To Mail":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"To Mail","Description":"To Mail","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":16}},"Start":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Start","Description":"Start","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","ConfigId":"3c2d781d-5089-4d1e-9061-fa5a895ae9a2","TestCallDetailField":17}}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[VRObjectTypeDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);

--[common].[MailMessageType]------------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('D87DD73B-B143-486B-AFE9-303F8D4C90AC','Test Call','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","SendTestCallResultObjList":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"SendTestCallResultObjList","VRObjectTypeDefinitionId":"7b6b09af-d2f8-47e0-b4fd-187a8075d4bb"},"Product":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Product","VRObjectTypeDefinitionId":"62b9a4da-0018-4514-bcfd-8268a58f53a2"}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[MailMessageType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);----------------------------------------------------------------------------------------------------end

--[common].[MailMessageTemplate]--------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[MessageTypeID],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('019A4F17-CDDC-47FC-B0E0-4F65473E173D','Test Call','D87DD73B-B143-486B-AFE9-303F8D4C90AC','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"SendTestCallResultObjList\",\"To Mail\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): Test call summary for supplier: @Model.GetVal(\"SendTestCallResultObjList\",\"Supplier Name\") on Country: @Model.GetVal(\"SendTestCallResultObjList\",\"Country Name\")"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Dears,\n<div>Kindly find below Test call summary</div>\n<br />Supplier: @Model.GetVal(\"SendTestCallResultObjList\",\"Supplier Name\")\n<br />Country: @Model.GetVal(\"SendTestCallResultObjList\",\"Country Name\")\n<br />Zone: @Model.GetVal(\"SendTestCallResultObjList\",\"Zone Name\")\n<br />Test Date: @Model.GetVal(\"SendTestCallResultObjList\",\"Creation Date\")\n<br /><br />\n<table style=\"border-collapse: collapse;\" border=\"1\" cellpadding=\"10\">\n<tr>\n    <th>Source</th>\n    <th>Destination </th>\n    <th>Received Cli </th>\n    <th>Ring Duration </th>\n    <th>Release Code </th>\n\t<th>Start </th>\n    <th>Pdd </th>\n    <th>Mos </th>\n    <th>Call Result </th>\n</tr>\n<!--repeated section-->\n@if (Model.GetVal(\"SendTestCallResultObjList\",\"Source\") != null)\n{\nvar  source= Model.GetVal(\"SendTestCallResultObjList\",\"Source\").ToString();\nvar sourceSplit= source.Split('';'');\n\nvar destination= Model.GetVal(\"SendTestCallResultObjList\",\"Destination\").ToString();\nstring[] destinationSplit= destination.Split('';'');\n\nvar receivedCli= Model.GetVal(\"SendTestCallResultObjList\",\"Received Cli\").ToString();\nstring[] receivedCliSplit= receivedCli.Split('';'');\n\nvar ringDuration= Model.GetVal(\"SendTestCallResultObjList\",\"Ring Duration\").ToString();\nstring[] ringDurationSplit= ringDuration.Split('';'');\n\nvar releaseCode= Model.GetVal(\"SendTestCallResultObjList\",\"Release Code\").ToString();\nstring[] releaseCodeSplit= releaseCode.Split('';'');\n\nvar start= Model.GetVal(\"SendTestCallResultObjList\",\"Start\").ToString();\nstring[] startSplit= start.Split('';'');\n\nvar Pdd= Model.GetVal(\"SendTestCallResultObjList\",\"Pdd\").ToString();\nstring[] pddSplit= Pdd.Split('';'');\n\n\nvar Mos= Model.GetVal(\"SendTestCallResultObjList\",\"Mos\").ToString();\nstring[] mosSplit= Mos.Split('';'');\n\nvar testResult= Model.GetVal(\"SendTestCallResultObjList\",\"Call Test Result Description\").ToString();\nstring[] testResultSplit= testResult.Split('';'');\n\n\nint i = 0;\nforeach (var s in sourceSplit) {\nif (s != \"\"){\n\n<tr>\n    <td>@s </td>\n    <td>@destinationSplit[i] </td>\n    <td>@receivedCliSplit[i] </td>\n    <td>@ringDurationSplit[i] </td>\n    <td>@releaseCodeSplit[i] </td>\n\t<td>@startSplit[i] </td>\n    <td>@pddSplit[i] </td>\n    <td>@mosSplit[i] </td>\n    <td>@testResultSplit[i] </td>\n</tr>\n\n} i++;}\n\n}\n<!--end repeated section-->\n</table>\n\n<br />\n<div>Regards;</div>\n<div>@Model.GetVal(\"SendTestCallResultObjList\",\"User Name\")</div>"}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[MessageTypeID],[Settings]))merge	[common].[MailMessageTemplate] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[MessageTypeID] = s.[MessageTypeID],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[MessageTypeID],[Settings])	values(s.[ID],s.[Name],s.[MessageTypeID],s.[Settings]);----------------------------------------------------------------------------------------------------end--[common].[ExtensionConfiguration]---------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('D6381DFC-F4D8-499A-9D4D-0D0EAAAC5D32','TOne V2 Suppliers','TOne V2 Suppliers','QM_BE_SourceSupplierReader','{"Editor":"qm-be-sourcesupplierreader-tonev2"}'),('71FAA2F0-E8B5-4E32-9EDF-53C31DB8AB6A','TOne V2 Zones','TOne V2 Zones','QM_BE_SourceZoneReader','{"Editor":"qm-be-sourcezonereader-tonev2"}'),('15FBC0B8-806C-4642-8DA8-580311E16576','TOne V1 Suppliers','TOne V1 Suppliers','QM_BE_SourceSupplierReader','{"Editor":"qm-be-sourcesupplierreader-tonev1"}'),('098E574D-F4F4-4CEB-A03D-5B387A0BE8CB','Connector Zone Info','Connector Zone Info','QM_BE_ConnectorZoneReader','{"Editor":"qm-be-connectorzone-itest"}'),('BB371F00-B036-4745-BDBC-65637F253054','Connector Countries','Connector Countries','VRCommon_SourceCountryReader','{"Editor":"qm-clitester-sourcecountryreader-itest"}'),('F7559C74-4CE9-4F2A-BBB4-6E92F60F0643','Connector Zones','Connector Zones','QM_BE_SourceZoneReader','{"Editor":"qm-clitester-sourcezonereader-itest"}'),('3B68FAE1-C18F-4D15-8C56-74E12A56FD47','TOne V1 Zones','TOne V1 Zones','QM_BE_SourceZoneReader','{"Editor":"qm-be-sourcezonereader-tonev1"}'),('8CEA6378-48FB-4D91-A9B5-9E0C3209321E','Connector Profiles','Connector Profiles','QM_CLITester_SourceProfileReader','{"Editor":"qm-clitester-sourceprofilereader-itest"}'),('37CF73DD-DDF8-4AB5-BD4D-BB13DED9FE7F','Connector VI','Connector VI','QM_CLITester_ConnectorVI','{"Editor":"qm-clitester-testconnector-connectorvi"}'),('613180C1-DBE9-4382-8531-C43FA14DE858','VR_Qm_Clitester_TestCallDetailObjectType','Test Call Detail Information','VR_Common_ObjectType','{"Editor":"vr-qm-clitestertestcalldetailobjecttype", "PropertyEvaluatorExtensionType": "VR_Qm_Clitester_TestCallDetailPropertyEvaluator"}'),('9F336216-BF7A-4E5A-A327-CC19DE2362D5','Initiate Test - Connector','Initiate Test - Connector','QM_CLITester_ConnectorInitiateTest','{"Editor":"qm-clitester-testconnector-initiatetest-itest"}'),('3C2D781D-5089-4D1E-9061-FA5A895AE9A2','VR_Qm_Clitester_TestCallDetailPropertyEvaluator','Test Call Detail Property','VR_Qm_Clitester_TestCallDetailPropertyEvaluator','{"Editor":"vr-qm-clitester-testcalldetailpropertyevaluator"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[ExtensionConfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------endDelete from [runtime].[SchedulerTaskActionType] where Id in ('0A15BC35-A3A7-4ED3-B09B-1B41A7A9DDC9','7A35F562-319B-47B3-8258-EC1A704A82EB') --Exchange Rate, workflow--[runtime].[SchedulerTaskActionType]-------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[OldID],[Name],[ActionTypeInfo])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('D834DF4F-FA44-413F-BC5F-3B9D5CA758F9',205,'Download Test Result','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/TestProgressTemplate.html","SystemType":false,"Editor":"qm-clitester-testprogress","FQTN":"QM.CLITester.Business.TestProgressTaskAction, QM.CLITester.Business","RequiredPermissions":"QM_CLITester_TestCall:l"}'),('DB26E91D-9E36-4398-822C-4A41D868BA50',204,'Initiate Test','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/InitiateTestTemplate.html","SystemType":false,"Editor":"qm-clitester-initiatetest","FQTN":"QM.CLITester.Business.InitiateTestTaskAction, QM.CLITester.Business","RequiredPermissions":"QM_CLITester_TestCall:l"}'),('81212549-83AE-495B-8F34-5D7559165791',203,'Zone Synchronize','{"URL":"/Client/Modules/QM_BusinessEntity/Views/Zone/SchedulerTaskAction/ZoneSynchronizeTemplate.html","SystemType":false,"Editor":"qm-be-sourcezonereader","FQTN":"QM.BusinessEntity.Business.ZoneSyncTaskAction, QM.BusinessEntity.Business","RequiredPermissions":"QM_CLITester_TestCall:l"}'),('7E491110-062D-4AA1-87D9-79656A2880E7',201,'Supplier Synchronize','{"URL":"/Client/Modules/QM_BusinessEntity/Views/Supplier/SchedulerTaskAction/SupplierSynchronizeTemplate.html","SystemType":false,"Editor":"qm-be-sourcesupplierreader","FQTN":"QM.BusinessEntity.Business.SupplierSyncTaskAction, QM.BusinessEntity.Business","RequiredPermissions":"QM_CLITester_TestCall:l"}'),('92D77B93-B9DC-4A28-8FB4-E9C17BCCFB06',202,'Profile Synchronize','{"URL":"/Client/Modules/QM_CLITester/Views/Profile/SchedulerTaskAction/ProfileSynchronizeTemplate.html","SystemType":false,"Editor":"vr-qm-clitester-sourceprofilereader","FQTN":"QM.CLITester.Business.ProfileSyncTaskAction, QM.CLITester.Business","RequiredPermissions":"QM_CLITester_TestCall:l"}'),('A6918DD7-90F2-48EB-A221-F8721F169926',206,'Create Test Call','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/TestCallTemplate.html","SystemType":false,"Editor":"qm-clitester-testcall","FQTN":"QM.CLITester.Business.TestCallTaskAction, QM.CLITester.Business"}'),('6AF4B89D-7809-4221-BB92-FAC2F620C502',207,'Connector Zones Codes','{"URL": "/Client/Modules/QM_BusinessEntity/Views/ConnectorZoneInfo/ConnectorZoneInfoTemplate.html", "SystemType": false, "Editor": "qm-be-connectorzoneinfo", "FQTN": "QM.CLITester.iTestIntegration.VIConnectorSyncTaskAction, QM.CLITester.iTestIntegration", "RequiredPermissions": "QM_CLITester_TestCall:l"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldID],[Name],[ActionTypeInfo]))merge	[runtime].[SchedulerTaskActionType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldID] = s.[OldID],[Name] = s.[Name],[ActionTypeInfo] = s.[ActionTypeInfo]when not matched by target then	insert([ID],[OldID],[Name],[ActionTypeInfo])	values(s.[ID],s.[OldID],s.[Name],s.[ActionTypeInfo]);--update permission to existing users.GO
update	p
set 	p.EntityId= bem.Id
from	[sec].[Permission] p
		inner join [sec].[BusinessEntityModule] bem on p.EntityId=bem.OldId
where	ISNUMERIC(p.EntityId)>0 and p.EntityType=0
GO
update	p
set 	p.EntityId= bem.Id
from	[sec].[Permission] p
		inner join [sec].[BusinessEntity] bem on p.EntityId=bem.OldId
where	ISNUMERIC(p.EntityId)>0 and p.EntityType=1
GO