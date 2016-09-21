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
end


GO--delete useless views from ClearVoice product such 'My Scheduler Service' since it is replace with 'Schedule Test Calls', 'Organizational Charts', etc...
delete from [sec].[View] where [Id] in (7,1002,1003,1004,1005,1008,5002)
GO
--------------------------------------------------------------------------------------------------------------


--[sec].[View]-----------------------------10001 to 11000------------------------------------------------------
begin
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
---------------------------------------------------------------------------------------------------------------

end


--[sec].[BusinessEntityModule]-------------1001 to 1100---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------

--[sec].[BusinessEntity]------------------2701 to 3000----------------------------------------------------------
begin
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
----------------------------------------------------------------------------------------------------------------
end


--[sec].[SystemAction]------------------------------------------------------------------------------
begin
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
----------------------------------------------------------------------------------------------------
end


GO--delete useless [Setting] from ClearVoice product such 'System Currency', etc...
delete from [common].[Setting] where [Id] in (2)
GO
--[common].[Setting]--------------------------------------------------------------------------------beginset nocount on;set identity_insert [common].[Setting] on;;with cte_data([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(301,'Keep history in Test Calls Page','QM_CLITester_LastTestCall_Settings','General','{"Editor":"vr-qm-clitester-lasttestcall-settings-editor"}','{"$type":"QM.CLITester.Entities.LastTestCallsSettingsData, QM.CLITester.Entities","LastTestCall":60}',0),(302,'Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"Clear Voice","VersionNumber":"version 0.9"}}',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))merge	[common].[Setting] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]when not matched by target then	insert([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])	values(s.[Id],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);set identity_insert [common].[Setting] off;
----------------------------------------------------------------------------------------------------end
--[common].[VRObjectTypeDefinition]---------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('7B6B09AF-D2F8-47E0-B4FD-187A8075D4BB','TestCall','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultObjectType, QM.CLITester.Entities","ConfigId":3004},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Supplier Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Supplier Name","Description":"Supplier Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":0,"ConfigId":3005}},"User Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"User Name","Description":"User Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":1,"ConfigId":3005}},"Country Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Country Name","Description":"Country Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":2,"ConfigId":3005}},"Zone Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Zone Name","Description":"Zone Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":3,"ConfigId":3005}},"Call Test Status Description":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Call Test Status Description","Description":"Call Test Status Description","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":4,"ConfigId":3005}},"Call Test Result Description":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Call Test Result Description","Description":"Call Test Result Description","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":5,"ConfigId":3005}},"Schedule Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Schedule Name","Description":"Schedule Name","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":6,"ConfigId":3005}},"Pdd":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Pdd","Description":"Pdd","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":7,"ConfigId":3005}},"Mos":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Mos","Description":"Mos","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":8,"ConfigId":3005}},"Creation Date":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Creation Date","Description":"Creation Date","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":9,"ConfigId":3005}},"Source":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Source","Description":"Source","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":10,"ConfigId":3005}},"Destination":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Destination","Description":"Destination","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":11,"ConfigId":3005}},"Received Cli":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Received Cli","Description":"Received Cli","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":12,"ConfigId":3005}},"Ring Duration":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Ring Duration","Description":"Ring Duration","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":13,"ConfigId":3005}},"Release Code":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Release Code","Description":"Release Code","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":15,"ConfigId":3005}},"To Mail":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"To Mail","Description":"To Mail","PropertyEvaluator":{"$type":"QM.CLITester.Entities.VRObjectTypes.TestResultPropertyEvaluator, QM.CLITester.Entities","TestCallDetailField":16,"ConfigId":3005}}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[VRObjectTypeDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);

--[common].[MailMessageType]------------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('D87DD73B-B143-486B-AFE9-303F8D4C90AC','Test Call','{"$type":"Vanrise.Entities.VRMailMessageTypeSettings, Vanrise.Entities","Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","SendTestCallResultObjList":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"SendTestCallResultObjList","VRObjectTypeDefinitionId":"7b6b09af-d2f8-47e0-b4fd-187a8075d4bb"},"Product":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Product","VRObjectTypeDefinitionId":"62b9a4da-0018-4514-bcfd-8268a58f53a2"}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[MailMessageType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);----------------------------------------------------------------------------------------------------end

--[common].[MailMessageTemplate]--------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[MessageTypeID],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('019A4F17-CDDC-47FC-B0E0-4F65473E173D','Test Call','D87DD73B-B143-486B-AFE9-303F8D4C90AC','{"$type":"Vanrise.Entities.VRMailMessageTemplateSettings, Vanrise.Entities","To":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"SendTestCallResultObjList\",\"To Mail\")"},"CC":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities"},"Subject":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"@Model.GetVal(\"Product\",\"Product Name\"): Test call summary for supplier: @Model.GetVal(\"SendTestCallResultObjList\",\"Supplier Name\") on Country: @Model.GetVal(\"SendTestCallResultObjList\",\"Country Name\")"},"Body":{"$type":"Vanrise.Entities.VRExpression, Vanrise.Entities","ExpressionString":"Dears,\n<div>Kindly find below Test call summary</div>\n<br />Supplier: @Model.GetVal(\"SendTestCallResultObjList\",\"Supplier Name\")\n<br />Country: @Model.GetVal(\"SendTestCallResultObjList\",\"Country Name\")\n<br />Zone: @Model.GetVal(\"SendTestCallResultObjList\",\"Zone Name\")\n<br />Test Date: @Model.GetVal(\"SendTestCallResultObjList\",\"Creation Date\")\n<br /><br />\n<table style=\"border-collapse: collapse;\" border=\"1\" cellpadding=\"10\">\n<tr>\n    <th>Source</th>\n    <th>Destination </th>\n    <th>Received Cli </th>\n    <th>Ring Duration </th>\n    <th>Release Code </th>\n    <th>Pdd </th>\n    <th>Mos </th>\n    <th>Call Result </th>\n</tr>\n<!--repeated section-->\n@if (Model.GetVal(\"SendTestCallResultObjList\",\"Source\") != null)\n{\nvar  source= Model.GetVal(\"SendTestCallResultObjList\",\"Source\").ToString();\nvar sourceSplit= source.Split('';'');\n\nvar destination= Model.GetVal(\"SendTestCallResultObjList\",\"Destination\").ToString();\nstring[] destinationSplit= destination.Split('';'');\n\nvar receivedCli= Model.GetVal(\"SendTestCallResultObjList\",\"Received Cli\").ToString();\nstring[] receivedCliSplit= receivedCli.Split('';'');\n\nvar ringDuration= Model.GetVal(\"SendTestCallResultObjList\",\"Ring Duration\").ToString();\nstring[] ringDurationSplit= ringDuration.Split('';'');\n\nvar releaseCode= Model.GetVal(\"SendTestCallResultObjList\",\"Release Code\").ToString();\nstring[] releaseCodeSplit= releaseCode.Split('';'');\n\nvar Pdd= Model.GetVal(\"SendTestCallResultObjList\",\"Pdd\").ToString();\nstring[] pddSplit= Pdd.Split('';'');\n\nvar Mos= Model.GetVal(\"SendTestCallResultObjList\",\"Mos\").ToString();\nstring[] mosSplit= Mos.Split('';'');\n\nvar testResult= Model.GetVal(\"SendTestCallResultObjList\",\"Call Test Result Description\").ToString();\nstring[] testResultSplit= testResult.Split('';'');\n\n\nint i = 0;\nforeach (var s in sourceSplit) {\nif (s != \"\"){\n\n<tr>\n    <td>@s </td>\n    <td>@destinationSplit[i] </td>\n    <td>@receivedCliSplit[i] </td>\n    <td>@ringDurationSplit[i] </td>\n    <td>@releaseCodeSplit[i] </td>\n    <td>@pddSplit[i] </td>\n    <td>@mosSplit[i] </td>\n    <td>@testResultSplit[i] </td>\n</tr>\n\n} i++;}\n\n}\n<!--end repeated section-->\n</table>\n\n<br />\n<div>Regards;</div>\n<div>@Model.GetVal(\"SendTestCallResultObjList\",\"User Name\")</div>"}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[MessageTypeID],[Settings]))merge	[common].[MailMessageTemplate] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[MessageTypeID] = s.[MessageTypeID],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[MessageTypeID],[Settings])	values(s.[ID],s.[Name],s.[MessageTypeID],s.[Settings]);----------------------------------------------------------------------------------------------------end--common.ExtensionConfiguration---------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('D6381DFC-F4D8-499A-9D4D-0D0EAAAC5D32','TOne V2 Suppliers','TOne V2 Suppliers','QM_BE_SourceSupplierReader','{"Editor":"qm-be-sourcesupplierreader-tonev2"}'),('71FAA2F0-E8B5-4E32-9EDF-53C31DB8AB6A','TOne V2 Zones','TOne V2 Zones','QM_BE_SourceZoneReader','{"Editor":"qm-be-sourcezonereader-tonev2"}'),('15FBC0B8-806C-4642-8DA8-580311E16576','TOne V1 Suppliers','TOne V1 Suppliers','QM_BE_SourceSupplierReader','{"Editor":"qm-be-sourcesupplierreader-tonev1"}'),('098E574D-F4F4-4CEB-A03D-5B387A0BE8CB','Connector Zone Info','Connector Zone Info','QM_BE_ConnectorZoneReader','{"Editor":"qm-be-connectorzone-itest"}'),('BB371F00-B036-4745-BDBC-65637F253054','Connector Countries','Connector Countries','VRCommon_SourceCountryReader','{"Editor":"qm-clitester-sourcecountryreader-itest"}'),('F7559C74-4CE9-4F2A-BBB4-6E92F60F0643','Connector Zones','Connector Zones','QM_BE_SourceZoneReader','{"Editor":"qm-clitester-sourcezonereader-itest"}'),('3B68FAE1-C18F-4D15-8C56-74E12A56FD47','TOne V1 Zones','TOne V1 Zones','QM_BE_SourceZoneReader','{"Editor":"qm-be-sourcezonereader-tonev1"}'),('8CEA6378-48FB-4D91-A9B5-9E0C3209321E','Connector Profiles','Connector Profiles','QM_CLITester_SourceProfileReader','{"Editor":"qm-clitester-sourceprofilereader-itest"}'),('37CF73DD-DDF8-4AB5-BD4D-BB13DED9FE7F','Test Progress - Connector','Test Progress - Connector','QM_CLITester_ConnectorTestProgress','{"Editor":"qm-clitester-testconnector-testprogress-itest"}'),('613180C1-DBE9-4382-8531-C43FA14DE858','VR_Qm_Clitester_TestCallDetailObjectType','Test Call Detail Information','VR_Common_ObjectType','{"Editor":"vr-qm-clitestertestcalldetailobjecttype", "PropertyEvaluatorExtensionType": "VR_Qm_Clitester_TestCallDetailPropertyEvaluator"}'),('9F336216-BF7A-4E5A-A327-CC19DE2362D5','Initiate Test - Connector','Initiate Test - Connector','QM_CLITester_ConnectorInitiateTest','{"Editor":"qm-clitester-testconnector-initiatetest-itest"}'),('3C2D781D-5089-4D1E-9061-FA5A895AE9A2','VR_Qm_Clitester_TestCallDetailPropertyEvaluator','Test Call Detail Property','VR_Qm_Clitester_TestCallDetailPropertyEvaluator','{"Editor":"vr-qm-clitester-testcalldetailpropertyevaluator"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[ExtensionConfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end