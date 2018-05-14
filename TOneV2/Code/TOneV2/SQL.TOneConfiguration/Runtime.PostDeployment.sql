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
--[common].[extensionconfiguration]-----------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FD6520DB-26DC-4473-BB7E-1F583BEC3A19','BP Regulator','BP Regulator','VR_Runtime_RuntimeNodeConfiguration_RuntimeService','{"Editor":"bp-regulator-runtimeservice"}'),
('AB860C8F-78C9-44E6-AE2A-3A7B7E7D7D12','Business Process','Business Process','VR_Runtime_RuntimeNodeConfiguration_RuntimeService','{"Editor":"vr-businessprocess-runtimeservice"}'),
('2BB63679-43F1-4859-A883-5CA48009A8D1','Scheduler','Scheduler','VR_Runtime_RuntimeNodeConfiguration_RuntimeService','{"Editor":"vr-runtime-runtimenodeconfiguration-scheduler"}'),
('83028DF1-9BFE-494F-BD3F-9BE6ABE60833','Summary Queue Activation','Summary Queue Activation', 'VR_Runtime_RuntimeNodeConfiguration_RuntimeService','{"Editor":"vr-queueing-runtimeservice-summaryqueueactivation"}'),
('BDD330B6-3557-4AFC-8C69-A23890DA82E7','Data Source','Data Source', 'VR_Runtime_RuntimeNodeConfiguration_RuntimeService','{"Editor":"vr-integration-runtimeservice-datasource"}'),
('6B58776E-B72C-4852-A5A8-B1631A8873F1','Big Data','Big Data','VR_Runtime_RuntimeNodeConfiguration_RuntimeService','{"Editor":"vr-common-runtimeservice-bigdata"}'),
('40422995-F133-4EAB-BA64-D1EF9EB4E24A','Queue Activation','Queue Activation','VR_Runtime_RuntimeNodeConfiguration_RuntimeService'	,'{"Editor":"vr-queueing-runtimeservice-queueactivation"}'),
('7B035997-0941-48F1-B37D-F14A039DDB3E','Data Grouping Executor','Data Grouping Executor','VR_Runtime_RuntimeNodeConfiguration_RuntimeService'	,'{"Editor":"vr-common-runtimeservice-datagroupingexecutor"}'),
('49817A1B-4D9D-4F62-B5A5-F3D5AFDA0487','Data Grouping Distributor','Data Grouping Distributor','VR_Runtime_RuntimeNodeConfiguration_RuntimeService'	,'{"Editor":"vr-common-runtimeservice-datagroupingdistributor"}'),
('B08F3D51-CC02-4292-9AAB-FB9A8720EE16','Queue Regulator','Queue Regulator','VR_Runtime_RuntimeNodeConfiguration_RuntimeService'	,'{"Editor":"vr-queueing-runtimeservice-queueregulator"}	')
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

--[sec].[Module]------------------------------101 to 200------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('EEA077E8-CE38-4E87-89ED-34612D47A998','Runtime',null				,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,21,0)
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

--[sec].[View]-----------------------------5001 to 6000-------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('95ABA81F-01BB-40B5-8194-5F67718F6C04','Scheduler Service','Scheduler Service','#/view/Runtime/Views/SchedulerTaskManagement'								,'B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B',null,null,null,'{"$type":"Vanrise.Runtime.Business.SchedulerServiceViewSettings, Vanrise.Runtime.Business"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',10),
('C65ED28A-36D0-4047-BEC5-030D35B02308','My Scheduler Service','Scheduler Service','#/viewwithparams/Runtime/Views/SchedulerTaskManagement/{"myTasks":"1"}'	,'50624672-CD25-44FD-8580-0E3AC8E34C71',null,null,null,'{"$type":"Vanrise.Runtime.Business.UserSchedulerServiceViewSettings, Vanrise.Runtime.Business"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',125),

('0FBE2511-A7C6-441F-BEA7-9FA5CADADF96','Runtime Node Configurations','Runtime Node Configurations','#/view/Runtime/Views/Runtime/RuntimeNodeConfigurationManagement'	,'EEA077E8-CE38-4E87-89ED-34612D47A998','VR_Runtime/RuntimeNodeConfiguration/GetFilteredRuntimeNodesConfigurations',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5),
('CFEF119F-5527-4675-A3C9-0FF2C44E6C73','Runtime Nodes','Runtime Nodes','#/view/Runtime/Views/Runtime/RuntimeNodeManagement'											,'EEA077E8-CE38-4E87-89ED-34612D47A998','VR_Runtime/RuntimeNode/GetAllNodes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',10)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
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
('VR_Runtime/SchedulerTask/RunSchedulerTask',null),

('VR_Runtime/RuntimeNode/GetAllNodes','VR_SystemConfiguration: View'),
('VR_Runtime/RuntimeNodeConfiguration/GetFilteredRuntimeNodesConfigurations','VR_SystemConfiguration: View')
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
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('361B017B-9344-4292-9E48-D9AD056D5528','VR_Runtime_SchedulerTask','Scheduler Task','0BA03544-A3D8-4570-8855-5162B42B50AB',0,'["View","Configure","Run"]')
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
--------------------------------------------------------------------------------------------------------------
end

--[runtime].[SchedulerTaskTriggerType]--------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[TriggerTypeInfo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('295B4FAC-DBF9-456F-855E-60D0B176F86B','Timer','{"URL":"/Client/Modules/Runtime/Views/TriggerTemplates/TimerTriggerTemplate.html","Editor":"vr-runtime-tasktrigger-timer","FQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.TimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[TriggerTypeInfo]))
merge	[runtime].[SchedulerTaskTriggerType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[TriggerTypeInfo] = s.[TriggerTypeInfo]
when not matched by target then
	insert([ID],[Name],[TriggerTypeInfo])
	values(s.[ID],s.[Name],s.[TriggerTypeInfo]);
----------------------------------------------------------------------------------------------------
END

--[runtime].[SchedulerTaskActionType]---------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ActionTypeInfo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7A35F562-319B-47B3-8258-EC1A704A82EB','Workflow','{"URL":"/Client/Modules/Runtime/Views/ActionTemplates/WFActionTemplate.html","Editor":"vr-runtime-taskaction-workflow","SystemType":false,"FQTN":"Vanrise.BusinessProcess.Extensions.WFTaskAction.WFSchedulerTaskAction, Vanrise.BusinessProcess.Extensions.WFTaskAction","ExtendedSettings":{"$type":"Vanrise.BusinessProcess.Extensions.WFTaskAction.WFShcedulerTaskActionExtendedSettings, Vanrise.BusinessProcess.Extensions.WFTaskAction"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ActionTypeInfo]))
merge	[runtime].[SchedulerTaskActionType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ActionTypeInfo] = s.[ActionTypeInfo]
when not matched by target then
	insert([ID],[Name],[ActionTypeInfo])
	values(s.[ID],s.[Name],s.[ActionTypeInfo]);

--[logging].[LoggableEntity]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[UniqueName],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('405708EC-E1BF-49C6-9ABC-F25CD4994C17','VR_Runtime_SchedulerTask','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Runtime_SchedulerTask_ViewHistoryItem"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[UniqueName],[Settings]))
merge	[logging].[LoggableEntity] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[UniqueName],[Settings])
	values(s.[ID],s.[UniqueName],s.[Settings]);