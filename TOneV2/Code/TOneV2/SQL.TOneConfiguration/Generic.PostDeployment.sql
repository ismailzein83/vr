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
--[sec].[Module]---------------------------301 to 400---------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','Generic Data',null,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,2,0)
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
--------------------------------------------------------------------------------------------------------------
end

--[sec].[viewtype]-----------------------101 to 200-------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(101,'VR_GenericData_GenericRule','Generic Rule','{"ViewTypeId":101,"Name":"VR_GenericData_GenericRule","Title":"Generic Rule","Editor":"/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionEditor.html","EnableAdd":false}'),
(102,'VR_GenericData_GenericBusinessEntity','Business Entity','{"ViewTypeId":102,"Name":"VR_GenericData_GenericBusinessEntity","Title":"BusinessEntity","Editor":"/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html","EnableAdd":false}'),
(103,'VR_GenericData_RecordSearch','Record Search','{"ViewTypeId":103,"Name":"VR_GenericData_RecordSearch","Title":"Record Search","Editor":"/Client/Modules/VR_GenericData/Views/DataRecordStorage/DataRecordStorageSettingsEditor.html","EnableAdd":true}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Details]))
merge	[sec].[viewtype] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Title],[Details])
	values(s.[ID],s.[Name],s.[Title],s.[Details]);
----------------------------------------------------------------------------------------------------
end

--[sec].[View]-----------------------------3001 to 4000-------------------------------------------------------
begin

set nocount on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1E9577A9-EF49-475B-B32C-585A26063B04','Data Record Types','Data Record Type','#/view/VR_GenericData/Views/GenericDataRecord/DataRecordTypeManagement','4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/DataRecordType/GetFilteredDataRecordTypes',null,null,null,0,2),
('A3219A0D-87BA-4CA9-A9A1-BB1FB2BD732F','Generic Rule Definitions','Generic Rule Definition','#/view/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionManagement','4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/GenericRuleDefinition/GetFilteredGenericRuleDefinitions',null,null,null,0,3),
('F9137B44-C362-4823-B2A3-6EF9E5C40430','Data Transformation Definitions','Data Transformation Definition','#/view/VR_GenericData/Views/DataTransformationDefinition/DataTransformationDefinitionManagement','4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/DataTransformationDefinition/GetFilteredDataTransformationDefinitions',null,null,null,0,4),
('651DD50E-9CBB-49F5-8CA4-F3FDDBCB9C24','Data Stores','Data Store','#/view/VR_GenericData/Views/DataStore/DataStoreManagement','4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/DataStore/GetFilteredDataStores',null,null,null,0,5),
('47318833-AFE8-414F-A61D-F3A81727FD5B','Data Record Storages','Data Record Storage','#/view/VR_GenericData/Views/DataRecordStorage/DataRecordStorageManagement','4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/DataRecordStorage/GetFilteredDataRecordStorages',null,null,null,0,6),
('0CDB7EDF-3907-4EEC-B781-CF1E19A44C92','Business Entity Definitions','Business Entity Definitions','#/view/VR_GenericData/Views/GenericBusinessEntity/Definition/GenericBEDefinitionManagement','4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/BusinessEntityDefinition/GetFilteredBusinessEntityDefinitions',null,null,null,0,7),
('CA6F551E-8275-49EF-98EB-11DF3EA6BE98','Summary Transformation Definition','Summary Transformation Definition','#/view/VR_GenericData/Views/SummaryTransformationDefinition/SummaryTransformationDefinitionManagement','4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/SummaryTransformationDefinition/GetFilteredSummaryTransformationDefinitions',null,null,null,0,8),
('4564B29F-FCBB-4E49-BB2E-C84076BE8EF1','BE Lookup Rule Definition','BE Lookup Rule Definition Management','#/view/VR_GenericData/Views/BELookupRuleDefinition/BELookupRuleDefinitionManagement','4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/BELookupRuleDefinition/GetFilteredBELookupRuleDefinitions',null,null,null,0,9),
('044F280C-F049-4A4E-A8FC-530AA313FDFF','Data Record Field Choice','Data Record Field Choice','#/view/VR_GenericData/Views/DataRecordFieldChoice/DataRecordFieldChoiceManagement','4D5BE8B1-4E8D-414A-8A23-BB3DD17E35C7','VR_GenericData/DataRecordFieldChoice/GetFilteredDataRecordFieldChoices',null,null,null,0,10)
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
--------------------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntityModule]-------------301 to 400---------------------------------------------------------
begin
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(301,'Generic Data',-1,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([Id],[Name],[ParentId],[BreakInheritance])
	values(s.[Id],s.[Name],s.[ParentId],s.[BreakInheritance]);
set identity_insert [sec].[BusinessEntityModule] off;
--------------------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]-------------------601 to 900--------------------------------------------------------
begin
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(601,'VR_GenericData_BusinessEntityDefinition','Business Entity Definitions',301,0,'["View","Add","Edit"]'),
(602,'VR_GenericData_GenericRuleDefinition','Generic Rule Definition',301,0,'["View","Add","Edit"]'),
(603,'VR_GenericData_DataTransformationDefinition','Data Transformation Definition',301,0,'["View","Add","Edit","Compile"]'),
(604,'VR_GenericData_ExtensibleBEItem','Extensible BE Item',301,0,'["Add","Edit"]'),
(605,'VR_GenericData_DataRecordType','Data Record Type',301,0,'["View","Add","Edit"]'),
(606,'VR_GenericData_DataStore','Data Store',301,0,'["View","Add","Edit"]'),
(607,'VR_GenericData_DataRecordStorage','Data Record Storage',301,0,'["View","Add","Edit"]'),
(608,'VR_GenericData_SummaryTransformationDefinition','Summary Transformation Definition',301,0,'["View","Add","Edit"]'),
(609,'VR_GenericData_BELookupRuleDefinition','BE Lookup Rule Definition',301,0,'["View","Add","Edit"]'),
(610,'VR_GenericData_DataRecordFieldChoice','VR_GenericData_DataRecordFieldChoice',301,0,'["View","Add","Edit"]')
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
--------------------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]----------------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_GenericData/BusinessEntityDefinition/GetFilteredBusinessEntityDefinitions','VR_GenericData_BusinessEntityDefinition: View'),
('VR_GenericData/BusinessEntityDefinition/GetBusinessEntityDefinition',null),
('VR_GenericData/BusinessEntityDefinition/GetBusinessEntityDefinitionsInfo',null),
('VR_GenericData/BusinessEntityDefinition/UpdateBusinessEntityDefinition','VR_GenericData_BusinessEntityDefinition: Edit'),
('VR_GenericData/BusinessEntityDefinition/AddBusinessEntityDefinition','VR_GenericData_BusinessEntityDefinition: Add'),
('VR_GenericData/BusinessEntityDefinition/GetGenericBEDefinitionView',null),
('VR_GenericData/DataRecordFieldTypeConfig/GetDataRecordFieldTypes',null),
('VR_GenericData/DataRecordFieldTypeConfig/GetDataRecordFieldTypeConfig',null),
('VR_GenericData/DataRecordStorage/GetFilteredDataRecordStorages','VR_GenericData_DataRecordStorage: View'),
('VR_GenericData/DataRecordStorage/GetDataRecordsStorageInfo',null),
('VR_GenericData/DataRecordStorage/GetDataRecordStorage',null),
('VR_GenericData/DataRecordStorage/AddDataRecordStorage','VR_GenericData_DataRecordStorage: Add'),
('VR_GenericData/DataRecordStorage/UpdateDataRecordStorage','VR_GenericData_DataRecordStorage: Edit'),
('VR_GenericData/DataRecordType/GetDataRecordType',null),
('VR_GenericData/DataRecordType/GetFilteredDataRecordTypes','VR_GenericData_DataRecordType: View'),
('VR_GenericData/DataRecordType/UpdateDataRecordType','VR_GenericData_DataRecordType: Edit'),
('VR_GenericData/DataRecordType/AddDataRecordType','VR_GenericData_DataRecordType: Add'),
('VR_GenericData/DataRecordType/GetDataRecordFieldTypeTemplates',null),
('VR_GenericData/DataRecordType/GetDataRecordTypeInfo',null),
('VR_GenericData/DataStoreConfig/GetDataStoreConfigs',null),
('VR_GenericData/DataStoreConfig/GetDataStoreConfig',null),
('VR_GenericData/DataStore/GetFilteredDataStores','VR_GenericData_DataStore: View'),
('VR_GenericData/DataStore/GetDataStoresInfo',null),
('VR_GenericData/DataStore/GetDataStore',null),
('VR_GenericData/DataStore/AddDataStore','VR_GenericData_DataStore: Add'),
('VR_GenericData/DataStore/UpdateDataStore','VR_GenericData_DataStore: Edit'),
('VR_GenericData/DataTransformationDefinition/GetDataTransformationDefinition',null),
('VR_GenericData/DataTransformationDefinition/GetDataTransformationDefinitionRecords',null),
('VR_GenericData/DataTransformationDefinition/GetFilteredDataTransformationDefinitions','VR_GenericData_DataTransformationDefinition: View'),
('VR_GenericData/DataTransformationDefinition/UpdateDataTransformationDefinition','VR_GenericData_DataTransformationDefinition: Edit'),
('VR_GenericData/DataTransformationDefinition/AddDataTransformationDefinition','VR_GenericData_DataTransformationDefinition: Add'),
('VR_GenericData/DataTransformationDefinition/GetDataTransformationDefinitions',null),
('VR_GenericData/DataTransformationDefinition/TryCompileSteps','VR_GenericData_DataTransformationDefinition: Compile'),
('VR_GenericData/DataTransformationDefinition/ExportCompilationResult','VR_GenericData_DataTransformationDefinition: Compile'),
('VR_GenericData/DataTransformationStepConfig/GetDataTransformationSteps',null),
('VR_GenericData/ExpressionBuilderConfig/GetExpressionBuilderTemplates',null),
('VR_GenericData/ExtensibleBEItem/GetExtensibleBEItem',null),
('VR_GenericData/ExtensibleBEItem/UpdateExtensibleBEItem','VR_GenericData_ExtensibleBEItem: Edit'),
('VR_GenericData/ExtensibleBEItem/AddExtensibleBEItem','VR_GenericData_ExtensibleBEItem: Add'),
('VR_GenericData/ExtensibleBEItem/GetFilteredExtensibleBEItems',null),
('VR_GenericData/GenericBusinessEntity/GetFilteredGenericBusinessEntities',null),
('VR_GenericData/GenericBusinessEntity/GetGenericBusinessEntity',null),
('VR_GenericData/GenericBusinessEntity/AddGenericBusinessEntity',null),
('VR_GenericData/GenericBusinessEntity/UpdateGenericBusinessEntity',null),
('VR_GenericData/GenericBusinessEntity/GetGenericBusinessEntityInfo',null),
('VR_GenericData/GenericBusinessEntity/GetBusinessEntityTitle',null),
('VR_GenericData/GenericBusinessEntity/DeleteGenericBusinessEntity',null),
('VR_GenericData/GenericRule/GetFilteredGenericRules',null),
('VR_GenericData/GenericRule/GetGenericRule',null),
('VR_GenericData/GenericRule/AddGenericRule',null),
('VR_GenericData/GenericRule/UpdateGenericRule',null),
('VR_GenericData/GenericRuleDefinition/GetFilteredGenericRuleDefinitions','VR_GenericData_GenericRuleDefinition: View '),
('VR_GenericData/GenericRuleDefinition/GetGenericRuleDefinition',null),
('VR_GenericData/GenericRuleDefinition/AddGenericRuleDefinition','VR_GenericData_GenericRuleDefinition: Add'),
('VR_GenericData/GenericRuleDefinition/UpdateGenericRuleDefinition','VR_GenericData_GenericRuleDefinition: Edit'),
('VR_GenericData/GenericRuleDefinition/GetGenericRuleDefinitionsInfo',null),
('VR_GenericData/GenericRuleDefinition/GetGenericRuleDefinitionView',null),
('VR_GenericData/GenericRuleTypeConfig/GetGenericRuleTypes',null),
('VR_GenericData/GenericRuleTypeConfig/GetGenericRuleTypeByName',null),
('VR_GenericData/GenericRuleTypeConfig/GetGenericRuleTypeById',null),
('VR_GenericData/GenericUIRuntime/GetExtensibleBEItemRuntime',null),
('VR_GenericData/GenericUIRuntime/GetGenericManagementRuntime',null),
('VR_GenericData/GenericUIRuntime/GetGenericEditorRuntime',null),
('VR_GenericData/GenericUIRuntime/GetDataRecordTypesInfo',null),
('VR_GenericData/SummaryTransformationDefinition/GetSummaryTransformationDefinition',null),
('VR_GenericData/SummaryTransformationDefinition/GetFilteredSummaryTransformationDefinitions','VR_GenericData_SummaryTransformationDefinition: View'),
('VR_GenericData/SummaryTransformationDefinition/AddSummaryTransformationDefinition','VR_GenericData_SummaryTransformationDefinition: Add'),
('VR_GenericData/SummaryTransformationDefinition/UpdateSummaryTransformationDefinition','VR_GenericData_SummaryTransformationDefinition: Edit'),
('VR_GenericData/SummaryTransformationDefinition/GetSummaryTransformationDefinitionInfo',null),
('VR_GenericData/SummaryTransformationDefinition/GetSummaryBatchIntervalSourceTemplates',null),

('VR_GenericData/BELookupRuleDefinition/GetFilteredBELookupRuleDefinitions','VR_GenericData_BELookupRuleDefinition: View'),
('VR_GenericData/BELookupRuleDefinition/GetBELookupRuleDefinitionsInfo','VR_GenericData_BELookupRuleDefinition: View'),
('VR_GenericData/BELookupRuleDefinition/GetBELookupRuleDefinition','VR_GenericData_BELookupRuleDefinition: View'),
('VR_GenericData/BELookupRuleDefinition/AddBELookupRuleDefinition','VR_GenericData_BELookupRuleDefinition: Add'),
('VR_GenericData/BELookupRuleDefinition/UpdateBELookupRuleDefinition','VR_GenericData_BELookupRuleDefinition: Edit'),

('VR_GenericData/DataRecordFieldChoice/GetFilteredDataRecordFieldChoices','VR_GenericData_DataRecordFieldChoice: View'),
('VR_GenericData/DataRecordFieldChoice/GetDataRecordFieldChoicesInfo','VR_GenericData_DataRecordFieldChoice: View'),
('VR_GenericData/DataRecordFieldChoice/GetDataRecordFieldChoice','VR_GenericData_DataRecordFieldChoice: View'),
('VR_GenericData/DataRecordFieldChoice/AddDataRecordFieldChoice','VR_GenericData_DataRecordFieldChoice: Add'),
('VR_GenericData/DataRecordFieldChoice/UpdateDataRecordFieldChoice','VR_GenericData_DataRecordFieldChoice: Edit')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);
--------------------------------------------------------------------------------------------------------------
end

--[genericdata].[GenericRuleTypeConfig]-------------------------------------------------------------
begin
set nocount on;
set identity_insert [genericdata].[GenericRuleTypeConfig] on;
;with cte_data([ID],[Name],[Title],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'VR_MappingRule','Mapping Rule','{"GenericRuleTypeConfigId":"1","Name":"GenericRuleMapping","Title":"Mapping","Editor":"vr-genericdata-genericruledefinitionsettings-mapping","RuntimeEditor":"vr-genericdata-genericrulesettings-mapping-runtimeeditor","RuleTypeFQTN":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","RuleManagerFQTN":"Vanrise.GenericData.Transformation.MappingRuleManager, Vanrise.GenericData.Transformation","FilterEditor":"vr-genericdata-genericrulesettings-mapping-filtereditor"}'),
(2,'VR_NormalizationRule','Normalization Rule','{"GenericRuleTypeConfigId":"2","Name":"VR_NormalizationRule","Title":"Normalization Rule","Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-rules-normalizenumbersettings", "RuleTypeFQTN":"Vanrise.GenericData.Normalization.NormalizationRule, Vanrise.GenericData.Normalization", "RuleManagerFQTN":"Vanrise.GenericData.Normalization.NormalizationRuleManager, Vanrise.GenericData.Normalization"}'),
(3,'VR_RateTypeRule','Rate Type Rule','{"GenericRuleTypeConfigId":"3","Name":"VR_RateTypeRule","Title":"Rate Type Rule","Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-rules-pricingrulesettings-ratetype", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.RateTypeRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.RateTypeRuleManager, Vanrise.GenericData.Pricing"}'),
(4,'VR_TariffRule','Tariff Rule','{"GenericRuleTypeConfigId":"4","Name":"VR_TariffRule","Title":"Tariff Rule","Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-rules-pricingrulesettings-tariff", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.TariffRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.TariffRuleManager, Vanrise.GenericData.Pricing"}'),
(5,'VR_ExtraChargeRule','Extra Charge Rule','{"GenericRuleTypeConfigId":"5","Name":"VR_ExtraChargeRule","Title":"Extra Charge Rule","Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-rules-pricingrulesettings-extracharge", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.ExtraChargeRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.ExtraChargeRuleManager, Vanrise.GenericData.Pricing"}'),
(6,'VR_RateValueRule','Rate Value Rule','{"GenericRuleTypeConfigId":"6","Name":"VR_RateValueRule","Title":"Rate Value Rule","Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-rules-pricingrulesettings-ratevalue", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.RateValueRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.RateValueRuleManager, Vanrise.GenericData.Pricing"}'),
(8,'VR_BalanceAlert','Balance Alert Rule','{"GenericRuleTypeConfigId":"8","Name":"VR_BalanceAlert","Title":"Balance Alert Rule","Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-accountbalance-balancealertrulesettings", "RuleTypeFQTN":"Vanrise.AccountBalance.Entities.BalanceAlertRule, Vanrise.AccountBalance.Entities", "RuleManagerFQTN":" Vanrise.AccountBalance.Business.BalanceAlertRuleManager, Vanrise.AccountBalance.Business"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Details]))
merge	[genericdata].[GenericRuleTypeConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name], [Title] = s.[Title],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Title],[Details])
	values(s.[ID],s.[Name],[Title],s.[Details]);
set identity_insert [genericdata].[GenericRuleTypeConfig] off;
----------------------------------------------------------------------------------------------------
end

--[genericdata].[ExpressionBuilderConfig]-----------------------------------------------------------
begin
set nocount on;
set identity_insert [genericdata].[ExpressionBuilderConfig] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'RecordValue','{"ExpressionBuilderConfigId":"1","Name":"RecordValue","Title":"Record Value","Editor":"vr-genericdata-expressionbuilder-recordvalue"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[genericdata].[ExpressionBuilderConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);
set identity_insert [genericdata].[ExpressionBuilderConfig] off;
----------------------------------------------------------------------------------------------------
end

--[genericdata].[DataTransformationStepConfig]------------------------------------------------------
begin
set nocount on;
set identity_insert [genericdata].[DataTransformationStepConfig] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'AssignFieldStep','{"DataTransformationStepConfigId":"1","Name":"AssignFieldStep","Title":"Assign Value","Editor":"vr-genericdata-datatransformation-assignfieldstep","StepPreviewUIControl":"vr-genericdata-datatransformation-assignfieldstep-preview"}'),
(2,'AssignValueruleStep','{"DataTransformationStepConfigId":"2","Name":"AssignValueruleStep","Title":"Assign Value using Rule","Editor":"vr-genericdata-datatransformation-assignvaluerulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-assignvaluerulestep-preview"}'),
(3,'NormalizationRuleStep','{"DataTransformationStepConfigId":"3","Name":"NormalizationRuleStep","Title":"Normalization Rule","Editor":"vr-genericdata-datatransformation-normalizationrulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-normalizationrulestep-preview"}'),
(4,'RateValueRuleStep','{"DataTransformationStepConfigId":"4","Name":"RateValueRuleStep","Title":"Rate Value Rule","Editor":"vr-genericdata-datatransformation-ratevaluerulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-ratevaluerulestep-preview"}'),
(5,'RateTypeRuleStep','{"DataTransformationStepConfigId":"5","Name":"RateTypeRuleStep","Title":"Rate Type Rule","Editor":"vr-genericdata-datatransformation-ratetyperulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-ratetyperulestep-preview"}'),
(6,'ForLoopStep','{"DataTransformationStepConfigId":"6","Name":"ForLoopStep","Title":"For Loop","Editor":"vr-genericdata-datatransformation-forloopstep","StepPreviewUIControl":"vr-genericdata-datatransformation-forloopstep-preview"}'),
(7,'InitializeRecordStep','{"DataTransformationStepConfigId":"7","Name":"InitializeRecordStep","Title":"Initialize Record","Editor":"vr-genericdata-datatransformation-initializerecordstep","StepPreviewUIControl":"vr-genericdata-datatransformation-initializerecordstep-preview"}'),
(8,'AddItemToArrayStep','{"DataTransformationStepConfigId":"8","Name":"AddItemToArrayStep","Title":"Add Item To Array","Editor":"vr-genericdata-datatransformation-additemtoarraystep","StepPreviewUIControl":"vr-genericdata-datatransformation-additemtoarraystep-preview"}'),
(9,'IfElseStep','{"DataTransformationStepConfigId":"9","Name":"IfElseStep","Title":"If Else","Editor":"vr-genericdata-datatransformation-ifelsestep","StepPreviewUIControl":"vr-genericdata-datatransformation-ifelsestep-preview"}'),
(10,'ExtraChargeRuleStep','{"DataTransformationStepConfigId":"10","Name":"ExtraChargeRuleStep","Title":"Extra Charge Rule","Editor":"vr-genericdata-datatransformation-extrachargerulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-extrachargerulestep-preview"}'),
(11,'TariffRule','{"DataTransformationStepConfigId":"11","Name":"TariffRule","Title":"Tariff Rule","Editor":"vr-genericdata-datatransformation-tariffrulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-tariffrulestep-preview"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[genericdata].[DataTransformationStepConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);
set identity_insert [genericdata].[DataTransformationStepConfig] off;
----------------------------------------------------------------------------------------------------
end

--[genericdata].[DataStoreConfig]-------------------------------------------------------------------
begin

set nocount on;
set identity_insert [genericdata].[DataStoreConfig] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'SQL',' {"DataStoreConfigId":1,"Name":"SQL","Title":"SQL","Editor":"vr-genericdata-datastoresetting-sql","DataRecordSettingsEditor":"vr-genericdata-datarecordstoragesettings-sql"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[genericdata].[DataStoreConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);
set identity_insert [genericdata].[DataStoreConfig] off;
----------------------------------------------------------------------------------------------------
end

--[genericdata].[DataRecordFieldTypeConfig]---------------------------------------------------------
begin
set nocount on;
set identity_insert [genericdata].[DataRecordFieldTypeConfig] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Text','{"DataRecordFieldTypeConfigId":"1","Name":"Text","Title":"Text","Editor":"vr-genericdata-fieldtype-text","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-text-filtereditor","RuleFilterEditor":"vr-genericdata-fieldtype-text-rulefiltereditor"}'),
(2,'Number','{"DataRecordFieldTypeConfigId":"2","Name":"Number","Title":"Number","Editor":"vr-genericdata-fieldtype-number","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-number-filtereditor","RuleFilterEditor":"vr-genericdata-fieldtype-number-rulefiltereditor"}'),
(3,'DateTime','{"DataRecordFieldTypeConfigId":"3","Name":"DateTime","Title":"DateTime","Editor":"vr-genericdata-fieldtype-datetime","RuleFilterEditor":"vr-genericdata-fieldtype-datetime-rulefiltereditor"}'),
(6,'Choices','{"DataRecordFieldTypeConfigId":"6","Name":"Choices","Title":"Choices","Editor":"vr-genericdata-fieldtype-choices","RuntimeEditor":"vr-genericdata-fieldtype-choices-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-choices-filtereditor","RuleFilterEditor":"vr-genericdata-fieldtype-choices-rulefiltereditor"}'),
(7,'Boolean','{"DataRecordFieldTypeConfigId":"7","Name":"Boolean","Title":"Boolean","Editor":"vr-genericdata-fieldtype-boolean","RuleFilterEditor":"vr-genericdata-fieldtype-boolean-rulefiltereditor","RuntimeEditor":"vr-genericdata-fieldtype-boolean-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-boolean-filtereditor"}'),
(9,'Business Entity','{"DataRecordFieldTypeConfigId":"9","Name":"Business Entity","Title":"Business Entity","Editor":"vr-genericdata-fieldtype-businessentity","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-businessentity-filtereditor","RuleFilterEditor":"vr-genericdata-fieldtype-businessentity-rulefiltereditor"}'),
(10,'Array','{"DataRecordFieldTypeConfigId":"10","Name":"Array","Title":"Array","Editor":"vr-genericdata-fieldtype-array","RuntimeEditor":"vr-genericdata-fieldtype-array-runtimeeditor", "FilterEditor": "vr-genericdata-fieldtype-array-filtereditor"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[genericdata].[DataRecordFieldTypeConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details])
when not matched by source then
	delete;
set identity_insert [genericdata].[DataRecordFieldTypeConfig] off;
----------------------------------------------------------------------------------------------------
end

--[queue].[QueueActivatorConfig]----------------1 to 500--------------------------------------------
begin
set nocount on;
set identity_insert [queue].[QueueActivatorConfig] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Store Batch Queue Activator','{ "QueueActivatorConfigId": "2" , "Name": "Store Batch Queue Activator" ,"Title" : "Store Batch Queue Activator", "Editor" :"vr-genericdata-queueactivator-storebatch"}'),
(2,'Transform Batch Queue Activator','{ "QueueActivatorConfigId": "2" , "Name": "Transform  Batch Queue Activator" ,"Title" : "Transform  Batch Queue Activator", "Editor" :"vr-genericdata-queueactivator-transformbatch"}'),
(3,'Custom Activator','{ "QueueActivatorConfigId": "7" , "Name": "Custom Activator" ,"Title" : "Custom Activator", "Editor" :"vr-queueing-queueactivator-customactivator"}'),
(4,'Update Summary Queue Activator','{ "QueueActivatorConfigId": "3" , "Name": "Update Summary Queue Activator" ,"Title" : "Update Summary Queue Activator", "Editor" :"vr-genericdata-queueactivator-updatesummary"}'),
(5,'Generate Summary Queue Activator','{ "QueueActivatorConfigId": "4" , "Name": "Generate Summary Queue Activator" ,"Title" : "Generate Summary Queue Activator", "Editor" :"vr-genericdata-queueactivator-generatesummary"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[queue].[QueueActivatorConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);
set identity_insert [queue].[QueueActivatorConfig] off;
----------------------------------------------------------------------------------------------------
end

--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
begin

set nocount on;
set identity_insert [genericdata].[BusinessEntityDefinition] on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(16,'VR_Common_Country','Country','{"SelectorUIControl":"vr-common-country-selector","ManagerFQTN":"Vanrise.Common.Business.CountryManager,Vanrise.Common.Business", "IdType": "System.Int32"}'),
(20,'VR_Integration_DataSource','Data Source','{"SelectorUIControl":"vr-integration-datasource-selector","GroupSelectorUIControl":"","ManagerFQTN":"Vanrise.Integration.Business.DataSourceManager,Vanrise.Integration.Business","IdType":"System.Int32"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);
set identity_insert [genericdata].[BusinessEntityDefinition] off;
----------------------------------------------------------------------------------------------------
end

--[genericdata].[DataTransformationStepConfig]----------------21-500--------------------------------
begin
set nocount on;
set identity_insert [genericdata].[DataTransformationStepConfig] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'AssignFieldStep','{"DataTransformationStepConfigId":"1","Name":"AssignFieldStep","Title":"Assign Value","Editor":"vr-genericdata-datatransformation-assignfieldstep","StepPreviewUIControl":"vr-genericdata-datatransformation-assignfieldstep-preview"}'),
(2,'AssignValueruleStep','{"DataTransformationStepConfigId":"2","Name":"AssignValueruleStep","Title":"Assign Value using Rule","Editor":"vr-genericdata-datatransformation-assignvaluerulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-assignvaluerulestep-preview"}'),
(3,'NormalizationRuleStep','{"DataTransformationStepConfigId":"3","Name":"NormalizationRuleStep","Title":"Normalization Rule","Editor":"vr-genericdata-datatransformation-normalizationrulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-normalizationrulestep-preview"}'),
(4,'RateValueRuleStep','{"DataTransformationStepConfigId":"4","Name":"RateValueRuleStep","Title":"Rate Value Rule","Editor":"vr-genericdata-datatransformation-ratevaluerulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-ratevaluerulestep-preview"}'),
(5,'RateTypeRuleStep','{"DataTransformationStepConfigId":"5","Name":"RateTypeRuleStep","Title":"Rate Type Rule","Editor":"vr-genericdata-datatransformation-ratetyperulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-ratetyperulestep-preview"}'),
(6,'ForLoopStep','{"DataTransformationStepConfigId":"6","Name":"ForLoopStep","Title":"For Loop","Editor":"vr-genericdata-datatransformation-forloopstep","StepPreviewUIControl":"vr-genericdata-datatransformation-forloopstep-preview"}'),
(7,'InitializeRecordStep','{"DataTransformationStepConfigId":"7","Name":"InitializeRecordStep","Title":"Initialize Record","Editor":"vr-genericdata-datatransformation-initializerecordstep","StepPreviewUIControl":"vr-genericdata-datatransformation-initializerecordstep-preview"}'),
(8,'AddItemToArrayStep','{"DataTransformationStepConfigId":"8","Name":"AddItemToArrayStep","Title":"Add Item To Array","Editor":"vr-genericdata-datatransformation-additemtoarraystep","StepPreviewUIControl":"vr-genericdata-datatransformation-additemtoarraystep-preview"}'),
(9,'IfElseStep','{"DataTransformationStepConfigId":"9","Name":"IfElseStep","Title":"If Else","Editor":"vr-genericdata-datatransformation-ifelsestep","StepPreviewUIControl":"vr-genericdata-datatransformation-ifelsestep-preview"}'),
(10,'ExtraChargeRuleStep','{"DataTransformationStepConfigId":"10","Name":"ExtraChargeRuleStep","Title":"Extra Charge Rule","Editor":"vr-genericdata-datatransformation-extrachargerulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-extrachargerulestep-preview"}'),
(11,'TariffRule','{"DataTransformationStepConfigId":"11","Name":"TariffRule","Title":"Tariff Rule","Editor":"vr-genericdata-datatransformation-tariffrulestep","StepPreviewUIControl":"vr-genericdata-datatransformation-tariffrulestep-preview"}'),
(17,'Execute Transformation','{"DataTransformationStepConfigId":"17","Name":"ExecuteTransformation","Title":"Execute Transformation","Editor":"vr-genericdata-datatransformation-executetransformationstep","StepPreviewUIControl":"vr-genericdata-datatransformation-executetransformationstep-preview"}'),
(19,'BELookupRuleStep','{"DataTransformationStepConfigId":"19","Name":"BELookupRuleStep","Title":"BE Lookup Rule Step","Editor":"vr-genericdata-belookuprulestep","StepPreviewUIControl":"vr-genericdata-belookuprulestep-preview"}'),
(20,'LoadBEByIDStep','{"DataTransformationStepConfigId":"20","Name":"LoadBEByIDStep","Title":"Load BE By ID Step","Editor":"vr-genericdata-loadbebyidstep","StepPreviewUIControl":"vr-genericdata-loadbebyidstep-preview"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[genericdata].[DataTransformationStepConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);
set identity_insert [genericdata].[DataTransformationStepConfig] off;
----------------------------------------------------------------------------------------------------
end

--common.[extensionconfiguration]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('C47176FB-32EB-430C-B92D-D34DFADCDDF9','Time Interval','Time Interval','VR_GenericData_SummaryBatchIntervalSettings','{"Editor":"vr-genericdata-summarytransformation-timeinterval-selector"}'),('F663BF74-99DB-4746-8CBC-E74198E1786C','VR_GenericData_VRObjectTypes_DataRecordField','Field','VR_GenericData_DataRecordObjectType_PropertyEvaluator','{"Editor":"vr_genericdata_datarecordobjectfield"}'),('FC4B69F0-D577-4319-8D10-ED8F95E07441','VR_Generic_DataRecordFieldFormula_ParentBusinessEntity','Parent Business Entity','VR_Generic_DataRecordFieldFormula','{"Editor":"vr-genericdata-datarecordtypefields-formula-parentbusinessentity"}'),('BBC57155-0412-4371-83E5-1917A8BEA468','VR_GenericData_VRObjectTypes_DataRecordType','Data Record','VR_Common_ObjectType','{"Editor":"vr_genericdata_datarecordobjecttype", "PropertyEvaluatorExtensionType": "VR_GenericData_DataRecordObjectType_PropertyEvaluator"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[extensionconfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end