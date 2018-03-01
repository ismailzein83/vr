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
begin
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
end

--[common].[ExtensionConfiguration]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('367615E0-1A10-42CE-9E50-2A572463EE6F','SuspicionRule','Suspicion Rule','CDRAnalysis_FA_StrategyCriteriaConfigs','{"Editor":"cdranalysis-fa-strategy-criteria-suspicionrule"}')
--('7A21F763-6443-4BAF-825E-9421A33CE7DF','Decision Tree','Decision Tree','CDRAnalysis_FA_StrategyCriteriaConfigs','{"Editor":"cdranalysis-fa-strategy-criteria-decisiontree"}')
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

--[sec].[Module]------------------------------901 to 1000------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('8437594D-5472-4F4E-8EF2-D15C32834714','Network Infrastructure',null,null,'50624672-CD25-44FD-8580-0E3AC8E34C71',null,30,0),
('C49F3A08-1D96-4F56-B0C6-F81EB8AAC9CA','Fraud Analysis',null,null,null,'/images/menu-icons/other.png',11,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[DefaultViewId] = s.[DefaultViewId],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([ID],[Name],[Url],[DefaultViewId],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[ID],s.[Name],s.[Url],s.[DefaultViewId],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
--------------------------------------------------------------------------------------------------------------
end

GO--delete useless views from CDRAnalysis product
delete from [sec].[View] where [Id] in ('C65ED28A-36D0-4047-BEC5-030D35B02308',--'My Scheduler Service'
										'66DE2441-8A96-41E7-94EA-9F8AF38A3515',--'Style Definitions'
										'DCF8CA21-852C-41B9-9101-6990E545509D',--'Organizational Charts'
										'52C580DE-C91F-45E2-8E3A-46E0BA9E7EFD',--'Component Types'
										'8AC4B99E-01A0-41D1-AE54-09E679309086',--'Status Definitions'
										'604B2CB5-B839-4E51-8D13-3C1C84D05DEE',--'Countries'
										'A1CE55FE-6CF4-4F15-9BC2-8E1F8DF68561',--'Regions'
										'25994374-CB99-475B-8047-3CDB7474A083',--'Cities'
										'9F691B87-4936-4C4C-A757-4B3E12F7E1D9', --'Currencies'
										'E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9', --'Exchange Rates'
										'0F111ADC-B7F6-46A4-81BC-72FFDEB305EB', --'Time Zone'
										'2CF7E0BE-1396-4305-AA27-11070ACFC18F',--'Application Visibilities'
										'4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712',--'Rate Types'
										'2D39B12D-8FBF-4D4E-B2A5-5E3FE57580DF'--,'Locked Sessions'
										)
GO
--[sec].[View]-----------------------------9001 to 10000--------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7079BD63-BFE2-4519-9B1B-8158A2F3A12A','System Logs','System Logs',null,'BAAF681E-AB1C-4A64-9A35-3F3951398881',null,null,null,'{"$type":"Vanrise.Common.Business.MasterLogViewSettings, Vanrise.Common.Business","Items":[{"PermissionName":"VRCommon_System_Log: View General Logs","Directive":"vr-log-entry-search","Title":"General"},{"PermissionName":"VR_Integration_DataProcesses: View Logs","Directive":"vr-integration-log-search","Title":"Data Source"},{"PermissionName":"VR_Integration_DataProcesses: View Logs","Directive":"vr-integration-importedbatch-search","Title":"Imported Batch"},{"PermissionName":"VRCommon_System_Log: View General Logs","Directive":"bp-instance-log-search","Title":"Business Process"},{"PermissionName":"VRCommon_System_Log: View Action Audit","Directive":"vr-common-actionaudit-search","Title":"Action Audit"}]}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',15,null),

('F09A8CCF-F0EE-4EEF-BB19-24B1ABF902AE','Normalization Rule','Normalization Rule','#/view/PSTN_BusinessEntity/Views/Normalization/NormalizationRuleManagement'		,'1C7569FA-43C9-4853-AE4C-1152746A34FD','PSTN_BE/NormalizationRule/GetFilteredNormalizationRules',null,null,null,'C56841B2-E7EF-47A9-AFF5-DED8BCB1E670',31,null),

('EB2D5EA0-4AE1-4219-8152-487F13EF2B88','Switch Brands','Switch Brands','#/view/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchBrandManagement'				,'8437594D-5472-4F4E-8EF2-D15C32834714','PSTN_BE/SwitchBrand/GetFilteredBrands',null,null,null,'C56841B2-E7EF-47A9-AFF5-DED8BCB1E670',3,null),
('DB3B1776-CBCC-4490-92BC-529CD9F3B8BB','Switches','Switches','#/view/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchManagement'								,'8437594D-5472-4F4E-8EF2-D15C32834714','PSTN_BE/Switch/GetFilteredSwitches',null,null,null,'C56841B2-E7EF-47A9-AFF5-DED8BCB1E670',1,null),
('BFDB6651-DF90-4C12-85F0-5C3A78299AC8','Trunks','Trunks','#/view/PSTN_BusinessEntity/Views/NetworkInfrastructure/TrunkManagement'									,'8437594D-5472-4F4E-8EF2-D15C32834714','PSTN_BE/Trunk/GetFilteredTrunks',null,null,null,'C56841B2-E7EF-47A9-AFF5-DED8BCB1E670',2,null),

('949681F0-3701-4D43-85E5-1BB61BCE7F28','Strategy Execution Log','Strategy Execution Log','#/view/FraudAnalysis/Views/StrategyExecution/StrategyExecutionManagement','C49F3A08-1D96-4F56-B0C6-F81EB8AAC9CA','Fzero_FraudAnalysis/StrategyExecution/GetFilteredStrategyExecutions',null,null,null,'C56841B2-E7EF-47A9-AFF5-DED8BCB1E670',2,null),
('AA1DC522-730C-4F30-A41E-A847389F15EF','Suspicious Numbers','Suspicious Numbers','#/view/FraudAnalysis/Views/SuspiciousAnalysis/SuspicionAnalysis'					,'C49F3A08-1D96-4F56-B0C6-F81EB8AAC9CA','Fzero_FraudAnalysis/AccountCase/GetFilteredAccountSuspicionSummaries',null,null,null,'C56841B2-E7EF-47A9-AFF5-DED8BCB1E670',3,null),
('CF2BEAB7-A69F-4834-9818-CEE0E9B39E68','White Numbers','White Numbers','#/view/FraudAnalysis/Views/AccountStatus/AccountStatusManagement'							,'C49F3A08-1D96-4F56-B0C6-F81EB8AAC9CA','Fzero_FraudAnalysis/AccountStatus/GetAccountStatusesData',null,null,null,'C56841B2-E7EF-47A9-AFF5-DED8BCB1E670',4,null),
('9D9043EB-18E8-4C52-A69B-EE10954DCFA5','Strategies','Strategies','#/view/FraudAnalysis/Views/Strategy/StrategyManagement'											,'C49F3A08-1D96-4F56-B0C6-F81EB8AAC9CA','Fzero_FraudAnalysis/Strategy/GetFilteredStrategies',null,null,null,'C56841B2-E7EF-47A9-AFF5-DED8BCB1E670',1,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank],[IsDeleted] = s.[IsDeleted]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank],[IsDeleted])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank],s.[IsDeleted]);
---------------------------------------------------------------------------------------------------------------
end

DELETE FROM [sec].[BusinessEntityModule] WHERE [ID] IN ('16419FE1-ED56-49BA-B609-284A5E21FC07',--'Traffic'
														'520558FA-CF2F-440B-9B58-09C23B6A2E9B',--'Billing'
														'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',--'Business Entities'
														'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493')--'Lookups'

--[sec].[BusinessEntityModule]------------------------901 to 1000----------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('57F7F044-5D5E-4092-A462-485D58803798','Fraud Analysis','5A9E78AE-229E-41B9-9DBF-492997B42B61',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentId],[BreakInheritance]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]
when not matched by target then
	insert([ID],[Name],[ParentId],[BreakInheritance])
	values(s.[ID],s.[Name],s.[ParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]------------------2401 to 2700----------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('C4721065-0DB6-4171-88D7-9CE8DFBCEB6C','Normalization Rule','Normalization Rule'							,'61451603-E7B9-40C6-AE27-6CBA974E1B3B',0,'["View","Add","Edit", "Delete"]'),

('A6C35583-BFAF-443B-A89D-2C949C8AF6C8','Case Management','Case Management'									,'57F7F044-5D5E-4092-A462-485D58803798',0,'["View","Edit"]'),
('43962C85-E191-4B68-8AFC-9F02C1EC7A1F','Fzero_FraudAnalysis_WhiteListManagement','White List Management'	,'57F7F044-5D5E-4092-A462-485D58803798',0,'["View", "Add", "Edit","Delete", "Download Template", "Upload"]'),
('30061043-6C77-4A55-BFCC-F16C5F1DED04','Strategy Execution Log','Strategy Execution Log'					,'57F7F044-5D5E-4092-A462-485D58803798',0,'["View"]'),
('1A2E1BEE-95D4-44E2-A11E-F8AB763F5FF0','Strategy','Strategy'												,'57F7F044-5D5E-4092-A462-485D58803798',0,'["View","Add","Edit"]'),

('31618738-DF0B-418E-8CFF-744E851B97F8','BusinessProcess_BP_Data_Warehouse','Fill Data Warehouse'			,'04493174-83F0-44D6-BBE4-DBEB8B57875A',0,'["View", "StartInstance", "ScheduleTask"]'),
('B023B50E-37B7-40DE-A545-9F9566C1940B','BusinessProcess_BP_Execute_Strategy','Execute Strategy'			,'04493174-83F0-44D6-BBE4-DBEB8B57875A',0,'["View", "StartInstance", "ScheduleTask"]'),
('FAA991AE-D4D0-45D2-839C-A1BA0B12216E','BusinessProcess_BP_Related_Numbers','Related Numbers'				,'04493174-83F0-44D6-BBE4-DBEB8B57875A',0,'["View", "StartInstance", "ScheduleTask"]'),
('7B23B6B9-9CD5-4B82-B8D9-B3AEA00D3B44','BusinessProcess_BP_Case_Management','Case Management'				,'04493174-83F0-44D6-BBE4-DBEB8B57875A',0,'["View", "StartInstance", "ScheduleTask"]'),
('0AF27074-FC21-452C-8C40-D8DA9789BB4A','BusinessProcess_BP_Number_Profiling','Number Profiling'			,'04493174-83F0-44D6-BBE4-DBEB8B57875A',0,'["View", "StartInstance", "ScheduleTask"]'),
('BF32BE48-A469-4284-B422-DAD113913574','BusinessProcess_BP_Staging_To_CDR','Staging to CDR'				,'04493174-83F0-44D6-BBE4-DBEB8B57875A',0,'["View", "StartInstance", "ScheduleTask"]'),

('DC0153DB-8897-4181-9E2F-F4E4C4B417A7','Network Infrastructure','Network Infrastructure'					,'61451603-E7B9-40C6-AE27-6CBA974E1B3B',0,'["View","Add","Edit", "Delete"]')
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
----------------------------------------------------------------------------------------------------------------
end

--common.ExtensionConfiguration---------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('B1281B56-E156-40A2-ABCF-3766A80C63A0','Set Area Prefix','Set Area Prefix','PSTN_BE_SetArea','{"Editor":"vr-pstn-be-setareaprefix"}'),
('A34217B8-79A0-4EAE-BB96-59BADB29DC03','Replace Characters','Replace Characters','VRCommon_StyleFormating','{"Editor":"vr-pstn-be-replacestring"}'),
('D235FD23-F660-496E-9F71-67226154727A','Add Prefix','Add Prefix','PSTN_BE_AdjustNumberAction','{"Editor":"vr-pstn-be-addprefix"}'),
('819F222E-5A68-4563-9655-F4298C34453E','Substring','Substring','VRCommon_StyleFormating','{"Editor":"vr-pstn-be-substring"}')
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

--rules.RuleType------------------------------------------------------------------------------------
begin
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
----------------------------------------------------------------------------------------------------

end

--[sec].[SystemAction]------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('Fzero_FraudAnalysis/CDR/GetCDRs',null),
('Fzero_FraudAnalysis/AccountCase/GetAccountCase','Case Management: View'),
('Fzero_FraudAnalysis/AccountCase/GetFilteredAccountSuspicionSummaries','Case Management: View'),
('Fzero_FraudAnalysis/AccountCase/GetFilteredCasesByAccountNumber','Case Management: View'),
('Fzero_FraudAnalysis/AccountCase/GetLastAccountCase',null),
('Fzero_FraudAnalysis/AccountCase/UpdateAccountCase','Case Management: Edit'),
('Fzero_FraudAnalysis/AccountCaseHistory/GetFilteredAccountCaseHistoryByCaseID','Case Management: View'),
('Fzero_FraudAnalysis/NumberPrefix/GetPrefixes',null),
('Fzero_FraudAnalysis/NumberPrefix/UpdatePrefixes',null),
('Fzero_FraudAnalysis/NumberProfile/GetNumberProfiles',null),
('Fzero_FraudAnalysis/RelatedNumber/GetRelatedNumbersByAccountNumber',null),
('Fzero_FraudAnalysis/Strategy/AddStrategy','Strategy:Add'),
('Fzero_FraudAnalysis/Strategy/GetAggregates',null),
('Fzero_FraudAnalysis/Strategy/GetFilteredStrategies','Strategy: View'),
('Fzero_FraudAnalysis/Strategy/GetFilters',null),
('Fzero_FraudAnalysis/Strategy/GetStrategiesInfo',null),
('Fzero_FraudAnalysis/Strategy/GetStrategy',null),
('Fzero_FraudAnalysis/Strategy/UpdateStrategy','Strategy: Edit'),
('Fzero_FraudAnalysis/StrategyExecution/GetFilteredStrategyExecutions','Strategy Execution Log: View'),
('Fzero_FraudAnalysis/StrategyExecutionItem/GetFilteredDetailsByCaseID',null),
('Fzero_FraudAnalysis/AccountStatus/GetAccountStatusesData','Fzero_FraudAnalysis_WhiteListManagement: View'),
('Fzero_FraudAnalysis/AccountStatus/GetAccountStatus',null),
('Fzero_FraudAnalysis/AccountStatus/UpdateAccountStatus','Fzero_FraudAnalysis_WhiteListManagement: Edit'),
('Fzero_FraudAnalysis/AccountStatus/AddAccountStatus','Fzero_FraudAnalysis_WhiteListManagement: Add'),
('Fzero_FraudAnalysis/AccountStatus/DownloadAccountStatusesTemplate','Fzero_FraudAnalysis_WhiteListManagement: Download Template'),
('Fzero_FraudAnalysis/AccountStatus/UploadAccountStatuses','Fzero_FraudAnalysis_WhiteListManagement: Upload'),
('Fzero_FraudAnalysis/AccountStatus/DeleteAccountStatus','Fzero_FraudAnalysis_WhiteListManagement: Delete'),
('PSTN_BE/NormalizationRule/AddRule','Normalization Rule: Add'),
('PSTN_BE/NormalizationRule/DeleteRule','Normalization Rule: Delete'),
('PSTN_BE/NormalizationRule/GetFilteredNormalizationRules','Normalization Rule: View'),
('PSTN_BE/NormalizationRule/GetNormalizationRuleAdjustNumberActionSettingsTemplates',null),
('PSTN_BE/NormalizationRule/GetNormalizationRuleSetAreaSettingsTemplates',null),
('PSTN_BE/NormalizationRule/GetRule',null),
('PSTN_BE/NormalizationRule/UpdateRule','Normalization Rule: Edit'),
('PSTN_BE/Switch/AddSwitch','Network Infrastructure: Add'),
('PSTN_BE/Switch/DeleteSwitch','Network Infrastructure: Delete'),
('PSTN_BE/Switch/GetFilteredSwitches','Network Infrastructure: View'),
('PSTN_BE/Switch/GetSwitchAssignedDataSources',null),
('PSTN_BE/Switch/GetSwitchById',null),
('PSTN_BE/Switch/GetSwitches',null),
('PSTN_BE/Switch/GetSwitchesInfo',null),
('PSTN_BE/Switch/UpdateSwitch','Network Infrastructure: Edit'),
('PSTN_BE/SwitchBrand/AddBrand','Network Infrastructure: Add'),
('PSTN_BE/SwitchBrand/DeleteBrand','Network Infrastructure: Delete'),
('PSTN_BE/SwitchBrand/GetBrandById',null),
('PSTN_BE/SwitchBrand/GetBrands',null),
('PSTN_BE/SwitchBrand/GetFilteredBrands','Network Infrastructure: View'),
('PSTN_BE/SwitchBrand/UpdateBrand','Network Infrastructure: Edit'),
('PSTN_BE/Trunk/AddTrunk','Network Infrastructure: Add'),
('PSTN_BE/Trunk/DeleteTrunk','Network Infrastructure: Delete'),
('PSTN_BE/Trunk/GetFilteredTrunks','Network Infrastructure: View'),
('PSTN_BE/Trunk/GetTrunkById',null),
('PSTN_BE/Trunk/GetTrunks',null),
('PSTN_BE/Trunk/GetTrunksBySwitchIds',null),
('PSTN_BE/Trunk/GetTrunksInfo',null),
('PSTN_BE/Trunk/UpdateTrunk','Network Infrastructure: Edit')
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
----------------------------------------------------------------------------------------------------
end

--[bp].[BPTaskType]----------------------10001 to 20000---------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('AC11B03F-6CF2-4942-BC88-04ED3952FA1B','Vanrise.Fzero.FraudAnalysis.MainExtensions.BPTaskDataConfirmCancelStrategyonCaseCount','{"$type":"Vanrise.BusinessProcess.Entities.BPTaskTypeSettings, Vanrise.BusinessProcess.Entities","Editor":"/Client/Modules/FraudAnalysis/Views/BPTask/BPTaskConfirmCancelStrategyonCaseCount.html","AutoOpenTask":true}')
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
----------------------------------------------------------------------------------------------------
end

--[bp].[BPDefinition]---------------------1001 to 2000----------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('39628760-ED6E-415D-9B50-36F6249F1B72','Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput','Execute Strategy','Vanrise.Fzero.FraudAnalysis.BP.ExecuteStrategyProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-cdr-fraudanalysis-executestrategy-manual","ScheduledExecEditor":"vr-cdr-fraudanalysis-executestrategy","IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B023B50E-37B7-40DE-A545-9F9566C1940B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B023B50E-37B7-40DE-A545-9F9566C1940B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B023B50E-37B7-40DE-A545-9F9566C1940B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('762780A4-CFCA-42FE-B21B-66F49FCC273B','Vanrise.Fzero.FraudAnalysis.BP.Arguments.FillDataWarehouseProcessInput','Fill Data Warehouse','Vanrise.Fzero.FraudAnalysis.BP.FillDataWarehouseProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-cdr-fraudanalysis-filldatawarehouse-manual","ScheduledExecEditor":"vr-cdr-fraudanalysis-filldatawarehouse","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"31618738-DF0B-418E-8CFF-744E851B97F8","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"31618738-DF0B-418E-8CFF-744E851B97F8","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"31618738-DF0B-418E-8CFF-744E851B97F8","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('107038DE-3722-4A14-8174-8409CED5552F','Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyForNumberRangeProcessInput','Execute Strategy Number Range','Vanrise.Fzero.FraudAnalysis.BP.ExecuteStrategyForNumberRangeProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":true,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B023B50E-37B7-40DE-A545-9F9566C1940B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B023B50E-37B7-40DE-A545-9F9566C1940B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B023B50E-37B7-40DE-A545-9F9566C1940B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('F0C6D6E8-AEB2-42AB-988D-91F863AD6098','Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingForNumberRangeProcessInput','Number Profiling Number Range','Vanrise.Fzero.FraudAnalysis.BP.NumberProfilingForNumberRangeProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":true,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"0AF27074-FC21-452C-8C40-D8DA9789BB4A","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"0AF27074-FC21-452C-8C40-D8DA9789BB4A","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"0AF27074-FC21-452C-8C40-D8DA9789BB4A","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('6D717D20-3B38-4663-83B7-A2AF1083F456','Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput','Number Profiling Process','Vanrise.Fzero.FraudAnalysis.BP.NumberProfilingProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-cdr-fraudanalysis-numberprofiling-manual","ScheduledExecEditor":"vr-cdr-fraudanalysis-numberprofiling","IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"0AF27074-FC21-452C-8C40-D8DA9789BB4A","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"0AF27074-FC21-452C-8C40-D8DA9789BB4A","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"0AF27074-FC21-452C-8C40-D8DA9789BB4A","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('FB28C0A5-F6F6-43C8-B70A-B3C0B407B7CC','Vanrise.Fzero.FraudAnalysis.BP.Arguments.CancelStrategyExecutionProcessInput','Cancel Strategy Execution','Vanrise.Fzero.FraudAnalysis.BP.CancelStrategyExecutionProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"","ScheduledExecEditor":"","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":true,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B023B50E-37B7-40DE-A545-9F9566C1940B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B023B50E-37B7-40DE-A545-9F9566C1940B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B023B50E-37B7-40DE-A545-9F9566C1940B","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('10F5D95C-F3F3-4770-8A8C-E0034097B9DE','Vanrise.Fzero.FraudAnalysis.BP.Arguments.FindRelatedNumbersProcessInput','Find Related Numbers','Vanrise.Fzero.FraudAnalysis.BP.FindRelatedNumbersProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-cdr-fraudanalysis-findrelatednumbers-manual","ScheduledExecEditor":"vr-cdr-fraudanalysis-findrelatednumbers","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"FAA991AE-D4D0-45D2-839C-A1BA0B12216E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"FAA991AE-D4D0-45D2-839C-A1BA0B12216E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"FAA991AE-D4D0-45D2-839C-A1BA0B12216E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('17104223-9E07-406D-8621-E1ECDF5009C1','Vanrise.Fzero.CDRImport.BP.Arguments.StagingtoCDRProcessInput','Staging to CDR','Vanrise.Fzero.CDRImport.BP.StagingtoCDRProcess, Vanrise.Fzero.CDRImport.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"ManualExecEditor":"vr-cdr-pstnbe-stagingtocdr-manual","ScheduledExecEditor":"vr-cdr-pstnbe-stagingtocdr","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"BF32BE48-A469-4284-B422-DAD113913574","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"BF32BE48-A469-4284-B422-DAD113913574","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"BF32BE48-A469-4284-B422-DAD113913574","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('6C0A4661-74B0-4F33-8501-EBF8BDD5B324','Vanrise.Fzero.FraudAnalysis.BP.Arguments.FindRelatedNumbersForNumberRangeProcessInput','Find Related Numbers Number Range','Vanrise.Fzero.FraudAnalysis.BP.FindRelatedNumbersForNumberRangeProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":true,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"FAA991AE-D4D0-45D2-839C-A1BA0B12216E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"FAA991AE-D4D0-45D2-839C-A1BA0B12216E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"FAA991AE-D4D0-45D2-839C-A1BA0B12216E","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}'),
('E9C8595A-B9C7-411B-94A5-FC0F87DDC9CD','Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyCasesProcessInput','Assign Strategy Cases','Vanrise.Fzero.FraudAnalysis.BP.AssignStrategyCasesProcess, Vanrise.Fzero.FraudAnalysis.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"vr-cdr-fraudanalysis-assignstrategy-manual","ScheduledExecEditor":"vr-cdr-fraudanalysis-assignstrategy","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"7B23B6B9-9CD5-4B82-B8D9-B3AEA00D3B44","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"7B23B6B9-9CD5-4B82-B8D9-B3AEA00D3B44","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"7B23B6B9-9CD5-4B82-B8D9-B3AEA00D3B44","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([ID],[Name],[Title],[FQTN],[Config])
	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
----------------------------------------------------------------------------------------------------
end

--Delete from [common].[Setting] where [ID] in (	'1CB20F2C-A835-4320-AEC7-E034C5A756E9',--'Bank Details'
--												'1C833B2D-8C97-4CDD-A1C1-C1B4D9D299DE',--'System Currency'
--												'81F62AC3-BAE4-4A2F-A60D-A655494625EA' )--'Company Setting'
Delete from [common].[Setting] where ID = '4047054E-1CF4-4BE6-A005-6D4706757AD3'--,'Session Lock'
--[common].[Setting]---------------------------701 to 800-------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('509E467B-4562-4CA6-A32E-E50473B74D2C','Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"CDR Analysis","VersionNumber":"version #VersionNumber# ~ #VersionDate#"}}',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
----------------------------------------------------------------------------------------------------
end

--[queue].[ExecutionFlowDefinition]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[Title],[ExecutionTree],[Stages])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('4B1FE6DA-4C94-4D13-A283-168FC5E8C06F','NormalCDRImportDefinition','Normal CDR Import Definition',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities]], mscorlib","$values":[{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Map Normal CDRs","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.Fzero.CDRImport.QueueActivators.NormalCDRFlowStageItemType, Vanrise.Fzero.CDRImport.QueueActivators"},"QueueActivator":{"$type":"Vanrise.Queueing.Entities.CustomQueueActivator, Vanrise.Queueing.Entities","FQTN":"Vanrise.Fzero.CDRImport.QueueActivators.NormalizeCDRActivator, Vanrise.Fzero.CDRImport.QueueActivators","NbOfMaxConcurrentActivators":10,"ConfigId":3},"IsSequential":false},{"$type":"Vanrise.Queueing.Entities.QueueExecutionFlowStage, Vanrise.Queueing.Entities","StageName":"Save Normal CDRs","QueueNameTemplate":"Queue_#FlowId#_#StageName#","QueueTitleTemplate":"#StageName# Queue (#FlowName#)","QueueItemType":{"$type":"Vanrise.Fzero.CDRImport.QueueActivators.NormalCDRFlowStageItemType, Vanrise.Fzero.CDRImport.QueueActivators"},"QueueActivator":{"$type":"Vanrise.Queueing.Entities.CustomQueueActivator, Vanrise.Queueing.Entities","FQTN":"Vanrise.Fzero.CDRImport.QueueActivators.SaveCDRActivator, Vanrise.Fzero.CDRImport.QueueActivators","NbOfMaxConcurrentActivators":10,"ConfigId":3},"IsSequential":false}]}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ExecutionTree],[Stages]))
merge	[queue].[ExecutionFlowDefinition] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ExecutionTree] = s.[ExecutionTree],[Stages] = s.[Stages]
when not matched by target then
	insert([Id],[Name],[Title],[ExecutionTree],[Stages])
	values(s.[Id],s.[Name],s.[Title],s.[ExecutionTree],s.[Stages]);

--[queue].[ExecutionFlow]---------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[ExecutionFlowDefinitionID])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('245BF812-406F-46E2-9B6F-CAAEB3DB5A7E','CDR Import','4B1FE6DA-4C94-4D13-A283-168FC5E8C06F')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ExecutionFlowDefinitionID]))
merge	[queue].[ExecutionFlow] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
--when matched then
--	update set
--	[Name] = s.[Name],[ExecutionFlowDefinitionID] = s.[ExecutionFlowDefinitionID]
when not matched by target then
	insert([Id],[Name],[ExecutionFlowDefinitionID])
	values(s.[Id],s.[Name],s.[ExecutionFlowDefinitionID]);




--[integration].[DataSource]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('BE08FA12-7978-4774-B0DF-4CC543596A71','Import CDR - Sample Datasource','10323CCB-CBFD-4BBE-91F3-FC80E2D91630','{"$type":"Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterState, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments","LastImportedId":0}','FAB3BA63-02DB-4305-A58E-6309C92FAB39','{"$type":"Vanrise.Integration.Entities.DataSourceSettings, Vanrise.Integration.Entities","AdapterArgument":{"$type":"Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterArgument, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments","ConnectionString":"Server=192.168.110.185;Database=CDRAnalysisMobile_Research;User ID=Development;Password=dev!123","Query":"SELECT #TopRows# [Id]\n      ,[MSISDN]\n      ,[IMSI]\n      ,[ConnectDateTime]\n      ,[Destination]\n      ,[DurationInSeconds]\n      ,[DisconnectDateTime]\n      ,[Call_Class]\n      ,[IsOnNet]\n      ,[Call_Type]\n      ,[Sub_Type]\n      ,[IMEI]\n      ,[BTS_Id]\n      ,[Cell_Id]\n      ,[SwitchRecordId]\n      ,[Up_Volume]\n      ,[Down_Volume]\n      ,[Cell_Latitude]\n      ,[Cell_Longitude]\n      ,[In_Trunk]\n      ,[Out_Trunk]\n      ,[Service_Type]\n      ,[Service_VAS_Name]\n      ,[ReleaseCode]\n  FROM [dbo].[NormalCDRZein]\nWhere  \n(ID > ISNULL(@RangeStart, 0))\nAND (ID <= ISNULL(@RangeEnd, 9999999999999999))\nAND ID <= 29013376\nORDER BY ID","IdentifierColumnName":"ID","NumberOfParallelReader":3,"NumberOffSet":1000000,"MaxParallelRuntimeInstances":3,"CommandTimeoutInSeconds":0},"MapperCustomCode":"LogVerbose(\"Started\");\n            Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch();\n            batch.CDRs = new List<Vanrise.Fzero.CDRImport.Entities.CDR>();\n            Vanrise.Integration.Entities.DBReaderImportedData ImportedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));\n            PSTN.BusinessEntity.Business.TrunkManager trunkManager = new PSTN.BusinessEntity.Business.TrunkManager();\n            IDataReader reader = ImportedData.Reader;\n            Object index = ImportedData.LastImportedId;\n\n            while (reader.Read())\n            {\n                Vanrise.Fzero.CDRImport.Entities.CDR cdr = new Vanrise.Fzero.CDRImport.Entities.CDR();\n              \n                cdr.BTS = reader[\"BTS_Id\"].ToString();\n                cdr.IMSI = reader[\"IMSI\"] as string;\n                cdr.IMEI = reader[\"IMEI\"] as string;\n                cdr.Cell = reader[\"Call_Class\"] as string;\n                cdr.UpVolume = Utils.GetReaderValue<Decimal?>(reader, \"Up_Volume\");\n                cdr.DownVolume = Utils.GetReaderValue<Decimal?>(reader, \"Down_Volume\");\n                cdr.CellLatitude = Utils.GetReaderValue<Decimal?>(reader, \"Cell_Latitude\");\n                cdr.CellLongitude = Utils.GetReaderValue<Decimal?>(reader, \"Cell_Longitude\");\n                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, \"ConnectDateTime\");\n                cdr.DurationInSeconds = Utils.GetReaderValue<decimal>(reader, \"DurationInSeconds\");\n                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, \"DisconnectDateTime\");\n                cdr.ServiceTypeId = Utils.GetReaderValue<int?>(reader, \"Service_Type\");\n                cdr.ServiceVASName = reader[\"Service_VAS_Name\"] as string;\n                cdr.ReleaseCode = reader[\"ReleaseCode\"] as string;\n                cdr.Destination = reader[\"Destination\"] as string;\n                cdr.MSISDN = reader[\"MSISDN\"] as string;\n\n             \n                if (Utils.GetReaderValue<Byte?>(reader, \"IsOnNet\") == 1)\n                    cdr.IsOnNet = true;\n                else\n                    cdr.IsOnNet = false;\n\n                cdr.InTrunkId = trunkManager.GetTrunkIdBySymbol(reader[\"In_Trunk\"] as string);\n                cdr.OutTrunkId = trunkManager.GetTrunkIdBySymbol(reader[\"Out_Trunk\"] as string);\n\n                index = reader[\"ID\"];\n\n                batch.CDRs.Add(cdr);\n            }\n\n            ImportedData.LastImportedId = index;\n            batch.Datasource = dataSourceId;\n            mappedBatches.Add(\"Map Normal CDRs\", batch);\n\n            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();\n            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;\n            LogVerbose(\"Finished\");\n            return result;","ExecutionFlowId":"245bf812-406f-46e2-9b6f-caaeb3db5a7e"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings]))
merge	[integration].[DataSource] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[AdapterID] = s.[AdapterID],[AdapterState] = s.[AdapterState],[TaskId] = s.[TaskId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[AdapterID],[AdapterState],[TaskId],[Settings])
	values(s.[ID],s.[Name],s.[AdapterID],s.[AdapterState],s.[TaskId],s.[Settings]);


--[runtime].[ScheduleTask]--------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FAB3BA63-02DB-4305-A58E-6309C92FAB39','Data Source Task',0,0,'295B4FAC-DBF9-456F-855E-60D0B176F86B','B7CF41B9-F1B3-4C02-980D-B9FAFB4CFF68','{"$type":"Vanrise.Runtime.Entities.SchedulerTaskSettings, Vanrise.Runtime.Entities","TaskTriggerArgument":{"$type":"Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.IntervalTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments","Interval":2.0,"IntervalType":2,"TimerTriggerTypeFQTN":"Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger"},"StartEffDate":"2015-10-21T09:28:00+03:00"}',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId]))
merge	[runtime].[ScheduleTask] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[IsEnabled] = s.[IsEnabled],[TaskType] = s.[TaskType],[TriggerTypeId] = s.[TriggerTypeId],[ActionTypeId] = s.[ActionTypeId],[TaskSettings] = s.[TaskSettings],[OwnerId] = s.[OwnerId]
when not matched by target then
	insert([Id],[Name],[IsEnabled],[TaskType],[TriggerTypeId],[ActionTypeId],[TaskSettings],[OwnerId])
	values(s.[Id],s.[Name],s.[IsEnabled],s.[TaskType],s.[TriggerTypeId],s.[ActionTypeId],s.[TaskSettings],s.[OwnerId]);


--[genericdata].[DataRecordType]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('5E2F6708-206B-457C-84E3-5E9E43937795','Generic Type',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"GenericField","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":1}}]}',null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings]))
merge	[genericdata].[DataRecordType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ParentID] = s.[ParentID],[Fields] = s.[Fields],[ExtraFieldsEvaluator] = s.[ExtraFieldsEvaluator],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
	values(s.[ID],s.[Name],s.[ParentID],s.[Fields],s.[ExtraFieldsEvaluator],s.[Settings]);