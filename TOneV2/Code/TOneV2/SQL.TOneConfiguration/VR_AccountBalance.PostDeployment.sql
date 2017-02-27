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
('0FC411D1-90FD-417C-BFDF-EC0C35B1A666','VR_Notification_AccountBalanceNotification_Setting','Account Balance Notification','VR_Notification_VRNotificationTypeSettings'	,'{"Editor" : "vr-accountbalance-notificationtype-settings"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[ExtensionConfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);
----------------------------------------------------------------------------------------------------
END

--[sec].[BusinessEntityModule]-------------1801 to 1900---------------------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[OldId],[Name],[OldParentId],[ParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('520558FA-CF2F-440B-9B58-09C23B6A2E9B',null,'Billing',1,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0),('692D0589-D764-4DF5-857B-52A98D89FFD6',1801,'Account Balance',602	,'04493174-83F0-44D6-BBE4-DBEB8B57875A',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[OldParentId],[ParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[OldParentId] = s.[OldParentId],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[OldId],[Name],[OldParentId],[ParentId],[BreakInheritance])	values(s.[ID],s.[OldId],s.[Name],s.[OldParentId],s.[ParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
END

--sec.[View]------------------------------------------------------------------------------------------BEGIN--set nocount on;
--;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])--as (select * from (values----//////////////////////////////////////////////////////////////////////////////////////////////////--('BD521EE8-D42E-4AA0-81FE-04577E045CED','Account Balance Notifications','Account Balance Notifications',null,'94E29C06-8B09-4460-B495-5A7413C52C8C',null,null,null,'{"$type":"Vanrise.Notification.Entities.VRNotificationViewSettings, Vanrise.Notification.Entities","Settings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Notification.Entities.VRNotificationViewSettingItem, Vanrise.Notification.Entities]], mscorlib","$values":[{"$type":"Vanrise.Notification.Entities.VRNotificationViewSettingItem, Vanrise.Notification.Entities","VRNotificationTypeId":"6afcfad7-6e4e-4b0b-ae48-0e3c30c60654"}]}}','A196C40A-30B5-4297-B7B0-4344C41CE5A2',null,null,null)
----\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\--)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))--merge	[sec].[View] as t--using	cte_data as s--on		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]--when not matched by target then--	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])--	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);--------------------------------------------------------------------------------------------------------END

delete from [sec].[BusinessEntity] where [Id] in ('ABFCB015-204A-401A-BE50-86AAF3C50D27','2976F872-6FD5-4D2D-AD75-DC522F6EA630')
--[sec].[BusinessEntity]-------------------5701 to 6000-------------------------------------------------------
BEGIN
set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('2C404842-792F-4A6E-BC8A-3000455569C1',null,'VR_AccountBalance_AccountBalances','Account Balances, Statement, and Transactions','520558FA-CF2F-440B-9B58-09C23B6A2E9B',null,0,'["View","Add"]'),('08FB93FA-0719-4385-AD9E-0513E3966B26',5701,'BusinessProcess_BP_Account_Balance','Account Balance'				,'692D0589-D764-4DF5-857B-52A98D89FFD6',1801,0,'["View", "StartInstance", "ScheduleTask"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
END

DELETE FROM [bp].[BPDefinition] WHERE [ID] IN('DE4C0120-627F-447C-A942-27BAB2AB8F3E','B90D9D2A-5A8F-401F-9078-A190C4DF8E2A')
--[bp].[BPDefinition]----------------------6001 to 7000---------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[OldID],[Name],[Title],[FQTN],[Config])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('431B9A84-3A72-4D6D-B3BA-60DD6CDF23DD',6001,'Vanrise.AccountBalance.BP.Arguments.AccountBalanceUpdateProcessInput','Account Balance Updater','Vanrise.AccountBalance.BP.AccountBalanceUpdateProcess, Vanrise.AccountBalance.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-accountbalance-updateprocess-manual","ScheduledExecEditor":"vr-accountbalance-updateprocess-scheduled","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldID],[Name],[Title],[FQTN],[Config]))merge	[bp].[BPDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldID] = s.[OldID],[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]when not matched by target then	insert([ID],[OldID],[Name],[Title],[FQTN],[Config])	values(s.[ID],s.[OldID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
----------------------------------------------------------------------------------------------------
END


--[sec].[SystemAction]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([Name],[RequiredPermissions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('VR_AccountBalance/AccountStatement/GetFilteredAccountStatments','VR_AccountBalance_AccountBalances: View'),('VR_AccountBalance/BillingTransaction/GetFilteredBillingTransactions','VR_AccountBalance_AccountBalances: View'),('VR_AccountBalance/BillingTransaction/AddBillingTransaction','VR_AccountBalance_AccountBalances: Add'),('VR_AccountBalance/LiveBalance/GetFilteredAccountBalances','VR_AccountBalance_AccountBalances: View')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Name],[RequiredPermissions]))merge	[sec].[SystemAction] as tusing	cte_data as son		1=1 and t.[Name] = s.[Name]when matched then	update set	[RequiredPermissions] = s.[RequiredPermissions]when not matched by target then	insert([Name],[RequiredPermissions])	values(s.[Name],s.[RequiredPermissions]);

--[VR_AccountBalance].[BillingTransactionType]----------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[IsCredit],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('007869D9-6DC2-4F56-88A4-18C8C442E49E','Voice Usage',0,'{"ManualAdditionDisabled":true}'),('F178D94D-D622-4EBF-A1BA-2A4AF1067D6B','Payment',1,'{"ManualAdditionDisabled":false}'),('9D4A06C5-CAD0-4D18-8A70-B45272A5B54F','Debit',0,'{"ManualAdditionDisabled":false}'),('2876BFBB-1F34-4CD2-92B8-E74B54E1041D','Credit ',1,'{"ManualAdditionDisabled":false}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[IsCredit],[Settings]))merge	[VR_AccountBalance].[BillingTransactionType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[IsCredit] = s.[IsCredit],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[IsCredit],[Settings])	values(s.[ID],s.[Name],s.[IsCredit],s.[Settings]);--[common].[VRComponentType]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[ConfigID],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('6AFCFAD7-6E4E-4B0B-AE48-0E3C30C60654','Account Balance Notification','FDD73530-067F-4160-AB71-7852303C785C','{"$type":"Vanrise.Notification.Entities.VRNotificationTypeSettings, Vanrise.Notification.Entities","VRComponentTypeConfigId":"fdd73530-067f-4160-ab71-7852303c785c","ExtendedSettings":{"$type":"Vanrise.AccountBalance.MainExtensions.AccountBalanceNotification.AccountBalanceNotificationTypeSettings, Vanrise.AccountBalance.MainExtensions","ConfigId":"0fc411d1-90fd-417c-bfdf-ec0c35b1a666","SearchRuntimeEditor":"vr-accountbalance-notification-searcheditor","BodyRuntimeEditor":"vr-accountbalance-notification-bodyeditor"}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[ConfigID],[Settings]))merge	[common].[VRComponentType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[ConfigID] = s.[ConfigID],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[ConfigID],[Settings])	values(s.[ID],s.[Name],s.[ConfigID],s.[Settings]);