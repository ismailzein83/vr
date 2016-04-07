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
--[common].[TemplateConfig]----------------------70001 to 80000-------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[TemplateConfig] on;
;with cte_data([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(70001,'AddPrefix','VR_Rules_NormalizeNumberAction','vr-rules-normalizationnumbersettings-addprefix',null,null),
(70002,'ReplaceString','VR_Rules_NormalizeNumberAction','vr-rules-normalizationnumbersettings-replacestring',null,null),
(70003,'Substring','VR_Rules_NormalizeNumberAction','vr-rules-normalizationnumbersettings-substring',null,null),
(70004,'Days Of Week','VR_Rules_PricingRuleRateTypeSettings','vr-rules-pricingrulesettings-ratetype-daysofweek',null,null),
(70005,'Specific','VR_Rules_PricingRuleRateTypeSettings','vr-rules-pricingrulesettings-ratetype-specific',null,null),
(70006,'rules','VR_Rules_PricingRuleTariffSettings','vr-rules-pricingrulesettings-tariff-regular',null,null),
(70007,'Fixed Extra Charge','VR_Rules_PricingRuleExtraChargeSettings','vr-rules-pricingrulesettings-extracharge-fixed',null,null),
(70008,'Percentage Extra Charge','VR_Rules_PricingRuleExtraChargeSettings','vr-rules-pricingrulesettings-extracharge-percentage',null,null),
(70009,'Fixed Rate Value','VR_Rules_PricingRuleRateValueSettings','vr-rules-pricingrulesettings-ratevalue-fixed',null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings]))
merge	[common].[TemplateConfig] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ConfigType] = s.[ConfigType],[Editor] = s.[Editor],[BehaviorFQTN] = s.[BehaviorFQTN],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
	values(s.[ID],s.[Name],s.[ConfigType],s.[Editor],s.[BehaviorFQTN],s.[Settings]);
set identity_insert [common].[TemplateConfig] off;