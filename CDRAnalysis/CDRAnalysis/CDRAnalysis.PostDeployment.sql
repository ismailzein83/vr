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





--FraudAnalysis.Filter------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Abbreviation],[OperatorTypeAllowed],[Description],[Label],[ToolTip],[ExcludeHourly],[CompareOperator],[MinValue],[MaxValue],[DecimalPrecision],[timestamp])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'_Filter_1',0,'Ratio Incoming Calls on Outgoing Calls','Ratio','MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2',0,1,0.010,0.990,2),
(2,'_Filter_2',0,'Count of Distinct Called Parties','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0),
(3,'_Filter_3',0,'Count of Outgoing Calls','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0),
(4,'_Filter_4',1,'Count of Total Connected BTS','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,1,1.000,999999.000,0),
(5,'_Filter_5',0,'Total Originated Volume in Minutes','Volume','MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2',0,0,0.010,999999.000,2),
(6,'_Filter_6',1,'Count of Total IMEIs','Count','MinValue = 0, MaxValue = 100, DecimalPrecision = 0',0,0,0.000,100.000,0),
(7,'_Filter_7',0,'Ratio Average Incoming Duration on Average Outgoing Duration','Ratio','MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2',0,1,0.010,0.990,2),
(8,'_Filter_8',1,'Ratio OffNet Originated Calls on OnNet Originated Calls','Ratio','MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2',0,1,0.010,999999.000,2),
(9,'_Filter_9',0,'Count of Daily Active Hours','Count','MinValue = 1, MaxValue = 24, DecimalPrecision = 0',1,0,1.000,24.000,0),
(10,'_Filter_10',0,'Distinct Called Parties during Night Period','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',1,0,1.000,999999.000,0),
(11,'_Filter_11',1,'Count of Sent SMSs','Count','MinValue = 0, MaxValue = int.MaxValue, DecimalPrecision = 0',0,1,0.000,999999.000,0),
(12,'_Filter_12',0,'Ratio Distinct Called Parties on Total Outgoing Calls','Ratio','MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2',0,0,0.010,1.000,2),
(13,'_Filter_13',1,'Ratio International Outgoing Calls on Outgoing Calls','Ratio','MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2',0,0,0.010,1.000,2),
(14,'_Filter_14',0,'Count of Outgoing Calls during Peak Hours','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',1,0,1.000,999999.000,0),
(15,'_Filter_15',1,'Data Usage Volume in Mbytes','Volume','MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2',0,1,0.010,999999.000,2),
(16,'_Filter_16',0,'Count of Consecutive Calls','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0),
(17,'_Filter_17',0,'Count of Consecutive Failed Calls','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0),
(18,'_Filter_18',0,'Count Incoming “low duration” Calls on Count Incoming Calls','Ratio','MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2',0,0,0.010,1.000,2),
(19,'_Filter_19',2,'Different Destination Zones','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0),
(20,'_Filter_20',2,'Different Source Zones','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Abbreviation],[OperatorTypeAllowed],[Description],[Label],[ToolTip],[ExcludeHourly],[CompareOperator],[MinValue],[MaxValue],[DecimalPrecision],[timestamp]))
merge	[FraudAnalysis].[Filter] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Abbreviation] = s.[Abbreviation],[OperatorTypeAllowed] = s.[OperatorTypeAllowed],[Description] = s.[Description],[Label] = s.[Label],[ToolTip] = s.[ToolTip],[ExcludeHourly] = s.[ExcludeHourly],[CompareOperator] = s.[CompareOperator],[MinValue] = s.[MinValue],[MaxValue] = s.[MaxValue],[DecimalPrecision] = s.[DecimalPrecision],[timestamp] = s.[timestamp]
when not matched by target then
	insert([ID],[Abbreviation],[OperatorTypeAllowed],[Description],[Label],[ToolTip],[ExcludeHourly],[CompareOperator],[MinValue],[MaxValue],[DecimalPrecision],[timestamp])
	values(s.[ID],s.[Abbreviation],s.[OperatorTypeAllowed],s.[Description],s.[Label],s.[ToolTip],s.[ExcludeHourly],s.[CompareOperator],s.[MinValue],s.[MaxValue],s.[DecimalPrecision],s.[timestamp])
when not matched by source then
	delete;


--FraudAnalysis.Aggregate---------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[OperatorTypeAllowed],[NumberPrecision])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Count Out Calls',0,'NoDecimal'),
(2,'Count In Calls',0,'NoDecimal'),
(3,'Total Data Volume',1,'LongPrecision'),
(4,'Count Out Fails',0,'NoDecimal'),
(5,'Count In Fails',0,'NoDecimal'),
(6,'Count Out SMSs',1,'NoDecimal'),
(7,'Count Out OffNets',0,'NoDecimal'),
(8,'Count Out OnNets',0,'NoDecimal'),
(9,'Count Out Inters',0,'NoDecimal'),
(10,'Count In Inters',0,'NoDecimal'),
(11,'Call Out Dur Avg',0,'LongPrecision'),
(12,'Call In Dur Avg',0,'LongPrecision'),
(13,'Total Out Volume',1,'LongPrecision'),
(14,'Total In Volume',1,'LongPrecision'),
(15,'Total IMEI',1,'NoDecimal'),
(16,'Total BTS',1,'NoDecimal'),
(17,'Diff Output Numbers',0,'NoDecimal'),
(18,'Diff Input Numbers',0,'NoDecimal'),
(19,'Count In OffNets',0,'NoDecimal'),
(20,'Count In OnNets',0,'NoDecimal'),
(21,'Diff Output Numbers NightCalls',0,'NoDecimal'),
(22,'Count Out Calls Peak Hours',0,'NoDecimal'),
(23,'Count Consecutive Calls',0,'NoDecimal'),
(24,'Count Active Hours',0,'NoDecimal'),
(25,'Count Fail Consecutive Calls',0,'NoDecimal'),
(26,'Count In Low Duration Calls',0,'NoDecimal'),
(27,'Diff Dest Zones',2,'NoDecimal'),
(28,'Diff Sources Zones',2,'NoDecimal')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[OperatorTypeAllowed],[NumberPrecision]))
merge	[FraudAnalysis].[Aggregate] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[OperatorTypeAllowed] = s.[OperatorTypeAllowed],[NumberPrecision] = s.[NumberPrecision]
when not matched by target then
	insert([ID],[Name],[OperatorTypeAllowed],[NumberPrecision])
	values(s.[ID],s.[Name],s.[OperatorTypeAllowed],s.[NumberPrecision])
when not matched by source then
	delete;

--[FraudAnalysis].[Filter]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Abbreviation],[OperatorTypeAllowed],[Description],[Label],[ToolTip],[ExcludeHourly],[CompareOperator],[MinValue],[MaxValue],[DecimalPrecision])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'_Filter_1',0,'Ratio Incoming Calls on Outgoing Calls','Ratio','MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2',0,1,0.010,0.990,2),
(2,'_Filter_2',0,'Count of Distinct Called Parties','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0),
(3,'_Filter_3',0,'Count of Outgoing Calls','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0),
(4,'_Filter_4',1,'Count of Total Connected BTS','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,1,1.000,999999.000,0),
(5,'_Filter_5',0,'Total Originated Volume in Minutes','Volume','MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2',0,0,0.010,999999.000,2),
(6,'_Filter_6',1,'Count of Total IMEIs','Count','MinValue = 0, MaxValue = 100, DecimalPrecision = 0',0,0,0.000,100.000,0),
(7,'_Filter_7',0,'Ratio Average Incoming Duration on Average Outgoing Duration','Ratio','MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2',0,1,0.010,0.990,2),
(8,'_Filter_8',1,'Ratio OffNet Originated Calls on OnNet Originated Calls','Ratio','MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2',0,1,0.010,999999.000,2),
(9,'_Filter_9',0,'Count of Daily Active Hours','Count','MinValue = 1, MaxValue = 24, DecimalPrecision = 0',1,0,1.000,24.000,0),
(10,'_Filter_10',0,'Distinct Called Parties during Night Period','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',1,0,1.000,999999.000,0),
(11,'_Filter_11',1,'Count of Sent SMSs','Count','MinValue = 0, MaxValue = int.MaxValue, DecimalPrecision = 0',0,1,0.000,999999.000,0),
(12,'_Filter_12',0,'Ratio Distinct Called Parties on Total Outgoing Calls','Ratio','MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2',0,0,0.010,1.000,2),
(13,'_Filter_13',1,'Ratio International Outgoing Calls on Outgoing Calls','Ratio','MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2',0,0,0.010,1.000,2),
(14,'_Filter_14',0,'Count of Outgoing Calls during Peak Hours','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',1,0,1.000,999999.000,0),
(15,'_Filter_15',1,'Data Usage Volume in Mbytes','Volume','MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2',0,1,0.010,999999.000,2),
(16,'_Filter_16',0,'Count of Consecutive Calls','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0),
(17,'_Filter_17',0,'Count of Consecutive Failed Calls','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0),
(18,'_Filter_18',0,'Count Incoming “low duration” Calls on Count Incoming Calls','Ratio','MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2',0,0,0.010,1.000,2),
(19,'_Filter_19',2,'Different Destination Zones','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0),
(20,'_Filter_20',2,'Different Source Zones','Count','MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0',0,0,1.000,999999.000,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Abbreviation],[OperatorTypeAllowed],[Description],[Label],[ToolTip],[ExcludeHourly],[CompareOperator],[MinValue],[MaxValue],[DecimalPrecision]))
merge	[FraudAnalysis].[Filter] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Abbreviation] = s.[Abbreviation],[OperatorTypeAllowed] = s.[OperatorTypeAllowed],[Description] = s.[Description],[Label] = s.[Label],[ToolTip] = s.[ToolTip],[ExcludeHourly] = s.[ExcludeHourly],[CompareOperator] = s.[CompareOperator],[MinValue] = s.[MinValue],[MaxValue] = s.[MaxValue],[DecimalPrecision] = s.[DecimalPrecision]
when not matched by target then
	insert([ID],[Abbreviation],[OperatorTypeAllowed],[Description],[Label],[ToolTip],[ExcludeHourly],[CompareOperator],[MinValue],[MaxValue],[DecimalPrecision])
	values(s.[ID],s.[Abbreviation],s.[OperatorTypeAllowed],s.[Description],s.[Label],s.[ToolTip],s.[ExcludeHourly],s.[CompareOperator],s.[MinValue],s.[MaxValue],s.[DecimalPrecision])
when not matched by source then
	delete;

--[FraudAnalysis].[Aggregate]-----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[OperatorTypeAllowed],[NumberPrecision])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Count Out Calls',0,'NoDecimal'),
(2,'Count In Calls',0,'NoDecimal'),
(3,'Total Data Volume',1,'LongPrecision'),
(4,'Count Out Fails',0,'NoDecimal'),
(5,'Count In Fails',0,'NoDecimal'),
(6,'Count Out SMSs',1,'NoDecimal'),
(7,'Count Out OffNets',0,'NoDecimal'),
(8,'Count Out OnNets',0,'NoDecimal'),
(9,'Count Out Inters',0,'NoDecimal'),
(10,'Count In Inters',0,'NoDecimal'),
(11,'Call Out Dur Avg',0,'LongPrecision'),
(12,'Call In Dur Avg',0,'LongPrecision'),
(13,'Total Out Volume',1,'LongPrecision'),
(14,'Total In Volume',1,'LongPrecision'),
(15,'Total IMEI',1,'NoDecimal'),
(16,'Total BTS',1,'NoDecimal'),
(17,'Diff Output Numbers',0,'NoDecimal'),
(18,'Diff Input Numbers',0,'NoDecimal'),
(19,'Count In OffNets',0,'NoDecimal'),
(20,'Count In OnNets',0,'NoDecimal'),
(21,'Diff Output Numbers NightCalls',0,'NoDecimal'),
(22,'Count Out Calls Peak Hours',0,'NoDecimal'),
(23,'Count Consecutive Calls',0,'NoDecimal'),
(24,'Count Active Hours',0,'NoDecimal'),
(25,'Count Fail Consecutive Calls',0,'NoDecimal'),
(26,'Count In Low Duration Calls',0,'NoDecimal'),
(27,'Diff Dest Zones',2,'NoDecimal'),
(28,'Diff Sources Zones',2,'NoDecimal')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[OperatorTypeAllowed],[NumberPrecision]))
merge	[FraudAnalysis].[Aggregate] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[OperatorTypeAllowed] = s.[OperatorTypeAllowed],[NumberPrecision] = s.[NumberPrecision]
when not matched by target then
	insert([ID],[Name],[OperatorTypeAllowed],[NumberPrecision])
	values(s.[ID],s.[Name],s.[OperatorTypeAllowed],s.[NumberPrecision])
when not matched by source then
	delete;