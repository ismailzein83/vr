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

--[FraudAnalysis].[CallClass]---
set nocount on;
set identity_insert [FraudAnalysis].[CallClass] on;
;with cte_data([ID],[Description],[NetType])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'ZAINIQ',1),
(2,'VAS',1),
(3,'INV',1),
(4,'INT',2),
(5,'KOREKTEL',0),
(6,'ASIACELL',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Description],[NetType]))
merge	[FraudAnalysis].[CallClass] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Description] = s.[Description],[NetType] = s.[NetType]
when not matched by target then
	insert([ID],[Description],[NetType])
	values(s.[ID],s.[Description],s.[NetType])
when not matched by source then
	delete;
set identity_insert [FraudAnalysis].[CallClass] off;

--[FraudAnalysis].[CallType]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [FraudAnalysis].[CallType] on;
;with cte_data([ID],[Code],[Description])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,1,'outgoing Voice'),
(2,2,'Incoming Voice Call'),
(3,29,'call Forward'),
(4,30,'Incoming Sms'),
(5,31,'Outgoing Sms'),
(6,26,'Roaming call forward')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Code],[Description]))
merge	[FraudAnalysis].[CallType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Code] = s.[Code],[Description] = s.[Description]
when not matched by target then
	insert([ID],[Code],[Description])
	values(s.[ID],s.[Code],s.[Description])
when not matched by source then
	delete;
set identity_insert [FraudAnalysis].[CallType] off;


--[FraudAnalysis].[CaseStatus]----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Open'),
(2,'Pending'),
(3,'Closed: Fraud'),
(4,'Closed: White List'),
(5,'Cancelled')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name]))
merge	[FraudAnalysis].[CaseStatus] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name]
when not matched by target then
	insert([ID],[Name])
	values(s.[ID],s.[Name])
when not matched by source then
	delete;


--[FraudAnalysis].[Period]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [FraudAnalysis].[Period] on;
;with cte_data([ID],[Description])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Hourly'),
(2,'Daily')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Description]))
merge	[FraudAnalysis].[Period] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Description] = s.[Description]
when not matched by target then
	insert([ID],[Description])
	values(s.[ID],s.[Description])
when not matched by source then
	delete;
set identity_insert [FraudAnalysis].[Period] off;


--[FraudAnalysis].[SubType]-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [FraudAnalysis].[SubType] on;
;with cte_data([ID],[Description])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Prepaid'),
(2,'Postpaid')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Description]))
merge	[FraudAnalysis].[SubType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Description] = s.[Description]
when not matched by target then
	insert([ID],[Description])
	values(s.[ID],s.[Description])
when not matched by source then
	delete;
set identity_insert [FraudAnalysis].[SubType] off;


--[FraudAnalysis].[SuspicionLevel]------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [FraudAnalysis].[SuspicionLevel] on;
;with cte_data([ID],[Name])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Clean'),
(2,'Suspicious'),
(3,'Highly Suspicious'),
(4,'Fraud')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name]))
merge	[FraudAnalysis].[SuspicionLevel] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name]
when not matched by target then
	insert([ID],[Name])
	values(s.[ID],s.[Name])
when not matched by source then
	delete;
set identity_insert [FraudAnalysis].[SuspicionLevel] off;




