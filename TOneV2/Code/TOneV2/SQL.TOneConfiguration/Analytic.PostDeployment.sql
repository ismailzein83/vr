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
--[sec].[Module]------------------------------1501 to 1600------------------------------------------------------
------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1501,'Analytics Management',null,-100,null,20,0)
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

--[sec].[viewtype]------------------------------201 to 300------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(201,'VR_Analytic','Analytic','{"ViewTypeId":201,"Name":"VR_Analytic","Title":"Analytic","Editor":"/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticReportSettingsEditor.html","EnableAdd":true}'),
(202,'VR_AnalyticReport','Analytic Report','{"ViewTypeId":201,"Name":"VR_Analytic","Title":"Analytic","Editor":"/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticReportViewEditor.html","EnableAdd":true}')	
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Details]))
merge	[sec].[viewtype] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Title],[Details])
	values(s.[ID],s.[Name],s.[Title],s.[Details]);

--[sec].[View]-----------------------------15001 to 16000---------------------------------------------------------
-------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(15001,'Tables','Analytic Table Management','#/view/Analytic/Views/GenericAnalytic/Definition/AnalyticTableManagement',1501,'VR_Analytic/AnalyticTable/GetFilteredAnalyticTables',null,null,null,0,1),
(15002,'Reports','Analytic Report Management','#/view/Analytic/Views/GenericAnalytic/Definition/AnalyticReportManagement',1501,'VR_Analytic/AnalyticReport/GetFilteredAnalyticReports',null,null,null,0,2)
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

--[sec].[BusinessEntityModule]----------1401 to 1500------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1401,'Analytics',-1,0)
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

--[sec].[BusinessEntity]----------------4201 to 4500------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(4201,'VR_Analytic_AnalyticTable','Analytic Table',1401,0,'["View","Add","Edit"]'),
(4202,'VR_Analytic_AnalyticReport','Analytic Report',1401,0,'["View","Add","Edit"]')
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

--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_Analytic/AnalyticTable/GetAnalyticTablesInfo',null),
('VR_Analytic/AnalyticTable/GetFilteredAnalyticTables','VR_Analytic_AnalyticTable: View'),
('VR_Analytic/AnalyticTable/GetTableById',null),
('VR_Analytic/AnalyticTable/UpdateAnalyticTable',null),
('VR_Analytic/AnalyticTable/AddAnalyticTable','VR_Analytic_AnalyticTable: Add'),
('VR_Analytic/AnalyticReport/GetAnalyticReportsInfo',null),
('VR_Analytic/AnalyticReport/GetAnalyticReportById',null),
('VR_Analytic/AnalyticReport/UpdateAnalyticReport','VR_Analytic_AnalyticReport: Edit'),
('VR_Analytic/AnalyticReport/AddAnalyticReport','VR_Analytic_AnalyticReport: Add'),
('VR_Analytic/AnalyticReport/GetFilteredAnalyticReports','VR_Analytic_AnalyticReport: View'),
('VR_Analytic/AnalyticReport/GetAnalyticReportConfigTypes',null)
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

--[common].[ExtensionConfiguration]-------------------1		to 1000---------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings],[CreatedTime])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'VR_Analytic_WidgetSettings_Grid','Grid','Analytic_AnalyticWidgetsSettings','{"Editor":"vr-analytic-widgets-grid-definition","RuntimeEditor":"vr-analytic-datagrid-analyticrecords"}','2016-04-22 12:47:10.867'),
(3,'VR_Analytic_WidgetSettings_Chart','Chart','Analytic_AnalyticWidgetsSettings','{"Editor":"vr-analytic-widgets-chart-definition","RuntimeEditor":"vr-analytic-chart-toprecords"}','2016-04-22 12:47:10.867'),
(4,'VR_Analytic_Report_History','History','Analytic_AnalyticReportConfigType','{"Editor":"vr-analytic-analyticreport-history-definition","RuntimeEditor":"vr-analytic-analyticreport-history-runtime"}','2016-05-06 15:28:25.023'),
(5,'VR_Analytic_Report_RealTime','Real Time','Analytic_AnalyticReportConfigType','{"Editor":"vr-analytic-analyticreport-realtime-definition","RuntimeEditor":"vr-analytic-analyticreport-realtime-runtime"}','2016-05-10 10:12:22.380'),
(6,'VR_Analytic_Report_History_GenericSearch','Generic Search Settings','Analytic_AnalyticHistoryReportSettings','{"Editor":"vr-analytic-reportsearchsettings-genericsearch","RuntimeEditor":""}','2016-05-10 11:02:48.403'),
(7,'VR_Analytic_Report_RealTime_DefaultSearch','Default Real Time Search','Analytic_RealTimeSearchSettings','{"Editor":"vr-analytic-analyticreport-realtime-searchsettings-defaultsearch","RuntimeEditor":""}','2016-05-10 11:03:51.603'),
(8,'VR_Analytic_Report_RealTime_Chart','Real Time Chart','Analytic_RealTimeWidgetSettings','{"Editor":"vr-analytic-realtime-widgets-chart-definition","RuntimeEditor":"vr-analytic-realtime-chart-toprecords"}','2016-05-10 12:34:47.540'),
(9,'VR_Analytic_WidgetSettings_PieChart','Pie Chart','Analytic_AnalyticWidgetsSettings','{"Editor":"vr-analytic-widgets-piechart-definition","RuntimeEditor":"vr-analytic-piechart-toprecords"}','2016-05-10 15:46:09.287'),
(10,'VR_Analytic_Report_RealTime_TimeVariation','Time Variation','Analytic_RealTimeWidgetSettings','{"Editor":"vr-analytic-realtime-widgets-timevariation-definition","RuntimeEditor":"vr-analytic-realtime-chart-timevariation"}','2016-05-12 10:42:04.020'),
(13,'VR_Analytic_Report_RecordSearch','Record Search','Analytic_AnalyticReportConfigType','{"Editor":"vr-analytic-analyticreport-recordsearch-definition","RuntimeEditor":"vr-analytic-analyticreport-recordsearch-runtime"}','2016-05-19 14:39:45.347'),
(14,'VR_Analytic_Report_MeasureStyle_Compare','Compare Condition','Analytic_MeasureStyleRuleTemplates','{"Editor":"vr-analytic-measurestyle-stylerulecondition-compare","RuntimeEditor":""}','2016-05-27 16:52:56.683'),
(15,'VR_Analytic_Report_MeasureStyle_Range','Range Condition','Analytic_MeasureStyleRuleTemplates','{"Editor":"vr-analytic-measurestyle-stylerulecondition-range","RuntimeEditor":""}','2016-05-27 16:53:16.430')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ConfigType],[Settings],[CreatedTime]))
merge	[common].[ExtensionConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[ConfigType],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);