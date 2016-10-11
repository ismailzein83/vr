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
begin
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('e73c4aba-fd03-4137-b047-f3fb4f7eed03','CDR Tools',null,null,'/images/menu-icons/CDR Compare Tool.png',4,0),
('e73c4aba-fd03-4137-b047-f3fb4f7eed03','Supplier Rate Management',null,null,'/images/menu-icons/Supplier Rate Managment.png',5,0)
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
--------------------------------------------------------------------------------------------------------------
end

--[sec].[View]-----------------------------14001 to 15000-----------------------------------------------------
begin
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('9a6e34b4-138c-407c-90c1-40bff59b767c','CDR Comparison','CDR Comparison','#/view/CDRComparison/Views/CDRComparison','a6c324ae-e0dd-4ddc-bb61-10b560f5c3b5','CDRComparison/CDRSourceConfig/GetCDRSourceConfigs & CDRComparison/CDRComparison/GetCDRSourceTemplateConfigs & CDRComparison/CDRComparison/GetFileReaderTemplateConfigs & CDRComparison/CDRSource/ReadSample & CDRComparison/FileCDRSource/GetMaxUncompressedFileSizeInMegaBytes',null,null,null,0,1),
('de48546c-84c0-45b1-9ba2-cffd67b2cc57','Input Template Conversion','Input Template Conversion','#/view/XBooster_PriceListConversion/Views/PriceListConversion','bb288557-717b-476d-bfc0-1c87d5af9d93','XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListConversion/ConvertAndDownloadPriceList',null,null,null,0,1),
('0a1c68fa-b4e3-492b-9f32-b9fb4bd6926f','Output Template','Output Template','#/view/XBooster_PriceListConversion/Views/PriceListTemplateManagement','bb288557-717b-476d-bfc0-1c87d5af9d93','XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetFilteredInputPriceListTemplates',null,null,null,0,2)

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
--------------------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]-------------------3901 to 4200-------------------------------------------------------
begin
set nocount on;;with cte_data([ID],[OldId],[Name],[Title],[OldModuleId],[ModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('40A08CDE-12B5-43CD-A905-6FD56B8A17CD',3901,'CDRComparison_CompareCDRs','Compare CDRs',1,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0,'["View"]'),('D58CDA0E-F25F-472A-A527-8C7B03F17B6F',3903,'BusinessProcess_BP_CDR_Comparison','CDR Comparison',602,'04493174-83F0-44D6-BBE4-DBEB8B57875A',0,'["View", "StartInstance", "ScheduleTask"]'),('4FFE2153-3813-4239-9332-B6F82C52F315',3902,'XBooster_PriceListConversion','Pricelist Conversion',1,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0,'["View","Add","Edit","Convert"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[Title],[OldModuleId],[ModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[OldModuleId] = s.[OldModuleId],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([ID],[OldId],[Name],[Title],[OldModuleId],[ModuleId],[BreakInheritance],[PermissionOptions])	values(s.[ID],s.[OldId],s.[Name],s.[Title],s.[OldModuleId],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[SystemAction]------------------------------------------------------------------------------
begin
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
----------------------------------------------------------------------------------------------------
end
--common.ExtensionConfiguration---------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('FF8E5A92-6404-4F69-B62D-1537DAECB184','Delimited File','Delimited File','CDRComparison_FileReader','{"Editor":"cdrcomparison-flatfilereader"}'),('CC187D32-7C50-44B6-8BCB-790B06E5662C','Constant','Constant','XBooster_PriceListConversion_OutputFieldMapping','{"Editor":"xbooster-pricelistconversion-outputfieldvalue-constant"}'),('66268265-AF69-42D7-AAED-9185EA2FE466','Basic','Basic','XBooster_PriceListConversion_OutputPriceListConfiguration','{"Editor":"xbooster-pricelistconversion-outputpricelistconfiguration-basic"}'),('26CBDF89-7D0E-4AE0-BFF4-949EF4CB1B65','Pricelist Field','Pricelist Field','XBooster_PriceListConversion_OutputFieldMapping','{"Editor":"xbooster-pricelistconversion-outputfieldvalue-pricelistfield"}'),('613BF2AA-0F36-44F7-B311-9C34F023B273','Basic','Basic','XBooster_PriceListConversion_InputPriceListConfiguration','{"Editor":"xbooster-pricelistconversion-inputpricelistconfiguration-basic"}'),('536A5A84-3874-4CA2-8C5E-A7071FA9F79D','File','File','CDRComparison_CDRSource','{"Editor":"cdrcomparison-filecdrsource"}'),('61E3F8D9-5E5B-49EF-9AE3-DF77BF1D674F','Excel File','Excel File','CDRComparison_FileReader','{"Editor":"cdrcomparison-excelfilereader"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[ExtensionConfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end
--[bp].[BPTaskType]-------------------------30001 to 40000----------------------------------------------
begin
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
----------------------------------------------------------------------------------------------------
end

--[bp].[BPDefinition]----------------------3001 to 4000------------------------------------------------
begin
set nocount on;set identity_insert [bp].[BPDefinition] on;;with cte_data([ID],[Name],[Title],[FQTN],[Config])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(3001,'CDRComparison.BP.Arguments.CDRComparsionProcessInput','CDR Comparison','CDRComparison.BP.CDRComparisonProcess, CDRComparison.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":3,"ManualExecEditor":"","ScheduledExecEditor":"","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"D58CDA0E-F25F-472A-A527-8C7B03F17B6F","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"D58CDA0E-F25F-472A-A527-8C7B03F17B6F","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["StartInstance"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"D58CDA0E-F25F-472A-A527-8C7B03F17B6F","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["ScheduleTask"]}}]}}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[FQTN],[Config]))merge	[bp].[BPDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]when not matched by target then	insert([ID],[Name],[Title],[FQTN],[Config])	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config]);set identity_insert [bp].[BPDefinition] off;
----------------------------------------------------------------------------------------------------
end

--[common].[Setting]---------------------------401 to 500-------------------------------------------
begin
set nocount on;;with cte_data([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('998B07F2-6FB9-43DD-9C6F-364D13A1859D',401,'Product Info','VR_Common_ProductInfoTechnicalSettings','General','{"Editor" : "vr-common-productinfotechnicalsettings-editor"}','{"$type":"Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities","ProductInfo":{"$type":"Vanrise.Entities.ProductInfo, Vanrise.Entities","ProductName":"XBooster","VersionNumber":"version 0.9"}}',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))merge	[common].[Setting] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldID] = s.[OldID],[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]when not matched by target then	insert([ID],[OldID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])	values(s.[ID],s.[OldID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
----------------------------------------------------------------------------------------------------

end
