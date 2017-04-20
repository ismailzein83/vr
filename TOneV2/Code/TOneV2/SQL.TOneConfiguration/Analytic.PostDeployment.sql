﻿/*
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
--common.[extensionconfiguration]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('83AD690C-CF9F-49CA-BAD0-07B7AA575BEA','VR_Analytic','Analytic','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticReportSettingsEditor.html","EnableAdd":true}'),
('82FF3B8A-0C39-4376-9602-B84A240FBF82','VR_AnalyticReport','Analytic Report','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticReportViewEditor.html","EnableAdd":true}'),
('8C5B1E66-20F0-4B26-BC4F-01060B3C3DAA','VR_Analytic_Report_MeasureStyle_Range','Range Condition','Analytic_MeasureStyleRuleTemplates','{"Editor":"vr-analytic-measurestyle-stylerulecondition-range","RuntimeEditor":""}'),
('4E972A40-2887-412B-B192-8E4E9739631C','VR_Analytic_Report_MeasureStyle_Compare','Compare Condition','Analytic_MeasureStyleRuleTemplates','{"Editor":"vr-analytic-measurestyle-stylerulecondition-compare","RuntimeEditor":""}'),

('DBEFFA6E-E75E-497F-8ACF-8F15469D9B90','VR_Analytic_Report_RealTime_Chart','Real Time Chart','Analytic_RealTimeWidgetSettings','{"Editor":"vr-analytic-realtime-widgets-chart-definition","RuntimeEditor":"vr-analytic-realtime-chart-toprecords"}'),
('CADC9403-6668-48E9-B452-D398B62921AB','VR_Analytic_Report_RealTime_TimeVariation','Time Variation','Analytic_RealTimeWidgetSettings','{"Editor":"vr-analytic-realtime-widgets-timevariation-definition","RuntimeEditor":"vr-analytic-realtime-chart-timevariation"}'),

('DC962A83-2FDA-456F-9940-15E9BE787D89','VR_Analytic_DARecordAggregate_SumAggregate','Sum','Analytic_DARecordAggregate','{"Editor" : "vr-analytic-sumaggregate"}'),
('982B1256-3F3E-4CA6-9D23-8844B060062D','VR_Analytic_DARecordAggregate_DistinctCountAggregate','Distinct Count','Analytic_DARecordAggregate','{"Editor" : "vr-analytic-distinctcountaggregate"}'),
('DAD39EDB-65B1-4C40-935C-7E6339267055','VR_Analytic_DARecordAggregate_CountAggregate','Count','Analytic_DARecordAggregate','{"Editor" : "vr-analytic-countaggregate"}'),

('635B2CE2-F787-4F46-832E-69B78D422FD5','VR_Analytic_Report_RealTime','Real Time','Analytic_AnalyticReportConfigType','{"Editor":"vr-analytic-analyticreport-realtime-definition","RuntimeEditor":"vr-analytic-analyticreport-realtime-runtime"}'),
('E5FB0790-5428-44B4-BB1F-4F79B69CD6EF','VR_Analytic_Report_History','History','Analytic_AnalyticReportConfigType','{"Editor":"vr-analytic-analyticreport-history-definition","RuntimeEditor":"vr-analytic-analyticreport-history-runtime"}'),
('82AA89F6-4D19-4168-A499-CDD2875F1702','VR_Analytic_Report_RecordSearch','Record Search','Analytic_AnalyticReportConfigType','{"Editor":"vr-analytic-analyticreport-recordsearch-definition","RuntimeEditor":"vr-analytic-analyticreport-recordsearch-runtime"}'),

('D050DEB3-700E-437B-86D1-510A81C0C14C','VR_Analytic_WidgetSettings_Chart','Chart','Analytic_AnalyticWidgetsSettings','{"Editor":"vr-analytic-widgets-chart-definition","RuntimeEditor":"vr-analytic-chart-toprecords"}'),
('7A2A35E2-543A-42C7-B97F-E05EE8D09A00','VR_Analytic_WidgetSettings_Grid','Grid','Analytic_AnalyticWidgetsSettings','{"Editor":"vr-analytic-widgets-grid-definition","RuntimeEditor":"vr-analytic-datagrid-analyticrecords"}'),
('CD2AFBD8-5C2F-4E50-8F36-F57C35A0C10F','VR_Analytic_WidgetSettings_PieChart','Pie Chart','Analytic_AnalyticWidgetsSettings','{"Editor":"vr-analytic-widgets-piechart-definition","RuntimeEditor":"vr-analytic-piechart-toprecords"}'),
('A1CB1C46-0FFA-41B0-82B0-2CCE407AD86C','VR_Analytic_Report_RealTime_DefaultSearch','Default Real Time Search','Analytic_RealTimeSearchSettings','{"Editor":"vr-analytic-analyticreport-realtime-searchsettings-defaultsearch","RuntimeEditor":""}'),

('E2A332A2-74FA-4C42-A5D1-33FBDA093946','VR_Analytic_Report_ItemAction_OpenRecordSearch','Open Record Search','Analytic_ItemActionTemplates','{"Editor":"vr-analytic-analyticitemaction-openrecordsearch"}'),

('BCFFD15E-1A84-46E0-8BD6-45B36049E29E','VR_Analytic_TimeRangeFilter_PreviousPeriodFilter','Previous Period','Analytic_TimeRangeFilter','{"Editor" : "vr-analytic-previousperiodfilter"}'),
('9EA37C6B-FA33-42C1-A9C9-228BEF0878A3','VR_Analytic_TimeRangeFilter_LastPeriodFilter','Last Period','Analytic_TimeRangeFilter','{"Editor" : "vr-analytic-lastperiodfilter"}'),

('BCC9AD0B-46EC-4ED1-B79F-47B4518F76B8','VR_Analytic_Report_History_GenericSearch','Generic Search Settings','Analytic_AnalyticHistoryReportSettings','{"Editor":"vr-analytic-reportsearchsettings-genericsearch","RuntimeEditor":""}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[extensionconfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end

--[sec].[Module]------------------------------1501 to 1600------------------------------------------------------
begin

set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A7E56800-22DC-40C3-B143-454B3291772D','Analytics Management',null,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,10,0)
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
------------------------------------------------------------------------------------------------------------
end

--[sec].[View]-----------------------------15001 to 16000---------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1FAA2B35-B804-4AD7-9D98-0F933CD36150','Tables','Analytic Table Management','#/view/Analytic/Views/GenericAnalytic/Definition/AnalyticTableManagement','A7E56800-22DC-40C3-B143-454B3291772D','VR_Analytic/AnalyticTable/GetFilteredAnalyticTables',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2),
('3DE2E950-F619-4C54-A999-507BF2E0CD39','Reports','Analytic Report Management','#/view/Analytic/Views/GenericAnalytic/Definition/AnalyticReportManagement','A7E56800-22DC-40C3-B143-454B3291772D','VR_Analytic/AnalyticReport/GetFilteredAnalyticReports',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',3),
('D69AAFD7-6630-43A1-AA57-548EA0E9C1EE','Data Analysis Definitions','Data Analysis Definitions','#/view/Analytic/Views/DataAnalysis/DataAnalysisDefinition/DataAnalysisDefinitionManagement','A7E56800-22DC-40C3-B143-454B3291772D','VR_Analytic/DataAnalysisDefinition/GetFilteredDataAnalysisDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',4)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
-------------------------------------------------------------------------------------------------------------
end


--[sec].[BusinessEntity]----------------4201 to 4500------------------------------------------------
begin
set nocount on;;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('6FDD628E-C876-4DBF-92BF-77225A88C3B9','VR_Analytic_AnalyticReport','Analytic Reports','7913ACD9-38C5-43B3-9612-BEFF66606F22',0,'["View","Add","Edit"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
----------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Analytic/AnalyticTable/GetAnalyticTablesInfo',null),
('VR_Analytic/AnalyticTable/GetFilteredAnalyticTables','VR_SystemConfiguration: View'),
('VR_Analytic/AnalyticTable/GetTableById',null),
('VR_Analytic/AnalyticTable/UpdateAnalyticTable',null),
('VR_Analytic/AnalyticTable/AddAnalyticTable','VR_SystemConfiguration: Add'),

('VR_Analytic/AnalyticReport/GetAnalyticReportsInfo',null),
('VR_Analytic/AnalyticReport/GetAnalyticReportById',null),
('VR_Analytic/AnalyticReport/UpdateAnalyticReport','VR_Analytic_AnalyticReport: Edit'),
('VR_Analytic/AnalyticReport/AddAnalyticReport','VR_Analytic_AnalyticReport: Add'),
('VR_Analytic/AnalyticReport/GetFilteredAnalyticReports','VR_Analytic_AnalyticReport: View'),
('VR_Analytic/AnalyticReport/GetAnalyticReportConfigTypes',null),('VR_Analytic/DataAnalysisDefinition/GetFilteredDataAnalysisDefinitions','VR_SystemConfiguration: View'),('VR_Analytic/DataAnalysisDefinition/GetDataAnalysisDefinition',null),('VR_Analytic/DataAnalysisDefinition/AddDataAnalysisDefinition','VR_SystemConfiguration: Add'),('VR_Analytic/DataAnalysisDefinition/UpdateDataAnalysisDefinition','VR_SystemConfiguration: Edit'),('VR_Analytic/DataAnalysisDefinition/GetDataAnalysisDefinitionSettingsExtensionConfigs',null),('VR_Analytic/DataAnalysisDefinition/GetDataAnalysisDefinitionsInfo',null)
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

--[logging].[LoggableEntity]----------------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[UniqueName],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('61D3E1C1-3D2D-46ED-8021-EE5194BB30F6','VR_Analytic_AnalyticItemConfig_1','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticItemConfig_ViewHistoryItem"}'),('55AA5497-D304-418A-8F03-DBFCDF54BA53','VR_Analytic_AnalyticItemConfig_2','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticItemConfig_ViewHistoryItem"}'),('0D3A3D7A-AA41-4D21-BC24-EFEB3CCDFCCC','VR_Analytic_AnalyticItemConfig_3','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticItemConfig_ViewHistoryItem"}'),('4ABAC893-E6BB-4B94-BC2F-D80567247853','VR_Analytic_AnalyticItemConfig_4','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticItemConfig_ViewHistoryItem"}'),('6B9C0B08-E80D-42CB-AF8E-586C4F9507DF','VR_Notification_AlertRule_71aad15b-32a2-435a-a8b1-f7b07ae60de3','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Notification_AlertRule_ViewHistoryItem"}'),('BBEC00FA-0495-438B-A10F-86C8B7E1BC0C','VR_Analytic_DataAnalysisDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_DataAnalysisDefinition_ViewHistoryItem"}'),('8A52BD2F-91A7-4EBD-A68C-720DBC2369D4','VR_Analytic_DataAnalysisItemDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_DataAnalysisItemDefinition_ViewHistoryItem"}'),('AD3C8E78-517D-404C-A72B-F7027EB37B78','VR_Analytic_AnalyticReport','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticReport_ViewHistoryItem"}'),('5360CA20-02EF-451F-8CBE-62E68BC1C05F','VR_Analytic_AnalyticTable','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticTable_ViewHistoryItem"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[UniqueName],[Settings]))merge	[logging].[LoggableEntity] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]when not matched by target then	insert([ID],[UniqueName],[Settings])	values(s.[ID],s.[UniqueName],s.[Settings]);