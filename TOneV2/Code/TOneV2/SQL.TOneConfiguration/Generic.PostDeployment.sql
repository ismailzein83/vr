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
;with cte_data([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(301,'GenericData','Generic Data','Generic Data',1,null,1,0)
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

--[sec].[View]-----------------------------3001 to 4000-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[RequiredPermissions],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(3001,'Generic Rule Definitions','Generic Rule Definition','#/view/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionManagement',301,null,null,null,null,null,0,3),
(3002,'Data Transformation Definitions','Data Transformation Definition','#/view/VR_GenericData/Views/DataTransformationDefinition/DataTransformationDefinitionManagement',301,null,null,null,null,null,0,4),
(3003,'Data Record Types','Data Record Type','#/view/VR_GenericData/Views/GenericDataRecord/DataRecordTypeManagement',301,null,null,null,null,null,0,5),
(3004,'Data Stores','Data Store','#/view/VR_GenericData/Views/DataStore/DataStoreManagement',301,null,null,null,null,null,0,6),
(3005,'Data Record Storages','Data Record Storage','#/view/VR_GenericData/Views/DataRecordStorage/DataRecordStorageManagement',301,null,null,null,null,null,0,7),
(3006,'Business Entity Definitions','Business Entity Definitions','#/view/VR_GenericData/Views/GenericBusinessEntity/Definition/GenericBEDefinitionManagement',301,null,null,null,null,null,0,2)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[RequiredPermissions],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[RequiredPermissions] = s.[RequiredPermissions],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[RequiredPermissions],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[RequiredPermissions],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
set identity_insert [sec].[View] off;

--[sec].[BusinessEntityModule]-------------301 to 400---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------

--[sec].[BusinessEntity]-------------------601 to 900--------------------------------------------------------
--------------------------------------------------------------------------------------------------------------

--[sec].[SystemAction]----------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------

--[genericdata].[GenericRuleTypeConfig]-------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [genericdata].[GenericRuleTypeConfig] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'GenericRuleMapping','{"GenericRuleTypeConfigId":"1","Name":"GenericRuleMapping","Title":"Mapping","Editor":"vr-genericdata-genericruledefinitionsettings-mapping","RuntimeEditor":"vr-genericdata-genericrulesettings-mapping-runtimeeditor","RuleTypeFQTN":"Vanrise.GenericData.Transformation.Entities.MappingRule, Vanrise.GenericData.Transformation.Entities","RuleManagerFQTN":"Vanrise.GenericData.Transformation.MappingRuleManager, Vanrise.GenericData.Transformation","FilterEditor":"vr-genericdata-genericrulesettings-mapping-filtereditor"}'),
(2,'VR_NormalizationRule','{"GenericRuleTypeConfigId":"2","Name":"VR_NormalizationRule","Title":"Normalization Rule","Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-rules-normalizenumbersettings", "RuleTypeFQTN":"Vanrise.GenericData.Normalization.NormalizationRule, Vanrise.GenericData.Normalization", "RuleManagerFQTN":"Vanrise.GenericData.Normalization.NormalizationRuleManager, Vanrise.GenericData.Normalization"}'),
(3,'VR_RateTypeRule','{"GenericRuleTypeConfigId":"4","Name":"VR_RateTypeRule","Title":"Rate Type Rule","Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-rules-pricingrulesettings-ratetype", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.RateTypeRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.RateTypeRuleManager, Vanrise.GenericData.Pricing"}'),
(4,'VR_TariffRule','{"GenericRuleTypeConfigId":"6","Name":"VR_TariffRule","Title":"Tariff Rule","Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-rules-pricingrulesettings-tariff", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.TariffRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.TariffRuleManager, Vanrise.GenericData.Pricing"}'),
(5,'VR_ExtraChargeRule','{"GenericRuleTypeConfigId":"7","Name":"VR_ExtraChargeRule","Title":"Extra Charge Rule","Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-rules-pricingrulesettings-extracharge", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.ExtraChargeRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.ExtraChargeRuleManager, Vanrise.GenericData.Pricing"}'),
(6,'VR_RateValueRule','{"GenericRuleTypeConfigId":"11","Name":"VR_RateValueRule","Title":"Rate Value Rule","Editor":"vr-genericdata-genericruledefinitionsettings-nosettings","RuntimeEditor":"vr-rules-pricingrulesettings-ratevalue", "RuleTypeFQTN":"Vanrise.GenericData.Pricing.RateValueRule, Vanrise.GenericData.Pricing", "RuleManagerFQTN":"Vanrise.GenericData.Pricing.RateValueRuleManager, Vanrise.GenericData.Pricing"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[genericdata].[GenericRuleTypeConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);
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
	values(s.[ID],s.[Name],s.[Details])
when not matched by source then
	delete;
set identity_insert [genericdata].[DataStoreConfig] off;

--[genericdata].[DataRecordFieldTypeConfig]---------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [genericdata].[DataRecordFieldTypeConfig] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Text','{"DataRecordFieldTypeConfigId":"1","Name":"Text","Title":"Text","Editor":"vr-genericdata-fieldtype-text","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-text-filtereditor"}'),
(2,'Number','{"DataRecordFieldTypeConfigId":"2","Name":"Number","Title":"Number","Editor":"vr-genericdata-fieldtype-number","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-number-filtereditor"}'),
(3,'DateTime','{"DataRecordFieldTypeConfigId":"3","Name":"DateTime","Title":"DateTime","Editor":"vr-genericdata-fieldtype-datetime","RuntimeEditor":"vr-genericdata-fieldtype-datetime-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-datetime-filtereditor"}'),
(4,'Choices','{"DataRecordFieldTypeConfigId":"6","Name":"Choices","Title":"Choices","Editor":"vr-genericdata-fieldtype-choices","RuntimeEditor":"vr-genericdata-fieldtype-choices-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-choices-filtereditor"}'),
(5,'Boolean','{"DataRecordFieldTypeConfigId":"7","Name":"Boolean","Title":"Boolean","Editor":"vr-genericdata-fieldtype-boolean","RuntimeEditor":"vr-genericdata-fieldtype-boolean-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-boolean-filtereditor"}'),
(6,'Business Entity','{"DataRecordFieldTypeConfigId":"8","Name":"Business Entity","Title":"Business Entity","Editor":"vr-genericdata-fieldtype-businessentity","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","FilterEditor":"vr-genericdata-fieldtype-businessentity-filtereditor"}'),
(7,'Array','{"DataRecordFieldTypeConfigId":"9","Name":"Array","Title":"Array","Editor":"vr-genericdata-fieldtype-array","RuntimeEditor":"vr-genericdata-fieldtype-array-runtimeeditor", "FilterEditor": "vr-genericdata-fieldtype-array-filtereditor"}')
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
	values(s.[ID],s.[Name],s.[Details]);
set identity_insert [genericdata].[DataRecordFieldTypeConfig] off;