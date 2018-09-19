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
('551E5CAE-69CA-478B-B823-8E2CEDBC1841','Data Processes',null,'1037157D-BBC9-4B28-B53F-908936CEC137',null,10,0)
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
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('8D33039E-FD2D-476B-A122-C7BBB238D201','Execution Flow Definitions','Execution Flow Definitions','#/view/Queueing/Views/ExecutionFlowDefinition/ExecutionFlowDefinitionManagement'	,'FC9D12D3-9CBF-4D99-8748-5C2BDD6C5ED9','VR_Queueing/ExecutionFlowDefinition/GetFilteredExecutionFlowDefinitions',null,null,'0','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2),

('3EE0BB0A-C14A-4325-8449-AD7A148CF088','Execution Flows','Execution Flows','#/view/Queueing/Views/ExecutionFlow/ExecutionFlowManagement'											,'551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Queueing/ExecutionFlow/GetFilteredExecutionFlows',null,null,'0','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',3),
('844DA1DD-E5CF-42F2-A14D-39219237EF4B','Queues','Queues','#/view/Queueing/Views/QueueInstance/QueueInstanceManagement'																,'551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Queueing/QueueInstance/GetFilteredQueueInstances',null,null,'0','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',4),
('F6D44233-74BD-4103-B4A6-39B8AC300185','Queue Items','Queue Items','#/view/Queueing/Views/QueueItemHeader/QueueItemHeaderManagement'												,'551E5CAE-69CA-478B-B823-8E2CEDBC1841','VR_Queueing/QueueItemHeader/GetFilteredQueueItemHeader',null,null,'0','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
---------------------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Queueing/ExecutionFlow/GetExecutionFlows',null),
('VR_Queueing/ExecutionFlow/GetExecutionFlow','VR_SystemConfiguration: View'),
('VR_Queueing/ExecutionFlow/UpdateExecutionFlow','VR_SystemConfiguration: Edit'),
('VR_Queueing/ExecutionFlow/AddExecutionFlow','VR_SystemConfiguration: Add'),
('VR_Queueing/ExecutionFlow/GetFilteredExecutionFlows','VR_Integration_DataProcesses: View Logs'),
('VR_Queueing/ExecutionFlowDefinition/GetExecutionFlowDefinitions',null),
('VR_Queueing/ExecutionFlowDefinition/GetFilteredExecutionFlowDefinitions','VR_SystemConfiguration: View'),
('VR_Queueing/ExecutionFlowDefinition/GetExecutionFlowDefinition','VR_SystemConfiguration: View'),
('VR_Queueing/ExecutionFlowDefinition/UpdateExecutionFlowDefinition','VR_SystemConfiguration: Edit'),
('VR_Queueing/ExecutionFlowDefinition/AddExecutionFlowDefinition','VR_SystemConfiguration: Add'),
('VR_Queueing/QueueActivatorConfig/GetQueueActivatorsConfig',null),
('VR_Queueing/Queueing/GetQueueItemTypes',null),
('VR_Queueing/Queueing/GetItemStatusList',null),
('VR_Queueing/Queueing/GetQueueInstances',null),
('VR_Queueing/Queueing/GetHeaders',null),
('VR_Queueing/Queueing/GetQueueItemHeaders',null),
('VR_Queueing/QueueInstance/GetItemTypes',null),
('VR_Queueing/QueueInstance/GetFilteredQueueInstances','VR_Integration_DataProcesses: View Logs'),
('VR_Queueing/QueueInstance/GetQueueInstances',null),
('VR_Queueing/QueueItemHeader/GetFilteredQueueItemHeader','VR_Integration_DataProcesses: View Logs'),
('VR_Queueing/QueueItemHeader/GetItemStatusSummary','VR_Integration_DataProcesses: View Logs'),
('VR_Queueing/QueueItemHeader/GetExecutionFlowStatusSummary','VR_Integration_DataProcesses: View Logs'),
('VR_Queueing/QueueItemType/GetItemTypes',null),
('VR_Queueing/QueueStage/GetStageNames',null),
('VR_Queueing/ExecutionControlData/UpdateExecutionPaused','VR_Integration_DataProcesses: Manage Import Process')
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

