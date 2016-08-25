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
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(301,'Generic Data',null,-100,null,10,0)
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

--[sec].[viewtype]-----------------------101 to 200-------------------------------------------------
----------------------------------------------------------------------------------------------------
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

--[sec].[View]-----------------------------3001 to 4000-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(3001,'Data Record Types','Data Record Type','#/view/VR_GenericData/Views/GenericDataRecord/DataRecordTypeManagement',301,'VR_GenericData/DataRecordType/GetFilteredDataRecordTypes',null,null,null,0,1),
(3002,'Generic Rule Definitions','Generic Rule Definition','#/view/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionManagement',301,'VR_GenericData/GenericRuleDefinition/GetFilteredGenericRuleDefinitions',null,null,null,0,2),
(3003,'Data Transformation Definitions','Data Transformation Definition','#/view/VR_GenericData/Views/DataTransformationDefinition/DataTransformationDefinitionManagement',301,'VR_GenericData/DataTransformationDefinition/GetFilteredDataTransformationDefinitions',null,null,null,0,3),
(3004,'Data Stores','Data Store','#/view/VR_GenericData/Views/DataStore/DataStoreManagement',301,'VR_GenericData/DataStore/GetFilteredDataStores',null,null,null,0,4),
(3005,'Data Record Storages','Data Record Storage','#/view/VR_GenericData/Views/DataRecordStorage/DataRecordStorageManagement',301,'VR_GenericData/DataRecordStorage/GetFilteredDataRecordStorages',null,null,null,0,5),
(3006,'Business Entity Definitions','Business Entity Definitions','#/view/VR_GenericData/Views/GenericBusinessEntity/Definition/GenericBEDefinitionManagement',301,'VR_GenericData/BusinessEntityDefinition/GetFilteredBusinessEntityDefinitions',null,null,null,0,6),
(3007,'Summary Transformation Definition','Summary Transformation Definition','#/view/VR_GenericData/Views/SummaryTransformationDefinition/SummaryTransformationDefinitionManagement',301,'VR_GenericData/SummaryTransformationDefinition/GetFilteredSummaryTransformationDefinitions',null,null,null,0,8),
(3008,'BE Lookup Rule Definition','BE Lookup Rule Definition Management','#/view/VR_GenericData/Views/BELookupRuleDefinition/BELookupRuleDefinitionManagement',301,'VR_GenericData/BELookupRuleDefinition/GetFilteredBELookupRuleDefinitions',null,null,null,0,11),
(3009,'Data Record Field Choice','Data Record Field Choice','#/view/VR_GenericData/Views/DataRecordFieldChoice/DataRecordFieldChoiceManagement',301,'VR_GenericData/DataRecordFieldChoice/GetFilteredDataRecordFieldChoices',null,null,null,0,12)
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

--[sec].[BusinessEntityModule]-------------301 to 400---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(301,'Generic Data',4,0)
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

--[sec].[BusinessEntity]-------------------601 to 900--------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
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

--[sec].[SystemAction]----------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
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

--[genericdata].[GenericRuleTypeConfig]-------------------------------------------------------------
----------------------------------------------------------------------------------------------------
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

--[genericdata].[ExpressionBuilderConfig]-----------------------------------------------------------
----------------------------------------------------------------------------------------------------
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

--[genericdata].[DataTransformationStepConfig]------------------------------------------------------
----------------------------------------------------------------------------------------------------
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

--[genericdata].[DataStoreConfig]-------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
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

--[genericdata].[DataRecordFieldTypeConfig]---------------------------------------------------------
----------------------------------------------------------------------------------------------------
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

--[common].[TemplateConfig]-----------------60001 to 70000------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[TemplateConfig] on;
;with cte_data([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(60001,'Time Interval','VR_GenericData_SummaryBatchIntervalSettings','vr-genericdata-summarytransformation-timeinterval-selector',null,null)
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

--[queue].[QueueActivatorConfig]----------------1 to 500--------------------------------------------
----------------------------------------------------------------------------------------------------
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

--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
----------------------------------------------------------------------------------------------------
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

--[genericdata].[DataTransformationStepConfig]----------------21-500--------------------------------
----------------------------------------------------------------------------------------------------
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

--[common].[ExtensionConfiguration]-----------------4001	to 5000---------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings],[CreatedTime])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(4001,'VR_GenericData_VRObjectTypes_DataRecordType','Data Record Type','VR_Common_ObjectType','{"Editor":"vr_genericdata_datarecordobjecttype", "PropertyEvaluatorExtensionType": "VR_GenericData_DataRecordObjectType_PropertyEvaluator"}','2016-08-01 10:04:43.020'),
(4002,'VR_Generic_DataRecordFieldFormula_ParentBusinessEntity','Parent Business Entity','VR_Generic_DataRecordFieldFormula','{"Editor":"vr-genericdata-datarecordtypefields-formula-parentbusinessentity"}','2016-06-30 15:38:26.300'),
(4003,'VR_GenericData_VRObjectTypes_DataRecordField','Data Record Field','VR_GenericData_DataRecordObjectType_PropertyEvaluator',null,'2016-08-03 11:35:09.467')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings],[CreatedTime]))
merge	[common].[ExtensionConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[ConfigType],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);