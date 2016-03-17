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
--sec.WidgetDefinition------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert sec.[WidgetDefinition] on;
;with cte_data([ID],[Name],[DirectiveName],[Setting])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Report                                            ','vr-bi-datagrid                                    ','{"DirectiveTemplateURL":"/Client/Modules/BI/Directives/Templates/vr-bi-datagrid-directive-template.html","Sections":[1]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                '),
(2,'Chart                                             ','vr-bi-chart                                       ','{"DirectiveTemplateURL":"/Client/Modules/BI/Directives/Templates/vr-bi-chart-directive-template.html","Sections":[1]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   '),
(3,'Summary                                           ','vr-bi-summary                                     ','{"DirectiveTemplateURL":"/Client/Modules/BI/Directives/Templates/vr-bi-summary-directive-template.html","Sections":[0]}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 ')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DirectiveName],[Setting]))
merge	[sec].[WidgetDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DirectiveName] = s.[DirectiveName],[Setting] = s.[Setting]
when not matched by target then
	insert([ID],[Name],[DirectiveName],[Setting])
	values(s.[ID],s.[Name],s.[DirectiveName],s.[Setting])
when not matched by source then
	delete;
set identity_insert [sec].[WidgetDefinition] off;
--[BI].[SchemaConfiguration]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [BI].[SchemaConfiguration] on;
;with cte_data([ID],[Name],[DisplayName],[Type],[Configuration],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'CaseStatus','Case Status',0,'{"ColumnID":"[Dim Case Status].[Pk Case Status Id]","ColumnName":"[Dim Case Status].[Name]"}',null),
(2,'CaseCount','Case Count',1,'{"ColumnName":"[Measures].[Fact Cases Count]","Exepression":"","Unit":""}',null),
(4,'CaseId-Fact Cases','Case Id -  Fact Cases',1,'{"ColumnName":"[Measures].[MS Case Id - Fact Cases]","Exepression":"","Unit":""}',null),
(6,'CaseCDRId','Case CDR Id',1,'{"ColumnName":"[Measures].[MS CDR Id - Fact Cases]","Exepression":"","Unit":""}',null),
(8,'CaseMSDuration','Case MS Duration',1,'{"ColumnName":"[Measures].[MS Duration - Fact Cases]","Exepression":"","Unit":""}',null),
(9,'CallMSDuration','Call MS Duration',1,'{"ColumnName":"[Measures].[MS Duration]","Exepression":"","Unit":""}',null),
(10,'CallMSCDRId','Call MS CDR Id',1,'{"ColumnName":"[Measures].[MS CDR Id]","Exepression":"","Unit":""}',null),
(11,'CallCount','Call Count',1,'{"ColumnName":"[Measures].[Fact Calls Count]","Exepression":"","Unit":""}',null),
(12,'CallMSCaseId','Call MS Case Id',1,'{"ColumnName":"[Measures].[MS Case Id]","Exepression":"","Unit":""}',null),
(13,'CaseUser','Case User',0,'{"ColumnID":"[Case User].[Pk User Id]","ColumnName":"[Case User].[Name]"}',null),
(14,'BTS','BTS',0,'{"ColumnID":"[Dim BTS].[Pk BTS Id]","ColumnName":"[Dim BTS].[Name]"}',null),
(15,'CallClass','Call Class',0,'{"ColumnID":"[Dim Call Class].[Pk Call Class Id]","ColumnName":"[Dim Call Class].[Name]"}',null),
(16,'CallType','Call Type',0,'{"ColumnID":"[Dim Call Type].[Pk Call Type Id]","ColumnName":"[Dim Call Type].[Name]"}',null),
(17,'Network Type','Network Type',0,'{"ColumnID":"[Dim Network Type].[Pk Net Type Id]","ColumnName":"[Dim Network Type].[Name]"}',null),
(18,'Period','Period',0,'{"ColumnID":"[Dim Period].[Pk Period Id]","ColumnName":"[Dim Period].[Name]"}',null),
(19,'Strategy','Strategy',0,'{"ColumnID":"[Dim Strategy].[Pk Strategy Id]","ColumnName":"[Dim Strategy].[Name]"}',null),
(20,'Strategy Kind','Strategy Kind',0,'{"ColumnID":"[Dim Strategy Kind].[PK Kind Id]","ColumnName":"[Dim Strategy Kind].[Name]"}',null),
(21,'Subscriber Type','Subscriber Type',0,'{"ColumnID":"[Dim Subscriber Type].[Pk Subscriber Type Id]","ColumnName":"[Dim Subscriber Type].[Name]"}',null),
(22,'Suspicion Level','Suspicion Level',0,'{"ColumnID":"[Dim Suspicion Level].[Pk Suspicion Level Id]","ColumnName":"[Dim Suspicion Level].[Name]"}',null),
(23,'Strategy User','Strategy User',0,'{"ColumnID":"[Strategy User].[Pk User Id]","ColumnName":"[Strategy User].[Name]"}',null),
(24,'MSISDNDistinctCountCases',' MSISDN Distinct Count Cases',1,'{"ColumnName":"[Measures].[MS MSISDN Distinct Count Cases]","Exepression":"","Unit":""}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DisplayName],[Type],[Configuration],[Rank]))
merge	[BI].[SchemaConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DisplayName] = s.[DisplayName],[Type] = s.[Type],[Configuration] = s.[Configuration],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[DisplayName],[Type],[Configuration],[Rank])
	values(s.[ID],s.[Name],s.[DisplayName],s.[Type],s.[Configuration],s.[Rank])
when not matched by source then
	delete;
set identity_insert [BI].[SchemaConfiguration] off;

--[sec].[Module]------------------------------901 to 1000------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(901,'Fraud Analysis','Fraud Analysis','Fraud Analysis',null,'/images/menu-icons/other.png',11,0),
(902,'Reports','Reports','Reports',null,'/images/menu-icons/busines intel.png',14,0),
(903,'Network Infrastructure','Network Infrastructure','Network Infrastructure',1,null,30,0),
(904,'Dynamic Management','Dynamic Management','Dynamic Management',1,null,15,0),
(905,'Business Intelligence','Business Intelligence','BI',null,'/images/menu-icons/busines intel.png',16,1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
set identity_insert [sec].[Module] off;

--[sec].[View]-----------------------------9001 to 10000--------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(9001,'Normalization Rule','Normalization Rule','#/view/PSTN_BusinessEntity/Views/Normalization/NormalizationRuleManagement',1,'Root/PSTN_BusinessEntity Module:View',null,null,0,31),
(9002,'Strategies','Strategies','#/view/FraudAnalysis/Views/Strategy/StrategyManagement',901,'Root/Strategy Module:View',null,null,0,1),
(9003,'Strategy Execution Log','Strategy Execution Log','#/view/FraudAnalysis/Views/StrategyExecution/StrategyExecutionManagement',	901,'	Root/Suspicion Analysis Module:View',null,null,0,	2	),
(9004,'Suspicious Numbers','Suspicious Numbers','#/view/FraudAnalysis/Views/SuspiciousAnalysis/SuspicionAnalysis',901,'Root/Suspicion Analysis Module:View',null,null,0,3),
(9005,'Cases Productivity','Cases Productivity','#/view/FraudAnalysis/Views/Reports/CasesProductivity',902,'Root/Reporting Module:View',null,null,0,1),
(9006,'Detected Lines Summary','Detected Lines Summary','#/view/FraudAnalysis/Views/Reports/BlockedLines',902,'Root/Reporting Module:View',null,null,0,2),
(9007,'Detected Lines Details','Detected Lines Details','#/view/FraudAnalysis/Views/Reports/LinesDetected',902,'Root/Reporting Module:View',null,null,0,3),
(9008,'Switches','Switches','#/view/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchManagement',903,'Root/PSTN_BusinessEntity Module:View',null,null,0,1),
(9009,'Trunks','Trunks','#/view/PSTN_BusinessEntity/Views/NetworkInfrastructure/TrunkManagement',903,'Root/PSTN_BusinessEntity Module:View',null,null,0,2),
(9010,'Switch Brands','Switch Brands','#/view/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchBrandManagement',903,'Root/PSTN_BusinessEntity Module:View',null,null,0,3),
(9011,'Widgets','Widgets Management','#/view/Security/Views/WidgetsPages/WidgetManagement',904,'Root/Administration Module/Dynamic Pages:View',null,null,0,1),
(9012,'Pages','Dynamic Pages Management','#/view/Security/Views/DynamicPages/DynamicPageManagement',904,'Root/Administration Module/Dynamic Pages:View',null,null,0,2)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Type],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Type],s.[Rank]);
set identity_insert [sec].[View] off;

--[sec].[BusinessEntityModule]------------------------901 to 1000----------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[Title],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(901,'Fraud Analysis Module','Fraud Analysis Module',1,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([Id],[Name],[Title],[ParentId],[BreakInheritance])
	values(s.[Id],s.[Name],s.[Title],s.[ParentId],s.[BreakInheritance]);
set identity_insert [sec].[BusinessEntityModule] off;


--[sec].[BusinessEntity]------------------2401 to 2700----------------------------------------------------------
----------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(2401,'Network Infrastructure','Network Infrastructure',2,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(2402,'Normalization Rule','Normalization Rule',2,0,'["View","Add","Edit", "Delete", "Full Control"]'),
(2403,'Number Prefixes','Number Prefixes',2,0,'["View","Edit"]'),
(2404,'Strategy','Strategy',901,0,'["View","Add","Edit", "Full Control"]'),
(2405,'Case Management','Case Management',901,0,'["View","Edit"]'),
(2406,'Strategy Execution','Strategy Execution',901,0,'["View"]'),
(2407,'Related Numbers','Related Numbers',901,0,'["View"]'),
(2408,'CDR','CDR',901,0,'["View"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
set identity_insert [sec].[BusinessEntity] off;

--[common].[TemplateConfig]----------20001 to 30000---------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[TemplateConfig] on;
;with cte_data([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(30001,'Add Prefix','PSTN_BE_AdjustNumberAction','vr-pstn-be-addprefix','PSTN.BusinessEntity.MainExtensions.Normalization.RuleTypes.NormalizeNumber.Actions.AddPrefixActionBehavior, PSTN.BusinessEntity.MainExtensions',null),
(30002,'Substring','PSTN_BE_AdjustNumberAction','vr-pstn-be-substring','PSTN.BusinessEntity.MainExtensions.Normalization.RuleTypes.NormalizeNumber.Actions.SubstringActionBehavior, PSTN.BusinessEntity.MainExtensions',null),
(30003,'Replace Characters','PSTN_BE_AdjustNumberAction','vr-pstn-be-replacestring','PSTN.BusinessEntity.MainExtensions.Normalization.RuleTypes.NormalizeNumber.Actions.ReplaceStringActionBehavior, PSTN.BusinessEntity.MainExtensions',null),
(30004,'Set Area Prefix','PSTN_BE_SetArea','vr-pstn-be-setareaprefix','PSTN.BusinessEntity.MainExtensions.Normalization.RuleTypes.SetArea.Behaviors.SetAreaPrefixBehavior, PSTN.BusinessEntity.MainExtensions',null)
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

--rules.RuleType------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [rules].[RuleType] on;
;with cte_data([ID],[Type])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'PSTN.BusinessEntity.Entities.NormalizationRule')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Type]))
merge	[rules].[RuleType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Type] = s.[Type]
when not matched by target then
	insert([ID],[Type])
	values(s.[ID],s.[Type])
when not matched by source then
	delete;
set identity_insert [rules].[RuleType] off;
--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('PSTN_BE/SwitchBrand/GetFilteredBrands','Network Infrastructure:View'),
('PSTN_BE/SwitchBrand/GetBrands','Network Infrastructure:View'),
('PSTN_BE/SwitchBrand/GetBrandById','Network Infrastructure:View'),
('PSTN_BE/SwitchBrand/UpdateBrand','Network Infrastructure:View & Network Infrastructure:Add'),
('PSTN_BE/SwitchBrand/AddBrand','Network Infrastructure:View & Network Infrastructure:Edit'),
('PSTN_BE/SwitchBrand/DeleteBrand','Network Infrastructure:View & Network Infrastructure:Delete'),
('PSTN_BE/Switch/GetFilteredSwitches','Network Infrastructure:View'),
('PSTN_BE/Switch/GetSwitchById','Network Infrastructure:View'),
('PSTN_BE/Switch/GetSwitches','Network Infrastructure:View'),
('PSTN_BE/Switch/GetSwitchesInfo','Network Infrastructure:View'),
('PSTN_BE/Switch/GetSwitchAssignedDataSources','Network Infrastructure:View'),
('PSTN_BE/Switch/UpdateSwitch','Network Infrastructure:View & Network Infrastructure:Edit'),
('PSTN_BE/Switch/AddSwitch','Network Infrastructure:View & Network Infrastructure:Add'),
('PSTN_BE/Switch/DeleteSwitch','Network Infrastructure:View & Network Infrastructure:Delete'),
('PSTN_BE/Trunk/GetFilteredTrunks','Network Infrastructure:View'),
('PSTN_BE/Trunk/GetTrunkById','Network Infrastructure:View'),
('PSTN_BE/Trunk/GetTrunksBySwitchIds','Network Infrastructure:View'),
('PSTN_BE/Trunk/GetTrunks','Network Infrastructure:View'),
('PSTN_BE/Trunk/AddTrunk','Network Infrastructure:View & Network Infrastructure:Add'),
('PSTN_BE/Trunk/UpdateTrunk','Network Infrastructure:View & Network Infrastructure:Edit'),
('PSTN_BE/Trunk/DeleteTrunk','Network Infrastructure:View & Network Infrastructure:Delete'),
('PSTN_BE/Trunk/GetTrunksInfo','Network Infrastructure:View'),
('PSTN_BE/NormalizationRule/GetFilteredNormalizationRules','Normalization Rule:View'),
('PSTN_BE/NormalizationRule/GetRule','Normalization Rule:View'),
('PSTN_BE/NormalizationRule/GetNormalizationRuleAdjustNumberActionSettingsTemplates','Normalization Rule:View'),
('PSTN_BE/NormalizationRule/GetNormalizationRuleSetAreaSettingsTemplates','Normalization Rule:View'),
('PSTN_BE/NormalizationRule/AddRule','Normalization Rule:View & Normalization Rule:Add'),
('PSTN_BE/NormalizationRule/UpdateRule','Normalization Rule:View &Normalization Rule:Edit'),
('PSTN_BE/NormalizationRule/DeleteRule','Normalization Rule:View &Normalization Rule:Delete'),
('Fzero_FraudAnalysis/AccountCase/GetLastAccountCase','Case Management:View'),
('Fzero_FraudAnalysis/AccountCase/GetFilteredAccountSuspicionSummaries','Case Management:View'),
('Fzero_FraudAnalysis/AccountCase/UpdateAccountCase','Case Management:View & Case Management:Edit'),
('Fzero_FraudAnalysis/AccountCase/GetAccountCase','Case Management:View'),
('Fzero_FraudAnalysis/AccountCase/GetFilteredCasesByAccountNumber','Case Management:View'),
('Fzero_FraudAnalysis/AccountCaseHistory/GetFilteredAccountCaseHistoryByCaseID','Case Management:View'),
('api/CDR/GetCDRs','CDR:View'),
('Fzero_FraudAnalysis/NumberPrefix/GetPrefixes','Number Prefixes:View'),
('Fzero_FraudAnalysis/NumberPrefix/UpdatePrefixes','Number Prefixes:View & Number Prefixes:Edit'),
('Fzero_FraudAnalysis/NumberProfile/GetNumberProfiles','Strategy Module:View'),
('Fzero_FraudAnalysis/RelatedNumber/GetRelatedNumbersByAccountNumber','Related Numbers :View'),
('Fzero_FraudAnalysis/Strategy/GetFilteredStrategies','Strategy:View'),
('Fzero_FraudAnalysis/Strategy/GetStrategiesInfo','Strategy:View'),
('Fzero_FraudAnalysis/Strategy/GetStrategy','Strategy:View'),
('Fzero_FraudAnalysis/Strategy/GetFilters','Strategy:View'),
('Fzero_FraudAnalysis/Strategy/GetAggregates','Strategy:View'),
('Fzero_FraudAnalysis/Strategy/AddStrategy','Strategy:View & Strategy:Add'),
('Fzero_FraudAnalysis/Strategy/UpdateStrategy','Strategy:View & Strategy:Edit'),
('Fzero_FraudAnalysis/StrategyExecution/GetFilteredStrategyExecutions','Strategy Execution:View'),
('Fzero_FraudAnalysis/StrategyExecutionItem/GetFilteredDetailsByCaseID','Strategy Execution:View')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[Name] = s.[Name],[RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);