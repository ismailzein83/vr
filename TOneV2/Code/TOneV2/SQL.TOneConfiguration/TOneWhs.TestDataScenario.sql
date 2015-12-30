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
	
--[rules].[Rule]------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [rules].[Rule] on;
;with cte_data([ID],[TypeID],[RuleDetails],[BED],[EED])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,10,'{"$type":"TOne.WhS.Routing.Entities.RouteRule, TOne.WhS.Routing.Entities","Criteria":{"$type":"TOne.WhS.Routing.Entities.RouteRuleCriteria, TOne.WhS.Routing.Entities","RoutingProductId":1,"ExcludedCodes":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]}},"Settings":{"$type":"TOne.WhS.Routing.Business.RegularRouteRule, TOne.WhS.Routing.Business","ConfigId":15},"RuleId":0,"BeginEffectiveTime":"2015-12-29T16:51:00Z"}','2015-12-29 16:51:00.000',null),
(2,6,'{"$type":"TOne.WhS.CDRProcessing.Entities.CustomerIdentificationRule, TOne.WhS.CDRProcessing.Entities","Criteria":{"$type":"TOne.WhS.CDRProcessing.Entities.CustomerIdentificationRuleCriteria, TOne.WhS.CDRProcessing.Entities","InTrunks":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]},"InCarriers":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["1-0:9"]},"CDPNPrefixes":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]}},"Settings":{"$type":"TOne.WhS.CDRProcessing.Entities.CustomerIdentificationRuleSettings, TOne.WhS.CDRProcessing.Entities","CustomerId":1},"Description":"Protel","RuleId":0,"BeginEffectiveTime":"2014-12-29T00:00:00Z"}','2014-12-29 00:00:00.000',null),
(3,6,'{"$type":"TOne.WhS.CDRProcessing.Entities.CustomerIdentificationRule, TOne.WhS.CDRProcessing.Entities","Criteria":{"$type":"TOne.WhS.CDRProcessing.Entities.CustomerIdentificationRuleCriteria, TOne.WhS.CDRProcessing.Entities","InTrunks":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]},"InCarriers":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["1-0:10"]},"CDPNPrefixes":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]}},"Settings":{"$type":"TOne.WhS.CDRProcessing.Entities.CustomerIdentificationRuleSettings, TOne.WhS.CDRProcessing.Entities","CustomerId":2},"Description":"Spactron","RuleId":0,"BeginEffectiveTime":"2014-12-29T00:00:00Z"}','2014-12-29 00:00:00.000',null),
(4,7,'{"$type":"TOne.WhS.CDRProcessing.Entities.SupplierIdentificationRule, TOne.WhS.CDRProcessing.Entities","Criteria":{"$type":"TOne.WhS.CDRProcessing.Entities.SupplierIdentificationRuleCriteria, TOne.WhS.CDRProcessing.Entities","OutTrunks":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]},"OutCarriers":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["1-0:11"]},"CDPNPrefixes":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]}},"Settings":{"$type":"TOne.WhS.CDRProcessing.Entities.SupplierIdentificationRuleSettings, TOne.WhS.CDRProcessing.Entities","SupplierId":3},"Description":"Sama","RuleId":0,"BeginEffectiveTime":"2014-12-29T00:00:00Z"}','2014-12-29 00:00:00.000',null),
(5,7,'{"$type":"TOne.WhS.CDRProcessing.Entities.SupplierIdentificationRule, TOne.WhS.CDRProcessing.Entities","Criteria":{"$type":"TOne.WhS.CDRProcessing.Entities.SupplierIdentificationRuleCriteria, TOne.WhS.CDRProcessing.Entities","OutTrunks":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]},"OutCarriers":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["1-0:12"]},"CDPNPrefixes":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]}},"Settings":{"$type":"TOne.WhS.CDRProcessing.Entities.SupplierIdentificationRuleSettings, TOne.WhS.CDRProcessing.Entities","SupplierId":4},"Description":"MHD","RuleId":0,"BeginEffectiveTime":"2014-12-29T00:00:00Z"}','2014-12-29 00:00:00.000',null),
(6,7,'{"$type":"TOne.WhS.CDRProcessing.Entities.SupplierIdentificationRule, TOne.WhS.CDRProcessing.Entities","Criteria":{"$type":"TOne.WhS.CDRProcessing.Entities.SupplierIdentificationRuleCriteria, TOne.WhS.CDRProcessing.Entities","OutTrunks":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]},"OutCarriers":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["1-0:13"]},"CDPNPrefixes":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]}},"Settings":{"$type":"TOne.WhS.CDRProcessing.Entities.SupplierIdentificationRuleSettings, TOne.WhS.CDRProcessing.Entities","SupplierId":5},"Description":"NetTalk","RuleId":0,"BeginEffectiveTime":"2014-12-29T00:00:00Z"}','2014-12-29 00:00:00.000',null),
(7,12,'{"$type":"TOne.WhS.CDRProcessing.Entities.SwitchIdentificationRule, TOne.WhS.CDRProcessing.Entities","Description":"Default Switch Identity","Criteria":{"$type":"TOne.WhS.CDRProcessing.Entities.SwitchIdentificationRuleCriteria, TOne.WhS.CDRProcessing.Entities","DataSources":{"$type":"System.Collections.Generic.List`1[[System.Int32, mscorlib]], mscorlib","$values":[1]}},"Settings":{"$type":"TOne.WhS.CDRProcessing.Entities.SwitchIdentificationRuleSettings, TOne.WhS.CDRProcessing.Entities","SwitchId":1},"RuleId":0,"BeginEffectiveTime":"2014-12-29T00:00:00Z"}','2014-12-29 00:00:00.000',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[TypeID],[RuleDetails],[BED],[EED]))
merge	[rules].[Rule] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[TypeID] = s.[TypeID],[RuleDetails] = s.[RuleDetails],[BED] = s.[BED],[EED] = s.[EED]
when not matched by target then
	insert([ID],[TypeID],[RuleDetails],[BED],[EED])
	values(s.[ID],s.[TypeID],s.[RuleDetails],s.[BED],s.[EED])
when not matched by source then
	delete;
set identity_insert [rules].[Rule] off;
	
