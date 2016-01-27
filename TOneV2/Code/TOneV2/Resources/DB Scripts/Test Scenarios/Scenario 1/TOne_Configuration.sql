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
--common.Country------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[SourceID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Lebanon',null),
(2,'Syria',null),
(3,'Germany',null),
(4,'France',null),
(5,'Iran',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[SourceID]))
merge	[common].[Country] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[SourceID] = s.[SourceID]
when not matched by target then
	insert([ID],[Name],[SourceID])
	values(s.[ID],s.[Name],s.[SourceID])
when not matched by source then
	delete;
	
--common.Currency-----------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[Currency] on;
;with cte_data([ID],[Symbol],[Name])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'USD','USD')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Symbol],[Name]))
merge	[common].[Currency] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Symbol] = s.[Symbol],[Name] = s.[Name]
when not matched by target then
	insert([ID],[Symbol],[Name])
	values(s.[ID],s.[Symbol],s.[Name])
when not matched by source then
	delete;
set identity_insert [common].[Currency] off;

--[common].[City]-----------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[City] on;
;with cte_data([ID],[Name],[CountryID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Beirut',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[CountryID]))
merge	[common].[City] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[CountryID] = s.[CountryID]
when not matched by target then
	insert([ID],[Name],[CountryID])
	values(s.[ID],s.[Name],s.[CountryID])
when not matched by source then
	delete;
set identity_insert [common].[City] off;

--common.IDManager----------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([TypeID],[LastTakenID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,0),
(2,0),
(3,0),
(4,0),
(5,0),
(6,5),
(7,0),
(8,0),
(9,0),
(10,0),
(12,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([TypeID],[LastTakenID]))
merge	[common].[IDManager] as t
using	cte_data as s
on		1=1 and t.[TypeID] = s.[TypeID]
when matched then
	update set
	[LastTakenID] = s.[LastTakenID]
when not matched by target then
	insert([TypeID],[LastTakenID])
	values(s.[TypeID],s.[LastTakenID])
when not matched by source then
	delete;
	

TRUNCATE TABLE StatisticManagement.StatisticBatch