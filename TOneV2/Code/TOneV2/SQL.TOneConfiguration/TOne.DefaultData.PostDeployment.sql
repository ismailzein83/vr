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
*/--default data

declare @TariffRuleTypeIdTable as table
(ID int)
insert into @TariffRuleTypeIdTable 
exec [rules].[sp_RuleType_InsertIfNotExistsAndGetID] 'Vanrise.GenericData.Pricing.TariffRule' 

declare @TariffRuleTypeId int
select @TariffRuleTypeId=ID from @TariffRuleTypeIdTable
--[rules].[Rule]------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([TypeID],[RuleDetails],[BED],[EED],[SourceID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(@TariffRuleTypeId,'{"$type":"Vanrise.GenericData.Pricing.TariffRule, Vanrise.GenericData.Pricing","Settings":{"$type":"Vanrise.Rules.Pricing.MainExtensions.Tariff.RegularTariffSettings, Vanrise.Rules.Pricing.MainExtensions","ConfigId":"35acc9c2-0675-4347-ba3e-a81025c1be12","CallFee":0.0,"FirstPeriod":0,"FirstPeriodRate":0.0,"FractionUnit":0,"PricingUnit":60,"CurrencyId":155},"DefinitionId":"f24cb510-0b65-48c8-a723-1f6ebfeea9e8","RuleId":0,"Description":"Default Sale Tariff Rule","BeginEffectiveTime":"2016-12-23T00:00:00","RefreshTimeSpan":"01:00:00"}','2010-01-01 00:00:00.000',null,null),
(@TariffRuleTypeId,'{"$type":"Vanrise.GenericData.Pricing.TariffRule, Vanrise.GenericData.Pricing","Settings":{"$type":"Vanrise.Rules.Pricing.MainExtensions.Tariff.RegularTariffSettings, Vanrise.Rules.Pricing.MainExtensions","ConfigId":"35acc9c2-0675-4347-ba3e-a81025c1be12","CallFee":0.0,"FirstPeriod":0,"FirstPeriodRate":0.0,"FractionUnit":0,"PricingUnit":60,"CurrencyId":155},"DefinitionId":"5aeb0dad-4bb8-44b4-acbe-c8c917e88b58","RuleId":0,"Description":"Default Purchase Tariff Rule","BeginEffectiveTime":"2016-12-23T00:00:00","RefreshTimeSpan":"01:00:00"}','2010-01-01 00:00:00.000',null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([TypeID],[RuleDetails],[BED],[EED],[SourceID]))
merge	[rules].[Rule] as t
using	cte_data as s
on		1=1 and t.[RuleDetails] = s.[RuleDetails]
--when matched then
--	update set
--	[TypeID] = s.[TypeID],[RuleDetails] = s.[RuleDetails],[BED] = s.[BED],[EED] = s.[EED],[SourceID] = s.[SourceID]
when not matched by target then
	insert([TypeID],[RuleDetails],[BED],[EED],[SourceID])
	values(s.[TypeID],s.[RuleDetails],s.[BED],s.[EED],s.[SourceID]);

--[TOneWhS_BE].[ZoneServiceConfig]----------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;set identity_insert [TOneWhS_BE].[ZoneServiceConfig] on;;with cte_data([ID],[Symbol],[Settings],[SourceID])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(1,'WHS','{"$type":"TOne.WhS.BusinessEntity.Entities.ServiceConfigSetting, TOne.WhS.BusinessEntity.Entities","Name":"Wholesale","Color":"#C0C0C0","ParentId":2,"Description":"Wholesale Marker","Weight":1}','1'),(2,'RTL','{"$type":"TOne.WhS.BusinessEntity.Entities.ServiceConfigSetting, TOne.WhS.BusinessEntity.Entities","Name":"Retail","Color":"#0000FF","ParentId":3,"Description":"Retail Flagged Service","Weight":3}','3'),(3,'PRM','{"$type":"TOne.WhS.BusinessEntity.Entities.ServiceConfigSetting, TOne.WhS.BusinessEntity.Entities","Name":"Premium","Color":"#FFA500","ParentId":4,"Description":"Premium Quality Service","Weight":7}','7'),(4,'CLI','{"$type":"TOne.WhS.BusinessEntity.Entities.ServiceConfigSetting, TOne.WhS.BusinessEntity.Entities","Name":"CLI","Color":"#FF0000","Description":"CLI Provided Service","Weight":15}','15'),(5,'DRC','{"$type":"TOne.WhS.BusinessEntity.Entities.ServiceConfigSetting, TOne.WhS.BusinessEntity.Entities","Name":"Direct","Color":"#00FF00","Description":"Direct Connection","Weight":16}','16'),(6,'TRS','{"$type":"TOne.WhS.BusinessEntity.Entities.ServiceConfigSetting, TOne.WhS.BusinessEntity.Entities","Name":"Transit","Color":"#FFFF00","Description":"Transit Connections (Like in Ministries)","Weight":32}','32'),(7,'VID','{"$type":"TOne.WhS.BusinessEntity.Entities.ServiceConfigSetting, TOne.WhS.BusinessEntity.Entities","Name":"Test Service","Color":"#800000","Description":"Call Generator Tester","Weight":64}','64'),(8,'3GM','{"$type":"TOne.WhS.BusinessEntity.Entities.ServiceConfigSetting, TOne.WhS.BusinessEntity.Entities","Name":"Blocked Service","Color":"#000000","Description":"Faulty Route Block","Weight":128}','128')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Symbol],[Settings],[SourceID]))merge	[TOneWhS_BE].[ZoneServiceConfig] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Symbol] = s.[Symbol],[Settings] = s.[Settings],[SourceID] = s.[SourceID]when not matched by target then	insert([ID],[Symbol],[Settings],[SourceID])	values(s.[ID],s.[Symbol],s.[Settings],s.[SourceID]);set identity_insert [TOneWhS_BE].[ZoneServiceConfig] off;--[TOneWhS_BE].[SellingNumberPlan]----------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;set identity_insert [TOneWhS_BE].[SellingNumberPlan] on;;with cte_data([ID],[Name])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(1,'Default')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name]))merge	[TOneWhS_BE].[SellingNumberPlan] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name]when not matched by target then	insert([ID],[Name])	values(s.[ID],s.[Name]);set identity_insert [TOneWhS_BE].[SellingNumberPlan] off;--[TOneWhS_BE].[SellingProduct]-------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;set identity_insert [TOneWhS_BE].[SellingProduct] on;;with cte_data([ID],[Name],[DefaultRoutingProductID],[SellingNumberPlanID],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(1,'Default',null,1,'null')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[DefaultRoutingProductID],[SellingNumberPlanID],[Settings]))merge	[TOneWhS_BE].[SellingProduct] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[DefaultRoutingProductID] = s.[DefaultRoutingProductID],[SellingNumberPlanID] = s.[SellingNumberPlanID],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[DefaultRoutingProductID],[SellingNumberPlanID],[Settings])	values(s.[ID],s.[Name],s.[DefaultRoutingProductID],s.[SellingNumberPlanID],s.[Settings]);set identity_insert [TOneWhS_BE].[SellingProduct] off;--[TOneWhS_BE].[RoutingProduct]-------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;set identity_insert [TOneWhS_BE].[RoutingProduct] on;;with cte_data([ID],[Name],[SellingNumberPlanID],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(1,'LCR',1,'{"$type":"TOne.WhS.BusinessEntity.Entities.RoutingProductSettings, TOne.WhS.BusinessEntity.Entities","DefaultServiceIds":{"$type":"System.Collections.Generic.HashSet`1[[System.Int32, mscorlib]], System.Core","$values":[1]},"ZoneRelationType":0,"SupplierRelationType":0}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[SellingNumberPlanID],[Settings]))merge	[TOneWhS_BE].[RoutingProduct] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[SellingNumberPlanID] = s.[SellingNumberPlanID],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[SellingNumberPlanID],[Settings])	values(s.[ID],s.[Name],s.[SellingNumberPlanID],s.[Settings]);set identity_insert [TOneWhS_BE].[RoutingProduct] off;DECLARE	@SaleEntityRoutingProductTypeId int
declare @SaleEntityRoutingProductTypeIdTable as table (TypeId int)
insert into @SaleEntityRoutingProductTypeIdTable exec [common].[sp_Type_InsertIfNotExistsAndGetID] 'TOne.WhS.BusinessEntity.Business.SaleEntityRoutingProductManager, TOne.WhS.BusinessEntity.Business, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
SELECT	@SaleEntityRoutingProductTypeId = TypeId from @SaleEntityRoutingProductTypeIdTabledeclare @SaleEntityRoutingProductIdTable as table
(ID bigint)
insert into @SaleEntityRoutingProductIdTable 
exec [common].[sp_IDManager_ReserveIDRange] @SaleEntityRoutingProductTypeId, 1

declare @SaleEntityRoutingProductId bigint
select @SaleEntityRoutingProductId=ID from @SaleEntityRoutingProductIdTable--[TOneWhS_BE].[SaleEntityRoutingProduct]---------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[OwnerType],[OwnerID],[ZoneID],[RoutingProductID],[BED],[EED])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(@SaleEntityRoutingProductId,0,1,null,1,'2000-01-01 00:00:00.000',null)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OwnerType],[OwnerID],[ZoneID],[RoutingProductID],[BED],[EED]))merge	[TOneWhS_BE].[SaleEntityRoutingProduct] as tusing	cte_data as son		1=1 and t.[OwnerType]=s.[OwnerType] and t.[OwnerID]=s.[OwnerID] and t.[ZoneID] is null--when matched then--	update set--	[OwnerType] = s.[OwnerType],[OwnerID] = s.[OwnerID],[ZoneID] = s.[ZoneID],[RoutingProductID] = s.[RoutingProductID],[BED] = s.[BED],[EED] = s.[EED]when not matched by target then	insert([ID],[OwnerType],[OwnerID],[ZoneID],[RoutingProductID],[BED],[EED])	values(s.[ID],s.[OwnerType],s.[OwnerID],s.[ZoneID],s.[RoutingProductID],s.[BED],s.[EED]);declare @RouteRuleTypeIdTable as table
(ID int)
insert into @RouteRuleTypeIdTable 
exec [rules].[sp_RuleType_InsertIfNotExistsAndGetID] 'TOne.WhS.Routing.Entities.RouteRule' 

declare @RouteRuleTypeId int
select @RouteRuleTypeId=ID from @RouteRuleTypeIdTable--[rules].[Rule]------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([TypeID],[RuleDetails],[BED],[EED],[SourceID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(@RouteRuleTypeId,'{"$type":"TOne.WhS.Routing.Entities.RouteRule, TOne.WhS.Routing.Entities","Criteria":{"$type":"TOne.WhS.Routing.Entities.RouteRuleCriteria, TOne.WhS.Routing.Entities","ConfigId":"6fd3f59f-33f1-4d42-8364-7030ae79b249","RoutingProductId":1,"ExcludedCodes":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":[]}},"Settings":{"$type":"TOne.WhS.Routing.Business.LCRRouteRule, TOne.WhS.Routing.Business","ConfigId":"31b3226e-a2b2-40d5-8c33-83c6601e8730","UseOrderedExecution":true,"CorrespondentType":3},"CorrespondentType":3,"BED":"2000-01-01T00:00:00","RuleId":0,"BeginEffectiveTime":"2000-01-01T00:00:00","RefreshTimeSpan":"01:00:00"}','2000-01-01 00:00:00.000',null,'Default route rule (from script)')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([TypeID],[RuleDetails],[BED],[EED],[SourceID]))
merge	[rules].[Rule] as t
using	cte_data as s
on		1=1 and t.[SourceID] = s.[SourceID]
--when matched then
--	update set
--	[TypeID] = s.[TypeID],[RuleDetails] = s.[RuleDetails],[BED] = s.[BED],[EED] = s.[EED],[SourceID] = s.[SourceID]
when not matched by target then
	insert([TypeID],[RuleDetails],[BED],[EED],[SourceID])
	values(s.[TypeID],s.[RuleDetails],s.[BED],s.[EED],s.[SourceID]);--end Default data