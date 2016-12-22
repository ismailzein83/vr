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
('FBFE2B36-12F6-40C1-8163-26CFE2D23501','Show warning','Show warning','VR_BP_BPBusinessRuleActionType','{ "Description":"Show warning", "Editor":"businessprocess-bp-business-rule-warning-item-action"}'),
('715F7F90-2C23-4185-AEB8-EDA947DE3978','Stop execution','Stop execution','VR_BP_BPBusinessRuleActionType','{"Description":"Stop execution", "Editor":"businessprocess-bp-business-rule-stop-execution-action"}'),
('2C3ED299-C955-44B3-A708-E9B53A24CB0E','Select Minimum BED','Select Minimum BED','VR_BP_BPBusinessRuleActionType','{"Description":"Select Minimum BED","Editor":"whs-spl-business-rule-action-selectserviceminbed"}'),
('BA3427FE-B8BE-4546-B433-CE0D8CE9FCB1','Exclude','Exclude','VR_BP_BPBusinessRuleActionType','{ "Description":"Exclude", "Editor":"businessprocess-bp-business-rule-exclude-item-action"}'),
('72C926F1-D019-408F-84AF-6613D2033473','Show information','Show information','VR_BP_BPBusinessRuleActionType','{"Description":"Show information","Editor":"businessprocess-bp-business-rule-information-action"}')
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

--[sec].[Module]---------------------------601 to 700---------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B','Business Processes',null,'1037157D-BBC9-4B28-B53F-908936CEC137',null,20,0)--'System Processes'
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[Id],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
--------------------------------------------------------------------------------------------------------------
end

DELETE FROM [sec].[View] WHERE [ID] IN ('AA0B6783-ED68-41E8-B7C9-99BE96672476','79E759A9-3324-4B82-87E3-2065CD86C3F7','B23B61C1-D5AE-44A9-BD6E-F3009166395B','3C53D6DC-BBE7-49CA-A222-8211AF25DD31')
--[sec].[View]-----------------------------6001 to 7000-------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1D10975D-5B1D-4DDC-BBF2-F8F9BA89FF5A','Management','Business Processes Management','#/view/BusinessProcess/Views/BPDefinition/BPDefinitionManagement','B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B',null,null,null,'{"$type":"Vanrise.BusinessProcess.Entities.BPViewSettings, Vanrise.BusinessProcess.Entities"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,2),
--('AA0B6783-ED68-41E8-B7C9-99BE96672476','Monitor','Monitor','#/view/BusinessProcess/Views/BPInstance/BPInstanceMonitor','B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B','BusinessProcess_BP/BPInstance/GetUpdated',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,3),
--('79E759A9-3324-4B82-87E3-2065CD86C3F7','Log','Log History','#/view/BusinessProcess/Views/BPInstance/BPInstanceHistory','B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B','BusinessProcess_BP/BPInstance/GetFilteredBPInstances',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,4),
--('B23B61C1-D5AE-44A9-BD6E-F3009166395B','My Tasks','Tasks','#/view/BusinessProcess/Views/BPTask/BPTaskMonitor','B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B','BusinessProcess_BP/BPTask/GetMyUpdatedTasks',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,5),
--('3C53D6DC-BBE7-49CA-A222-8211AF25DD31','Business Rules','Business Rules','#/view/BusinessProcess/Views/BPBusinessRule/BPBusinessRuleSetManagement','B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B','BusinessProcess_BP/BPBusinessRuleSet/GetFilteredBPBusinessRuleSets',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,6),
('BEEEEB03-8CB2-44F4-9769-A95B46D4F40D','BP Technical Definition','BP Technical Definition','#/view/BusinessProcess/Views/BPDefinition/BPTechnicalDefinitionManagement','FC9D12D3-9CBF-4D99-8748-5C2BDD6C5ED9','BusinessProcess_BP/BPDefinition/GetFilteredBPDefinitionsForTechnical',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);
--------------------------------------------------------------------------------------------------------------
end

DELETE FROM [sec].[BusinessEntityModule] WHERE [ID] = 'E53820F4-9736-4202-970F-F40C660EC08E'
--[sec].[BusinessEntityModule]-------------601 to 700---------------------------------------------------------
begin
set nocount on;;with cte_data([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('0C719C34-F553-4B90-8013-D967A57FAB14',601,'Business Process','B6B8F582-4759-43FB-9220-AA7662C366EA',2,0),('04493174-83F0-44D6-BBE4-DBEB8B57875A',602,'WorkFlows','B6B8F582-4759-43FB-9220-AA7662C366EA',601,0)--,--('E53820F4-9736-4202-970F-F40C660EC08E',603,'Business Process','954705D8-ABC5-41AB-BDB2-FEC686C7BE09',-1,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[ParentId] = s.[ParentId],[OldParentId] = s.[OldParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])	values(s.[ID],s.[OldId],s.[Name],s.[ParentId],s.[OldParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]-------------------1501 to 1800---------------------------------------------------------
begin
set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('E1A6D6CB-0433-4B82-9945-A25887700823',1501,'BusinessProcess_BP_BPDefinition','Management'							,'0C719C34-F553-4B90-8013-D967A57FAB14',601,0,'["View"]'),('5658DA29-1AAC-4CC2-A621-BD96816E933E',1502,'BusinessProcess_BP_BPTask','BPTask'									,'0C719C34-F553-4B90-8013-D967A57FAB14',601,0,'["View"]'),('6EAFABC4-BDB8-469E-B215-C1D82B1BEA47',1503,'BusinessProcess_BP_BPInstance_Log','BPInstance Tracking'				,'0C719C34-F553-4B90-8013-D967A57FAB14',601,0,'["View","Monitor"]'),('F6A8698F-6056-4CD5-BE9F-EA34BF6B8716',1504,'BusinessProcess_BP_BusinessRuleSet','Business Rule Set'				,'0C719C34-F553-4B90-8013-D967A57FAB14',601,0,'["View", "Add", "Edit"]'),('2BD8386C-5ADB-4E80-83A3-740C19F042F2',1505,'BusinessProcess_BP_BPTechnicalDefinition','BP Technical Definition'	,'954705D8-ABC5-41AB-BDB2-FEC686C7BE09',603,0,'["View", "Edit"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]----------------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('BusinessProcess_BP/BPDefinition/GetFilteredBPDefinitions',null),
('BusinessProcess_BP/BPDefinition/GetBPDefinitionsInfo',null),
('BusinessProcess_BP/BPDefinition/GetBPDefintion',null),
('BusinessProcess_BP/BPDefinition/GetDefinitions',null),
('BusinessProcess_BP/BPInstance/GetUpdated','BusinessProcess_BP_BPInstance_Log: Monitor'),
('BusinessProcess_BP/BPInstance/GetBeforeId',null),
('BusinessProcess_BP/BPInstance/GetFilteredBPInstances','BusinessProcess_BP_BPInstance_Log: View'),
('BusinessProcess_BP/BPInstance/GetBPInstance',null),
('BusinessProcess_BP/BPInstance/CreateNewProcess',null),
('BusinessProcess_BP/BPInstanceTracking/GetFilteredBPInstanceTracking',null),
('BusinessProcess_BP/BPInstanceTracking/GetUpdated',null),
('BusinessProcess_BP/BPInstanceTracking/GetBeforeId',null),
('BusinessProcess_BP/BPTask/GetProcessTaskUpdated',null),
('BusinessProcess_BP/BPTask/GetProcessTaskBeforeId',null),
('BusinessProcess_BP/BPTask/GetMyUpdatedTasks','BusinessProcess_BP_BPTask: View'),
('BusinessProcess_BP/BPTask/GetMyTasksBeforeId',null),
('BusinessProcess_BP/BPTask/ExecuteTask',null),
('BusinessProcess_BP/BPTask/GetTask',null),
('BusinessProcess_BP/BPTaskType/GetBPTaskTypeByTaskId',null),
('BusinessProcess_BP/BPValidationMessage/GetUpdated',null),
('BusinessProcess_BP/BPValidationMessage/GetBeforeId',null),
('BusinessProcess_BP/BPValidationMessage/GetFilteredBPValidationMessage',null),
('BusinessProcess_BP/BPBusinessRuleSet/GetFilteredBPBusinessRuleSets','BusinessProcess_BP_BusinessRuleSet: View'),
('BusinessProcess_BP/BPBusinessRuleSet/UpdateBusinessRuleSet','BusinessProcess_BP_BusinessRuleSet: Edit'),
('BusinessProcess_BP/BPBusinessRuleSet/AddBusinessRuleSet','BusinessProcess_BP_BusinessRuleSet: Add'),
('BusinessProcess_BP/BPDefinition/GetFilteredBPDefinitionsForTechnical','BusinessProcess_BP_BPTechnicalDefinition: View'),
('BusinessProcess_BP/BPDefinition/UpdateBPDefinition','BusinessProcess_BP_BPTechnicalDefinition: Edit')
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
--------------------------------------------------------------------------------------------------------------
end

