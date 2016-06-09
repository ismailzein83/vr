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
--[QM_BE].[ConnectorResultMapping]------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [QM_BE].[ConnectorResultMapping] on;
;with cte_data([ID],[ConnectorType],[ResultID],[ResultName],[ConnectorResults])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'CLITester_Connector_VI',0,'NotCompleted','Processing|Awaiting CLI Result|Awaiting Result|Analysis Incomplete'),
(2,'CLITester_Connector_VI',1,'Succeeded','CLI Success'),
(3,'CLITester_Connector_VI',3,'Failed','CLI Failure'),
(4,'CLITester_Connector_VI',4,'NotAnswered','Call Failure|Call Timeout|No answer'),
(5,'CLITester_Connector_VI',5,'Fas','Terminated elsewhere')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[ConnectorType],[ResultID],[ResultName],[ConnectorResults]))
merge	[QM_BE].[ConnectorResultMapping] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[ConnectorType] = s.[ConnectorType],[ResultID] = s.[ResultID],[ResultName] = s.[ResultName],[ConnectorResults] = s.[ConnectorResults]
when not matched by target then
	insert([ID],[ConnectorType],[ResultID],[ResultName],[ConnectorResults])
	values(s.[ID],s.[ConnectorType],s.[ResultID],s.[ResultName],s.[ConnectorResults])
when not matched by source then
	delete;
set identity_insert [QM_BE].[ConnectorResultMapping] off;
