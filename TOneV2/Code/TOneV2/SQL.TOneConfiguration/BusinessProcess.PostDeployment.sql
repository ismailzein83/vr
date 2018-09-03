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
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FBFE2B36-12F6-40C1-8163-26CFE2D23501','Show warning','Show Warning'				,'VR_BP_BPBusinessRuleActionType','{ "Description":"Show warning", "Editor":"businessprocess-bp-business-rule-warning-item-action"}'),
('715F7F90-2C23-4185-AEB8-EDA947DE3978','Stop execution','Stop Execution'			,'VR_BP_BPBusinessRuleActionType','{"Description":"Stop execution", "Editor":"businessprocess-bp-business-rule-stop-execution-action"}'),
('BA3427FE-B8BE-4546-B433-CE0D8CE9FCB1','Exclude','Exclude'							,'VR_BP_BPBusinessRuleActionType','{ "Description":"Exclude", "Editor":"businessprocess-bp-business-rule-exclude-item-action"}'),
('72C926F1-D019-408F-84AF-6613D2033473','Show information','Show Information'		,'VR_BP_BPBusinessRuleActionType','{"Description":"Show information","Editor":"businessprocess-bp-business-rule-information-action"}'),

('A30953F6-D62E-4755-BDDE-DF87C0716864','VRWorkflow_ArgumentVariableType_GenericVariableType','Generic Variable','BusinessProcess_VRWorkflowVariableTypeConfig','{"Editor":"businessprocess-vr-workflow-genericargumentvariabletype"}'),
('A6078B0F-EFA2-414F-8A25-549628DA1762','VRWorkflow_ArgumentVariableType_CustomClassType','Custom Class'		,'BusinessProcess_VRWorkflowVariableTypeConfig','{"Editor":"businessprocess-vr-workflow-customclasstype"}'),

('82C21F82-3636-4174-BA83-11709140D959','VRWorkflowAssignActivity','Assign'						,'BP_VR_Workflow_Activity','{"Editor":"businessprocess-vr-workflowactivity-assign"}'),
('F8D16729-620A-4780-80DD-2C37F1ED6A3C','VRWorkflowCustomLogicActivity','Custom Code'			,'BP_VR_Workflow_Activity','{"Editor":"businessprocess-vr-workflowactivity-customcode"}'),
('A9C74099-C36E-45E6-8318-44C7B9A2B381','VRWorkflowCallHttpServiceActivity','Call Http Service'	,'BP_VR_Workflow_Activity','{"Editor":"businessprocess-vr-workflowactivity-callhttpservice"}'),
('FEABAAFD-C3E9-4DBB-AABF-45DB33D33517','VRWorkflowDelayActivity','Delay'						,'BP_VR_Workflow_Activity','{"Editor":"businessprocess-vr-workflowactivity-delay"}'),
('A0DE8C69-7427-4F95-9A4D-9ECD8658D7B2','VRWorkflowWriteTrackingMessageActivity','Log Message'	,'BP_VR_Workflow_Activity','{"Editor":"businessprocess-vr-workflowactivity-writetrackingmessage"}'),
('9292B3BE-256F-400F-9BC6-A0423FA0B30F','VRWorkflowSequenceActivity','Sequence'					,'BP_VR_Workflow_Activity','{"Editor":"businessprocess-vr-workflowactivity-sequence"}'),
('BA3A107D-20DA-4456-AC70-A2948DFE3725','VRWorkflowForEachActivity','ForEach'					,'BP_VR_Workflow_Activity','{"Editor":"businessprocess-vr-workflowactivity-foreach"}'),
('173258F8-2AC9-4214-BCE2-D3DB6D902423','VRWorkflowSubProcessActivity','Subprocess'				,'BP_VR_Workflow_Activity','{"Editor":"businessprocess-vr-workflowactivity-subprocess"}'),
('40B7E3E9-F8E0-4C2C-9ED7-F79CC4A68473','VRWorkflowIfElseActivity','If Else'					,'BP_VR_Workflow_Activity','{"Editor":"businessprocess-vr-workflowactivity-ifelse"}')
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

--[sec].[View]-----------------------------6001 to 7000-------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1D10975D-5B1D-4DDC-BBF2-F8F9BA89FF5A','Management','Business Processes','#/view/BusinessProcess/Views/BPDefinition/BPDefinitionManagement','B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B',null,null,null,'{"$type":"Vanrise.BusinessProcess.Entities.BPViewSettings, Vanrise.BusinessProcess.Entities"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2),
--('B23B61C1-D5AE-44A9-BD6E-F3009166395B','My Tasks','Tasks','#/view/BusinessProcess/Views/BPTask/BPTaskMonitor','B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B','BusinessProcess_BP/BPTask/GetMyUpdatedTasks',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5),
('3C53D6DC-BBE7-49CA-A222-8211AF25DD31','Business Rules','Business Rules','#/view/BusinessProcess/Views/BPBusinessRule/BPBusinessRuleSetManagement','1C7569FA-43C9-4853-AE4C-1152746A34FD','BusinessProcess_BP/BPBusinessRuleSet/GetFilteredBPBusinessRuleSets',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',110),
('BEEEEB03-8CB2-44F4-9769-A95B46D4F40D','BP Technical Definitions','BP Technical Definitions','#/view/BusinessProcess/Views/BPDefinition/BPTechnicalDefinitionManagement','FC9D12D3-9CBF-4D99-8748-5C2BDD6C5ED9','BusinessProcess_BP/BPDefinition/GetFilteredBPDefinitionsForTechnical',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',1),

('20CFA17A-1C79-4ABE-BF85-F40C42B03FAC','Workflows','Workflows','#/view/BusinessProcess/Views/VRWorkflow/VRWorkflowManagement','FC9D12D3-9CBF-4D99-8748-5C2BDD6C5ED9','BusinessProcess/VRWorkflow/GetFilteredVRWorkflows',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2)

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

--[sec].[BusinessEntityModule]-------------601 to 700---------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('04493174-83F0-44D6-BBE4-DBEB8B57875A','WorkFlows','B6B8F582-4759-43FB-9220-AA7662C366EA',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([ID],[Name],[ParentId],[BreakInheritance])
	values(s.[ID],s.[Name],s.[ParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
end

--[bp].[BPDefinition]-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1B724508-3927-4035-89A9-93EA19B28A98','Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.CustomCodeBPArgument','Custom Code','Vanrise.BusinessProcess.Extensions.WFTaskAction.CustomCodeBP, Vanrise.BusinessProcess.Extensions.WFTaskAction','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ScheduledExecEditor":"bp-customcode-task","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.BusinessProcess.Business.CustomCodeTaskBPSettings, Vanrise.BusinessProcess.Business"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Manage"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Manage"]}}]}}}}')
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

--[runtime].[ScheduleTask]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('374BCEA5-D243-4595-9A27-A8C1FC36DA1A','Custom Code (Sample)',0,1,'295B4FAC-DBF9-456F-855E-60D0B176F86B','7A35F562-319B-47B3-8258-EC1A704A82EB','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskActionArgument":{"$type":"Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.WFTaskActionArgument, Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments","BPDefinitionID":"1b724508-3927-4035-89a9-93ea19b28a98","ProcessInputArguments":{"$type":"Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.CustomCodeBPArgument, Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments","Name":"Custom Code (Sample)","TaskCode":"try{\n\tList<int> list = new List<int>() { 1, 2, 3 };\nVRExcelFile file = new VRExcelFile();\nVRExcelSheet sheet = new VRExcelSheet();\nsheet.SheetName = \"First Result\";\nfile.AddSheet(sheet);\nint x = 0;\nforeach (var i in list)\n{\n   \n\tVRExcelCell cell = new VRExcelCell()\n\t{\n\t\tValue = i,\n\t\tRowIndex = x,\n\t\tColumnIndex = 0\n\t};\n\tx++;\n\tsheet.AddCell(cell);\n}\n\nbyte[] content  = file.GenerateExcelFile() ;\n\nList<UtcMailMessageAttachment> attachments = new List<UtcMailMessageAttachment>();\n\nattachments.Add(new UtcMailMessageAttachment(){\n\tName = \"testExcel.xlsx\",\n\tContent = content\n});\n\n\n\tUctMailMessage message  =  new UctMailMessage(){ \n\t\t To = \"support@vanrise.com\",\n\t\t Subject  = \"Custom Code (Sample)\",\n\t\t Body = \"This is The Body\",\n\t\t Attachments = attachments\n\t};\n\tcontext.SendMail(message);\n}\n\ncatch(Exception ex){\n\tcontext.LogException(ex);\n}\ncontext.LogError(\"Custom code error log.\",null);\n\ncontext.LogWarning(\"Custom code warning log.\",null);\n\ncontext.LogInfo(\"Custom code task is done\", null);","ProcessName":"Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.CustomCodeBPArgument","UserId":0}},"TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":30.0,"IntervalType":0,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger","IgnoreSkippedIntervals":true},"StartEffDate":"2018-04-11T15:41:25.053"}',-1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId]))
merge	[runtime].[ScheduleTask] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[IsEnabled] = s.[IsEnabled],[TaskType] = s.[TaskType],[TriggerTypeId] = s.[TriggerTypeId],[ActionTypeId] = s.[ActionTypeId],[TaskSettings] = s.[TaskSettings],[OwnerId] = s.[OwnerId]
when not matched by target then
	insert([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
	values(s.[Id],s.[Name],s.[IsEnabled],s.[TaskType],s.[TriggerTypeId],s.[ActionTypeId],s.[TaskSettings],s.[OwnerId]);

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
--('BusinessProcess_BP/BPBusinessRuleSet/GetFilteredBPBusinessRuleSets',null),
('BusinessProcess_BP/BPBusinessRuleSet/UpdateBusinessRuleSet',null),
('BusinessProcess_BP/BPBusinessRuleSet/AddBusinessRuleSet',null),
('BusinessProcess_BP/BPDefinition/GetFilteredBPDefinitionsForTechnical','VR_SystemConfiguration: View'),
('BusinessProcess_BP/BPDefinition/UpdateBPDefinition','VR_SystemConfiguration: Edit'),
('BusinessProcess_BP/BPDefinition/AddBPDefinition','VR_SystemConfiguration: Add'),
('BusinessProcess_BP/VRWorkflow/UpdateVRWorkflow','VR_SystemConfiguration:Edit'),
('BusinessProcess_BP/VRWorkflow/InsertVRWorkflow','VR_SystemConfiguration:Add'),
('BusinessProcess_BP/VRWorkflow/GetFilteredVRWorkflows','VR_SystemConfiguration:View')
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

--[logging].[LoggableEntity]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[UniqueName],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7A63FC63-F681-4189-9712-30E0D42A3A97','VR_BusinessProcess_BPDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_BusinessProcess_BPDefinition_ViewHistoryItem"}'),
('00C0D416-3067-4E05-B7A5-DB710703F30D','BusinessProcess_VR_Workflow','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"BusinessProcess_BP_VRWorkflow_ViewHistoryItem"}')
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