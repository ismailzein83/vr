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
--[common].[ExtensionConfiguration]-------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('EE2C731E-A5F7-481E-B350-7771C8A0F3BC','TOne Account Balance','TOne Account Balance','VR_AccountBalance_NotificationTypeExtendedSettingsConfig'		,'{"Editor":"whs-accountbalance-notificationtype-settings"}'),
('B49238AC-54E2-4F3F-8C91-C864B7389566','VR_Notification_GenericFinancialAccountBalanceType_Settings','Financial Account Balance Notification','VR_AccountBalance_NotificationTypeExtendedSettingsConfig','{"Editor" : "vr-accountbalance-financialaccountbalancenotificationtype"}')

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

--[sec].[View]--------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('9ED29AE9-1A08-49DD-B083-242078BBBE71','Financial Alerts','Financial Alerts',null,'2fbf14b9-fba8-4926-96f2-10e9846f55a4',null,null,null,'{"$type":"Vanrise.Notification.Entities.VRNotificationViewSettings, Vanrise.Notification.Entities","Settings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Notification.Entities.VRNotificationViewSettingItem, Vanrise.Notification.Entities]], mscorlib","$values":[{"$type":"Vanrise.Notification.Entities.VRNotificationViewSettingItem, Vanrise.Notification.Entities","VRNotificationTypeId":"d307a418-81c0-424c-9b2b-5890de5a69cb"}]}}','A196C40A-30B5-4297-B7B0-4344C41CE5A2',30,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank],s.[IsDeleted]);




--[VRNotification].[VRAlertRuleType]----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('24056159-1454-47D5-82BA-5017943A2A42','Customer Prepaid','{"$type":"Vanrise.AccountBalance.Business.Extensions.AccountBalanceAlertRuleTypeSettings, Vanrise.AccountBalance.Business","ConfigId":"ba79cb79-d058-4382-88fc-db1c154b5374","NotificationTypeId":"d307a418-81c0-424c-9b2b-5890de5a69cb","AccountTypeId":"5488a9a2-0200-4895-b6ed-13f35c94d54c","ThresholdExtensionType":"WhS_AccountBalance_AlertThreshold_PrePaid","Behavior":{"$type":"Vanrise.AccountBalance.Business.Extensions.AccountBalanceAlertRuleBehavior, Vanrise.AccountBalance.Business"},"SettingEditor":"vr-notification-vrbalancealertrule-settings","CriteriaDefinition":{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteria, Vanrise.GenericData.Entities","Fields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"Financial Account","Title":"Financial Account","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"ca290901-8259-4a2d-82af-1b5fefb5e40d","IsNullable":false,"OrderType":1},"RuleStructureBehaviorType":0,"Priority":1,"ShowInBasicSearch":false,"IgnoreCase":false,"ValueObjectName":"Account","ValuePropertyName":"FinancialAccountId"}]}},"Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Account":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Account","VRObjectTypeDefinitionId":"60e921b7-1440-4ceb-ae6c-04881649c5cd"}},"Security":{"$type":"Vanrise.Notification.Entities.AlertRuleTypeSecurity, Vanrise.Notification.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3d6b033d-07a5-4f90-bf81-b295c95a27a7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Rules"]}}]}},"AddPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3d6b033d-07a5-4f90-bf81-b295c95a27a7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Add Rules"]}}]}},"EditPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3d6b033d-07a5-4f90-bf81-b295c95a27a7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Edit Rules"]}}]}}}}'),
('7C0870F8-4AB1-405D-A9F1-A4B41BCE58D1','Netting','{"$type":"Vanrise.AccountBalance.Business.Extensions.AccountBalanceAlertRuleTypeSettings, Vanrise.AccountBalance.Business","ConfigId":"ba79cb79-d058-4382-88fc-db1c154b5374","NotificationTypeId":"d307a418-81c0-424c-9b2b-5890de5a69cb","AccountTypeId":"87709be7-082a-46c2-840b-9f380f3ec2f8","ThresholdExtensionType":"WhS_AccountBalance_AlertThreshold_PostPaid","Behavior":{"$type":"Vanrise.AccountBalance.Business.Extensions.AccountBalanceAlertRuleBehavior, Vanrise.AccountBalance.Business"},"SettingEditor":"vr-notification-vrbalancealertrule-settings","CriteriaDefinition":{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteria, Vanrise.GenericData.Entities","Fields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"FinancialAccount","Title":"Financial Account","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"16661e6b-f227-4a8a-a5f5-50ccc52cc15b","IsNullable":false,"OrderType":1},"RuleStructureBehaviorType":0,"Priority":1,"ShowInBasicSearch":false,"IgnoreCase":false,"ValueObjectName":"Account","ValuePropertyName":"FinancialAccountId"}]}},"Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Account":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Account","VRObjectTypeDefinitionId":"60e921b7-1440-4ceb-ae6c-04881649c5cd"}},"Security":{"$type":"Vanrise.Notification.Entities.AlertRuleTypeSecurity, Vanrise.Notification.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3d6b033d-07a5-4f90-bf81-b295c95a27a7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Rules"]}}]}},"AddPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3d6b033d-07a5-4f90-bf81-b295c95a27a7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Add Rules"]}}]}},"EditPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3d6b033d-07a5-4f90-bf81-b295c95a27a7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Edit Rules"]}}]}}}}'),
('0C7A8EB1-E905-4CB5-BFFC-D9BB08939788','Customer Postpaid','{"$type":"Vanrise.AccountBalance.Business.Extensions.AccountBalanceAlertRuleTypeSettings, Vanrise.AccountBalance.Business","ConfigId":"ba79cb79-d058-4382-88fc-db1c154b5374","NotificationTypeId":"d307a418-81c0-424c-9b2b-5890de5a69cb","AccountTypeId":"ebc69b46-d23b-4ada-99f1-b597e7be76b3","ThresholdExtensionType":"WhS_AccountBalance_AlertThreshold_PostPaid","Behavior":{"$type":"Vanrise.AccountBalance.Business.Extensions.AccountBalanceAlertRuleBehavior, Vanrise.AccountBalance.Business"},"SettingEditor":"vr-notification-vrbalancealertrule-settings","CriteriaDefinition":{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteria, Vanrise.GenericData.Entities","Fields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities","FieldName":"FinancialAccount","Title":"Financial Account","FieldType":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","BusinessEntityDefinitionId":"89bc46ef-28f0-43ac-9f4a-d3f9c2ea2ef1","IsNullable":false,"OrderType":1},"RuleStructureBehaviorType":0,"Priority":1,"ShowInBasicSearch":false,"IgnoreCase":false,"ValueObjectName":"Account","ValuePropertyName":"FinancialAccountId"}]}},"Objects":{"$type":"Vanrise.Entities.VRObjectVariableCollection, Vanrise.Entities","Account":{"$type":"Vanrise.Entities.VRObjectVariable, Vanrise.Entities","ObjectName":"Account","VRObjectTypeDefinitionId":"60e921b7-1440-4ceb-ae6c-04881649c5cd"}},"Security":{"$type":"Vanrise.Notification.Entities.AlertRuleTypeSecurity, Vanrise.Notification.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3d6b033d-07a5-4f90-bf81-b295c95a27a7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Rules"]}}]}},"AddPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3d6b033d-07a5-4f90-bf81-b295c95a27a7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Add Rules"]}}]}},"EditPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3d6b033d-07a5-4f90-bf81-b295c95a27a7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Edit Rules"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[VRNotification].[VRAlertRuleType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);


--[common].[VRComponentType]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D307A418-81C0-424C-9B2B-5890DE5A69CB','Account Balance Notification','FDD73530-067F-4160-AB71-7852303C785C','{"$type":"Vanrise.Notification.Entities.VRNotificationTypeSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"fdd73530-067f-4160-ab71-7852303c785c","VRAlertLevelDefinitionId":"48fc148c-299a-4717-bd03-401bb79c082e","ExtendedSettings":{"$type":"Vanrise.AccountBalance.Business.AccountBalanceNotificationTypeSettings, Vanrise.AccountBalance.Business","ConfigId":"0fc411d1-90fd-417c-bfdf-ec0c35b1a666","SearchRuntimeEditor":"vr-accountbalance-notification-searcheditor","BodyRuntimeEditor":"vr-accountbalance-notification-bodyeditor","AccountColumnHeader":"Financial Account","ShowAccountTypeColumn":false,"AccountBalanceNotificationTypeExtendedSettings":{"$type":"TOne.WhS.AccountBalance.Business.TOneAccountBalanceNotificationTypeSettings, TOne.WhS.AccountBalance.Business","ConfigId":"ee2c731e-a5f7-481e-b350-7771c8a0f3bc","NotificationQueryEditor":"whs-accountbalance-notificationtype-searcheditor"}},"Security":{"$type":"Vanrise.Notification.Entities.VRNotificationTypeSecurity, Vanrise.Notification.Entities","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"3d6b033d-07a5-4f90-bf81-b295c95a27a7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Alerts"]}}]}},"HideRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ConfigID],[Settings]))
merge	[common].[VRComponentType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ConfigID] = s.[ConfigID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ConfigID],[Settings])
	values(s.[ID],s.[Name],s.[ConfigID],s.[Settings]);
