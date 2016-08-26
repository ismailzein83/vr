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
--[sec].[Module]---------------------------1401 to 1500-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1401,'CDR Tools',null,null,'/images/menu-icons/CDR Compare Tool.png',4,0),
(1402,'Supplier Rate Management',null,null,'/images/menu-icons/Supplier Rate Managment.png',5,0)
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

--[sec].[View]-----------------------------14001 to 15000-----------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(14001,'CDR Comparison','CDR Comparison','#/view/CDRComparison/Views/CDRComparison',1401,'CDRComparison/CDRSourceConfig/GetCDRSourceConfigs & CDRComparison/CDRComparison/GetCDRSourceTemplateConfigs & CDRComparison/CDRComparison/GetFileReaderTemplateConfigs & CDRComparison/CDRSource/ReadSample & CDRComparison/FileCDRSource/GetMaxUncompressedFileSizeInMegaBytes',null,null,null,0,1),
(14002,'Input Template Conversion','Input Template Conversion','#/view/XBooster_PriceListConversion/Views/PriceListConversion',1402,'XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListConversion/ConvertAndDownloadPriceList',null,null,null,0,1),
(14003,'Output Template','Output Template','#/view/XBooster_PriceListConversion/Views/PriceListTemplateManagement',1402,'XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetFilteredInputPriceListTemplates',null,null,null,0,2)

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

--[sec].[BusinessEntity]-------------------3901 to 4200-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(3901,'CDRComparison_CompareCDRs','Compare CDRs',1,0,'["View"]'),
(3902,'XBooster_PriceListConversion','PriceList Conversion',1,0,'["View","Add","Edit","Convert"]'),
(3903,'BusinessProcess_BP_CDR_Comparison','CDR Comparison',602,0,'["View", "StartInstance", "ScheduleTask"]')
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
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('CDRComparison/CDRSourceConfig/GetCDRSourceConfigs','CDRComparison_CompareCDRs: View'),
('CDRComparison/CDRComparison/GetCDRSourceTemplateConfigs','CDRComparison_CompareCDRs: View'),
('CDRComparison/CDRComparison/GetFileReaderTemplateConfigs','CDRComparison_CompareCDRs: View'),
('CDRComparison/CDRSource/ReadSample','CDRComparison_CompareCDRs: View'),
('CDRComparison/FileCDRSource/GetMaxUncompressedFileSizeInMegaBytes','CDRComparison_CompareCDRs: View'),
('XBooster_PriceListConversion/PriceListTemplate/UpdateOutputPriceListTemplate','XBooster_PriceListConversion:Edit'),
('XBooster_PriceListConversion/PriceListTemplate/AddOutputPriceListTemplate','XBooster_PriceListConversion:Add'),
('XBooster_PriceListConversion/PriceListTemplate/UpdateInputPriceListTemplate','XBooster_PriceListConversion:Edit'),
('XBooster_PriceListConversion/PriceListTemplate/AddInputPriceListTemplate','XBooster_PriceListConversion:Add'),
('XBooster_PriceListConversion/PriceListTemplate/GetFilteredInputPriceListTemplates','XBooster_PriceListConversion:View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetFilteredInputPriceListTemplates','XBooster_PriceListConversion:View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetPriceListTemplate','XBooster_PriceListConversion:View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetOutputPriceListTemplates','XBooster_PriceListConversion:View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetInputPriceListTemplates','XBooster_PriceListConversion:View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetInputPriceListConfigurationTemplateConfigs','XBooster_PriceListConversion:View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetOutputFieldMappingTemplateConfigs','XBooster_PriceListConversion:View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListConversion/ConvertAndDownloadPriceList','XBooster_PriceListConversion:Convert')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);

--[common].[TemplateConfig]----------------------80001 to 90000-------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[TemplateConfig] on;
;with cte_data([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(80001,'File','CDRComparison_CDRSource','cdrcomparison-filecdrsource',null,null),
(80002,'Delimited File','CDRComparison_FileReader','cdrcomparison-flatfilereader',null,null),
(80003,'Excel File','CDRComparison_FileReader','cdrcomparison-excelfilereader',null,null),
(80004,'Basic','XBooster_PriceListConversion_OutputPriceListConfiguration','xbooster-pricelistconversion-outputpricelistconfiguration-basic',null,null),
(80005,'Basic','XBooster_PriceListConversion_InputPriceListConfiguration','xbooster-pricelistconversion-inputpricelistconfiguration-basic',null,null),
(80006,'Pricelist Field','XBooster_PriceListConversion_OutputFieldMapping','xbooster-pricelistconversion-outputfieldvalue-pricelistfield',null,null),
(80007,'Constant','XBooster_PriceListConversion_OutputFieldMapping','xbooster-pricelistconversion-outputfieldvalue-constant',null,null)
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

--[bp].[BPTaskType]-------------------------30001 to 40000----------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [bp].[BPTaskType] on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(30001,'CDRComparison.BP.Arguments.ComparisonResultTaskData','{"$type":"Vanrise.BusinessProcess.Entities.BPTaskTypeSettings, Vanrise.BusinessProcess.Entities","Editor":"/Client/Modules/CDRComparison/Views/CDRComparisonResultTask.html", "AutoOpenTask":true}'),
(30002,'CDRComparison.BP.Arguments.SettingTaskData','{"$type":"Vanrise.BusinessProcess.Entities.BPTaskTypeSettings, Vanrise.BusinessProcess.Entities","Editor":"/Client/Modules/CDRComparison/Views/CDRComparisonSettingsTask.html", "AutoOpenTask":true}'),
(30003,'CDRComparison.BP.Arguments.CDRComparisonConfigTaskData','{"$type":"Vanrise.BusinessProcess.Entities.BPTaskTypeSettings, Vanrise.BusinessProcess.Entities","Editor":"/Client/Modules/CDRComparison/Views/CDRComparisonConfigTask.html", "AutoOpenTask":true}')
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

--[bp].[BPDefinition]----------------------3001 to 4000------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [bp].[BPDefinition] on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(3001,'CDRComparison.BP.Arguments.CDRComparsionProcessInput','CDR Comparison','CDRComparison.BP.CDRComparisonProcess, CDRComparison.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":3,"ManualExecEditor":"","ScheduledExecEditor":"","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":3903,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":3903,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":3903,"PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}')
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
set identity_insert [bp].[BPDefinition] off;