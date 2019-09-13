﻿



















--Make sure to use same .json file using DEVTOOLS under http://192.168.110.185:8037




























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


--[common].[extensionconfiguration]-------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('9B441D3F-1A1D-4060-8BB1-740CEF377E0D','Fixed Extra Charge','Fixed Extra Charge','VR_Rules_PricingRuleExtraChargeSettings'				,'{"Editor":"vr-rules-pricingrulesettings-extracharge-fixed"}'),
('53F5FB2A-0390-4821-A795-9146615CF584','Fixed Rate Value','Fixed Rate Value','VR_Rules_PricingRuleRateValueSettings'					,'{"Editor":"vr-rules-pricingrulesettings-ratevalue-fixed"}'),

('35ACC9C2-0675-4347-BA3E-A81025C1BE12','Custom','Custom','VR_Rules_PricingRuleTariffSettings'											,'{"Editor":"vr-rules-pricingrulesettings-tariff-regular"}'),
('F155FBF2-4F7E-444E-B197-16FDEAFE4FAA','SixtyOne','60/1','VR_Rules_PricingRuleTariffSettings'											,'{"Editor":"vr-rules-pricingrulesettings-tariff-sixtyone"}'),
('E973CB63-4652-4E3D-9679-59B98DE952CD','SixtySixty','60/60','VR_Rules_PricingRuleTariffSettings'										,'{"Editor":"vr-rules-pricingrulesettings-tariff-sixtysixty"}'),
('8B86004A-754B-45C2-ADB5-CAEB220287B0','Ceiling','Ceiling','VR_Rules_PricingRuleTariffSettings'										,'{"Editor":"vr-rules-pricingrulesettings-tariff-ceiling"}'),

('9387367E-4BBC-4F8A-958F-AF27EFFC7EC4','Percentage Extra Charge','Percentage Extra Charge','VR_Rules_PricingRuleExtraChargeSettings'	,'{"Editor":"vr-rules-pricingrulesettings-extracharge-percentage"}'),
('D642AA26-C072-43EF-98F1-EE84F05F4069','Days Of Week','Days Of Week','VR_Rules_PricingRuleRateTypeSettings'							,'{"Editor":"vr-rules-pricingrulesettings-ratetype-daysofweek"}'),
('F87EE98B-673F-4095-8658-F6AA7E3966D3','Specific','Specific','VR_Rules_PricingRuleRateTypeSettings'									,'{"Editor":"vr-rules-pricingrulesettings-ratetype-specific"}'),
('8C340085-E102-4504-A49B-329480CC7605','Percentage Tax','Percentage Tax','VR_Rules_PricingRuleTaxAction'								,'{"Editor":"vr-rules-pricingrulesettings-tax-percentage"}'),

('B285D8DD-B628-4DF0-B28C-114EBB9BED5A','Substring','Substring','VR_Rules_NormalizeNumberAction'										,'{"Editor":"vr-rules-normalizationnumbersettings-substring"}'),
('2B333F37-21B2-436C-92F5-CFAA9912B388','AddPrefix','AddPrefix','VR_Rules_NormalizeNumberAction'										,'{"Editor":"vr-rules-normalizationnumbersettings-addprefix"}'),
('12A627F4-5E64-4957-B3F9-E0B890955037','ReplaceString','ReplaceString','VR_Rules_NormalizeNumberAction'								,'{"Editor":"vr-rules-normalizationnumbersettings-replacestring"}'),
('6DD13404-F488-4D59-A2A0-2135D3826B28','Remove','Remove','VR_Rules_NormalizeNumberAction'												,'{"Editor":"vr-rules-normalizationnumbersettings-remove"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings]))
merge	[common].[extensionconfiguration] as t
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

