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
--[runtime].[SchedulerTaskActionType]---------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ActionTypeInfo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Workflow','{"URL":"/Client/Modules/Runtime/Views/ActionTemplates/WFActionTemplate.html","SystemType":false,"Editor":"vr-runtime-taskaction-workflow","FQTN":"Vanrise.BusinessProcess.Extensions.WFTaskAction.WFSchedulerTaskAction, Vanrise.BusinessProcess.Extensions.WFTaskAction"}'),
(2,'Data Source','{"URL":"", "SystemType":true, "FQTN":"Vanrise.Integration.Business.DSSchedulerTaskAction, Vanrise.Integration.Business"}'),
(3,'Supplier Synchronize','{"URL":"/Client/Modules/QM_BusinessEntity/Views/Supplier/SchedulerTaskAction/SupplierSynchronizeTemplate.html","SystemType":false,"Editor":"qm-be-sourcesupplierreader","FQTN":"QM.BusinessEntity.Business.SupplierSyncTaskAction, QM.BusinessEntity.Business"}'),
(4,'Profile Synchronize','{"URL":"/Client/Modules/QM_CLITester/Views/Profile/SchedulerTaskAction/ProfileSynchronizeTemplate.html","SystemType":false,"Editor":"vr-qm-clitester-sourceprofilereader","FQTN":"QM.CLITester.Business.ProfileSyncTaskAction, QM.CLITester.Business"}'),
(5,'Zone Synchronize','{"URL":"/Client/Modules/QM_BusinessEntity/Views/Zone/SchedulerTaskAction/ZoneSynchronizeTemplate.html","SystemType":false,"Editor":"qm-be-sourcezonereader","FQTN":"QM.BusinessEntity.Business.ZoneSyncTaskAction, QM.BusinessEntity.Business"}'),
(6,'Initiate Test','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/InitiateTestTemplate.html","SystemType":false,"Editor":"qm-clitester-initiatetest","FQTN":"QM.CLITester.Business.InitiateTestTaskAction, QM.CLITester.Business"}'),
(7,'Download Test Result','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/TestProgressTemplate.html","SystemType":false,"Editor":"qm-clitester-testprogress","FQTN":"QM.CLITester.Business.TestProgressTaskAction, QM.CLITester.Business"}'),
(8,'Create Test Call','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/TestCallTemplate.html","SystemType":false,"Editor":"qm-clitester-testcall","FQTN":"QM.CLITester.Business.TestCallTaskAction, QM.CLITester.Business"}')
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
	values(s.[ID],s.[Name],s.[ActionTypeInfo])
when not matched by source then
	delete;
