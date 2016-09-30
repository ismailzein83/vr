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

--[sec].[BusinessEntityModule]-------------1901 to 2000---------------------------------------------------------
BEGIN
set nocount on;;with cte_data([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('3E1D06D6-1B60-4BA1-957A-CFE6CA168E5A',1901,'Notification','04493174-83F0-44D6-BBE4-DBEB8B57875A',602,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[ParentId] = s.[ParentId],[OldParentId] = s.[OldParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])	values(s.[ID],s.[OldId],s.[Name],s.[ParentId],s.[OldParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
END

--[sec].[BusinessEntity]-------------------6001 to 6300-------------------------------------------------------
BEGIN
set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('FE898B2B-4D50-4926-A6EF-207EDBACC21A',6001,'BusinessProcess_BP_Create_Action_Notification','Create Action Notification','3E1D06D6-1B60-4BA1-957A-CFE6CA168E5A',1901,0,'["View", "StartInstance", "ScheduleTask"]'),('FF0EDE69-EC7B-4C9E-944D-E580E8323CED',6002,'VR_Notification_VRAlertRuleType','Alert Rule Type','61451603-E7B9-40C6-AE27-6CBA974E1B3B',2,0,'["View","Add","Edit"]'),('695A74F9-5352-4835-B7F7-22EE9B077BB8',6003,'VR_Notification_VRAlertRule','Alert Rule','61451603-E7B9-40C6-AE27-6CBA974E1B3B',2,0,'["View","Add","Edit"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
END


--[bp].[BPDefinition]----------------------7001 to 8000---------------------------------------------
BEGIN
set nocount on;
set identity_insert [bp].[BPDefinition] on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(7001,'Vanrise.Notification.BP.Arguments.CreateActionProcessInput','Create Action Notification','Vanrise.Notification.BP.CreateActionProcess, Vanrise.Notification.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"","ScheduledExecEditor":"","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":true,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":6001,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":6001,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":6001,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}')
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
set identity_insert [bp].[BPDefinition] off;
----------------------------------------------------------------------------------------------------
END


--sec.[View]----------------------------------------------------------------------------------------BEGINset nocount on;;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('05234A49-DFA3-44C3-A552-80E6C793A67A','Alert Rule Types','Alert Rule Type','#/view/VR_Notification/Views/VRAlertRuleType/VRAlertRuleTypeManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VR_Notification/VRAlertRuleType/GetFilteredVRAlertRuleTypes',null,null,null,0,45),('FF5E429A-CFF0-4095-8AD0-987FD3CAD502','Alert Rules','Alert Rule','#/view/VR_Notification/Views/VRAlertRule/VRAlertRuleManagement','50624672-CD25-44FD-8580-0E3AC8E34C71','VR_Notification/VRAlertRule/GetFilteredVRAlertRules',null,null,null,0,100)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);----------------------------------------------------------------------------------------------------END--[sec].[SystemAction]------------------------------------------------------------------------------BEGINset nocount on;;with cte_data([Name],[RequiredPermissions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('VR_Notification/VRAlertRuleType/GetFilteredVRAlertRuleTypes','VR_Notification_VRAlertRuleType: View'),('VR_Notification/VRAlertRuleType/GetVRAlertRuleType',null),('VR_Notification/VRAlertRuleType/AddVRAlertRuleType','VR_Notification_VRAlertRuleType: Add'),('VR_Notification/VRAlertRuleType/UpdateVRAlertRuleType','VR_Notification_VRAlertRuleType: Edit'),('VR_Notification/VRAlertRuleType/GetVRAlertRuleTypeSettingsExtensionConfigs',null),('VR_Notification/VRAlertRuleType/GetVRAlertRuleTypesInfo',null),('VR_Notification/VRAlertRule/GetFilteredVRAlertRules','VR_Notification_VRAlertRule: View'),('VR_Notification/VRAlertRule/GetVRAlertRule',null),('VR_Notification/VRAlertRule/AddVRAlertRule','VR_Notification_VRAlertRule: Add'),('VR_Notification/VRAlertRule/UpdateVRAlertRule','VR_Notification_VRAlertRule: Edit')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Name],[RequiredPermissions]))merge	[sec].[SystemAction] as tusing	cte_data as son		1=1 and t.[Name] = s.[Name]when matched then	update set	[RequiredPermissions] = s.[RequiredPermissions]when not matched by target then	insert([Name],[RequiredPermissions])	values(s.[Name],s.[RequiredPermissions]);	----------------------------------------------------------------------------------------------------END