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
--[BI].[SchemaConfiguration]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [BI].[SchemaConfiguration] on;
;with cte_data([ID],[Name],[DisplayName],[Type],[Configuration],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'BTS Distinct Count','BTS Distinct Count',1,'{"ColumnName":"[Measures].[BTS Distinct Count]","Exepression":"","Unit":""}',null),
(2,'Call Type Distinct Count','Call Type Distinct Count',1,'{"ColumnName":"[Measures].[Call Type Distinct Count]","Exepression":"","Unit":""}',null),
(4,'Fact Calls Count','Fact Calls Count',1,'{"ColumnName":"[Measures].[Fact Calls Count]","Exepression":"","Unit":""}',null),
(6,'Case Distinct Count','Case Distinct Count',1,'{"ColumnName":"[Measures].[Case Distinct Count]","Exepression":"","Unit":""}',null),
(8,'Fact Cases Rows','Fact Cases Rows',1,'{"ColumnName":"[Measures].[Fact Cases Rows]","Exepression":"","Unit":""}',null),
(9,'Volume per Minutes','Volume per Minutes',1,'{"ColumnName":"[Measures].[Volume per Minutes]","Exepression":"","Unit":""}',null),
(10,'IMEI Distinct Count','IMEI Distinct Count',1,'{"ColumnName":"[Measures].[IMEI Distinct Count]","Exepression":"","Unit":""}',null),
(11,'Strategy Distinct Count','Strategy Distinct Count',1,'{"ColumnName":"[Measures].[Strategy Distinct Count]","Exepression":"","Unit":""}',null),
(13,'Subscriber Type Distinct Count','Subscriber Type Distinct Count',1,'{"ColumnName":"[Measures].[Subscriber Type Distinct Count]","Exepression":"","Unit":""}',null),
(14,'Daily_Avg_BTS_MSISDN','Daily Avg  BTS MSISDN',1,'{"ColumnName":"[Measures].[Daily_Avg_BTS_MSISDN]","Exepression":"","Unit":""}',null),
(15,'Daily_Avg_Called_Parties_MSISDN','Daily Avg Called Parties MSISDN',1,'{"ColumnName":"[Measures].[Daily_Avg_Called_Parties_MSISDN]","Exepression":"","Unit":""}',null),
(16,'Daily_Avg_Duration_In_MSISDN','Daily Avg Duration In MSISDN',1,'{"ColumnName":"[Measures].[Daily_Avg_Duration_In_MSISDN]","Exepression":"","Unit":""}',null),
(17,'Daily_Avg_Duration_Out_MSISDN','Daily Avg Duration Out MSISDN',1,'{"ColumnName":"[Measures].[Daily_Avg_Duration_Out_MSISDN]","Exepression":"","Unit":""}',null),
(18,'Daily_Avg_In_Calls_MSISDN','Daily Avg In Calls MSISDN',1,'{"ColumnName":"[Measures].[Daily_Avg_In_Calls_MSISDN]","Exepression":"","Unit":""}',null),
(27,'Daily_Avg_In_SMS_MSISDN','Daily Avg In SMS MSISDN',1,'{"ColumnName":"[Measures].[Daily_Avg_In_SMS_MSISDN]","Exepression":"","Unit":""}',null),
(28,'Daily_Avg_Off_Net_Originated_MSISDN','Daily Avg Off Net Originated MSISDN',1,'{"ColumnName":"[Measures].[Daily_Avg_Off_Net_Originated_MSISDN]","Exepression":"","Unit":""}',null),
(29,'Daily_Avg_On_Net_Originated_MSISDN','Daily Avg On Net Originated MSISDN',1,'{"ColumnName":"[Measures].[Daily_Avg_On_Net_Originated_MSISDN]","Exepression":"","Unit":""}',null),
(30,'Daily_Avg_Out_SMS_MSISDN','Daily Avg Out SMS MSISDN',1,'{"ColumnName":"[Measures].[Daily_Avg_Out_SMS_MSISDN]","Exepression":"","Unit":""}',null),
(31,'Daily_Perc_Off_Net_Originated','Daily Perc Off Net Originated',1,'{"ColumnName":"[Measures].[Daily_Perc_Off_Net_Originated]","Exepression":"","Unit":""}',null),
(32,'Daily_Perc_Off_Net_Terminated','Daily Perc Off Net Terminated',1,'{"ColumnName":"[Measures].[Daily_Perc_Off_Net_Terminated]","Exepression":"","Unit":""}',null),
(33,'Daily_Perc_On_Net_Originated','Daily_Perc_On_Net_Originated',1,'{"ColumnName":"[Measures].[Daily_Perc_On_Net_Originated]","Exepression":"","Unit":""}',null),
(34,'Daily_Perc_On_Net_Terminated','Daily_Perc_On_Net_Terminated',1,'{"ColumnName":"[Measures].[Daily_Perc_On_Net_Terminated]","Exepression":"","Unit":""}',null),
(35,'Daily_Total_In_Calls','Daily_Total_In_Calls',1,'{"ColumnName":"[Measures].[Daily_Total_In_Calls]","Exepression":"","Unit":""}',null),
(36,'Daily_Total_In_International','Daily_Total_In_International',1,'{"ColumnName":"[Measures].[Daily_Total_In_International]","Exepression":"","Unit":""}',null),
(37,'Daily_Total_In_Sms','Daily_Total_In_Sms',1,'{"ColumnName":"[Measures].[Daily_Total_In_Sms]","Exepression":"","Unit":""}',null),
(38,'Daily_Total_In_Volume','Daily_Total_In_Volume',1,'{"ColumnName":"[Measures].[Daily_Total_In_Volume]","Exepression":"","Unit":""}',null),
(39,'Daily_Total_Out_Callls','Daily_Total_Out_Callls',1,'{"ColumnName":"[Measures].[Daily_Total_Out_Callls]","Exepression":"","Unit":""}',null),
(40,'Daily_Total_Out_International','Daily_Total_Out_International',1,'{"ColumnName":"[Measures].[Daily_Total_Out_International]","Exepression":"","Unit":""}',null),
(41,'Daily_Total_Out_Sms','Daily_Total_Out_Sms',1,'{"ColumnName":"[Measures].[Daily_Total_Out_Sms]","Exepression":"","Unit":""}',null),
(42,'Daily_Total_Out_Volume','Daily_Total_Out_Volume',1,'{"ColumnName":"[Measures].[Daily_Total_Out_Volume]","Exepression":"","Unit":""}',null),
(43,'Hourly_Avg_BTS_MSISDN','Hourly_Avg_BTS_MSISDN',1,'{"ColumnName":"[Measures].[Hourly_Avg_BTS_MSISDN]","Exepression":"","Unit":""}',null),
(44,'Hourly_Avg_Called_Parties_MSISDN','Hourly_Avg_Called_Parties_MSISDN',1,'{"ColumnName":"[Measures].[Hourly_Avg_Called_Parties_MSISDN]","Exepression":"","Unit":""}',null),
(45,'Hourly_Avg_Duration_In_MSISDN','Hourly_Avg_Duration_In_MSISDN',1,'{"ColumnName":"[Measures].[Hourly_Avg_Duration_In_MSISDN]","Exepression":"","Unit":""}',null),
(46,'Hourly_Avg_Duration_Out_MSISDN','Hourly_Avg_Duration_Out_MSISDN',1,'{"ColumnName":"[Measures].[Hourly_Avg_Duration_Out_MSISDN]","Exepression":"","Unit":""}',null),
(47,'Hourly_Avg_In_Calls_MSISDN','Hourly_Avg_In_Calls_MSISDN',1,'{"ColumnName":"[Measures].[Hourly_Avg_In_Calls_MSISDN]","Exepression":"","Unit":""}',null),
(48,'Hourly_Avg_In_SMS_MSISDN','Hourly_Avg_In_SMS_MSISDN',1,'{"ColumnName":"[Measures].[Hourly_Avg_In_SMS_MSISDN]","Exepression":"","Unit":""}',null),
(49,'Hourly_Avg_Off_Net_Originated_MSISDN','Hourly_Avg_Off_Net_Originated_MSISDN',1,'{"ColumnName":"[Measures].[Hourly_Avg_Off_Net_Originated_MSISDN]","Exepression":"","Unit":""}',null),
(50,'Hourly_Avg_On_Net_Originated_MSISDN','Hourly_Avg_On_Net_Originated_MSISDN',1,'{"ColumnName":"[Measures].[Hourly_Avg_On_Net_Originated_MSISDN]","Exepression":"","Unit":""}',null),
(51,'Hourly_Avg_Out_Calls_MSISDN','Hourly_Avg_Out_Calls_MSISDN',1,'{"ColumnName":"[Measures].[Hourly_Avg_Out_Calls_MSISDN]","Exepression":"","Unit":""}',null),
(52,'Hourly_Avg_Out_SMS_MSISDN','Hourly_Avg_Out_SMS_MSISDN',1,'{"ColumnName":"[Measures].[Hourly_Avg_Out_SMS_MSISDN]","Exepression":"","Unit":""}',null),
(53,'Hourly_Perc_Off_Net_Originated','Hourly_Perc_Off_Net_Originated',1,'{"ColumnName":"[Measures].[Hourly_Perc_Off_Net_Originated]","Exepression":"","Unit":""}',null),
(54,'Hourly_Perc_Off_Net_Terminated','Hourly_Perc_Off_Net_Terminated',1,'{"ColumnName":"[Measures].[Hourly_Perc_Off_Net_Terminated]","Exepression":"","Unit":""}',null),
(55,'Hourly_Perc_On_Net_Originated','Hourly_Perc_On_Net_Originated',1,'{"ColumnName":"[Measures].[Hourly_Perc_On_Net_Originated]","Exepression":"","Unit":""}',null),
(56,'Hourly_Perc_On_Net_Terminated','Hourly_Perc_On_Net_Terminated',1,'{"ColumnName":"[Measures].[Hourly_Perc_On_Net_Terminated]","Exepression":"","Unit":""}',null),
(57,'Hourly_Total_In_Calls','Hourly_Total_In_Calls',1,'{"ColumnName":"[Measures].[Hourly_Total_In_Calls]","Exepression":"","Unit":""}',null),
(58,'Hourly_Total_In_International','Hourly_Total_In_International',1,'{"ColumnName":"[Measures].[Hourly_Total_In_International]","Exepression":"","Unit":""}',null),
(59,'Hourly_Total_In_Sms','Hourly_Total_In_Sms',1,'{"ColumnName":"[Measures].[Hourly_Total_In_Sms]","Exepression":"","Unit":""}',null),
(60,'Hourly_Total_In_Volume','Hourly_Total_In_Volume',1,'{"ColumnName":"[Measures].[Hourly_Total_In_Volume]","Exepression":"","Unit":""}',null),
(61,'Hourly_Total_Out_Callls','Hourly_Total_Out_Callls',1,'{"ColumnName":"[Measures].[Hourly_Total_Out_Callls]","Exepression":"","Unit":""}',null),
(62,'Hourly_Total_Out_International','Hourly_Total_Out_International',1,'{"ColumnName":"[Measures].[Hourly_Total_Out_International]","Exepression":"","Unit":""}',null),
(63,'Hourly_Total_Out_Sms','Hourly_Total_Out_Sms',1,'{"ColumnName":"[Measures].[Hourly_Total_Out_Sms]","Exepression":"","Unit":""}',null),
(64,'Hourly_Total_Out_Volume','Hourly_Total_Out_Volume',1,'{"ColumnName":"[Measures].[Hourly_Total_Out_Volume]","Exepression":"","Unit":""}',null),
(65,'Account Status','Account Status',0,'{"ColumnID":"[Dim Account Status].[Pk Account Status Id]","ColumnName":"[Dim Account Status].[Name]"}',null),
(66,'BTS','BTS',0,'{"ColumnID":"[Dim BTS].[Pk BTS Id]","ColumnName":"[Dim BTS].[Pk BTS Id]"}',null),
(67,'Call Class','Call Class',0,'{"ColumnID":"[Dim Call Class].[Pk Call Class Id]","ColumnName":"[Dim Call Class].[Name]"}',null),
(68,'Call Type','Call Type',0,'{"ColumnID":"[Dim Call Type].[Pk Call Type Id]","ColumnName":"[Dim Call Type].[Name]"}',null),
(69,'Case Status','Case Status',0,'{"ColumnID":"[Dim Case Status].[Pk Case Status Id]","ColumnName":"[Dim Case Status].[Name]"}',null),
(70,'IMEI','IMEI',0,'{"ColumnID":"[Dim IMEI].[IMEI]","ColumnName":"[Dim IMEI].[IMEI]"}',null),
(71,'MSISDN','MSISDN',0,'{"ColumnID":"[Dim MSISDN].[MSISDN]","ColumnName":"[Dim MSISDN].[MSISDN]"}',null),
(72,'Network Type','Network Type',0,'{"ColumnID":"[Dim Network Type].[Pk Net Type Id]","ColumnName":"[Dim Network Type].[Name]"}',null),
(73,'Period','Period',0,'{"ColumnID":"[Dim Period].[Pk Period Id]","ColumnName":"[Dim Period].[Name]"}',null),
(74,'Strategy','Strategy',0,'{"ColumnID":"[Dim Strategy].[Pk Strategy Id]","ColumnName":"[Dim Strategy].[Name]"}',null),
(75,'Strategy Kind','Strategy Kind',0,'{"ColumnID":"[Dim Strategy Kind].[PK Kind Id]","ColumnName":"[Dim Strategy Kind].[Name]"}',null),
(76,'Subscriber Type','Subscriber Type',0,'{"ColumnID":"[Dim Subscriber Type].[Pk Subscriber Type Id]","ColumnName":"[Dim Subscriber Type].[Name]"}',null),
(77,'Suspicion Level','Suspicion Level',0,'{"ColumnID":"[Dim Suspicion Level].[Pk Suspicion Level Id]","ColumnName":"[Dim Suspicion Level].[Name]"}',null),
(78,'Users','Users',0,'{"ColumnID":"[Dim Users].[Pk User Id]","ColumnName":"[Dim Users].[Name]"}',null),
(80,'Day','Day',0,'{"ColumnID":"[Dim Time].[Date Instance]","ColumnName":"[Dim Time].[Day]"}',null),
(81,'Day Name','Day Name',0,'{"ColumnID":"[Dim Time].[Date Instance]","ColumnName":"[Dim Time].[Day Name]"}',null),
(83,'Hour','Hour',0,'{"ColumnID":"[Dim Time].[Date Instance]","ColumnName":"[Dim Time].[Hour]"}',null),
(84,'Month','Month',0,'{"ColumnID":"[Dim Time].[Date Instance]","ColumnName":"[Dim Time].[Month]"}',null),
(85,'Month Name','Month Name',0,'{"ColumnID":"[Dim Time].[Date Instance]","ColumnName":"[Dim Time].[Month Name]"}',null),
(86,'Week','Week',0,'{"ColumnID":"[Dim Time].[Date Instance]","ColumnName":"[Dim Time].[Week]"}',null),
(87,'Year','Year',0,'{"ColumnID":"[Dim Time].[Date Instance]","ColumnName":"[Dim Time].[Year]"}',null),
(88,'Default Time','Default Time',2,'{"Date":"[Dim Time].[Date]","Year":"[Dim Time].[Year]","MonthOfYear":"[Dim Time].[Month Of Year]","WeekOfMonth":"[Dim Time].[Week Of Month]","DayOfMonth":"[Dim Time].[Day Of Month]","Hour":"[Dim Time].[Hour]","IsDefault":"True"}',null)
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
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(901,'Fraud Analysis',null,null,'/images/menu-icons/other.png',11,0),
--(902,'Reports',null,null,'/images/menu-icons/busines intel.png',14,0),
(903,'Network Infrastructure',null,1,null,30,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[Id],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
set identity_insert [sec].[Module] off;

--[sec].[View]-----------------------------9001 to 10000--------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(9001,'Normalization Rule','Normalization Rule','#/view/PSTN_BusinessEntity/Views/Normalization/NormalizationRuleManagement',1,'PSTN_BE/NormalizationRule/GetFilteredNormalizationRules',null,null,null,0,31),
(9002,'Strategies','Strategies','#/view/FraudAnalysis/Views/Strategy/StrategyManagement',901,'Fzero_FraudAnalysis/Strategy/GetFilteredStrategies',null,null,null,0,1),
(9003,'Strategy Execution Log','Strategy Execution Log','#/view/FraudAnalysis/Views/StrategyExecution/StrategyExecutionManagement',901,'Fzero_FraudAnalysis/StrategyExecution/GetFilteredStrategyExecutions',null,null,null,0,2),
(9004,'Suspicious Numbers','Suspicious Numbers','#/view/FraudAnalysis/Views/SuspiciousAnalysis/SuspicionAnalysis',901,'Fzero_FraudAnalysis/AccountCase/GetFilteredAccountSuspicionSummaries',null,null,null,0,3),
(9005,'White Numbers','White Numbers','#/view/FraudAnalysis/Views/AccountStatus/AccountStatusManagement',901,'Fzero_FraudAnalysis/AccountStatus/GetAccountStatusesData',null,null,null,0,4),
--(9006,'Cases Productivity','Cases Productivity','#/view/FraudAnalysis/Views/Reports/CasesProductivity',902,null,null,null,null,0,1),
--(9007,'Detected Lines Summary','Detected Lines Summary','#/view/FraudAnalysis/Views/Reports/BlockedLines',902,null,null,null,null,0,2),
--(9008,'Detected Lines Details','Detected Lines Details','#/view/FraudAnalysis/Views/Reports/LinesDetected',902,null,null,null,null,0,3),
(9008,'Switches','Switches','#/view/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchManagement',903,'PSTN_BE/Switch/GetFilteredSwitches',null,null,null,0,1),
(9009,'Trunks','Trunks','#/view/PSTN_BusinessEntity/Views/NetworkInfrastructure/TrunkManagement',903,'PSTN_BE/Trunk/GetFilteredTrunks',null,null,null,0,2),
(9010,'Switch Brands','Switch Brands','#/view/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchBrandManagement',903,'PSTN_BE/SwitchBrand/GetFilteredBrands',null,null,null,0,3)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[Id],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
set identity_insert [sec].[View] off;

--[sec].[BusinessEntityModule]------------------------901 to 1000----------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(901,'Fraud Analysis Module',1,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([Id],[Name],[ParentId],[BreakInheritance])
	values(s.[Id],s.[Name],s.[ParentId],s.[BreakInheritance]);
set identity_insert [sec].[BusinessEntityModule] off;

--[sec].[BusinessEntity]------------------2401 to 2700----------------------------------------------------------
----------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(2401,'Network Infrastructure','Network Infrastructure',2,0,'["View","Add","Edit", "Delete"]'),
(2402,'Normalization Rule','Normalization Rule',2,0,'["View","Add","Edit", "Delete"]'),
(2403,'Strategy','Strategy',901,0,'["View","Add","Edit"]'),
(2404,'Case Management','Case Management',901,0,'["View","Edit"]'),
(2405,'Strategy Execution Log','Strategy Execution Log',901,0,'["View"]'),
(2406,'Fzero_FraudAnalysis_WhiteListManagement','White List Management',901,0,'["View", "Add", "Edit","Delete", "Download Template", "Upload"]'),
(2450,'BusinessProcess_BP_Execute_Strategy','Execute Strategy',602,0,'["View", "StartInstance", "ScheduleTask"]'),
(2451,'BusinessProcess_BP_Number_Profiling','Number Profiling',602,0,'["View", "StartInstance", "ScheduleTask"]'),
(2452,'BusinessProcess_BP_Staging_To_CDR','Staging to CDR',602,0,'["View", "StartInstance", "ScheduleTask"]'),
(2453,'BusinessProcess_BP_Case_Management','Case Management',602,0,'["View", "StartInstance", "ScheduleTask"]'),
(2454,'BusinessProcess_BP_Related_Numbers','Related Numbers',602,0,'["View", "StartInstance", "ScheduleTask"]'),
(2455,'BusinessProcess_BP_Data_Warehouse','Fill Data Warehouse',602,0,'["View", "StartInstance", "ScheduleTask"]')
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
	values(s.[ID],s.[Type]);
set identity_insert [rules].[RuleType] off;

--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('Fzero_FraudAnalysis/CDR/GetCDRs',null),
('Fzero_FraudAnalysis/AccountCase/GetAccountCase','Case Management:View'),
('Fzero_FraudAnalysis/AccountCase/GetFilteredAccountSuspicionSummaries','Case Management:View'),
('Fzero_FraudAnalysis/AccountCase/GetFilteredCasesByAccountNumber','Case Management:View'),
('Fzero_FraudAnalysis/AccountCase/GetLastAccountCase',null),
('Fzero_FraudAnalysis/AccountCase/UpdateAccountCase','Case Management:Edit'),
('Fzero_FraudAnalysis/AccountCaseHistory/GetFilteredAccountCaseHistoryByCaseID','Case Management:View'),
('Fzero_FraudAnalysis/NumberPrefix/GetPrefixes',null),
('Fzero_FraudAnalysis/NumberPrefix/UpdatePrefixes',null),
('Fzero_FraudAnalysis/NumberProfile/GetNumberProfiles',null),
('Fzero_FraudAnalysis/RelatedNumber/GetRelatedNumbersByAccountNumber',null),
('Fzero_FraudAnalysis/Strategy/AddStrategy','Strategy:Add'),
('Fzero_FraudAnalysis/Strategy/GetAggregates',null),
('Fzero_FraudAnalysis/Strategy/GetFilteredStrategies','Strategy:View'),
('Fzero_FraudAnalysis/Strategy/GetFilters',null),
('Fzero_FraudAnalysis/Strategy/GetStrategiesInfo',null),
('Fzero_FraudAnalysis/Strategy/GetStrategy',null),
('Fzero_FraudAnalysis/Strategy/UpdateStrategy','Strategy:Edit'),
('Fzero_FraudAnalysis/StrategyExecution/GetFilteredStrategyExecutions','Strategy Execution Log:View'),
('Fzero_FraudAnalysis/StrategyExecutionItem/GetFilteredDetailsByCaseID',null),
('Fzero_FraudAnalysis/AccountStatus/GetAccountStatusesData','Fzero_FraudAnalysis_WhiteListManagement: View'),
('Fzero_FraudAnalysis/AccountStatus/GetAccountStatus',null),
('Fzero_FraudAnalysis/AccountStatus/UpdateAccountStatus','Fzero_FraudAnalysis_WhiteListManagement: Edit'),
('Fzero_FraudAnalysis/AccountStatus/AddAccountStatus','Fzero_FraudAnalysis_WhiteListManagement: Add'),
('Fzero_FraudAnalysis/AccountStatus/DownloadAccountStatusesTemplate','Fzero_FraudAnalysis_WhiteListManagement: Download Template'),
('Fzero_FraudAnalysis/AccountStatus/UploadAccountStatuses','Fzero_FraudAnalysis_WhiteListManagement: Upload'),
('Fzero_FraudAnalysis/AccountStatus/DeleteAccountStatus','Fzero_FraudAnalysis_WhiteListManagement: Delete'),
('PSTN_BE/NormalizationRule/AddRule','Normalization Rule:Add'),
('PSTN_BE/NormalizationRule/DeleteRule','Normalization Rule:Delete'),
('PSTN_BE/NormalizationRule/GetFilteredNormalizationRules','Normalization Rule:View'),
('PSTN_BE/NormalizationRule/GetNormalizationRuleAdjustNumberActionSettingsTemplates',null),
('PSTN_BE/NormalizationRule/GetNormalizationRuleSetAreaSettingsTemplates',null),
('PSTN_BE/NormalizationRule/GetRule',null),
('PSTN_BE/NormalizationRule/UpdateRule','Normalization Rule:Edit'),
('PSTN_BE/Switch/AddSwitch','Network Infrastructure:Add'),
('PSTN_BE/Switch/DeleteSwitch','Network Infrastructure:Delete'),
('PSTN_BE/Switch/GetFilteredSwitches','Network Infrastructure:View'),
('PSTN_BE/Switch/GetSwitchAssignedDataSources',null),
('PSTN_BE/Switch/GetSwitchById',null),
('PSTN_BE/Switch/GetSwitches',null),
('PSTN_BE/Switch/GetSwitchesInfo',null),
('PSTN_BE/Switch/UpdateSwitch','Network Infrastructure:Edit'),
('PSTN_BE/SwitchBrand/AddBrand','Network Infrastructure:Add'),
('PSTN_BE/SwitchBrand/DeleteBrand','Network Infrastructure:Delete'),
('PSTN_BE/SwitchBrand/GetBrandById',null),
('PSTN_BE/SwitchBrand/GetBrands',null),
('PSTN_BE/SwitchBrand/GetFilteredBrands','Network Infrastructure:View'),
('PSTN_BE/SwitchBrand/UpdateBrand','Network Infrastructure:Edit'),
('PSTN_BE/Trunk/AddTrunk','Network Infrastructure:Add'),
('PSTN_BE/Trunk/DeleteTrunk','Network Infrastructure:Delete'),
('PSTN_BE/Trunk/GetFilteredTrunks','Network Infrastructure:View'),
('PSTN_BE/Trunk/GetTrunkById',null),
('PSTN_BE/Trunk/GetTrunks',null),
('PSTN_BE/Trunk/GetTrunksBySwitchIds',null),
('PSTN_BE/Trunk/GetTrunksInfo',null),
('PSTN_BE/Trunk/UpdateTrunk','Network Infrastructure:Edit')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);

--[bp].[BPTaskType]----------------------10001 to 20000---------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [bp].[BPTaskType] on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(10001,'Vanrise.Fzero.FraudAnalysis.MainExtensions.BPTaskDataConfirmCancelStrategyonCaseCount','{"$type":"Vanrise.BusinessProcess.Entities.BPTaskTypeSettings, Vanrise.BusinessProcess.Entities","Editor":"/Client/Modules/FraudAnalysis/Views/BPTask/BPTaskConfirmCancelStrategyonCaseCount.html","AutoOpenTask":true}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[bp].[BPTaskType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);
set identity_insert [bp].[BPTaskType] off;

--[bp].[BPDefinition]---------------------1001 to 2000----------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [bp].[BPDefinition] on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config],[CreatedTime])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1001,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput','Execute Strategy','Vanrise.Fzero.FraudAnalysis.BP.ExecuteStrategyProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-cdr-fraudanalysis-executestrategy-manual","ScheduledExecEditor":"vr-cdr-fraudanalysis-executestrategy","IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2450,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2450,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2450,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}','2016-03-23 17:19:46.013'),
(1002,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyForNumberRangeProcessInput','Execute Strategy Number Range','Vanrise.Fzero.FraudAnalysis.BP.ExecuteStrategyForNumberRangeProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":true,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2450,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2450,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2450,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}','2016-03-23 17:19:46.013'),
(1003,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput','Number Profiling Process','Vanrise.Fzero.FraudAnalysis.BP.NumberProfilingProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-cdr-fraudanalysis-numberprofiling-manual","ScheduledExecEditor":"vr-cdr-fraudanalysis-numberprofiling","IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2451,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2451,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2451,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}','2016-03-23 17:19:46.013'),
(1004,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingForNumberRangeProcessInput','Number Profiling Number Range','Vanrise.Fzero.FraudAnalysis.BP.NumberProfilingForNumberRangeProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":true,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2451,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2451,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2451,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}','2016-03-23 17:19:46.013'),
(1005,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyCasesProcessInput','Assign Strategy Cases','Vanrise.Fzero.FraudAnalysis.BP.AssignStrategyCasesProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"vr-cdr-fraudanalysis-assignstrategy-manual","ScheduledExecEditor":"vr-cdr-fraudanalysis-assignstrategy","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2453,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2453,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2453,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}','2016-03-23 17:19:46.013'),
(1006,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.FindRelatedNumbersProcessInput','Find Related Numbers','Vanrise.Fzero.FraudAnalysis.BP.FindRelatedNumbersProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-cdr-fraudanalysis-findrelatednumbers-manual","ScheduledExecEditor":"vr-cdr-fraudanalysis-findrelatednumbers","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2454,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2454,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2454,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}','2016-03-23 17:19:46.013'),
(1007,'Vanrise.Fzero.CDRImport.BP.Arguments.StagingtoCDRProcessInput','Staging to CDR','Vanrise.Fzero.CDRImport.BP.StagingtoCDRProcess, Vanrise.Fzero.CDRImport.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-cdr-pstnbe-stagingtocdr-manual","ScheduledExecEditor":"vr-cdr-pstnbe-stagingtocdr","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2452,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2452,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2452,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}','2016-03-23 17:19:46.013'),
(1008,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.FillDataWarehouseProcessInput','Fill Data Warehouse','Vanrise.Fzero.FraudAnalysis.BP.FillDataWarehouseProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-cdr-fraudanalysis-filldatawarehouse-manual","ScheduledExecEditor":"vr-cdr-fraudanalysis-filldatawarehouse","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2455,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2455,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2455,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}','2016-03-23 17:19:46.013'),
(1009,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.CancelStrategyExecutionProcessInput','Cancel Strategy Execution','Vanrise.Fzero.FraudAnalysis.BP.CancelStrategyExecutionProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"","ScheduledExecEditor":"","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":true,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2450,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2450,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2450,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}','2016-03-23 17:19:46.013'),
(1010,'Vanrise.Fzero.FraudAnalysis.BP.Arguments.FindRelatedNumbersForNumberRangeProcessInput','Find Related Numbers Number Range','Vanrise.Fzero.FraudAnalysis.BP.FindRelatedNumbersForNumberRangeProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":true,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2454,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2454,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":2454,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}','2016-03-23 17:19:46.013')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[FQTN],[Config],[CreatedTime]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([ID],[Name],[Title],[FQTN],[Config])
	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
set identity_insert [bp].[BPDefinition] off;

--[common].[Setting]---------------------------701 to 800-------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[Setting] on;
;with cte_data([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(701,'Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"CDR Analysis","VersionNumber":"version 0.9"}}',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[Id],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
set identity_insert [common].[Setting] off;