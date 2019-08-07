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

--[genericdata].[GenericRuleDefinition]----------------------------------------------------------------------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[DevProjectID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3D638C43-0191-464C-9E6E-CAAA5A2E2FDC',null,'Selling Rule','{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinition, Vanrise.GenericData.Entities","GenericRuleDefinitionId":"3d638c43-0191-464c-9e6e-caaa5a2e2fdc","Name":"Selling Rule","Title":"Selling Rule","CriteriaDefinition":{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteria, Vanrise.GenericData.Entities","ConfigId":"6b1a593a-e5e5-4ca4-834a-9a20a1fd16ba","Fields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"AccountManager","Title":"Account Manager","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","OrderType":1,"BusinessEntityDefinitionId":"0146109f-4e5d-4d66-be2f-15d689c960ee","IsNullable":false,"DependantFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityTypeDependantField, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[]},"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","ShowDefaultValue":true,"StoreValueSerialized":false,"CanRoundValue":false},"RuleStructureBehaviorType":0,"Priority":1,"ShowInBasicSearch":false,"IgnoreCase":false},{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"RoutingProduct","Title":"Routing Product","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","OrderType":1,"BusinessEntityDefinitionId":"ddf86702-636e-4620-9dee-91241cd4f50a","IsNullable":false,"DependantFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityTypeDependantField, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[]},"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","ShowDefaultValue":true,"StoreValueSerialized":false,"CanRoundValue":false},"RuleStructureBehaviorType":0,"Priority":5,"ShowInBasicSearch":false,"IgnoreCase":false},{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"Customer","Title":"Customer","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","OrderType":1,"BusinessEntityDefinitionId":"ba5a57bd-1f03-440f-a469-463a48762b8f","IsNullable":true,"DependantFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityTypeDependantField, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[]},"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","ShowDefaultValue":true,"StoreValueSerialized":false,"CanRoundValue":false},"RuleStructureBehaviorType":0,"Priority":2,"ShowInBasicSearch":false,"IgnoreCase":false},{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"SellingProduct","Title":"Selling Product","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","OrderType":1,"BusinessEntityDefinitionId":"79581bd3-1acc-4344-a67e-9bb591aac457","IsNullable":false,"DependantFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityTypeDependantField, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[]},"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","ShowDefaultValue":true,"StoreValueSerialized":false,"CanRoundValue":false},"RuleStructureBehaviorType":0,"Priority":3,"ShowInBasicSearch":false,"IgnoreCase":false},{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"SaleZone","Title":"Sale Zone","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","OrderType":1,"BusinessEntityDefinitionId":"900d0e5d-0fa7-428e-a83b-cd64e16f7415","IsNullable":false,"DependantFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityTypeDependantField, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[]},"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","ShowDefaultValue":true,"StoreValueSerialized":false,"CanRoundValue":false},"RuleStructureBehaviorType":0,"Priority":4,"ShowInBasicSearch":false,"IgnoreCase":false}]},"Groups":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaGroup, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaGroup, Vanrise.GenericData.Entities","GroupId":"f5307f42-113c-1e87-9148-7f8856bbc1b6","GroupTitle":"Type","Fields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaGroupField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaGroupField, Vanrise.GenericData.Entities","FieldName":"Customer"},{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaGroupField, Vanrise.GenericData.Entities","FieldName":"SellingProduct"}]}}]}},"SettingsDefinition":{"$type":"TOne.WhS.Sales.Entities.SellingRuleDefinitionSettings, TOne.WhS.Sales.Entities","ConfigId":"a249cbd8-ea0d-48f0-a630-ad3b4f6087de","GridSettingTitle":"Threshold","SupportUpload":false},"Security":{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionSecurity, Vanrise.GenericData.Entities","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"AddRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Manage"]}}]}},"EditRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Manage"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[DevProjectID],[Name],[Details]))
merge	[genericdata].[GenericRuleDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[DevProjectID] = s.[DevProjectID],[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[DevProjectID],[Name],[Details])
	values(s.[ID],s.[DevProjectID],s.[Name],s.[Details]);
	
		
--[common].[ExtensionConfiguration]--------------------------------------------------------------------------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('E78BD4E6-6D9C-4432-B564-5B5A1DD1EF73','MinMarginPercentage','Min Margin %','WHS_Sales_Rules_SellingRuleThresholdSetting','{"Editor":"vr-whs-sales-sellingrulesettings-threshold-minmarginpercentage"}'),
('2B794920-FE60-4186-80DE-1CB05512F338','MaxMarginPercentage','Max Margin %','WHS_Sales_Rules_SellingRuleThresholdSetting','{"Editor":"vr-whs-sales-sellingrulesettings-threshold-maxmarginpercentage"}'),
('066FCA8E-F4B8-4643-957D-AFAC44339470','MinRate','Min Rate','WHS_Sales_Rules_SellingRuleThresholdSetting','{"Editor":"vr-whs-sales-sellingrulesettings-threshold-fixed"}'),

('A249CBD8-EA0D-48F0-A630-AD3B4F6087DE','VR_WHS_SellingRule','Selling Rule','VR_GenericData_GenericRuleTypeConfig','{"Editor":"vr-whs-sales-genericruledefinitionsettings-sellingrule","RuntimeEditor":"vr-whs-sales-sellingrulesettings", "RuleTypeFQTN":"TOne.WhS.Sales.Entities.SellingRule,TOne.WhS.Sales.Entities", "RuleManagerFQTN":"TOne.WhS.Sales.Business.SellingRuleManager, TOne.WhS.Sales.Business"}'),
('ADFE8423-2666-4B45-A881-446D8C368E4C','WhS_AccountManagerAssignment','Account Manager Assignment','VR_GenericData_GenericBEOnBeforeInsertHandlerSettings','{ "Editor": "whs-be-accountmanagerassignment-onbeforesavehandler" }'),
('A6F4D0D4-3562-4151-8ED8-984CE7A83C20','DataRecordStorageAccountManagerPermanentFilter','Account Manager','VR_GenericData_PermanentFilterSettings','{"Editor": "whs-be-datarecordstorage-permanentfilter-accountmanager"}'),
('4A3AD674-9ADB-40C6-BEFD-A1813F08F333','VR_Analytic_AccountManagerAnalyticTablePermanentFilter','Account Manager','VR_Analytic_PermanentFilterSettings','{"Editor": "whs-be-analytic-permanentfilter-accountmanager" }')
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
	
	
--[genericdata].[BusinessEntityDefinition]-------------------------------------------------------------------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[DevProjectID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('79581BD3-1ACC-4344-A67E-9BB591AAC457',null,'WHS_BE_SellingProduct','Selling Product','{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-whs-be-sellingproduct-selector","ManagerFQTN":"TOne.WhS.BusinessEntity.Business.SellingProductManager, TOne.WhS.BusinessEntity.Business", "IdType": "System.Int32"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[DevProjectID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[DevProjectID] = s.[DevProjectID],[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[DevProjectID],[Name],[Title],[Settings])
	values(s.[ID],s.[DevProjectID],s.[Name],s.[Title],s.[Settings]);
	

--[sec].[Module]---------------------------1301 to 1400---------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic],[RenderedAsView])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('DBEFC1B4-2538-4A2B-BF47-7CAD1CE31C42','Sale',null,'1C7569FA-43C9-4853-AE4C-1152746A34FD',null,null,0,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic],[RenderedAsView]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic], [RenderedAsView] = s.[RenderedAsView]
when not matched by target then
	insert([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic],[RenderedAsView])
	values(s.[Id],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic],s.[RenderedAsView]);
--------------------------------------------------------------------------------------------------------------
end


--[sec].[View]-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[DevProjectID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('DC7132CA-8212-4F48-BC92-2948D601EA19',null,'Account Manager','Account Manager',null,'937F4A80-74FD-43BA-BCC1-F674445170BB',null,null,null,'{"$type":"Vanrise.GenericData.Business.GenericBEViewSettings, Vanrise.GenericData.Business","Settings":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Business.GenericBEViewSettingItem, Vanrise.GenericData.Business]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Business.GenericBEViewSettingItem, Vanrise.GenericData.Business","BusinessEntityDefinitionId":"0146109f-4e5d-4d66-be2f-15d689c960ee"}]}}','B99B2B0A-9A80-49FC-B68F-C946E1628595',2,null),
('C8E0DA0A-F0AA-4677-8386-2A8D0CA42E0B',null,'Selling Rules','Selling Rules',null,'DBEFC1B4-2538-4A2B-BF47-7CAD1CE31C42',null,null,null,'{"$type":"Vanrise.GenericData.Entities.GenericRuleViewSettings, Vanrise.GenericData.Entities","RuleDefinitionIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["3d638c43-0191-464c-9e6e-caaa5a2e2fdc"]}}','729BE766-F3D7-4BCC-9678-CCCF57BD4AAD',10,null),
('8C00BBB3-D71F-4A7A-A51B-007FDECD8211',null,'Sale Pricing Rules','Sale Pricing Rules',null,'DBEFC1B4-2538-4A2B-BF47-7CAD1CE31C42',null,null,null,'{"$type":"Vanrise.GenericData.Entities.GenericRuleViewSettings, Vanrise.GenericData.Entities","RuleDefinitionIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["90a47a0a-3ef9-4941-bc21-ca0be44fc5a4","8a637067-0056-4bae-b4d5-f80f00c0141b","f24cb510-0b65-48c8-a723-1f6ebfeea9e8"]}}','729BE766-F3D7-4BCC-9678-CCCF57BD4AAD',5,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[DevProjectID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set 
	[DevProjectID] = s.[DevProjectID],[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank], [IsDeleted]=s.[IsDeleted]
when not matched by target then
	insert([ID],[DevProjectID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
	values(s.[ID],s.[DevProjectID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank],s.[IsDeleted]);


--[sec].[BusinessEntity]-------------------3301 to 3600-------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('470455E5-9415-4AAA-BD32-E997272044D8','WhS_AccountManager','Account Manager','61451603-E7B9-40C6-AE27-6CBA974E1B3B',0,'["View","Add","Edit"]')
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
--------------------------------------------------------------------------------------------------------------
end


--[bp].[BPBusinessRuleDefinition]----------------------------------------------------------------------------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[BPDefintionId],[Settings],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('B14809F3-0C6E-40A9-A126-35A22F1F7954','RatePlan_ValidateAfterProcessing','8ABA2EC4-04FD-4BB1-A593-B651943C6411','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"New rate less than Selling Rule threshold","Condition":{"$type":"TOne.WhS.Sales.Business.BusinessRules.SellingRuleCondition, TOne.WhS.Sales.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"],"ExecutionDependsOnRules":[]}',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[BPDefintionId],[Settings],[Rank]))
merge  [bp].[BPBusinessRuleDefinition] as t
using  cte_data as s
on            1=1 and t.[ID] = s.[ID]
when matched then
       update set
       [Name] = s.[Name],[BPDefintionId] = s.[BPDefintionId],[Settings] = s.[Settings],[Rank] = s.[Rank]
when not matched by target then
       insert([ID],[Name],[BPDefintionId],[Settings],[Rank])
       values(s.[ID],s.[Name],s.[BPDefintionId],s.[Settings],s.[Rank]);


--[bp].[BPBusinessRuleAction]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [bp].BPBusinessRuleAction on;
;with cte_data([ID],[Settings],[BusinessRuleDefinitionId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(141,'{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','B14809F3-0C6E-40A9-A126-35A22F1F7954')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Settings],[BusinessRuleDefinitionId]))
merge  [bp].[BPBusinessRuleAction] as t
using  cte_data as s
on            1=1 and t.[ID] = s.[ID]
when matched then
       update set
       [Settings] = s.[Settings],[BusinessRuleDefinitionId] = s.[BusinessRuleDefinitionId]
when not matched by target then
       insert([ID],[Settings],[BusinessRuleDefinitionId])
       values(s.[ID],s.[Settings],s.[BusinessRuleDefinitionId]);
set identity_insert [bp].BPBusinessRuleAction off;