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
--[sec].[Module]---------------------------501 to 600---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------

--[sec].[View]-----------------------------5001 to 6000-------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('95ABA81F-01BB-40B5-8194-5F67718F6C04','Scheduler Service','Scheduler Service','#/view/Runtime/Views/SchedulerTaskManagement','BAAF681E-AB1C-4A64-9A35-3F3951398881',null,null,null,'{"$type":"Vanrise.Runtime.Business.SchedulerServiceViewSettings, Vanrise.Runtime.Business"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',25),
('C65ED28A-36D0-4047-BEC5-030D35B02308','My Scheduler Service','Scheduler Service','#/viewwithparams/Runtime/Views/SchedulerTaskManagement/{"myTasks":"1"}','50624672-CD25-44FD-8580-0E3AC8E34C71',null,null,null,'{"$type":"Vanrise.Runtime.Business.UserSchedulerServiceViewSettings, Vanrise.Runtime.Business"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',30)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
--------------------------------------------------------------------------------------------------------------

end

--[sec].[SystemAction]------------------------------------------------------------------------------
begin

;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Runtime/SchedulerTask/GetFilteredTasks',null),
('VR_Runtime/SchedulerTask/GetFilteredMyTasks',null),
('VR_Runtime/SchedulerTask/GetTask',null),
('VR_Runtime/SchedulerTask/GetSchedulesInfo',null),
('VR_Runtime/SchedulerTask/GetSchedulerTaskTriggerTypes',null),
('VR_Runtime/SchedulerTask/GetSchedulerTaskActionTypes',null),
('VR_Runtime/SchedulerTask/AddTask',null),
('VR_Runtime/SchedulerTask/UpdateTask',null),
('VR_Runtime/SchedulerTask/DeleteTask',null),
('VR_Runtime/SchedulerTask/RunSchedulerTask',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[Name] = s.[Name],[RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);
----------------------------------------------------------------------------------------------------

end

--[sec].[BusinessEntityModule]-------------501 to 600---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------

--[sec].[BusinessEntity]-------------------1201 to 1500-------------------------------------------------------
begin
set nocount on;;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('361B017B-9344-4292-9E48-D9AD056D5528','VR_Runtime_SchedulerTask','Scheduler Task','0BA03544-A3D8-4570-8855-5162B42B50AB',0,'["View","Configure","Run"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
end

--[runtime].[SchedulerTaskTriggerType]--------------------------------------------------------------BEGINset nocount on;;with cte_data([ID],[Name],[TriggerTypeInfo])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('295B4FAC-DBF9-456F-855E-60D0B176F86B','Timer','{"URL":"/Client/Modules/Runtime/Views/TriggerTemplates/TimerTriggerTemplate.html","Editor":"vr-runtime-tasktrigger-timer","FQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.TimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[TriggerTypeInfo]))merge	[runtime].[SchedulerTaskTriggerType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[TriggerTypeInfo] = s.[TriggerTypeInfo]when not matched by target then	insert([ID],[Name],[TriggerTypeInfo])	values(s.[ID],s.[Name],s.[TriggerTypeInfo]);
----------------------------------------------------------------------------------------------------
END

--[runtime].[SchedulerTaskActionType]-------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[ActionTypeInfo])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('7A35F562-319B-47B3-8258-EC1A704A82EB','Workflow','{"URL":"/Client/Modules/Runtime/Views/ActionTemplates/WFActionTemplate.html","Editor":"vr-runtime-taskaction-workflow","SystemType":false,"FQTN":"Vanrise.BusinessProcess.Extensions.WFTaskAction.WFSchedulerTaskAction, Vanrise.BusinessProcess.Extensions.WFTaskAction","ExtendedSettings":{"$type":"Vanrise.BusinessProcess.Extensions.WFTaskAction.WFShcedulerTaskActionExtendedSettings, Vanrise.BusinessProcess.Extensions.WFTaskAction"}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[ActionTypeInfo]))merge	[runtime].[SchedulerTaskActionType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[ActionTypeInfo] = s.[ActionTypeInfo]when not matched by target then	insert([ID],[Name],[ActionTypeInfo])	values(s.[ID],s.[Name],s.[ActionTypeInfo]);--[logging].[LoggableEntity]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[UniqueName],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('405708EC-E1BF-49C6-9ABC-F25CD4994C17','VR_Runtime_SchedulerTask','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Runtime_SchedulerTask_ViewHistoryItem"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[UniqueName],[Settings]))merge	[logging].[LoggableEntity] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]when not matched by target then	insert([ID],[UniqueName],[Settings])	values(s.[ID],s.[UniqueName],s.[Settings]);