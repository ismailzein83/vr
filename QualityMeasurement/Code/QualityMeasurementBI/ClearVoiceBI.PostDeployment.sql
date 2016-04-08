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
--dbo.[Dim_CallTestStatus]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([pk_CallTestStatusId],[CallTestStatus])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(0,'New'),
(10,'Initiated'),
(20,'Initiation Failed With Retry'),
(30,'Partially Completed'),
(40,'Progress Failed With Retry'),
(50,'Completed'),
(60,'Initiation Failed With No Retry'),
(70,'Progress Failed With No Retry')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([pk_CallTestStatusId],[CallTestStatus]))
merge	[dbo].[Dim_CallTestStatus] as t
using	cte_data as s
on		1=1 and t.[pk_CallTestStatusId] = s.[pk_CallTestStatusId]
when matched then
	update set
	[CallTestStatus] = s.[CallTestStatus]
when not matched by target then
	insert([pk_CallTestStatusId],[CallTestStatus])
	values(s.[pk_CallTestStatusId],s.[CallTestStatus])
when not matched by source then
	delete;

--dbo.[Dim_CallTestResult]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([pk_CallTestResultId],[CallTestResult])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(0,'Not Completed'),
(1,'Succeeded'),
(2,'Partially Succeeded'),
(3,'Failed'),
(4,'Not Answered'),
(5,'FAS')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([pk_CallTestResultId],[CallTestResult]))
merge	[dbo].[Dim_CallTestResult] as t
using	cte_data as s
on		1=1 and t.[pk_CallTestResultId] = s.[pk_CallTestResultId]
when matched then
	update set
	[CallTestResult] = s.[CallTestResult]
when not matched by target then
	insert([pk_CallTestResultId],[CallTestResult])
	values(s.[pk_CallTestResultId],s.[CallTestResult])
when not matched by source then
	delete;