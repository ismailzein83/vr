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
--[runtime].[SchedulerTaskActionType]----------1 to 100---------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ActionTypeInfo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Data Source','{"URL":"", "SystemType":true, "FQTN":"Vanrise.Integration.Business.DSSchedulerTaskAction, Vanrise.Integration.Business"}')
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

MERGE INTO integration.[AdapterType] AS Target 
USING (VALUES 
	(1,'File Receive Adapter','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/FileReceiveAdapterTemplate.html","Editor":"vr-integration-adapter-file","FQTN":"Vanrise.Integration.Adapters.FileReceiveAdapter.FileReceiveAdapter, Vanrise.Integration.Adapters.FileReceiveAdapter"}'),
	(2,'FTP Receive Adapter','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/FTPReceiveAdapterTemplate.html","Editor":"vr-integration-adapter-ftp","FQTN":"Vanrise.Integration.Adapters.FTPReceiveAdapter.FTPReceiveAdapter, Vanrise.Integration.Adapters.FTPReceiveAdapter"}'),
	(3,'SQL Receive Adapter','{"AdapterTemplateURL":"/Client/Modules/Integration/Views/AdapterTemplates/DBReceiveAdapterTemplate.html","Editor":"vr-integration-adapter-db","FQTN":"Vanrise.Integration.Adapters.SQLReceiveAdapter.SQLReceiveAdapter, Vanrise.Integration.Adapters.SQLReceiveAdapter"}')
) 
AS Source ([ID], [Name], [Info])
ON Target.[ID] = Source.[ID] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[ID] = Source.[ID],
			[Name] = Source.[Name],
			[Info]  = Source.[Info]
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([ID], [Name], [Info])
VALUES ([ID], [Name], [Info]);