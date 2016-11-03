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
--[sec].[Module]------------------------------701 to 800------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('2F940ED6-799C-4411-8FE3-339600964FE1','Queueing',null,'50624672-CD25-44FD-8580-0E3AC8E34C71',null,15,1)
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

--[sec].[View]-----------------------------7001 to 8000--------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('8D33039E-FD2D-476B-A122-C7BBB238D201','Execution Flow Definitions','Execution Flow Definitions','#/view/Queueing/Views/ExecutionFlowDefinition/ExecutionFlowDefinitionManagement','2F940ED6-799C-4411-8FE3-339600964FE1','VR_Queueing/ExecutionFlowDefinition/GetFilteredExecutionFlowDefinitions',null,null,'0','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,2),
('3EE0BB0A-C14A-4325-8449-AD7A148CF088','Execution Flows','Execution Flows','#/view/Queueing/Views/ExecutionFlow/ExecutionFlowManagement','2F940ED6-799C-4411-8FE3-339600964FE1','VR_Queueing/ExecutionFlow/GetFilteredExecutionFlows',null,null,'0','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,3),
('844DA1DD-E5CF-42F2-A14D-39219237EF4B','Queues','Queues','#/view/Queueing/Views/QueueInstance/QueueInstanceManagement','2F940ED6-799C-4411-8FE3-339600964FE1','VR_Queueing/QueueInstance/GetFilteredQueueInstances',null,null,'0','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,4),
('F6D44233-74BD-4103-B4A6-39B8AC300185','Queue Items','Queue Items','#/view/Queueing/Views/QueueItemHeader/QueueItemHeaderManagement','2F940ED6-799C-4411-8FE3-339600964FE1','VR_Queueing/QueueItemHeader/GetFilteredQueueItemHeader',null,null,'0','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,5)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);
---------------------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Queueing/ExecutionFlow/GetExecutionFlows',null),
('VR_Queueing/ExecutionFlow/GetExecutionFlow',null),
('VR_Queueing/ExecutionFlow/UpdateExecutionFlow','VR_Queueing_ExecutionFlow: Edit'),
('VR_Queueing/ExecutionFlow/AddExecutionFlow','VR_Queueing_ExecutionFlow: Add'),
('VR_Queueing/ExecutionFlow/GetFilteredExecutionFlows','VR_Queueing_ExecutionFlow: View'),
('VR_Queueing/ExecutionFlowDefinition/GetExecutionFlowDefinitions',null),
('VR_Queueing/ExecutionFlowDefinition/GetFilteredExecutionFlowDefinitions','VR_Queueing_ExecutionFlowDefinition: View'),
('VR_Queueing/ExecutionFlowDefinition/GetExecutionFlowDefinition',null),
('VR_Queueing/ExecutionFlowDefinition/UpdateExecutionFlowDefinition','VR_Queueing_ExecutionFlowDefinition: Edit'),
('VR_Queueing/ExecutionFlowDefinition/AddExecutionFlowDefinition','VR_Queueing_ExecutionFlowDefinition: Add'),
('VR_Queueing/QueueActivatorConfig/GetQueueActivatorsConfig',null),
('VR_Queueing/Queueing/GetQueueItemTypes',null),
('VR_Queueing/Queueing/GetItemStatusList',null),
('VR_Queueing/Queueing/GetQueueInstances',null),
('VR_Queueing/Queueing/GetHeaders',null),
('VR_Queueing/Queueing/GetQueueItemHeaders',null),
('VR_Queueing/QueueInstance/GetItemTypes',null),
('VR_Queueing/QueueInstance/GetFilteredQueueInstances','VR_Queueing_QueueInstance: View'),
('VR_Queueing/QueueInstance/GetQueueInstances',null),
('VR_Queueing/QueueItemHeader/GetFilteredQueueItemHeader','VR_Queueing_QueueItemHeader: View'),
('VR_Queueing/QueueItemHeader/GetItemStatusSummary',null),
('VR_Queueing/QueueItemHeader/GetExecutionFlowStatusSummary',null),
('VR_Queueing/QueueItemType/GetItemTypes',null),
('VR_Queueing/QueueStage/GetStageNames',null)
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
end

--[sec].[BusinessEntityModule]--------------------701 to 800----------------------------------------
begin
set nocount on;;with cte_data([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('B21654DA-1A2F-4E5A-887C-865FD9320807',701,'Queueing','5A9E78AE-229E-41B9-9DBF-492997B42B61',2,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[ParentId] = s.[ParentId],[OldParentId] = s.[OldParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])	values(s.[ID],s.[OldId],s.[Name],s.[ParentId],s.[OldParentId],s.[BreakInheritance]);
----------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]----------------------------------------------------------------------------
begin
set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('824BDDC8-7D9E-4D8C-AD24-F0492900B88A',1801,'VR_Queueing_ExecutionFlow','Execution Flow','B21654DA-1A2F-4E5A-887C-865FD9320807',701,0,'["View","Add","Edit"]'),('CD23900F-BDC6-4AF5-9DD2-5A6CE011B20A',1802,'VR_Queueing_ExecutionFlowDefinition','Execution Flow Definition','B21654DA-1A2F-4E5A-887C-865FD9320807',701,0,'["View","Add","Edit"]'),('8BA2172B-744A-4142-A50F-5335ECD22EBA',1803,'VR_Queueing_QueueInstance','Queue Instance','B21654DA-1A2F-4E5A-887C-865FD9320807',701,0,'["View"]'),('09FB693B-8B12-4233-ABF8-5B66FA60CC2B',1804,'VR_Queueing_QueueItemHeader','Queue Item','B21654DA-1A2F-4E5A-887C-865FD9320807',701,0,'["View"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);
----------------------------------------------------------------------------------------------------
end
