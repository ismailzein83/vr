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
--[common].[ExtensionConfiguration]-----------------------------------------------------------------
BEGIN
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
----------------------------------------------------------------------------------------------------
END

--[sec].[Module]---------------------------601 to 700---------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B','Business Processes',null,'1037157D-BBC9-4B28-B53F-908936CEC137',null,20,0)
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

--[sec].[View]-----------------------------6001 to 7000-------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1D10975D-5B1D-4DDC-BBF2-F8F9BA89FF5A','Management','Business Processes Management','#/view/BusinessProcess/Views/BPDefinition/BPDefinitionManagement','B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B',null,null,null,'{"$type":"Vanrise.BusinessProcess.Entities.BPViewSettings, Vanrise.BusinessProcess.Entities"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2),
--('B23B61C1-D5AE-44A9-BD6E-F3009166395B','My Tasks','Tasks','#/view/BusinessProcess/Views/BPTask/BPTaskMonitor','B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B','BusinessProcess_BP/BPTask/GetMyUpdatedTasks',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5),
--('3C53D6DC-BBE7-49CA-A222-8211AF25DD31','Business Rules','Business Rules','#/view/BusinessProcess/Views/BPBusinessRule/BPBusinessRuleSetManagement','B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B','BusinessProcess_BP/BPBusinessRuleSet/GetFilteredBPBusinessRuleSets',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',6),
('BEEEEB03-8CB2-44F4-9769-A95B46D4F40D','BP Technical Definition','BP Technical Definition','#/view/BusinessProcess/Views/BPDefinition/BPTechnicalDefinitionManagement','FC9D12D3-9CBF-4D99-8748-5C2BDD6C5ED9','BusinessProcess_BP/BPDefinition/GetFilteredBPDefinitionsForTechnical',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntityModule]-------------601 to 700---------------------------------------------------------
begin
set nocount on;;with cte_data([ID],[Name],[ParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('04493174-83F0-44D6-BBE4-DBEB8B57875A','WorkFlows','B6B8F582-4759-43FB-9220-AA7662C366EA',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[ParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[Name],[ParentId],[BreakInheritance])	values(s.[ID],s.[Name],s.[ParentId],s.[BreakInheritance]);
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
('BusinessProcess_BP/BPInstance/GetUpdated',null),
('BusinessProcess_BP/BPInstance/GetBeforeId',null),
('BusinessProcess_BP/BPInstance/GetFilteredBPInstances',null),
('BusinessProcess_BP/BPInstance/GetBPInstance',null),
('BusinessProcess_BP/BPInstance/CreateNewProcess',null),
('BusinessProcess_BP/BPInstanceTracking/GetFilteredBPInstanceTracking',null),
('BusinessProcess_BP/BPInstanceTracking/GetUpdated',null),
('BusinessProcess_BP/BPInstanceTracking/GetBeforeId',null),
('BusinessProcess_BP/BPTask/GetProcessTaskUpdated',null),
('BusinessProcess_BP/BPTask/GetProcessTaskBeforeId',null),
('BusinessProcess_BP/BPTask/GetMyUpdatedTasks',null),
('BusinessProcess_BP/BPTask/GetMyTasksBeforeId',null),
('BusinessProcess_BP/BPTask/ExecuteTask',null),
('BusinessProcess_BP/BPTask/GetTask',null),
('BusinessProcess_BP/BPTaskType/GetBPTaskTypeByTaskId',null),
('BusinessProcess_BP/BPValidationMessage/GetUpdated',null),
('BusinessProcess_BP/BPValidationMessage/GetBeforeId',null),
('BusinessProcess_BP/BPValidationMessage/GetFilteredBPValidationMessage',null),
('BusinessProcess_BP/BPBusinessRuleSet/GetFilteredBPBusinessRuleSets',null),
('BusinessProcess_BP/BPBusinessRuleSet/UpdateBusinessRuleSet',null),
('BusinessProcess_BP/BPBusinessRuleSet/AddBusinessRuleSet',null),
('BusinessProcess_BP/BPDefinition/GetFilteredBPDefinitionsForTechnical','VR_SystemConfiguration: View'),
('BusinessProcess_BP/BPDefinition/UpdateBPDefinition','VR_SystemConfiguration: Edit')
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

