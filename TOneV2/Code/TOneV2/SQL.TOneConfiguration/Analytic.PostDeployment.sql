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
--common.[extensionconfiguration]-------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('83AD690C-CF9F-49CA-BAD0-07B7AA575BEA','VR_Analytic','Analytic','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticReportSettingsEditor.html","EnableAdd":true}'),
('82FF3B8A-0C39-4376-9602-B84A240FBF82','VR_AnalyticReport','Analytic Report','VR_Security_ViewTypeConfig','{"Editor":"/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticReportViewEditor.html","EnableAdd":true}'),

('8C5B1E66-20F0-4B26-BC4F-01060B3C3DAA','VR_Analytic_Report_MeasureStyle_Range','Range Condition','Analytic_MeasureStyleRuleTemplates','{"Editor":"vr-analytic-measurestyle-stylerulecondition-range","RuntimeEditor":""}'),
('4E972A40-2887-412B-B192-8E4E9739631C','VR_Analytic_Report_MeasureStyle_Compare','Compare Condition','Analytic_MeasureStyleRuleTemplates','{"Editor":"vr-analytic-measurestyle-stylerulecondition-compare","RuntimeEditor":""}'),

('DBEFFA6E-E75E-497F-8ACF-8F15469D9B90','VR_Analytic_Report_RealTime_Chart','Real Time Chart','Analytic_RealTimeWidgetSettings','{"Editor":"vr-analytic-realtime-widgets-chart-definition","RuntimeEditor":"vr-analytic-realtime-chart-toprecords"}'),
('CADC9403-6668-48E9-B452-D398B62921AB','VR_Analytic_Report_RealTime_TimeVariation','Time Variation','Analytic_RealTimeWidgetSettings','{"Editor":"vr-analytic-realtime-widgets-timevariation-definition","RuntimeEditor":"vr-analytic-realtime-chart-timevariation"}'),

('DC962A83-2FDA-456F-9940-15E9BE787D89','VR_Analytic_DARecordAggregate_SumAggregate','Sum','Analytic_DARecordAggregate','{"Editor" : "vr-analytic-sumaggregate"}'),
('982B1256-3F3E-4CA6-9D23-8844B060062D','VR_Analytic_DARecordAggregate_DistinctCountAggregate','Distinct Count','Analytic_DARecordAggregate','{"Editor" : "vr-analytic-distinctcountaggregate"}'),
('DAD39EDB-65B1-4C40-935C-7E6339267055','VR_Analytic_DARecordAggregate_CountAggregate','Count','Analytic_DARecordAggregate','{"Editor" : "vr-analytic-countaggregate"}'),
('F50143C9-62B4-4A6F-8C94-B9EED7970DCF','VR_Analytic_DARecordAggregate_MaxAggregate','Max','Analytic_DARecordAggregate','{"Editor" : "vr-analytic-maxaggregate"}'),
('CE3AC24F-45C2-4965-B2AA-AB09DA9B97F8','VR_Analytic_DARecordAggregate_MinAggregate','Min','Analytic_DARecordAggregate','{"Editor" : "vr-analytic-minaggregate"}'),


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

('BCC9AD0B-46EC-4ED1-B79F-47B4518F76B8','VR_Analytic_Report_History_GenericSearch','Generic Search Settings','Analytic_AnalyticHistoryReportSettings','{"Editor":"vr-analytic-reportsearchsettings-genericsearch","RuntimeEditor":""}'),

('10631F32-9116-4443-A73D-2D4B77111634','Postgres Data Provider','Postgres Data Provider','VR_Analytic_AnalyticDataProviderSettings'	,'{"Editor":"vr-analytic-postgres-dataprovider"}'),
('3CBA3F20-6535-4EBF-9704-DF65AC605671','Sql Data Provider','Sql Data Provider','VR_Analytic_AnalyticDataProviderSettings'				,'{"Editor":"vr-analytic-sql-dataprovider"}'),
('4f72f80c-3928-460f-aaf5-cc18a78c8265','Memory Data Provider','Memory Data Provider','VR_Analytic_AnalyticDataProviderSettings'		,'{"Editor":"vr-analytic-memory-dataprovider"}'),
('DD11C35D-3DA8-4094-9492-678D243EFE5A','Rest API Data Provider','Rest API Data Provider','VR_Analytic_AnalyticDataProviderSettings'	,'{"Editor":"vr-analytic-restapi-dataprovider"}'),
('017EAC3C-BDE1-451A-A3C6-3FBF808115E6','Specific Mapping','Specific Mapping','VR_Analytic_MeasureMappingRuleSettings','{"Editor":"vr-analytic-measuremappingrulesettings-specificmapping"}'),
('D8B533F7-3375-4BD3-8D2C-65C137E7BD51','Same Dimensions','Same Dimensions','VR_Analytic_DimensionMappingRuleSettings','{"Editor":"vr-analytic-dimension-same"}'),
('1E93563D-CCA6-48F4-BD34-A21B26A3B773','Specific Dimension Mapping','Specific Dimension Mapping','VR_Analytic_DimensionMappingRuleSettings','{"Editor":"vr-analytic-dimensionmappingrulessettings-specificmapping"}'),
('76DC3174-8FB1-445B-A118-B9E86DB46A5E','Record Search Query Definition','Record Search Query','VR_Analytic_QueryDefinitionSettings_AutomatedReport','{"Editor":"vr-recordsearchquerydefinition-automatedreport"}'),
('A5B07696-61B4-4371-B52C-770667E0EB05','VR_AutomatedReportQueryDefinition_Settings','Automated Report Query Definition','VR_Common_VRComponentType','{"Editor":"vr-analytic-automatedreportquerydefinition-settings"}'),
('6531E07F-CA21-4579-9E38-86A7B835A221','FTP Handler','FTP','VR_Analytic_HandlerSettings_AutomatedReport','{"Editor":"vr-analytic-ftphandler-automatedreporthandler"}'),
('C05375FD-4C3A-44B2-ACEE-A0EDEE56B488','Send Email Handler','Send Email','VR_Analytic_HandlerSettings_AutomatedReport','{"Editor":"vr-analytic-sendemailhandler-automatedreporthandler"}'),
('9FAAE9B2-931E-4B3F-BDA4-B0F3B7647488','Advanced Excel File Generator','Excel File','VR_Analytic_FileGenerator_AutomatedReport','{"Editor":"vr-analytic-advancedexcel-filegenerator"}'),
('9CC73443-2A1A-4405-A1ED-1DE27B9DCB42','Sequence','Sequence','VR_Analytic_AutomatedReportFileNameParts','{"Editor":"vr-analytic-automatedreportfilenamepart-sequence"}'),
('A194AAC8-0675-4100-8A8B-1FBE4105FE09','Date','Date','VR_Analytic_AutomatedReportFileNameParts','{"Editor":"vr-analytic-automatedreportfilenamepart-date"}'),
('1654683D-6168-47F4-B157-661A1FE88A95','DownloadFileAction','Download File Action','VR_Analytic_ReportGenerationAction_ReportAction','{"Editor":"vr-analytic-reportgeneration-settings-reportaction-downloadaction"}'),
('38974659-FB26-415E-82BC-2895E1D09238','StandardReportGenerationFilter','Standard','VR_Analytic_ReportGenerationAction_FilterConfig','{"Editor":"vr-analytic-reportgeneration-filter-standard"}'),
('4ecc5dc2-5781-437a-af6c-acaedc3c4a5d','Analytic Table Query Definition','Analytic Table Query','VR_Analytic_QueryDefinitionSettings_AutomatedReport','{"Editor":"vr-analytictablequerydefinition-automatedreport"}'),
('F139CA56-D602-4D91-9FC2-A14418D9831E','Generate Files Handler','Generate Files','VR_Analytic_HandlerSettings_AutomatedReport','{"Editor":"vr-analytic-generatefileshandler-automatedreporthandler"}'),
('F38308EF-433A-48C6-891B-34847929FD5A','FTP Action Type','FTP','VR_Analytic_AutomatedReport_HandlerSettings_ActionType','{"Editor":"vr-analytic-ftpactiontype-generatefileshandler"}'),
('D7D6D580-40BD-42C6-ABBB-7FA6B60A5462','Send Email Action Type','Send Email','VR_Analytic_AutomatedReport_HandlerSettings_ActionType','{"Editor":"vr-analytic-sendemailactiontype-generatefileshandler"}'),
('9EEBA2C1-0EC9-4DE0-B9E1-755A1D8CE0AA','Data Record Source','Data Record Source','VR_Analytic_DRSearchPageSubviewDefinitionSettingsConfig','{"Editor":"vr-analytic-datarecordsource-subviewdefinitionsettings"}'),
('E1ECAA21-462B-4AFD-B886-34A28A35A1FE','Flat File Generator','Flat File','VR_Analytic_FileGenerator_AutomatedReport','{"Editor":"vr-analytic-flatfile-filegenerator"}'),
('482D20EB-78B0-4633-B6F3-4B93B2ED1190','Report Generation Custom Code File Generator','Custom Report','VR_Analytic_FileGenerator_AutomatedReport','{"Editor":"vr-analytic-reportgenerationcustomcode-filegenerator"}'),
('92B03E4E-37FB-48A1-ACEE-841135C30B5E','VR_Analytic_ReportGenerationCustomCode','Report Generation Custom Code','VR_Common_VRComponentType','{"Editor":"vr-analytic-reportgenerationcustomcode-settings"}'),
('1A67CEF6-7472-4151-8A74-8AF12D245E27','OverallAnalytic','Overall Analytic','VR_Analytic_FiguresTileQuerySettings','{"Editor":"vr-analytic-overallanalytic-figurestilequery"}'),
('A0833F27-6718-40E1-BB69-344B0BD3CFAF','MostRelevantAnalytic','Most Relevant Analytic','VR_Analytic_FiguresTileQuerySettings','{"Editor":"vr-analytic-mostrelevant-figurestilequery"}'),
('E9CC9A31-DD48-45F5-A849-5402CCE3B7AF','VR_Analytic_WidgetSettings_Gauge','Gauge','Analytic_AnalyticWidgetsSettings','{"Editor":"vr-analytic-widgets-gauge-definition","RuntimeEditor":"vr-analytic-gauge-runtime"}'),
('76285A3E-C385-49C3-8833-DEA21A40471E','WidgetSettings','Analytic Widget','VRCommon_VRTileExtendedSettings','{"Editor":"vr-common-widgetsettings-definition"}'),
('2E88B323-DE47-4C6B-8AD5-CC941BB3611E','RDB Data Provider','RDB Data Provider','VR_Analytic_AnalyticDataProviderSettings',' {"Editor":"vr-analytic-rdb-dataprovider"}')

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
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1FAA2B35-B804-4AD7-9D98-0F933CD36150','Tables','Analytic Table','#/view/Analytic/Views/GenericAnalytic/Definition/AnalyticTableManagement'												,'A7E56800-22DC-40C3-B143-454B3291772D','VR_Analytic/AnalyticTable/GetFilteredAnalyticTables',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2),
('3DE2E950-F619-4C54-A999-507BF2E0CD39','Reports','Analytic Report','#/view/Analytic/Views/GenericAnalytic/Definition/AnalyticReportManagement'												,'A7E56800-22DC-40C3-B143-454B3291772D','VR_Analytic/AnalyticReport/GetFilteredAnalyticReports',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',3),
('D69AAFD7-6630-43A1-AA57-548EA0E9C1EE','Data Analysis Definitions','Data Analysis Definitions','#/view/Analytic/Views/DataAnalysis/DataAnalysisDefinition/DataAnalysisDefinitionManagement','A7E56800-22DC-40C3-B143-454B3291772D','VR_Analytic/DataAnalysisDefinition/GetFilteredDataAnalysisDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',4),
('ADBB44FE-5470-413C-A5F6-8AE8C585FA31','Report Generation','Report Generation','#/view/Analytic/Views/VRReportGeneration/VRReportGenerationManagement'										,'EB303A61-929A-4D33-BF50-18F40308BC86',null,null,null,'{"$type":"Vanrise.Analytic.Business.VRReportGenerationViewSettings, Vanrise.Analytic.Business"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',150)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
-------------------------------------------------------------------------------------------------------------
end


--[sec].[BusinessEntity]----------------4201 to 4500------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('85ABD90F-F9BF-4E0C-87ED-08F067EEA5D4','VR_Analytic_ReportGeneration','Report Generation','61451603-E7B9-40C6-AE27-6CBA974E1B3B',0,'["Manage"]')
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
('VR_Analytic/AnalyticReport/GetAnalyticReportConfigTypes',null),

('VR_Analytic/DataAnalysisDefinition/GetFilteredDataAnalysisDefinitions','VR_SystemConfiguration: View'),
('VR_Analytic/DataAnalysisDefinition/GetDataAnalysisDefinition',null),
('VR_Analytic/DataAnalysisDefinition/AddDataAnalysisDefinition','VR_SystemConfiguration: Add'),
('VR_Analytic/DataAnalysisDefinition/UpdateDataAnalysisDefinition','VR_SystemConfiguration: Edit'),
('VR_Analytic/DataAnalysisDefinition/GetDataAnalysisDefinitionSettingsExtensionConfigs',null),
('VR_Analytic/DataAnalysisDefinition/GetDataAnalysisDefinitionsInfo',null),
('VR_Analytic/VRReportGeneration/AddPublicVRReportGeneration','VR_Analytic_ReportGeneration: Manage'),
('VR_Analytic/VRReportGeneration/UpdatePublicVRReportGeneration','VR_Analytic_ReportGeneration: Manage')
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

--[logging].[LoggableEntity]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[UniqueName],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('61D3E1C1-3D2D-46ED-8021-EE5194BB30F6','VR_Analytic_AnalyticItemConfig_1','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticItemConfig_ViewHistoryItem"}'),
('55AA5497-D304-418A-8F03-DBFCDF54BA53','VR_Analytic_AnalyticItemConfig_2','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticItemConfig_ViewHistoryItem"}'),
('0D3A3D7A-AA41-4D21-BC24-EFEB3CCDFCCC','VR_Analytic_AnalyticItemConfig_3','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticItemConfig_ViewHistoryItem"}'),
('4ABAC893-E6BB-4B94-BC2F-D80567247853','VR_Analytic_AnalyticItemConfig_4','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticItemConfig_ViewHistoryItem"}'),

('6B9C0B08-E80D-42CB-AF8E-586C4F9507DF','VR_Notification_AlertRule_71aad15b-32a2-435a-a8b1-f7b07ae60de3','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Notification_AlertRule_ViewHistoryItem"}'),

('BBEC00FA-0495-438B-A10F-86C8B7E1BC0C','VR_Analytic_DataAnalysisDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_DataAnalysisDefinition_ViewHistoryItem"}'),
('8A52BD2F-91A7-4EBD-A68C-720DBC2369D4','VR_Analytic_DataAnalysisItemDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_DataAnalysisItemDefinition_ViewHistoryItem"}'),
('AD3C8E78-517D-404C-A72B-F7027EB37B78','VR_Analytic_AnalyticReport','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticReport_ViewHistoryItem"}'),
('5360CA20-02EF-451F-8CBE-62E68BC1C05F','VR_Analytic_AnalyticTable','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Analytic_AnalyticTable_ViewHistoryItem"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[UniqueName],[Settings]))
merge	[logging].[LoggableEntity] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[UniqueName],[Settings])
	values(s.[ID],s.[UniqueName],s.[Settings]);




--[bp].[BPDefinition]-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3FDB4499-AA32-4F21-B5FA-BBF59F621479','Vanrise.Analytic.BP.Arguments.VRAutomatedReportProcessInput','Automated Report','Vanrise.Analytic.BP.AutomatedReportProcess, Vanrise.Analytic.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ScheduledExecEditor":"vr-analytic-automatedreportprocess-schedualed","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Manage"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Manage"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Manage"]}}]}}},"BusinessRuleSetSupported":false}')
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


	 
--[genericdata].[DataStore]-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('42C7CA90-D596-42B9-9F37-CDEEDF799B2F','Logging Store','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataStoreSettings, Vanrise.GenericData.SQLDataStorage","ConfigId":"2aeec2de-ec44-4698-aaef-8e9dbf669d1e","ConnectionStringName":"LogDBConnString","IsRemoteDataStore":false}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[genericdata].[DataStore] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);


	 
--[genericdata].[DataRecordStorage]-----------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D6F84E47-E08F-44D4-B6FB-D00191486A25','Action Audit','0576A0BE-A4FD-4FF0-AA8E-256672D59595','42C7CA90-D596-42B9-9F37-CDEEDF799B2F','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageSettings, Vanrise.GenericData.SQLDataStorage","TableName":"ActionAudit","TableSchema":"logging","Columns":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ID","SQLDataType":"BIGINT","ValueExpression":"ID","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"UserID","SQLDataType":"INT","ValueExpression":"UserID","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"URLID","SQLDataType":"INT","ValueExpression":"URLID","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ModuleID","SQLDataType":"INT","ValueExpression":"ModuleID","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"EntityID","SQLDataType":"INT","ValueExpression":"EntityID","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ActionID","SQLDataType":"INT","ValueExpression":"ActionID","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ObjectID","SQLDataType":"VARCHAR(255)","ValueExpression":"ObjectID","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ObjectName","SQLDataType":"NVARCHAR(900)","ValueExpression":"ObjectName","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"ActionDescription","SQLDataType":"NVARCHAR(MAX)","ValueExpression":"ActionDescription","IsUnique":false,"IsIdentity":false},{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataRecordStorageColumn, Vanrise.GenericData.SQLDataStorage","ColumnName":"LogTime","SQLDataType":"DATETIME","ValueExpression":"LogTime","IsUnique":false,"IsIdentity":false}]},"NullableFields":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.SQLDataStorage.NullableField, Vanrise.GenericData.SQLDataStorage]], mscorlib","$values":[]},"IncludeQueueItemId":false,"DateTimeField":"LogTime","EnableUseCaching":false,"RequiredLimitResult":false,"DontReflectToDB":true,"DenyAPICall":false,"RequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"FieldsPermissions":{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordStorageFieldsPermission, Vanrise.GenericData.Entities]], mscorlib","$values":[]}}',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State]))
merge	[genericdata].[DataRecordStorage] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[DataRecordTypeID] = s.[DataRecordTypeID],[DataStoreID] = s.[DataStoreID],[Settings] = s.[Settings],[State] = s.[State]
when not matched by target then
	insert([ID],[Name],[DataRecordTypeID],[DataStoreID],[Settings],[State])
	values(s.[ID],s.[Name],s.[DataRecordTypeID],s.[DataStoreID],s.[Settings],s.[State]);


--[genericdata].[DataRecordType]--------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ParentID],[Fields],[ExtraFieldsEvaluator],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('0576A0BE-A4FD-4FF0-AA8E-256672D59595','Action Audit',null,'{"$type":"System.Collections.Generic.List`1[[Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities]], mscorlib","$values":[{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ID","Title":"ID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions","ConfigId":"75aef329-27bd-4108-b617-f5cc05ff2aa3","ViewerEditor":"vr-genericdata-fieldtype-number-viewereditor","RuntimeEditor":"vr-genericdata-fieldtype-number-runtimeeditor","DataPrecision":0,"DataType":2,"IsNullable":false,"CanRoundValue":true,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"UserID","Title":"User","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"217a8f71-1dd6-4613-8ae2-540a510f5ff5","IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"URLID","Title":"URL","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"527083e4-f795-46ae-a17e-cd2cff13e7a6","IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ModuleID","Title":"Module","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"a1eb7032-2d31-4f4e-8cec-5dddd14f4e17","IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"EntityID","Title":"Entity","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"2da03147-fe67-4cda-979e-c4abadc35079","IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ActionID","Title":"Action","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions","ConfigId":"2e16c3d4-837b-4433-b80e-7c02f6d71467","RuntimeEditor":"vr-genericdata-fieldtype-businessentity-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-businessentity-viewereditor","BusinessEntityDefinitionId":"9a09b2b1-dd18-49c9-9913-3ba97916a1cb","IsNullable":false,"OrderType":1,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ObjectID","Title":"Object ID","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","OrderType":1,"ViewerEditor":"vr-genericdata-fieldtype-text-viewereditor","DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ObjectName","Title":"Object Name","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","OrderType":1,"ViewerEditor":"vr-genericdata-fieldtype-text-viewereditor","DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"ActionDescription","Title":"Action Description","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions","ConfigId":"3f29315e-b542-43b8-9618-7de216cd9653","RuntimeEditor":"vr-genericdata-fieldtype-text-runtimeeditor","OrderType":1,"ViewerEditor":"vr-genericdata-fieldtype-text-viewereditor","DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false},{"$type":"Vanrise.GenericData.Entities.DataRecordField, Vanrise.GenericData.Entities","Name":"LogTime","Title":"Log Time","Type":{"$type":"Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions","ConfigId":"b8712417-83ab-4d4b-9ee1-109d20ceb909","RuntimeEditor":"vr-genericdata-fieldtype-datetime-runtimeeditor","ViewerEditor":"vr-genericdata-fieldtype-datetime-viewereditor","DataType":0,"IsNullable":false,"DetailViewerEditor":"vr-genericdata-datarecordfield-defaultdetailviewer","OrderType":0,"StoreValueSerialized":false,"CanRoundValue":false},"IsInheritedFromExtraField":false}]}',null,'{"$type":"Vanrise.GenericData.Entities.DataRecordTypeSettings, Vanrise.GenericData.Entities","IdField":"ID"}')
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



--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('9A09B2B1-DD18-49C9-9913-3BA97916A1CB','LKUPAction','LKUP Action','{"$type":"Vanrise.GenericData.Business.LKUPBusinessEntityDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"99e22964-f94e-4bcd-8383-22a613e5ae7f","DefinitionEditor":"vr-genericdata-lookupbusinessentity-editor","IdType":"System.String","SelectorUIControl":"vr-genericdata-lookupbusinessentity-selector","ManagerFQTN":"Vanrise.GenericData.Business.LKUPBusinessEntityManager, Vanrise.GenericData.Business","SelectorSingularTitle":"Action","SelectorPluralTitle":"Actions","ExtendedSettings":{"$type":"Vanrise.Common.MainExtensions.VRActionAuditLKUPBEDefinitionSettings, Vanrise.Common.MainExtensions","ConfigId":"442bcfad-1407-4158-82e7-e1b7a0ab458b","Type":4},"Security":{"$type":"Vanrise.GenericData.Business.LKUPBEDefinitionSecurity, Vanrise.GenericData.Business","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"AddRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"EditRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}}'),
('A1EB7032-2D31-4F4E-8CEC-5DDDD14F4E17','LKUPModule','LKUP Module','{"$type":"Vanrise.GenericData.Business.LKUPBusinessEntityDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"99e22964-f94e-4bcd-8383-22a613e5ae7f","DefinitionEditor":"vr-genericdata-lookupbusinessentity-editor","IdType":"System.String","SelectorUIControl":"vr-genericdata-lookupbusinessentity-selector","ManagerFQTN":"Vanrise.GenericData.Business.LKUPBusinessEntityManager, Vanrise.GenericData.Business","SelectorSingularTitle":"Module","SelectorPluralTitle":"Modules","ExtendedSettings":{"$type":"Vanrise.Common.MainExtensions.VRActionAuditLKUPBEDefinitionSettings, Vanrise.Common.MainExtensions","ConfigId":"442bcfad-1407-4158-82e7-e1b7a0ab458b","Type":2},"Security":{"$type":"Vanrise.GenericData.Business.LKUPBEDefinitionSecurity, Vanrise.GenericData.Business","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"AddRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"EditRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}}'),
('2DA03147-FE67-4CDA-979E-C4ABADC35079','LKUPEntity','LKUP Entity','{"$type":"Vanrise.GenericData.Business.LKUPBusinessEntityDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"99e22964-f94e-4bcd-8383-22a613e5ae7f","DefinitionEditor":"vr-genericdata-lookupbusinessentity-editor","IdType":"System.String","SelectorUIControl":"vr-genericdata-lookupbusinessentity-selector","ManagerFQTN":"Vanrise.GenericData.Business.LKUPBusinessEntityManager, Vanrise.GenericData.Business","SelectorSingularTitle":"Entity","SelectorPluralTitle":"Entities","ExtendedSettings":{"$type":"Vanrise.Common.MainExtensions.VRActionAuditLKUPBEDefinitionSettings, Vanrise.Common.MainExtensions","ConfigId":"442bcfad-1407-4158-82e7-e1b7a0ab458b","Type":3},"Security":{"$type":"Vanrise.GenericData.Business.LKUPBEDefinitionSecurity, Vanrise.GenericData.Business","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"AddRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"EditRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}}'),
('527083E4-F795-46AE-A17E-CD2CFF13E7A6','LKUPURL','LKUP URL','{"$type":"Vanrise.GenericData.Business.LKUPBusinessEntityDefinitionSettings, Vanrise.GenericData.Business","ConfigId":"99e22964-f94e-4bcd-8383-22a613e5ae7f","DefinitionEditor":"vr-genericdata-lookupbusinessentity-editor","IdType":"System.String","SelectorUIControl":"vr-genericdata-lookupbusinessentity-selector","ManagerFQTN":"Vanrise.GenericData.Business.LKUPBusinessEntityManager, Vanrise.GenericData.Business","SelectorSingularTitle":"URL","SelectorPluralTitle":"URLs","ExtendedSettings":{"$type":"Vanrise.Common.MainExtensions.VRActionAuditLKUPBEDefinitionSettings, Vanrise.Common.MainExtensions","ConfigId":"442bcfad-1407-4158-82e7-e1b7a0ab458b","Type":1},"Security":{"$type":"Vanrise.GenericData.Business.LKUPBEDefinitionSecurity, Vanrise.GenericData.Business","ViewRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"AddRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}},"EditRequiredPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[]}}}}'),
('A79ADBC8-0FF0-4857-BACA-A2C0D9FB22BF','AnalyticDefaultStatusDefinition','Analytic Default Status Definition','{"$type":"Vanrise.Common.Business.StatusDefinitionBESettings, Vanrise.Common.Business","ConfigId":"3f26b7e1-33d8-4428-9a3c-986805289c91","DefinitionEditor":"vr-common-statusdefinitionbe-editor","IdType":"System.Guid","ManagerFQTN":"Vanrise.Common.Business.StatusDefinitionManager, Vanrise.Common.Business","SelectorUIControl":"vr-common-statusdefinition-selector"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);



	--[common].[VRComponentType]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('6CBB0FC3-A0A9-4D1C-BC3A-8557D7692018','Action Audit','A5B07696-61B4-4371-B52C-770667E0EB05','{"$type":"Vanrise.Analytic.Entities.VRAutomatedReportQueryDefinitionSettings, Vanrise.Analytic.Entities","VRComponentTypeConfigId":"a5b07696-61b4-4371-b52c-770667e0eb05","ExtendedSettings":{"$type":"Vanrise.Analytic.MainExtensions.AutomatedReport.Queries.RecordSearchQueryDefinitionSettings, Vanrise.Analytic.MainExtensions","ConfigId":"76dc3174-8fb1-445b-a118-b9e86db46a5e","DataRecordTypeId":"0576a0be-a4fd-4ff0-aa8e-256672d59595","DataRecordStorages":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.MainExtensions.AutomatedReport.Queries.RecordSearchQueryDefinitionDataRecordStorage, Vanrise.Analytic.MainExtensions]], mscorlib","$values":[{"$type":"Vanrise.Analytic.MainExtensions.AutomatedReport.Queries.RecordSearchQueryDefinitionDataRecordStorage, Vanrise.Analytic.MainExtensions","DataRecordStorageId":"d6f84e47-e08f-44d4-b6fb-d00191486a25","IsSelected":true}]},"RuntimeEditor":"vr-analytic-recordsearchquerydefinitionsettings-runtimeeditor"}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ConfigID],[Settings]))
merge	[common].[VRComponentType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ConfigID] = s.[ConfigID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ConfigID],[Settings])
	values(s.[ID],s.[Name],s.[ConfigID],s.[Settings]);


--[common].[Setting]---------------------------------------101 to 200----------------------------------------
--override technical settings
begin
set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('e435ae79-4382-472d-9f9d-d65053882d93','Automated Report','VR_Analytic_AutomatedReportSettings','General','{"Editor":"vr-analytic-automatedreport-settings"}','{"$type":"Vanrise.Analytic.Entities.VRAutomatedReportSettings, Vanrise.Analytic.Entities","FileNameParts":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.VRAutomatedReportFileNamePart, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.VRAutomatedReportFileNamePart, Vanrise.Analytic.Entities","VariableName":"DateTime","Description":"Date Time (dd-MM-yyyy)","Settings":{"$type":"Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.VRAutomatedReportDateFileNamePart, Vanrise.Analytic.MainExtensions","ConfigId":"a194aac8-0675-4100-8a8b-1fbe4105fe09","DateFormat":"dd-MM-yyyy"}},{"$type":"Vanrise.Analytic.Entities.VRAutomatedReportFileNamePart, Vanrise.Analytic.Entities","VariableName":"YearlySequence","Description":"Yearly  Sequence","Settings":{"$type":"Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.VRAutomatedReportSequenceFileNamePart, Vanrise.Analytic.MainExtensions","ConfigId":"9cc73443-2a1a-4405-a1ed-1de27b9dcb42","DateCounterType":0,"PaddingLeft":0}}]}}',1)
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

--update settings by removing wrong default value
UPDATE	[common].[Setting]
SET		[Data]= null
where	[ID]= 'FC50368F-68DF-4EF4-9367-895EFA393D73' and [Data] = '{"$type":"Vanrise.Analytic.Entities.KPISettings, Vanrise.Analytic.Entities","AnalyticTablesKPISettings":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"c0f56f28-e0af-4cbb-937a-0225cdf86e6a","MeasureStyleRules":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.MeasureStyleRule, Vanrise.Analytic.Entities]], mscorlib","$values":[]}},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"ed2f727d-b654-4708-90f4-05cd515dd809"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"45e3d8c6-96b0-4940-b9fa-0b645d8202b9"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"6cd535c0-ac49-46bb-aecf-0eae33823b20","MeasureStyleRules":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.MeasureStyleRule, Vanrise.Analytic.Entities]], mscorlib","$values":[]}},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"a2c2b20a-f74d-41d4-a01c-158899c704d4","MeasureStyleRules":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.MeasureStyleRule, Vanrise.Analytic.Entities]], mscorlib","$values":[]}},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"4f4c1dc0-6024-4ab9-933d-20f456360112","MeasureStyleRules":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.MeasureStyleRule, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.MeasureStyleRule, Vanrise.Analytic.Entities","MeasureName":"CountCDRs","Rules":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.StyleRule, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.StyleRule, Vanrise.Analytic.Entities","RecordFilter":{"$type":"Vanrise.GenericData.Entities.NumberRecordFilter, Vanrise.GenericData.Entities","CompareOperator":2,"Value":1.0,"FieldName":"CountCDRs"},"StyleValue":0,"StyleCode":"Red"},{"$type":"Vanrise.Analytic.Entities.StyleRule, Vanrise.Analytic.Entities","RecordFilter":{"$type":"Vanrise.GenericData.Entities.NumberRecordFilter, Vanrise.GenericData.Entities","CompareOperator":4,"Value":1.0,"FieldName":"CountCDRs"},"StyleValue":1,"StyleCode":"Green"}]}},{"$type":"Vanrise.Analytic.Entities.MeasureStyleRule, Vanrise.Analytic.Entities","MeasureName":"Amount","Rules":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.StyleRule, Vanrise.Analytic.Entities]], mscorlib","$values":[{"$type":"Vanrise.Analytic.Entities.StyleRule, Vanrise.Analytic.Entities","RecordFilter":{"$type":"Vanrise.GenericData.Entities.NumberRecordFilter, Vanrise.GenericData.Entities","CompareOperator":2,"Value":1.0,"FieldName":"Amount"},"StyleValue":2,"StyleCode":"Yellow"},{"$type":"Vanrise.Analytic.Entities.StyleRule, Vanrise.Analytic.Entities","RecordFilter":{"$type":"Vanrise.GenericData.Entities.NumberRecordFilter, Vanrise.GenericData.Entities","CompareOperator":4,"Value":1.0,"FieldName":"Amount"},"StyleValue":3,"StyleCode":"Blue"}]}}]}},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"23d30cd1-069c-4555-9173-288efe450e3b","MeasureStyleRules":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.MeasureStyleRule, Vanrise.Analytic.Entities]], mscorlib","$values":[]}},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"d68c5031-63b7-4ade-8c9b-2b1d67d2314e","MeasureStyleRules":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.MeasureStyleRule, Vanrise.Analytic.Entities]], mscorlib","$values":[]}},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"bfd79fe4-1f5e-423e-b071-4657c47f5c5b","MeasureStyleRules":{"$type":"System.Collections.Generic.List`1[[Vanrise.Analytic.Entities.MeasureStyleRule, Vanrise.Analytic.Entities]], mscorlib","$values":[]}},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"f94323ef-86d9-4752-b458-48bb4a1e24cd"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"efe9d65f-0b0e-4466-918f-4d8982a368f6"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"657436e7-9dba-4364-867a-6a49e3bf87b0"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"8105889d-133f-495d-8d1e-7a74d6a51876"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"3af9e02e-0292-4b8e-a307-7b323f32ea13"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"7ab26f93-16e8-4e25-9ea5-8958d32e5b08"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"5fa5eea4-2bcb-4f8d-a4fb-8cb61a13c792"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"f91f0557-fee5-4607-8457-a8e48fa4f5dd"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"d722f557-9cdc-4634-a86e-a941bf51c035"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"8b2de429-b0d6-4ce1-a5f2-c7c1c3bd2f47"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"8e3baf48-a2d3-4b09-8198-cbb4ad6f76f5"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"952c7db8-116f-484e-9f23-cec81b9491f2"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"ecbce87b-fb59-4a7a-bcc2-e0efe4010e37"},{"$type":"Vanrise.Analytic.Entities.AnalyticTableKPISettings, Vanrise.Analytic.Entities","AnalyticTableId":"90f86f51-ff24-4672-b875-e6d21d307ce7"}]}}'

--[common].[Setting]-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FC50368F-68DF-4EF4-9367-895EFA393D73','KPI Settings','VR_Analytic_KPISettings','General','{"Editor":"vr-analytic-kpi-settings-editor"}',null,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);



	--[common].[StatusDefinition]------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[BusinessEntityDefinitionID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
--('0C43E0B1-C9D7-45B2-9567-58FAC15AD724','Blue','A79ADBC8-0FF0-4857-BACA-A2C0D9FB22BF','{"$type":"Vanrise.Entities.StatusDefinitionSettings, Vanrise.Entities","StyleDefinitionId":"1e644b07-528a-47b5-a40a-a9e8a0fc868a","HasInitialCharge":false,"HasRecurringCharge":false,"IsActive":false,"IsInvoiceActive":false,"IsAccountBalanceActive":false}'),
('5349B731-1110-4AB6-8BCD-7C7EB8CAE394','Good','A79ADBC8-0FF0-4857-BACA-A2C0D9FB22BF','{"$type":"Vanrise.Entities.StatusDefinitionSettings, Vanrise.Entities","StyleDefinitionId":"fac30bbc-68b1-4e8e-b5de-93015285c012","HasInitialCharge":false,"HasRecurringCharge":false,"IsActive":false,"IsInvoiceActive":false,"IsAccountBalanceActive":false}'),
--('C9C8495A-C20E-43EE-827D-6717112EEF06','Green Text','A79ADBC8-0FF0-4857-BACA-A2C0D9FB22BF','{"$type":"Vanrise.Entities.StatusDefinitionSettings, Vanrise.Entities","StyleDefinitionId":"2cb88d75-7eb6-4e6d-86fc-625eeca8e52b","HasInitialCharge":false,"HasRecurringCharge":false,"IsActive":false,"IsInvoiceActive":false,"IsAccountBalanceActive":false}'),
('5F12E34D-88F0-40BC-B332-EF9B3A4AEA03','Bad','A79ADBC8-0FF0-4857-BACA-A2C0D9FB22BF','{"$type":"Vanrise.Entities.StatusDefinitionSettings, Vanrise.Entities","StyleDefinitionId":"61a682f3-e00c-4b31-b2f5-26dd5f5e4c2f","HasInitialCharge":false,"HasRecurringCharge":false,"IsActive":false,"IsInvoiceActive":false,"IsAccountBalanceActive":false}'),
--('14256EBF-FCF4-49FB-9516-C7405550AD8D','Red Text','A79ADBC8-0FF0-4857-BACA-A2C0D9FB22BF','{"$type":"Vanrise.Entities.StatusDefinitionSettings, Vanrise.Entities","StyleDefinitionId":"907f265f-cf52-4c90-a219-56bcfa18f65b","HasInitialCharge":false,"HasRecurringCharge":false,"IsActive":false,"IsInvoiceActive":false,"IsAccountBalanceActive":false}'),
('54EC06E6-EA1D-43E6-9A98-983C624F1567','Fair','A79ADBC8-0FF0-4857-BACA-A2C0D9FB22BF','{"$type":"Vanrise.Entities.StatusDefinitionSettings, Vanrise.Entities","StyleDefinitionId":"a6f96839-2922-4cee-b0f6-f026f8bd8c11","HasInitialCharge":false,"HasRecurringCharge":false,"IsActive":false,"IsInvoiceActive":false,"IsAccountBalanceActive":false}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[BusinessEntityDefinitionID],[Settings]))
merge	[common].[StatusDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[BusinessEntityDefinitionID] = s.[BusinessEntityDefinitionID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[BusinessEntityDefinitionID],[Settings])
	values(s.[ID],s.[Name],s.[BusinessEntityDefinitionID],s.[Settings]);

