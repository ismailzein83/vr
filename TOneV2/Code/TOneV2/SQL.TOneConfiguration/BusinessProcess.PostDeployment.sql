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
--[sec].[Module]---------------------------601 to 700---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(601,'Business Process',null,1,null,30,0),
(602,'Business Process',null,-100,null,30,0)
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
set identity_insert [sec].[Module] off;

--[sec].[View]-----------------------------6001 to 7000-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(6001,'Management','Management','#/view/BusinessProcess/Views/BPDefinition/BPDefinitionManagement',601,null,null,null,'{"$type":"Vanrise.BusinessProcess.Entities.BPViewSettings, Vanrise.BusinessProcess.Entities"}',0,2),
(6002,'Monitor','Monitor','#/view/BusinessProcess/Views/BPInstance/BPInstanceMonitor',601,'BusinessProcess_BP/BPInstance/GetUpdated',null,null,null,0,2),
(6003,'Log','Log History','#/view/BusinessProcess/Views/BPInstance/BPInstanceHistory',601,'BusinessProcess_BP/BPInstance/GetFilteredBPInstances',null,null,null,0,3),
(6004,'My Tasks','Tasks','#/view/BusinessProcess/Views/BPTask/BPTaskMonitor',601,'BusinessProcess_BP/BPTask/GetMyUpdatedTasks',null,null,null,0,4),
(6005,'Business Rules','Business Rules','#/view/BusinessProcess/Views/BPBusinessRule/BPBusinessRuleSetManagement',601,'BusinessProcess_BP/BPBusinessRuleSet/GetFilteredBPBusinessRuleSets',null,null,null,0,5),
(6006,'BP Technical Definition','BP Technical Definition','#/view/BusinessProcess/Views/BPDefinition/BPTechnicalDefinitionManagement',602,'BusinessProcess_BP/BPDefinition/GetFilteredBPDefinitionsForTechnical',null,null,null,0,10)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
set identity_insert [sec].[View] off;

--[sec].[BusinessEntityModule]-------------601 to 700---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(601,'Business Process',2,0),
(602,'WorkFlows',601,0),
(603,'Business Process',-1,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([Id],[Name],[ParentId],[BreakInheritance])
	values(s.[Id],s.[Name],s.[ParentId],s.[BreakInheritance]);
set identity_insert [sec].[BusinessEntityModule] off;

--[sec].[BusinessEntity]-------------------1501 to 1800---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1501,'BusinessProcess_BP_BPDefinition','Management',601,0,'["View"]'),
(1502,'BusinessProcess_BP_BPTask','BPTask',601,0,'["View"]'),
(1503,'BusinessProcess_BP_BPInstance_Log','BPInstance Tracking',601,0,'["View","Monitor"]'),
(1504,'BusinessProcess_BP_BusinessRuleSet','Business Rule Set',601,0,'["View", "Add", "Edit"]'),
(1505,'BusinessProcess_BP_BPTechnicalDefinition','BP Technical Definition',603,0,'["View", "Edit"]')
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
set identity_insert [sec].[BusinessEntity] off;

--[sec].[SystemAction]----------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
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


--[bp].[BPDefinition]---------------------- -1 to -1000---------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [bp].[BPDefinition] on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-1,'Vanrise.Reprocess.BP.Arguments.ReProcessingProcessInput','Reprocess','Vanrise.Reprocess.BP.ReProcessingProcess, Vanrise.Reprocess.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false,"ManualExecEditor":"reprocess-reprocessprocess","HasBusinessRules":false,"HasChildProcesses":true}'),
(-2,'Vanrise.Reprocess.BP.Arguments.ReProcessingSubProcessInput','Reprocess By Range','Vanrise.Reprocess.BP.ReProcessingSubProcess, Vanrise.Reprocess.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false,"NotVisibleInManagementScreen":true}'),
(-3,'Vanrise.Reprocess.BP.Arguments.ReProcessingFinalizeProcessInput','Finalize By Range','Vanrise.Reprocess.BP.ReProcessingFinalizeProcess, Vanrise.Reprocess.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"RetryOnProcessFailed":false,"NotVisibleInManagementScreen":true}')
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