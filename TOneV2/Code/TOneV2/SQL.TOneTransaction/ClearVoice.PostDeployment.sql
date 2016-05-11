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
--[runtime].[SchedulerTaskActionType]--------201 to 300-------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ActionTypeInfo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(201,'Supplier Synchronize','{"URL":"/Client/Modules/QM_BusinessEntity/Views/Supplier/SchedulerTaskAction/SupplierSynchronizeTemplate.html","SystemType":false,"Editor":"qm-be-sourcesupplierreader","FQTN":"QM.BusinessEntity.Business.SupplierSyncTaskAction, QM.BusinessEntity.Business","RequiredPermissions":"QM_CLITester_TestCall:l"}'),
(202,'Profile Synchronize','{"URL":"/Client/Modules/QM_CLITester/Views/Profile/SchedulerTaskAction/ProfileSynchronizeTemplate.html","SystemType":false,"Editor":"vr-qm-clitester-sourceprofilereader","FQTN":"QM.CLITester.Business.ProfileSyncTaskAction, QM.CLITester.Business","RequiredPermissions":"QM_CLITester_TestCall:l"}'),
(203,'Zone Synchronize','{"URL":"/Client/Modules/QM_BusinessEntity/Views/Zone/SchedulerTaskAction/ZoneSynchronizeTemplate.html","SystemType":false,"Editor":"qm-be-sourcezonereader","FQTN":"QM.BusinessEntity.Business.ZoneSyncTaskAction, QM.BusinessEntity.Business","RequiredPermissions":"QM_CLITester_TestCall:l"}'),
(204,'Initiate Test','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/InitiateTestTemplate.html","SystemType":false,"Editor":"qm-clitester-initiatetest","FQTN":"QM.CLITester.Business.InitiateTestTaskAction, QM.CLITester.Business","RequiredPermissions":"QM_CLITester_TestCall:l"}'),
(205,'Download Test Result','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/TestProgressTemplate.html","SystemType":false,"Editor":"qm-clitester-testprogress","FQTN":"QM.CLITester.Business.TestProgressTaskAction, QM.CLITester.Business","RequiredPermissions":"QM_CLITester_TestCall:l"}'),
(206,'Create Test Call','{"URL":"/Client/Modules/QM_CLITester/Views/TestPage/SchedulerTaskAction/TestCallTemplate.html","SystemType":false,"Editor":"qm-clitester-testcall","FQTN":"QM.CLITester.Business.TestCallTaskAction, QM.CLITester.Business"}')
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
