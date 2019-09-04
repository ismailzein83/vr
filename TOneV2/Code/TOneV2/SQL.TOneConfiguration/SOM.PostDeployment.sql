﻿


















--Make sure to use same .json file using DEVTOOLS under http://192.168.110.185:8037
--Make sure that json file contains at least below existing data 




























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
--[sec].[BusinessEntityModule]----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FAC9D166-18B0-4907-AA6C-B6EB04F4350C','External System','5A9E78AE-229E-41B9-9DBF-492997B42B61',0)
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

--[sec].[BusinessEntity]----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D1F22BCE-CCE7-4EB1-9C07-3D1225103338','Billing_System','Billing System','FAC9D166-18B0-4907-AA6C-B6EB04F4350C',0,'["Invoke","ViewLogs"]')
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


--delete useless views from TOne product such 'My Scheduler Service'
delete from [sec].[View] where [Id] in ('C65ED28A-36D0-4047-BEC5-030D35B02308','66DE2441-8A96-41E7-94EA-9F8AF38A3515','604B2CB5-B839-4E51-8D13-3C1C84D05DEE','DCF8CA21-852C-41B9-9101-6990E545509D','A1CE55FE-6CF4-4F15-9BC2-8E1F8DF68561',
										'25994374-CB99-475B-8047-3CDB7474A083','9F691B87-4936-4C4C-A757-4B3E12F7E1D9', 'E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9', '0F111ADC-B7F6-46A4-81BC-72FFDEB305EB',
										'4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712','2D39B12D-8FBF-4D4E-B2A5-5E3FE57580DF'--,'Locked Sessions'
										--,'ADBB44FE-5470-413C-A5F6-8AE8C585FA31'--'Report Generation'
										)

--[bp].[BPDefinition]----------------------1 to 1000------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('C91D5438-B299-4438-B85B-2A383B1D7800','SOM.Main.BP.Arguments.LineSubscriptionProcessInput','Line Subscription','SOM.Main.BP.LineSubscriptionProcess, SOM.Main.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false}'),
('DB08D91A-A6D8-432D-AD70-85C5D7D7CB4F','SOM.Main.BP.Arguments.MoveLineProcessInput','Move Line','SOM.Main.BP.MoveLineProcess, SOM.Main.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false}'),
('5F13DE8A-5A91-4ACF-A334-E0B9A373A05E','SOM.Main.BP.Arguments.LineSubscriptionTerminationProcessInput','Line Subscription Termination','SOM.Main.BP.LineSubscriptionTerminationProcess, SOM.Main.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false}')
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
----------------------------------------------------------------------------------------------------
end

Delete from [common].[Setting] where ID = '4047054E-1CF4-4BE6-A005-6D4706757AD3'--,'Session Lock'

--[runtime].[RuntimeNodeConfiguration]--------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('35B84392-5A95-4567-80EC-A3386D10E155','Default','{"$type":"Vanrise.Runtime.Entities.RuntimeNodeConfigurationSettings, Vanrise.Runtime.Entities","Processes":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities]], mscorlib","0fd035c0-64d7-4d5a-9888-ac03e48c4096":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Scheduler Service Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":1,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","30be260c-c77e-4861-8c03-653cc52f9791":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Scheduler Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.Runtime.SchedulerService, Vanrise.Runtime","ServiceTypeUniqueName":"Vanrise.Runtime.SchedulerService, Vanrise.Runtime, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","Interval":"00:00:01"}}}}}},"949323e6-04bb-4d88-a722-afc1bf19c751":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Regulator Service Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":1,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","59e3194a-62bb-4e9d-b2a8-7b497f96f26a":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Regulator Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.BusinessProcess.BPRegulatorRuntimeService, Vanrise.BusinessProcess","ServiceTypeUniqueName":"Vanrise.BusinessProcess.BPRegulatorRuntimeService, Vanrise.BusinessProcess, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","Interval":"00:00:01"}}}}}},"c337541d-1af7-44ed-8b27-d1a57d9d628f":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Services Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":3,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","e4651be4-0191-450e-9786-9f52f2c3f974":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.BusinessProcess.BusinessProcessService, Vanrise.BusinessProcess","ServiceTypeUniqueName":"VR_BusinessProcess_BusinessProcessService","Interval":"00:00:01"}}}}}}}}',null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy]))
merge	[runtime].[RuntimeNodeConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[Settings] = s.[Settings],[CreatedBy] = s.[CreatedBy],[LastModifiedBy] = s.[LastModifiedBy]
when not matched by target then
	insert([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
	values(s.[ID],s.[Name],s.[Settings],s.[CreatedBy],s.[LastModifiedBy]);



--[runtime].[RuntimeNode]---------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('742F43C2-3485-4DA9-9CAD-A6BD60CC91C1','35B84392-5A95-4567-80EC-A3386D10E155','Node 1',null,null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy]))
merge	[runtime].[RuntimeNode] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[RuntimeNodeConfigurationID] = s.[RuntimeNodeConfigurationID],[Name] = s.[Name],[Settings] = s.[Settings],[CreatedBy] = s.[CreatedBy],[LastModifiedBy] = s.[LastModifiedBy]
when not matched by target then
	insert([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
	values(s.[ID],s.[RuntimeNodeConfigurationID],s.[Name],s.[Settings],s.[CreatedBy],s.[LastModifiedBy]);
