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
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(701,'Queueing','Queueing',null,1,null,3,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
set identity_insert [sec].[Module] off;
--[sec].[View]-----------------------------7001 to 8000--------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(7001,'Execution Flow Definitions','Execution Flow Definitions','#/view/Queueing/Views/ExecutionFlowDefinition/ExecutionFlowDefinitionManagement',701,null,null,null,'0',0,1),
(7002,'Execution Flows','Execution Flows','#/view/Queueing/Views/ExecutionFlow/ExecutionFlowManagement',701,null,null,null,'0',0,2),
(7003,'Queues','Queues','#/view/Queueing/Views/QueueInstance/QueueInstanceManagement',701,null,null,null,'0',0,3),
(7004,'Queue Items','Queue Items','#/view/Queueing/Views/QueueItemHeader/QueueItemHeaderManagement',701,null,null,null,'0',0,4)
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
--[queue].[QueueActivatorConfig]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
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
