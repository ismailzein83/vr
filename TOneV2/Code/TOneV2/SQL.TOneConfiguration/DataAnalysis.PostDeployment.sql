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
--[common].[ExtensionConfiguration]---------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('B3AF681B-72CE-4DD8-9090-CC727690F7E0','VR_Analytic_DataAnalysisDefinition_DAProfCalcSettings','Profiling and Calculation Settings','Analytic_DataAnalysisDefinitionSettings'					,'{"Editor":"vr-analytic-daprofcalc-settings"}'),('93F44A29-235D-4C3F-900E-6D7FE780CEF3','VR_Analytic_DataAnalysisItem_ExtraFields','Data Analysis Item','VR_GenericData_DataRecordTypeExtraField'												,'{"Editor":"vr-analytic-dataanalysisextrafields"}'),
('57033E80-65CB-4359-95F6-22A57084D027','VR_Notification_VRAlertRuleTypeSettings','DAProfCalcAlertRuleTypeSettings','Profiling And Calculation Alert','VR_Notification_VRAlertRuleTypeSettings'	,'{"Editor":"vr-analytic-daprofcalc-alertruletypesettings"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[ExtensionConfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);--[bp].[BPDefinition]--------------------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3C2DA0E1-5F24-4655-BA31-64530114E403','Vanrise.Analytic.BP.Arguments.DAProfCalcGenerateAlertInput','Data Analysis Profiling And Calculation Generate Alert Process','Vanrise.Analytic.BP.DAProfCalcGenerateAlertProcess, Vanrise.Analytic.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"vr-analytic-daprofcalc-generatealertprocessinput","IsPersistable":false,"HasChildProcesses":true,"HasBusinessRules":false,"NotVisibleInManagementScreen":false}'),
('06A5AD8E-79CE-43C9-886D-6AABFC03C1D4','Vanrise.Analytic.BP.Arguments.DAProfCalcForRangeProcessInput','Data Analysis Profiling And Calculation By Range','Vanrise.Analytic.BP.DAProfCalcForRangeProcess, Vanrise.Analytic.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":true}')
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