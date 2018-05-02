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

--[common].[ExtensionConfiguration]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FDD73530-067F-4160-AB71-7852303C785C','VR_Notification_NotificationComponentType_Settings','Notification Type','VR_Common_VRComponentType'								,'{"Editor":"vr-notification-vrnotification-componentsettings"}'),
('A196C40A-30B5-4297-B7B0-4344C41CE5A2','VR_Notification_NotificationDefinition','Notification','VR_Security_ViewTypeConfig'												,'{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"vr-notification-notification-vieweditor"}'),
('d96f17c8-29d7-4c0c-88dc-9d5fbca2178f','VR_Notification_VRActionDefinition','Action Definition','VR_Common_VRComponentType'												,'{"Editor":"vr-notification-vractiondefinition-settings"}'),
('0B44D3F3-AA62-4289-8EB3-D93269515036','Alert Level','Alert Level','VR_GenericData_BusinessEntityDefinitionSettingsConfig'													,'{"Editor":"vr-notification-alertleveldefinitionbe-editor"}'),
('AA2AD3F2-ED37-4212-BE3E-2CC507BC1205','VrAlertRuleTypeOverriddenConfiguration','AlertRuleType Overridden Configuration','VRCommon_OverriddenConfiguration','{"Editor":"vr-notification-overriddenconfiguration-alertruletype"}')
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

--[sec].[Module]------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('94E29C06-8B09-4460-B495-5A7413C52C8C','Notifications'	,null,null,null,115,1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[ID],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
----------------------------------------------------------------------------------------------------
END

--sec.[View]----------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('05234A49-DFA3-44C3-A552-80E6C793A67A','Action Rule Types','Action Rule Type','#/view/VR_Notification/Views/VRAlertRuleType/VRAlertRuleTypeManagement'	,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VR_Notification/VRAlertRuleType/GetFilteredVRAlertRuleTypes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',45),
('FD44ED02-9708-4F38-A237-E6C0BF35B634','Alert Levels','Alert Levels','#/view/VR_Notification/Views/VRAlertLevel/VRAlertLevelManagement'				,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VR_Notification/VRAlertRuleType/GetFilteredVRAlertRuleTypes',NULL,NULL,NULL,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',47),

('FF5E429A-CFF0-4095-8AD0-987FD3CAD502','Action Rules','Action Rule','#/view/VR_Notification/Views/VRAlertRule/VRAlertRuleManagement'					,'1C7569FA-43C9-4853-AE4C-1152746A34FD',null,null,null,'{"$type":"Vanrise.Notification.Business.VRAlertRuleViewSettings, Vanrise.Notification.Business"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',100)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
----------------------------------------------------------------------------------------------------
END

--[sec].[BusinessEntity]-------------------6001 to 6300-------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FE898B2B-4D50-4926-A6EF-207EDBACC21A','BusinessProcess_BP_Notifications','Notifications'					,'B6B8F582-4759-43FB-9220-AA7662C366EA',0,'["View", "StartInstance", "ScheduleTask"]')
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
END

--[bp].[BPDefinition]----------------------7001 to 8000---------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('0E36B76B-44B2-4976-9477-157DB0514069','Vanrise.Notification.BP.Arguments.ExecuteNotificationProcessInput','Execute Notification','Vanrise.Notification.BP.ExecuteNotificationProcess,Vanrise.Notification.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":true,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.Notification.Business.ExecuteNotificationBPDefinitionExtendedSettings, Vanrise.Notification.Business"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"fe898b2b-4d50-4926-a6ef-207edbacc21a","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"fe898b2b-4d50-4926-a6ef-207edbacc21a","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"fe898b2b-4d50-4926-a6ef-207edbacc21a","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('C508FE10-DDFA-4697-80FF-CD16042AF683','Vanrise.Notification.BP.Arguments.ClearNotificationInput','Clear Notification','Vanrise.Notification.BP.ClearNotificationProcess,Vanrise.Notification.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":true,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.Notification.Business.ClearNotificationBPDefinitionExtendedSettings, Vanrise.Notification.Business"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"fe898b2b-4d50-4926-a6ef-207edbacc21a","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"fe898b2b-4d50-4926-a6ef-207edbacc21a","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"fe898b2b-4d50-4926-a6ef-207edbacc21a","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([ID],[Name],[Title],[FQTN],[Config])
	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
----------------------------------------------------------------------------------------------------
END

--[sec].[SystemAction]------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Notification/VRAlertRuleType/GetFilteredVRAlertRuleTypes','VR_SystemConfiguration: View'),
('VR_Notification/VRAlertRuleType/GetVRAlertRuleType',null),
('VR_Notification/VRAlertRuleType/AddVRAlertRuleType','VR_SystemConfiguration: Add'),
('VR_Notification/VRAlertRuleType/UpdateVRAlertRuleType','VR_SystemConfiguration: Edit'),
('VR_Notification/VRAlertRuleType/GetVRAlertRuleTypeSettingsExtensionConfigs',null),
('VR_Notification/VRAlertRuleType/GetVRAlertRuleTypesInfo',null),

('VR_Notification/VRAlertRule/GetFilteredVRAlertRules',null),
('VR_Notification/VRAlertRule/GetVRAlertRule',null),
('VR_Notification/VRAlertRule/AddVRAlertRule',null),
('VR_Notification/VRAlertRule/UpdateVRAlertRule',null)
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
	----------------------------------------------------------------------------------------------------
END

--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('48FC148C-299A-4717-BD03-401BB79C082E','Default Alert Level Group','Default Alert Level Group','{"$type":"Vanrise.Notification.Business.VRAlertLevelBEDefinitionSettings, Vanrise.Notification.Business","ConfigId":"0b44d3f3-aa62-4289-8eb3-d93269515036","DefinitionEditor":"vr-notification-alertleveldefinitionbe-editor","IdType":"System.Guid","ManagerFQTN":"Vanrise.Notification.Business.VRAlertLevelManager,Vanrise.Notification.Business","SelectorUIControl":"vr-notification-alertlevel-selector"}')
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

--[VRNotification].[VRAlertLevel]-------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[BusinessEntityDefinitionID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('37555139-97FB-485F-8C54-605C02E9B750','Low','48FC148C-299A-4717-BD03-401BB79C082E','{"$type":"Vanrise.Notification.Entities.VRAlertLevelSettings, Vanrise.Notification.Entities","StyleDefinitionId":"1e644b07-528a-47b5-a40a-a9e8a0fc868a","Weight":2}'),
('69855125-2734-4C6F-89D6-857271E500F4','Medium','48FC148C-299A-4717-BD03-401BB79C082E','{"$type":"Vanrise.Notification.Entities.VRAlertLevelSettings, Vanrise.Notification.Entities","StyleDefinitionId":"f3fee864-02ef-4c0b-a68d-d9aeb5bac07e","Weight":4}'),
('A6FAB761-C7F5-47E2-9BEF-9DCFD7A6C776','Critical','48FC148C-299A-4717-BD03-401BB79C082E','{"$type":"Vanrise.Notification.Entities.VRAlertLevelSettings, Vanrise.Notification.Entities","StyleDefinitionId":"61a682f3-e00c-4b31-b2f5-26dd5f5e4c2f","Weight":10}'),
('AC44957C-9E86-4D3C-884E-A542FFF3D1DF','Urgent','48FC148C-299A-4717-BD03-401BB79C082E','{"$type":"Vanrise.Notification.Entities.VRAlertLevelSettings, Vanrise.Notification.Entities","StyleDefinitionId":"fac30bbc-68b1-4e8e-b5de-93015285c012","Weight":8}'),
('8E8F4277-0B34-4861-A26C-CBD88B3DA5D0','High','48FC148C-299A-4717-BD03-401BB79C082E','{"$type":"Vanrise.Notification.Entities.VRAlertLevelSettings, Vanrise.Notification.Entities","StyleDefinitionId":"a6f96839-2922-4cee-b0f6-f026f8bd8c11","Weight":6}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[BusinessEntityDefinitionID],[Settings]))
merge	[VRNotification].[VRAlertLevel] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[BusinessEntityDefinitionID] = s.[BusinessEntityDefinitionID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[BusinessEntityDefinitionID],[Settings])
	values(s.[ID],s.[Name],s.[BusinessEntityDefinitionID],s.[Settings]);


--[logging].[LoggableEntity]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[UniqueName],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('5C1F441E-3D2B-4794-A7EC-01DC5E277BD2','VR_Notification_AlertRuleType','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Notification_AlertRuleType_ViewHistoryItem"}'),

('2638419F-506C-44F9-89FE-1DB7C9F246B3','VR_Notification_AlertRule_08427b54-b63a-4b75-b42b-56e40425707a','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Notification_AlertRule_ViewHistoryItem"}'),
('861CF8F4-DB3F-4369-8F65-02811F68BCAC','VR_Common_ComponentType_fdd73530-067f-4160-ab71-7852303c785c','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_ComponentType__ViewHistoryItem"}'),
('1ED6247E-F252-4B3D-B5FE-9A14C204F51B','VR_Common_ComponentType_d96f17c8-29d7-4c0c-88dc-9d5fbca2178f','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_ComponentType__ViewHistoryItem"}'),
('937F3844-B545-4C1F-AD8D-D741D59E3487','VR_Notification_AlertLevel','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Notification_AlertLevel_ViewHistoryItem"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[UniqueName],[Settings]))
merge	[logging].[LoggableEntity] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[UniqueName],[Settings])
	values(s.[ID],s.[UniqueName],s.[Settings]);