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
('57033E80-65CB-4359-95F6-22A57084D027','VR_Notification_VRAlertRuleTypeSettings_DAProfCalcAlertRuleTypeSettings','Profiling and Calculation Alert Rule Type','VR_Notification_VRAlertRuleTypeSettings','{"Editor":"vr-analytic-daprofcalc-alertruletypesettings"}')
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

--[sec].[BusinessEntityModule]-------------1901 to 2000---------------------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('3E1D06D6-1B60-4BA1-957A-CFE6CA168E5A',1901,'Notification','04493174-83F0-44D6-BBE4-DBEB8B57875A',602,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[ParentId] = s.[ParentId],[OldParentId] = s.[OldParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])	values(s.[ID],s.[OldId],s.[Name],s.[ParentId],s.[OldParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
END

--[sec].[BusinessEntity]-------------------6001 to 6300-------------------------------------------------------
BEGIN
set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('FE898B2B-4D50-4926-A6EF-207EDBACC21A',6001,'BusinessProcess_BP_Create_Action_Notification','Create Action Notification','3E1D06D6-1B60-4BA1-957A-CFE6CA168E5A',1901,0,'["View", "StartInstance", "ScheduleTask"]'),('FF0EDE69-EC7B-4C9E-944D-E580E8323CED',6002,'VR_Notification_VRAlertRuleType','Action Rule Type','7913ACD9-38C5-43B3-9612-BEFF66606F22',2,0,'["View","Add","Edit"]'),('695A74F9-5352-4835-B7F7-22EE9B077BB8',6003,'VR_Notification_VRAlertRule','Action Rule','9BBD7C00-011D-4AC9-8B25-36D3E2A8F7CF',2,0,'["View","Add","Edit"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
END



Delete from [bp].[BPDefinition] where ID = '6BF82D5D-9012-43B5-9DB9-7B6E4A2FED85'
--[bp].[BPDefinition]----------------------7001 to 8000---------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[OldID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('0E36B76B-44B2-4976-9477-157DB0514069',null,'Vanrise.Notification.BP.Arguments.ExecuteNotificationProcessInput','Execute Notification Process','Vanrise.Notification.BP.ExecuteNotificationProcess,Vanrise.Notification.BP','{
  "$type": "Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities",
  "MaxConcurrentWorkflows": 1,
  "IsPersistable": false,
  "HasChildProcesses": false,
  "HasBusinessRules": true,
  "NotVisibleInManagementScreen": true
}'),
('CAE40A5B-3562-4C1B-AC22-57C1BE4CD158',null,'Vanrise.Notification.BP.Arguments.BalanceAlertThresholdUpdateProcessInput','Balance Alert Threshold Update','Vanrise.Notification.BP.BalanceAlertThresholdUpdateProcess,Vanrise.Notification.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"","ScheduledExecEditor":"","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false}'),
('C508FE10-DDFA-4697-80FF-CD16042AF683',null,'Vanrise.Notification.BP.Arguments.ClearNotificationInput','Clear Notification Process','Vanrise.Notification.BP.ClearNotificationProcess,Vanrise.Notification.BP','{
  "$type": "Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities",
  "MaxConcurrentWorkflows": 1,
  "IsPersistable": false,
  "HasChildProcesses": false,
  "HasBusinessRules": true,
  "NotVisibleInManagementScreen": true
}	'),
('431B9A84-3A72-4D6D-B3BA-60DD6CDF23DD',6001,'Vanrise.AccountBalance.BP.Arguments.AccountBalanceUpdateProcessInput','Account Balance','Vanrise.AccountBalance.BP.AccountBalanceUpdateProcess, Vanrise.AccountBalance.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-accountbalance-updateprocess-manual","ScheduledExecEditor":"vr-accountbalance-updateprocess-scheduled","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"08FB93FA-0719-4385-AD9E-0513E3966B26","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('7B797534-6BD7-4E78-9C21-8791251CA96D',null,'Vanrise.Notification.BP.Arguments.BalanceAlertCheckerProcessInput','Balance Alert Checker','Vanrise.Notification.BP.BalanceAlertCheckerProcess,Vanrise.Notification.BP','{
  "$type": "Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities",
  "MaxConcurrentWorkflows": 1,
  "IsPersistable": false,
  "HasChildProcesses": false,
  "HasBusinessRules": true,
  "NotVisibleInManagementScreen": false
}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[OldID],[Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[OldID] = s.[OldID],[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([ID],[OldID],[Name],[Title],[FQTN],[Config])
	values(s.[ID],s.[OldID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
----------------------------------------------------------------------------------------------------
END


--sec.[View]----------------------------------------------------------------------------------------BEGINset nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('05234A49-DFA3-44C3-A552-80E6C793A67A','Action Rule Types','Action Rule Type','#/view/VR_Notification/Views/VRAlertRuleType/VRAlertRuleTypeManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VR_Notification/VRAlertRuleType/GetFilteredVRAlertRuleTypes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,45),('FF5E429A-CFF0-4095-8AD0-987FD3CAD502','Action Rules','Action Rule','#/view/VR_Notification/Views/VRAlertRule/VRAlertRuleManagement','1C7569FA-43C9-4853-AE4C-1152746A34FD','VR_Notification/VRAlertRule/GetFilteredVRAlertRules',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,100)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);----------------------------------------------------------------------------------------------------END--[sec].[SystemAction]------------------------------------------------------------------------------BEGINset nocount on;;with cte_data([Name],[RequiredPermissions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('VR_Notification/VRAlertRuleType/GetFilteredVRAlertRuleTypes','VR_Notification_VRAlertRuleType: View'),('VR_Notification/VRAlertRuleType/GetVRAlertRuleType',null),('VR_Notification/VRAlertRuleType/AddVRAlertRuleType','VR_Notification_VRAlertRuleType: Add'),('VR_Notification/VRAlertRuleType/UpdateVRAlertRuleType','VR_Notification_VRAlertRuleType: Edit'),('VR_Notification/VRAlertRuleType/GetVRAlertRuleTypeSettingsExtensionConfigs',null),('VR_Notification/VRAlertRuleType/GetVRAlertRuleTypesInfo',null),('VR_Notification/VRAlertRule/GetFilteredVRAlertRules','VR_Notification_VRAlertRule: View'),('VR_Notification/VRAlertRule/GetVRAlertRule',null),('VR_Notification/VRAlertRule/AddVRAlertRule','VR_Notification_VRAlertRule: Add'),('VR_Notification/VRAlertRule/UpdateVRAlertRule','VR_Notification_VRAlertRule: Edit')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Name],[RequiredPermissions]))merge	[sec].[SystemAction] as tusing	cte_data as son		1=1 and t.[Name] = s.[Name]when matched then	update set	[RequiredPermissions] = s.[RequiredPermissions]when not matched by target then	insert([Name],[RequiredPermissions])	values(s.[Name],s.[RequiredPermissions]);	----------------------------------------------------------------------------------------------------END