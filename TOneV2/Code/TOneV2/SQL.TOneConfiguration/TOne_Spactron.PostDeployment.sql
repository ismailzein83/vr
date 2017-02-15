--[common].[ExtensionConfiguration]-------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('31B3226E-A2B2-40D5-8C33-83C6601E8730','LCR','LCR','WhS_Routing_RouteRuleSettingsType','{"Editor":"vr-whs-routing-routerulesettings-lcr", "DisplayOrder":"1","Priority":4}'),('0166E5C4-0F13-4741-BD77-2C771BCAFA24','SpecialRequest','Special Request','WhS_Routing_RouteRuleSettingsType','{"Editor":"vr-whs-routing-routerulesettings-specialrequest", "DisplayOrder":"2","Priority":3}'),('5A492AA2-9642-453C-8B18-967D745AD812','Advanced','Advanced','WhS_Routing_RouteRuleSettingsType','{"Editor":"vr-whs-routing-routerulesettings-regular", "DisplayOrder":"21"}'),('12E2CAD6-ABD9-4D2B-B2F3-51C3DF501DE9','Fixed','Fixed','WhS_Routing_RouteRuleSettingsType','{"Editor":"vr-whs-routing-routerulesettings-fixed", "DisplayOrder":"11","Priority":2}'),('BBB0CA31-0FCD-4035-A8ED-5D4BAD06C662','Block','Block','WhS_Routing_RouteRuleSettingsType'	,'{"Editor":"vr-whs-routing-routerulesettings-block", "DisplayOrder":"31","Priority":1}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))
merge	[common].[ExtensionConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[ConfigType],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);
----------------------------------------------------------------------------------------------------
end