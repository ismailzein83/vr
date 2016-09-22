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
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('2F940ED6-799C-4411-8FE3-339600964FE1','Queueing',null,'50624672-CD25-44FD-8580-0E3AC8E34C71',null,4,1)
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
--------------------------------------------------------------------------------------------------------------
end

--[sec].[View]-----------------------------7001 to 8000--------------------------------------------------------
begin
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('8D33039E-FD2D-476B-A122-C7BBB238D201','Execution Flow Definitions','Execution Flow Definitions','#/view/Queueing/Views/ExecutionFlowDefinition/ExecutionFlowDefinitionManagement','2F940ED6-799C-4411-8FE3-339600964FE1','VR_Queueing/ExecutionFlowDefinition/GetFilteredExecutionFlowDefinitions',null,null,'0',0,2),
('3EE0BB0A-C14A-4325-8449-AD7A148CF088','Execution Flows','Execution Flows','#/view/Queueing/Views/ExecutionFlow/ExecutionFlowManagement','2F940ED6-799C-4411-8FE3-339600964FE1','VR_Queueing/ExecutionFlow/GetFilteredExecutionFlows',null,null,'0',0,3),
('844DA1DD-E5CF-42F2-A14D-39219237EF4B','Queues','Queues','#/view/Queueing/Views/QueueInstance/QueueInstanceManagement','2F940ED6-799C-4411-8FE3-339600964FE1','VR_Queueing/QueueInstance/GetFilteredQueueInstances',null,null,'0',0,4),
('F6D44233-74BD-4103-B4A6-39B8AC300185','Queue Items','Queue Items','#/view/Queueing/Views/QueueItemHeader/QueueItemHeaderManagement','2F940ED6-799C-4411-8FE3-339600964FE1','VR_Queueing/QueueItemHeader/GetFilteredQueueItemHeader',null,null,'0',0,5)
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
---------------------------------------------------------------------------------------------------------------
end

--[queue].[QueueActivatorConfig]--------------------------------------------------------------------
begin
set nocount on;
set identity_insert [queue].[QueueActivatorConfig] on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Store Batch Queue Activator','{ "QueueActivatorConfigId": "2" , "Name": "Store Batch Queue Activator" ,"Title" : "Store Batch Queue Activator", "Editor" :"vr-genericdata-queueactivator-storebatch"}'),
(2,'Transform Batch Queue Activator','{ "QueueActivatorConfigId": "2" , "Name": "Transform  Batch Queue Activator" ,"Title" : "Transform  Batch Queue Activator", "Editor" :"vr-genericdata-queueactivator-transformbatch"}'),
(3,'Custom Activator','{ "QueueActivatorConfigId": "7" , "Name": "Custom Activator" ,"Title" : "Custom Activator", "Editor" :"vr-queueing-queueactivator-customactivator"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[queue].[QueueActivatorConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);
set identity_insert [queue].[QueueActivatorConfig] off;

----------------------------------------------------------------------------------------------------

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
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(701,'Queueing',2,0)
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
----------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]----------------------------------------------------------------------------
begin
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1801,'VR_Queueing_ExecutionFlow','Execution Flow',701,0,'["View","Add","Edit"]'),
(1802,'VR_Queueing_ExecutionFlowDefinition','Execution Flow Definition',701,0,'["View","Add","Edit"]'),
(1803,'VR_Queueing_QueueInstance','Queue Instance',701,0,'["View"]'),
(1804,'VR_Queueing_QueueItemHeader','Queue Item',701,0,'["View"]')
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
----------------------------------------------------------------------------------------------------

end
