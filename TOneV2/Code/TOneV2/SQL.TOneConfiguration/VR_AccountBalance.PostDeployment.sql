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
--[common].[ExtensionConfiguration]-----------------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7824DFFA-0EBF-4939-93E8-DEC6E5EDFA10','VR_AccountBalance_AccountType','Account Balance Type','VR_Common_VRComponentType'													,'{"Editor":"vr-accountbalance-accounttype-settings"}'),
('F0A06C27-2850-40ED-BF6C-F0E65BD3894D','Daily','Daily','VR_AccountBalance_AccountUsagePeriodSettingsConfig'																,'{"Editor":"vr-accountusage-periodsettings-daily"}'),
('6336D88E-3460-4388-B56C-322FBC336129','Billing Transaction Synchronizer','Billing Transaction Synchronizer','VR_BEBridge_BESynchronizer'									,'{"Editor":"vr-accountbalance-billingtransaction-synchronizer"}'),
('BA79CB79-D058-4382-88FC-DB1C154B5374','VR_Notification_VRAlertRuleTypeSettings_AccountBalanceRule','Account Balance Rule Type','VR_Notification_VRAlertRuleTypeSettings'	,'{"Editor":"vr-accountbalance-accountbalancerule-settings"}'),
('0FC411D1-90FD-417C-BFDF-EC0C35B1A666','VR_Notification_AccountBalanceNotification_Setting','Account Balance Notification','VR_Notification_VRNotificationTypeSettings'	,'{"Editor":"vr-accountbalance-notificationtype-settings"}'),

('4DE3ADCE-B8D4-4266-868D-6C78CB3738DF','VR_AccountBalance_FinancialTransactionsView','Financial Transactions','VR_Security_ViewTypeConfig'					,'{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"vr-accountbalance-billingtransaction-vieweditor"}'),
('CD4A108F-AE74-424B-924C-A1FF0D353A36','VR_AccountBalance_AccountStatmentView','Account Statment','VR_Security_ViewTypeConfig'								,'{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"vr-accountbalance-accountstatement-vieweditor"}'),
('C057CFBA-1E29-4C86-BB24-E3B504562E77','VR_AccountBalance_AccountBalancesView','Account Balances','VR_Security_ViewTypeConfig'								,'{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"vr-accountbalance-accountbalances-vieweditor"}'),
('831CB917-CAD7-4BC8-99B8-4B2EB839B647','VRAccountBalance_AccountTypeSource_LiveBalances','Live Balances','VR_AccountBalance_AccountTypeSources'								,'{"Editor" : "vr-accountbalance-accounttype-source-livebalance"}'),
('143BBD63-6E9D-46F4-8B3C-D899E9966120','VRAccountBalance_AccountTypeSource_BillingTransactionSummary','Billing Transaction Summary','VR_AccountBalance_AccountTypeSources'								,'{"Editor" : "vr-accountbalance-accounttype-source-billingtransaction"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[ExtensionConfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);
----------------------------------------------------------------------------------------------------
END

--[sec].[BusinessEntityModule]-------------1801 to 1900---------------------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[Name],[ParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('520558FA-CF2F-440B-9B58-09C23B6A2E9B','Billing','5A9E78AE-229E-41B9-9DBF-492997B42B61',0),('692D0589-D764-4DF5-857B-52A98D89FFD6','Account Balance','04493174-83F0-44D6-BBE4-DBEB8B57875A',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[ParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[Name],[ParentId],[BreakInheritance])	values(s.[ID],s.[Name],s.[ParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
END

--sec.[View]------------------------------------------------------------------------------------------BEGIN--set nocount on;
--;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])--as (select * from (values----//////////////////////////////////////////////////////////////////////////////////////////////////--('BD521EE8-D42E-4AA0-81FE-04577E045CED','Account Balance Notifications','Account Balance Notifications',null,'94E29C06-8B09-4460-B495-5A7413C52C8C',null,null,null,'{"$type":"Vanrise.Notification.Entities.VRNotificationViewSettings, Vanrise.Notification.Entities","Settings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Notification.Entities.VRNotificationViewSettingItem, Vanrise.Notification.Entities]], mscorlib","$values":[{"$type":"Vanrise.Notification.Entities.VRNotificationViewSettingItem, Vanrise.Notification.Entities","VRNotificationTypeId":"6afcfad7-6e4e-4b0b-ae48-0e3c30c60654"}]}}','A196C40A-30B5-4297-B7B0-4344C41CE5A2',null,null)
----\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\--)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))--merge	[sec].[View] as t--using	cte_data as s--on		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]--when not matched by target then--	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])--	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);--------------------------------------------------------------------------------------------------------END

--[sec].[BusinessEntity]-------------------5701 to 6000-------------------------------------------------------
BEGIN
set nocount on;;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('2C404842-792F-4A6E-BC8A-3000455569C1','VR_AccountBalance_AccountBalances','Account Balance','520558FA-CF2F-440B-9B58-09C23B6A2E9B',0,'["View","Add Financial Transactions"]'),('3D6B033D-07A5-4F90-BF81-B295C95A27A7','VR_AccountBalance_Notification','Account Balance Notification','520558FA-CF2F-440B-9B58-09C23B6A2E9B',0,'["View Rules","Add Rules","Edit Rules","View Alerts"]'),('08FB93FA-0719-4385-AD9E-0513E3966B26','VR_AccountBalance_Processes','Account Balance'				,'B6B8F582-4759-43FB-9220-AA7662C366EA',0,'["Start Process", "View Process Logs"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
END

--[bp].[BPDefinition]----------------------6001 to 7000---------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[Name],[Title],[FQTN],[Config])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('431B9A84-3A72-4D6D-B3BA-60DD6CDF23DD','Vanrise.AccountBalance.BP.Arguments.AccountBalanceUpdateProcessInput','Account Balance Updater','Vanrise.AccountBalance.BP.AccountBalanceUpdateProcess, Vanrise.AccountBalance.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-accountbalance-updateprocess-manual","ScheduledExecEditor":"vr-accountbalance-updateprocess-scheduled","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.AccountBalance.Business.AccountBalanceUpdateProcessBPSettings, Vanrise.AccountBalance.Business"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Process Logs"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[FQTN],[Config]))merge	[bp].[BPDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]when not matched by target then	insert([ID],[Name],[Title],[FQTN],[Config])	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
----------------------------------------------------------------------------------------------------
END


--[sec].[SystemAction]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([Name],[RequiredPermissions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('VR_AccountBalance/AccountStatement/GetFilteredAccountStatments',null),('VR_AccountBalance/BillingTransaction/GetFilteredBillingTransactions',null),('VR_AccountBalance/BillingTransaction/AddBillingTransaction',null),('VR_AccountBalance/LiveBalance/GetFilteredAccountBalances',null)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Name],[RequiredPermissions]))merge	[sec].[SystemAction] as tusing	cte_data as son		1=1 and t.[Name] = s.[Name]when matched then	update set	[RequiredPermissions] = s.[RequiredPermissions]when not matched by target then	insert([Name],[RequiredPermissions])	values(s.[Name],s.[RequiredPermissions]);

--[VR_AccountBalance].[BillingTransactionType]----------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[IsCredit],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('007869D9-6DC2-4F56-88A4-18C8C442E49E','Voice Usage',0,'{"ManualAdditionDisabled":true}'),('F178D94D-D622-4EBF-A1BA-2A4AF1067D6B','Payment',1,'{"ManualAdditionDisabled":false}'),('9D4A06C5-CAD0-4D18-8A70-B45272A5B54F','Debit',0,'{"ManualAdditionDisabled":false}'),('2876BFBB-1F34-4CD2-92B8-E74B54E1041D','Credit ',1,'{"ManualAdditionDisabled":false}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[IsCredit],[Settings]))merge	[VR_AccountBalance].[BillingTransactionType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[IsCredit] = s.[IsCredit],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[IsCredit],[Settings])	values(s.[ID],s.[Name],s.[IsCredit],s.[Settings]);----[common].[VRComponentType]--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;--;with cte_data([ID],[Name],[ConfigID],[Settings])--as (select * from (values----//////////////////////////////////////////////////////////////////////////////////////////////////--('6AFCFAD7-6E4E-4B0B-AE48-0E3C30C60654','Account Balance Notification','FDD73530-067F-4160-AB71-7852303C785C','{"$type":"Vanrise.Notification.Entities.VRNotificationTypeSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"fdd73530-067f-4160-ab71-7852303c785c","ExtendedSettings":{"$type":"Vanrise.AccountBalance.MainExtensions.AccountBalanceNotification.AccountBalanceNotificationTypeSettings, Vanrise.AccountBalance.MainExtensions","ConfigId":"0fc411d1-90fd-417c-bfdf-ec0c35b1a666","SearchRuntimeEditor":"vr-accountbalance-notification-searcheditor","BodyRuntimeEditor":"vr-accountbalance-notification-bodyeditor"}}')----\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\--)c([ID],[Name],[ConfigID],[Settings]))--merge	[common].[VRComponentType] as t--using	cte_data as s--on		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[ConfigID] = s.[ConfigID],[Settings] = s.[Settings]--when not matched by target then--	insert([ID],[Name],[ConfigID],[Settings])--	values(s.[ID],s.[Name],s.[ConfigID],s.[Settings]);--[logging].[LoggableEntity]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[UniqueName],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('6AEED49A-B73A-4D61-808E-9352933A458A','VR_Common_ComponentType_7824dffa-0ebf-4939-93e8-dec6e5edfa10','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_ComponentType__ViewHistoryItem"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[UniqueName],[Settings]))merge	[logging].[LoggableEntity] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]when not matched by target then	insert([ID],[UniqueName],[Settings])	values(s.[ID],s.[UniqueName],s.[Settings]);