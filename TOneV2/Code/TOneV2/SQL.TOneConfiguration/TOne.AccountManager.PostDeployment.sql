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
('3D638C43-0191-464C-9E6E-CAAA5A2E2FDC',null,'Selling Rule','{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinition, Vanrise.GenericData.Entities","GenericRuleDefinitionId":"3d638c43-0191-464c-9e6e-caaa5a2e2fdc","Name":"Selling Rule","Title":"Selling Rule","CriteriaDefinition":{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteria, Vanrise.GenericData.Entities","ConfigId":"6b1a593a-e5e5-4ca4-834a-9a20a1fd16ba","Fields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"AccountManager","Title":"Account Manager","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","OrderType":1,"BusinessEntityDefinitionId":"0146109f-4e5d-4d66-be2f-15d689c960ee","IsNullable":false,"DependantFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityTypeDependantField, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[]},"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"RuleStructureBehaviorType":0,"Priority":1,"ShowInBasicSearch":false,"IgnoreCase":false},{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"Customer","Title":"Customer","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","OrderType":1,"BusinessEntityDefinitionId":"ba5a57bd-1f03-440f-a469-463a48762b8f","IsNullable":false,"DependantFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityTypeDependantField, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[]},"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"RuleStructureBehaviorType":0,"Priority":2,"ShowInBasicSearch":false,"IgnoreCase":false},{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"SellingProduct","Title":"Selling Product","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","OrderType":1,"BusinessEntityDefinitionId":"79581bd3-1acc-4344-a67e-9bb591aac457","IsNullable":false,"DependantFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityTypeDependantField, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[]},"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"RuleStructureBehaviorType":0,"Priority":3,"ShowInBasicSearch":false,"IgnoreCase":false},{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"Sale zone","Title":"Sale zone","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","OrderType":1,"BusinessEntityDefinitionId":"900d0e5d-0fa7-428e-a83b-cd64e16f7415","IsNullable":false,"DependantFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityTypeDependantField, Vanrise.GenericData.MainExtensions]], mscorlib","$values":[]},"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"RuleStructureBehaviorType":0,"Priority":4,"ShowInBasicSearch":false,"IgnoreCase":false}]},"Groups":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaGroup, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaGroup, Vanrise.GenericData.Entities","GroupId":"f5307f42-113c-1e87-9148-7f8856bbc1b6","GroupTitle":"Type","Fields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaGroupField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaGroupField, Vanrise.GenericData.Entities","FieldName":"Customer"},{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaGroupField, Vanrise.GenericData.Entities","FieldName":"SellingProduct"}]}}]}},"SettingsDefinition":{"$type":"TOne.WhS.Sales.Entities.SellingRuleDefinitionSettings, TOne.WhS.Sales.Entities","ConfigId":"a249cbd8-ea0d-48f0-a630-ad3b4f6087de","GridSettingTitle":"Threshold","SupportUpload":false},"Security":{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionSecurity, Vanrise.GenericData.Entities","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"AddRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"EditRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}}')
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
('E78BD4E6-6D9C-4432-B564-5B5A1DD1EF73','Percentage','Percentage','WHS_Sales_Rules_SellingRuleThresholdSetting','{"Editor":"vr-whs-sales-sellingrulesettings-threshold-percentage"}'),
('A249CBD8-EA0D-48F0-A630-AD3B4F6087DE','VR_WHS_SellingRule','Selling Rule','VR_GenericData_GenericRuleTypeConfig','{"Editor":"vr-whs-sales-genericruledefinitionsettings-sellingrule","RuntimeEditor":"vr-whs-sales-sellingrulesettings", "RuleTypeFQTN":"TOne.WhS.Sales.Entities.SellingRule, 
TOne.WhS.Sales.Entities", "RuleManagerFQTN":"TOne.WhS.Sales.Business.SellingRuleManager, TOne.WhS.Sales.Business"}'),
('066FCA8E-F4B8-4643-957D-AFAC44339470','Rate','Rate','WHS_Sales_Rules_SellingRuleThresholdSetting','{"Editor":"vr-whs-sales-sellingrulesettings-threshold-fixed"}')
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
	
	
--[sec].[View]-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[DevProjectID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('C8E0DA0A-F0AA-4677-8386-2A8D0CA42E0B',null,'Selling Rule','Selling Rule',null,'80E0E78C-F7DA-481C-B8D3-1FF61188263B',null,null,null,'{"$type":"Vanrise.GenericData.Entities.GenericRuleViewSettings, Vanrise.GenericData.Entities","RuleDefinitionIds":{"$type":"System.Collections.Generic.List`1[[System.Guid, mscorlib]], mscorlib","$values":["3d638c43-0191-464c-9e6e-caaa5a2e2fdc"]}}','729BE766-F3D7-4BCC-9678-CCCF57BD4AAD',null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[DevProjectID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[DevProjectID] = s.[DevProjectID],[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank],[IsDeleted] = s.[IsDeleted]
when not matched by target then
	insert([ID],[DevProjectID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
	values(s.[ID],s.[DevProjectID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank],s.[IsDeleted]);

