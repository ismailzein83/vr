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
Delete from [common].[VRComponentType] where ID='6AFCFAD7-6E4E-4B0B-AE48-0E3C30C60654'
--[common].[ExtensionConfiguration]-----------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('F0A06C27-2850-40ED-BF6C-F0E65BD3894D','Daily','Daily','VR_AccountBalance_AccountUsagePeriodSettingsConfig'																	,'{"Editor":"vr-accountusage-periodsettings-daily"}'),
('7DF9BEA6-6DCA-41DD-8C9A-A9D8AD323512','30Min','30 Min','VR_AccountBalance_AccountUsagePeriodSettingsConfig'																	,'{"Editor":"vr-accountusage-periodsettings-thirtymin"}'),

('7824DFFA-0EBF-4939-93E8-DEC6E5EDFA10','VR_AccountBalance_AccountType','Account Balance Type','VR_Common_VRComponentType'														,'{"Editor":"vr-accountbalance-accounttype-settings"}'),
('831CB917-CAD7-4BC8-99B8-4B2EB839B647','VRAccountBalance_AccountTypeSource_LiveBalances','Live Balances','VR_AccountBalance_AccountTypeSources'								,'{"Editor":"vr-accountbalance-accounttype-source-livebalance"}'),
('143BBD63-6E9D-46F4-8B3C-D899E9966120','VRAccountBalance_AccountTypeSource_BillingTransactionSummary','Billing Transaction Summary','VR_AccountBalance_AccountTypeSources'		,'{"Editor":"vr-accountbalance-accounttype-source-billingtransaction"}'),

('6336D88E-3460-4388-B56C-322FBC336129','Billing Transaction Synchronizer','Billing Transaction Synchronizer','VR_BEBridge_BESynchronizer'										,'{"Editor":"vr-accountbalance-billingtransaction-synchronizer"}'),
('BA79CB79-D058-4382-88FC-DB1C154B5374','VR_Notification_VRAlertRuleTypeSettings_AccountBalanceRule','Account Balance','VR_Notification_VRAlertRuleTypeSettings'				,'{"Editor":"vr-accountbalance-accountbalancerule-settings"}'),
('0FC411D1-90FD-417C-BFDF-EC0C35B1A666','VR_Notification_AccountBalanceNotification_Setting','Account Balance Notification','VR_Notification_VRNotificationTypeSettings'		,'{"Editor":"vr-accountbalance-notificationtype-settings"}'),

('4DE3ADCE-B8D4-4266-868D-6C78CB3738DF','VR_AccountBalance_FinancialTransactionsView','Financial Transactions','VR_Security_ViewTypeConfig'										,'{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"vr-accountbalance-billingtransaction-vieweditor"}'),
('CD4A108F-AE74-424B-924C-A1FF0D353A36','VR_AccountBalance_AccountStatmentView','Account Statment','VR_Security_ViewTypeConfig'													,'{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"vr-accountbalance-accountstatement-vieweditor"}'),
('C057CFBA-1E29-4C86-BB24-E3B504562E77','VR_AccountBalance_AccountBalancesView','Account Balances','VR_Security_ViewTypeConfig'													,'{"Editor":"/Client/Modules/Security/Views/View/GenericViewEditor.html","EnableAdd":true,"DirectiveEditor":"vr-accountbalance-accountbalances-vieweditor"}'),
('85BB0A94-C4C4-47B3-A575-3C630C0D000D','Billing Transaction','Billing Transaction','VR_Invoice_InvoiceType_RDLCDataSourceSettings','{"Editor":"vr-invoicetype-datasourcesettings-billingtransaction"}'),
('53E07D3A-3D37-4F81-BAD0-225713E41B9A','InvToAccBalanceRelationDefinition','Invoice To Account Balance Relation','VR_Common_VRComponentType','{"Editor":"vr-invtoaccbalancerelation-definition-settings"}')
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
END


--[sec].[BusinessEntityModule]-------------1801 to 1900---------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('520558FA-CF2F-440B-9B58-09C23B6A2E9B','Billing','5A9E78AE-229E-41B9-9DBF-492997B42B61',0),
('692D0589-D764-4DF5-857B-52A98D89FFD6','Account Balance','04493174-83F0-44D6-BBE4-DBEB8B57875A',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([ID],[Name],[ParentId],[BreakInheritance])
	values(s.[ID],s.[Name],s.[ParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
END


--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('223C3B00-530C-4D53-96FC-E2C8990456C3','BalanceAccountType','Balance Account Type','{"$type":"Vanrise.Common.Business.VRComponentTypeBESettings, Vanrise.Common.Business","ConfigId":"0873c4d8-c6cc-4ddb-acca-e8ec850c9186","DefinitionEditor":"vr-common-componenttypebe-editor","IdType":"System.Guid","ManagerFQTN":"Vanrise.Common.Business.VRComponentTypeManager, Vanrise.Common.Business","SelectorUIControl":"","VRComponentTypeConfigId":"7824dffa-0ebf-4939-93e8-dec6e5edfa10"}')
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


--[sec].[BusinessEntity]-------------------5701 to 6000-------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('2C404842-792F-4A6E-BC8A-3000455569C1','VR_AccountBalance_AccountBalances','Account Balance','520558FA-CF2F-440B-9B58-09C23B6A2E9B',0,'["View","Add Financial Transactions"]'),
('3D6B033D-07A5-4F90-BF81-B295C95A27A7','VR_AccountBalance_Notification','Account Balance Notification','520558FA-CF2F-440B-9B58-09C23B6A2E9B',0,'["View Rules","Add Rules","Edit Rules","View Alerts"]'),
('08FB93FA-0719-4385-AD9E-0513E3966B26','VR_AccountBalance_Processes','Account Balance'				,'B6B8F582-4759-43FB-9220-AA7662C366EA',0,'["Start Process", "View Process Logs"]')
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


--[bp].[BPDefinition]----------------------6001 to 7000---------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('CAE40A5B-3562-4C1B-AC22-57C1BE4CD158','Vanrise.Notification.BP.Arguments.BalanceAlertThresholdUpdateProcessInput','Balance Alert Threshold Updater','Vanrise.Notification.BP.BalanceAlertThresholdUpdateProcess,Vanrise.Notification.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-notification-balancealertrule-alertthresholdprocess-manual","ScheduledExecEditor":"vr-notification-balancealertrule-alertthresholdprocess-scheduled","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.Notification.Business.BalanceAlertThresholdUpdateBPDefinitionSettings, Vanrise.Notification.Business"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Process Logs"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}}}'),
('7B797534-6BD7-4E78-9C21-8791251CA96D','Vanrise.Notification.BP.Arguments.BalanceAlertCheckerProcessInput','Balance Alert Checker','Vanrise.Notification.BP.BalanceAlertCheckerProcess,Vanrise.Notification.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"vr-notification-balancealertrule-alertcheckerprocess-manual","ScheduledExecEditor":"vr-notification-balancealertrule-alertcheckerprocess-scheduled","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":true,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.Notification.Business.BalanceAlertCheckerBPDefinitionSettings, Vanrise.Notification.Business"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Process Logs"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}}}'),
('431B9A84-3A72-4D6D-B3BA-60DD6CDF23DD','Vanrise.AccountBalance.BP.Arguments.AccountBalanceUpdateProcessInput','Account Balance Updater','Vanrise.AccountBalance.BP.AccountBalanceUpdateProcess, Vanrise.AccountBalance.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-accountbalance-updateprocess-manual","ScheduledExecEditor":"vr-accountbalance-updateprocess-scheduled","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.AccountBalance.Business.AccountBalanceUpdateProcessBPSettings, Vanrise.AccountBalance.Business"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View Process Logs"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}}}')
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
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_AccountBalance/AccountStatement/GetFilteredAccountStatments',null),
('VR_AccountBalance/BillingTransaction/GetFilteredBillingTransactions',null),
('VR_AccountBalance/BillingTransaction/AddBillingTransaction',null),
('VR_AccountBalance/LiveBalance/GetFilteredAccountBalances',null)
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

--[VR_AccountBalance].[BillingTransactionType]------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[IsCredit],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('007869D9-6DC2-4F56-88A4-18C8C442E49E','Voice Usage',0,'{"ManualAdditionDisabled":true}'),
('2b3d86ab-1689-49e8-a5fa-f65227a1ec4c','Invoice',0,'{"ManualAdditionDisabled":true}'),
('F178D94D-D622-4EBF-A1BA-2A4AF1067D6B','Payment',1,'{"ManualAdditionDisabled":false}'),
('9D4A06C5-CAD0-4D18-8A70-B45272A5B54F','Debit',0,'{"ManualAdditionDisabled":false}'),
('2876BFBB-1F34-4CD2-92B8-E74B54E1041D','Credit ',1,'{"ManualAdditionDisabled":false}'),
('D195F3C3-968C-4D93-9177-2056212C7EF6','Credit Note',1,'{"ManualAdditionDisabled":false}'),
('66C2CC21-9EC7-4EFA-BE80-E7AC2EA8FE00','Debit Note',0,'{"ManualAdditionDisabled":false}'),
('D243ADCD-D02A-4EB6-8B0D-206358391C18','Customer Payment',1,'{"ManualAdditionDisabled":false}'),
('DBEA500E-C85A-4F4A-BB4C-2F6E8CB02F7B','Supplier Voice Usage',1,'{"ManualAdditionDisabled":true}'),
('7B2BCDF9-70CB-4EE3-8EB7-763BC44CCDD6','Supplier Payment',0,'{"ManualAdditionDisabled":false}'),
('ACD5923A-8057-4C21-9E7B-EBC76B9CE1DF','Customer Voice Usage',0,'{"ManualAdditionDisabled":true}'),
('3a9a321f-754f-40ae-a8f8-e06f299607f4','Customer Invoice',0,'{"ManualAdditionDisabled":true}'),
('56cd572c-3815-4dfb-b1ed-ab8a550d5862','Supplier Invoice',1,'{"ManualAdditionDisabled":true}'),
('e544addf-9b84-4360-bc96-1cfeca4634bd','Customer Expenses Charge',0,'{"ManualAdditionDisabled":false}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[IsCredit],[Settings]))
merge	[VR_AccountBalance].[BillingTransactionType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[IsCredit] = s.[IsCredit],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[IsCredit],[Settings])
	values(s.[ID],s.[Name],s.[IsCredit],s.[Settings]);

--[logging].[LoggableEntity]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[UniqueName],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('6AEED49A-B73A-4D61-808E-9352933A458A','VR_Common_ComponentType_7824dffa-0ebf-4939-93e8-dec6e5edfa10','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_ComponentType__ViewHistoryItem"}')
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


--[genericdata].[DataRecordType]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('5DFD12AB-69BF-42FD-962F-CCF81261D934','UpdateBalanceRecord',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"BalanceAccountTypeId","Title":"Balance Account Type Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","BusinessEntityDefinitionId":"223c3b00-530c-4d53-96fc-e2c8990456c3","IsNullable":true,"ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"BalanceAccountId","Title":"Balance Account Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","OrderType":1,"ViewerEditor":"vr-genericdata-fieldtype-text-viewereditor","DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"FinancialAccountId","Title":"Financial Account Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":2,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"EffectiveOn","Title":"Effective On","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","RuntimeEditor":"vr-genericdata-fieldtype-datetime-runtimeeditor","DataType":0,"IsNullable":false,"ViewerEditor":"vr-genericdata-fieldtype-datetime-viewereditor","DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"Amount","Title":"Amount","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":0,"IsNullable":true,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"CurrencyId","Title":"Currency Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","BusinessEntityDefinitionId":"d41ea344-c3c0-4203-8583-019b6b3edb76","IsNullable":true,"ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"TransactionTypeId","Title":"Transaction Type Id","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldGuidType, Vanrise.GenericData.MainExtensions","ConfigId":"ebd22f77-6275-4194-8710-7bf3063dcb68","RuntimeEditor":"vr-genericdata-fieldtype-guid-runtimeeditor","OrderType":1,"ViewerEditor":"vr-genericdata-fieldtype-guid-viewereditor","IsNullable":false,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false}]}',null,'{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities","DateTimeField":"EffectiveOn"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings]))
merge	[genericdata].[DataRecordType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentID] = s.[ParentID],[Fields] = s.[Fields],[ExtraFieldsEvaluator] = s.[ExtraFieldsEvaluator],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
	values(s.[ID],s.[Name],s.[ParentID],s.[Fields],s.[ExtraFieldsEvaluator],s.[Settings]);