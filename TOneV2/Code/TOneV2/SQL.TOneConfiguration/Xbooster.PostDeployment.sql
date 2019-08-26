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
--[sec].[Module]---------------------------1401 to 1500-------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A6C324AE-E0DD-4DDC-BB61-10B560F5C3B5','CDR Tools',null,null,'/images/menu-icons/CDR Compare Tool.png',40,0),
('BB288557-717B-476D-BFC0-1C87D5AF9D93','Supplier Rate',null,null,'/images/menu-icons/Supplier Rate Managment.png',45,1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[ID],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);
--------------------------------------------------------------------------------------------------------------
end

GO--delete useless views from Xbooster product such 'My Scheduler Service', 'Style Definitions', 'Organizational Charts', Lookups, etc...
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
--[sec].[View]-----------------------------14001 to 15000-----------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
-- set audience name to admin groups
('1D10975D-5B1D-4DDC-BBF2-F8F9BA89FF5A','Management','Business Processes','#/view/BusinessProcess/Views/BPDefinition/BPDefinitionManagement','B7D68911-9501-48F4-A3ED-8AF7CDBB1A2B',null,'{"Groups":[1]}',null,'{"$type":"Vanrise.BusinessProcess.Entities.BPViewSettings, Vanrise.BusinessProcess.Entities"}','372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2),
('9A6E34B4-138C-407C-90C1-40BFF59B767C','CDR Comparison','CDR Comparison','#/view/CDRComparison/Views/CDRComparison','A6C324AE-E0DD-4DDC-BB61-10B560F5C3B5','CDRComparison/CDRSourceConfig/GetCDRSourceConfigs & CDRComparison/CDRComparison/GetCDRSourceTemplateConfigs & CDRComparison/CDRComparison/GetFileReaderTemplateConfigs & CDRComparison/CDRSource/ReadSample & CDRComparison/FileCDRSource/GetMaxUncompressedFileSizeInMegaBytes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',1),
('0A1C68FA-B4E3-492B-9F32-B9FB4BD6926F','Output Template','Output Template','#/view/XBooster_PriceListConversion/Views/PriceListTemplateManagement','BB288557-717B-476D-BFC0-1C87D5AF9D93','XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetFilteredInputPriceListTemplates',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',3),
('DE48546C-84C0-45B1-9BA2-CFFD67B2CC57','Input Template Conversion','Input Template Conversion','#/view/XBooster_PriceListConversion/Views/PriceListConversion','BB288557-717B-476D-BFC0-1C87D5AF9D93','XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListConversion/ConvertAndDownloadPriceList',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',2)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
--------------------------------------------------------------------------------------------------------------
end

DELETE FROM [sec].[BusinessEntityModule] WHERE [ID] IN ('16419FE1-ED56-49BA-B609-284A5E21FC07',--'Traffic'
														'520558FA-CF2F-440B-9B58-09C23B6A2E9B',--'Billing'
														'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',--'Business Entities'
														'9BBD7C00-011D-4AC9-8B25-36D3E2A8F7CF',--'Rules'
														--'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',--'Lookups'
														'B6B8F582-4759-43FB-9220-AA7662C366EA')--'System Processes'
--[sec].[BusinessEntityModule]------------------------201 to 300----------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('6BA2809F-2CE7-4C14-B336-BD6B6B2DC463','CDR Tools'				,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0),
('BD245ACC-C39B-4A48-91C5-E4F2AFEF68A9','Supplier Rate'			,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0)
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

--[sec].[BusinessEntity]-------------------3901 to 4200-------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('40A08CDE-12B5-43CD-A905-6FD56B8A17CD','CDRComparison_CompareCDRs','Compare CDRs'					,'6BA2809F-2CE7-4C14-B336-BD6B6B2DC463',0,'["View", "Start Process"]'),
('4FFE2153-3813-4239-9332-B6F82C52F315','XBooster_PriceListConversion','Pricelist Conversion'		,'BD245ACC-C39B-4A48-91C5-E4F2AFEF68A9',0,'["View","Add","Edit","Convert"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([ID],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[ID],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
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
('XBooster_PriceListConversion/PriceListTemplate/UpdateOutputPriceListTemplate','XBooster_PriceListConversion: Edit'),
('XBooster_PriceListConversion/PriceListTemplate/AddOutputPriceListTemplate','XBooster_PriceListConversion: Add'),
('XBooster_PriceListConversion/PriceListTemplate/UpdateInputPriceListTemplate','XBooster_PriceListConversion: Edit'),
('XBooster_PriceListConversion/PriceListTemplate/AddInputPriceListTemplate','XBooster_PriceListConversion: Add'),
('XBooster_PriceListConversion/PriceListTemplate/GetFilteredInputPriceListTemplates','XBooster_PriceListConversion: View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetFilteredInputPriceListTemplates','XBooster_PriceListConversion: View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetPriceListTemplate','XBooster_PriceListConversion: View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetOutputPriceListTemplates','XBooster_PriceListConversion: View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetInputPriceListTemplates','XBooster_PriceListConversion: View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetInputPriceListConfigurationTemplateConfigs','XBooster_PriceListConversion: View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListTemplate/GetOutputFieldMappingTemplateConfigs','XBooster_PriceListConversion: View'),
('XBooster_PriceListConversion/PriceListTemplate/XBooster_PriceListConversion/PriceListConversion/ConvertAndDownloadPriceList','XBooster_PriceListConversion: Convert')
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

--common.ExtensionConfiguration---------------------------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FF8E5A92-6404-4F69-B62D-1537DAECB184','Delimited File','Delimited File','CDRComparison_FileReader','{"Editor":"cdrcomparison-flatfilereader"}'),
('CC187D32-7C50-44B6-8BCB-790B06E5662C','Constant','Constant','XBooster_PriceListConversion_OutputFieldMapping','{"Editor":"xbooster-pricelistconversion-outputfieldvalue-constant"}'),
('66268265-AF69-42D7-AAED-9185EA2FE466','Basic','Basic','XBooster_PriceListConversion_OutputPriceListConfiguration','{"Editor":"xbooster-pricelistconversion-outputpricelistconfiguration-basic"}'),
('26CBDF89-7D0E-4AE0-BFF4-949EF4CB1B65','Pricelist Field','Pricelist Field','XBooster_PriceListConversion_OutputFieldMapping','{"Editor":"xbooster-pricelistconversion-outputfieldvalue-pricelistfield"}'),
('613BF2AA-0F36-44F7-B311-9C34F023B273','Basic','Basic','XBooster_PriceListConversion_InputPriceListConfiguration','{"Editor":"xbooster-pricelistconversion-inputpricelistconfiguration-basic"}'),
('536A5A84-3874-4CA2-8C5E-A7071FA9F79D','File','File','CDRComparison_CDRSource','{"Editor":"cdrcomparison-filecdrsource"}'),
('61E3F8D9-5E5B-49EF-9AE3-DF77BF1D674F','Excel File','Excel File','CDRComparison_FileReader','{"Editor":"cdrcomparison-excelfilereader"}')
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

--[bp].[BPTaskType]-------------------------30001 to 40000----------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('29CD0980-4565-4BFF-BD0F-5C829AFF1A01','CDRComparison.BP.Arguments.SettingTaskData','{"$type":"Vanrise.BusinessProcess.Entities.BPTaskTypeSettings, Vanrise.BusinessProcess.Entities","Editor":"/Client/Modules/CDRComparison/Views/CDRComparisonSettingsTask.html", "AutoOpenTask":true}'),
('58BD6835-A209-42FA-B8FB-60CF111BADA8','CDRComparison.BP.Arguments.CDRComparisonConfigTaskData','{"$type":"Vanrise.BusinessProcess.Entities.BPTaskTypeSettings, Vanrise.BusinessProcess.Entities","Editor":"/Client/Modules/CDRComparison/Views/CDRComparisonConfigTask.html", "AutoOpenTask":true}'),
('C408EEA7-E07E-4C37-B3CC-F4B5D6AD1693','CDRComparison.BP.Arguments.ComparisonResultTaskData','{"$type":"Vanrise.BusinessProcess.Entities.BPTaskTypeSettings, Vanrise.BusinessProcess.Entities","Editor":"/Client/Modules/CDRComparison/Views/CDRComparisonResultTask.html", "AutoOpenTask":true}')
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

--[bp].[BPDefinition]----------------------3001 to 4000------------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('AB684180-CDB6-4A38-994D-B79C0FB8B454','CDRComparison.BP.Arguments.CDRComparsionProcessInput','CDR Comparison','CDRComparison.BP.CDRComparisonProcess, CDRComparison.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":3,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"40A08CDE-12B5-43CD-A905-6FD56B8A17CD","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"40A08CDE-12B5-43CD-A905-6FD56B8A17CD","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"40A08CDE-12B5-43CD-A905-6FD56B8A17CD","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}}}')
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

Delete from [common].[Setting] where ID = '4047054E-1CF4-4BE6-A005-6D4706757AD3'--,'Session Lock'

--[common].[Setting]---------------------------401 to 500-------------------------------------------
begin
set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('093D631E-9F79-440C-8B3E-4194918E4768','CDR Comparison','CDRComparison_CDRComparisonSettings','General','{"Editor":"cdrcomparison-cdrcomparisonsettings-editor"}','{"$type":"CDRComparison.Entities.CDRComparisonSettingData, CDRComparison.Entities","TaskTimeoutInSeconds":300}',1)
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
----------------------------------------------------------------------------------------------------
end

GO
Delete from [runtime].[SchedulerTaskActionType] where Id='0A15BC35-A3A7-4ED3-B09B-1B41A7A9DDC9' --Exchange Rate
GO


--[sec].[Group]--------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([PSIdentifier],[Name],[Description],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_CCT','CCT',null,'{"$type":"Vanrise.Security.Business.StaticGroup, Vanrise.Security.Business","ConfigId":"be6619ae-687f-45e3-bd7b-90d1db4626b6"}'),
('VR_SupplierRate','Supplier Rate',null,'{"$type":"Vanrise.Security.Business.StaticGroup, Vanrise.Security.Business","ConfigId":"be6619ae-687f-45e3-bd7b-90d1db4626b6"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([PSIdentifier],[Name],[Description],[Settings]))
merge	[sec].[Group] as t
using	cte_data as s
on		1=1 and t.[PSIdentifier] = s.[PSIdentifier]
--when matched then
--	update set
--	[Name] = s.[Name],[Description] = s.[Description],[Settings] = s.[Settings]
when not matched by target then
	insert([PSIdentifier],[Name],[Description],[Settings])
	values(s.[PSIdentifier],s.[Name],s.[Description],s.[Settings]);
END

DECLARE @VR_CCTGroupId int = (SELECT ID FROM sec.[Group] where [PSIdentifier] = 'VR_CCT')
DECLARE @VR_SupplierRateGroupId int = (SELECT ID FROM sec.[Group] where [PSIdentifier] = 'VR_SupplierRate')

--[sec].[Permission]--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,@VR_CCTGroupId,0,'6ba2809f-2ce7-4c14-b336-bd6b6b2dc463','[{"Name":"Full Control","Value":1}]'),--CCT
(1,@VR_SupplierRateGroupId,0,'bd245acc-c39b-4a48-91c5-e4f2afef68a9','[{"Name":"Full Control","Value":1}]')--supplier rate
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags]))
merge	[sec].[Permission] as t
using	cte_data as s
on		1=1 and t.[HolderType] = s.[HolderType] and t.[HolderId] = s.[HolderId] and t.[EntityType] = s.[EntityType] and t.[EntityId] = s.[EntityId]
--when matched then
--	update set
--	[PermissionFlags] = s.[PermissionFlags]
when not matched by target then
	insert([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
	values(s.[HolderType],s.[HolderId],s.[EntityType],s.[EntityId],s.[PermissionFlags]);

--create user for online website to add users
--[sec].[User]--------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([Name],[Password],[Email],[TenantId],[LastLogin],[Description],[TempPassword],[TempPasswordValidTill],[EnabledTill],[ExtendedSettings],[SecurityProviderId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('API integration','APwg44n1ogeobyUVng+PRyNfhIE=','api@vanrise.com',1,null,'API Account used to connect from online website in order to add new users',null,null,null,null,'9554069B-795E-4BB1-BFF3-9AF0F47FC0FF')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[Password],[Email],[TenantId],[LastLogin],[Description],[TempPassword],[TempPasswordValidTill],[EnabledTill],[ExtendedSettings],[SecurityProviderId]))
merge	[sec].[User] as t
using	cte_data as s
on		1=1 and t.[Email] = s.[Email]
--when matched then
--	update set
--	[Name] = s.[Name],[Password] = s.[Password],[TenantId] = s.[TenantId],[LastLogin] = s.[LastLogin],[Description] = s.[Description],[TempPassword] = s.[TempPassword],[TempPasswordValidTill] = s.[TempPasswordValidTill],[EnabledTill] = s.[EnabledTill],[ExtendedSettings] = s.[ExtendedSettings]
when not matched by target then
	insert([Name],[Password],[Email],[TenantId],[LastLogin],[Description],[TempPassword],[TempPasswordValidTill],[EnabledTill],[ExtendedSettings],[SecurityProviderId])
	values(s.[Name],s.[Password],s.[Email],s.[TenantId],s.[LastLogin],s.[Description],s.[TempPassword],s.[TempPasswordValidTill],s.[EnabledTill],s.[ExtendedSettings],s.[SecurityProviderId]);
----------------------------------------------------------------------------------------------------
END

DECLARE @APIAccountID int = (SELECT ID from [sec].[User] WHERE Email = 'api@vanrise.com')

DELETE FROM [sec].[Permission] WHERE [HolderId]=CONVERT(VARCHAR, @APIAccountID) AND [HolderType]=0
--[sec].[Permission]--------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(0,CONVERT(VARCHAR, @APIAccountID),1,'720069f9-753c-45e3-bf31-ec3c5aa1dd33','[{"Name":"Edit","Value":1}]'),
(0,CONVERT(VARCHAR, @APIAccountID),1,'b4158657-230e-40bf-b88c-f2b2ca8835de','[{"Name":"Add","Value":1},{"Name":"View","Value":1}]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags]))
merge	[sec].[Permission] as t
using	cte_data as s
on		1=1 and t.[HolderType] = s.[HolderType] and t.[HolderId] = s.[HolderId] and t.[EntityType] = s.[EntityType] and t.[EntityId] = s.[EntityId]
when matched then
	update set
	[PermissionFlags] = s.[PermissionFlags]
when not matched by target then
	insert([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
	values(s.[HolderType],s.[HolderId],s.[EntityType],s.[EntityId],s.[PermissionFlags]);
----------------------------------------------------------------------------------------------------	
END


--[runtime].[RuntimeNodeConfiguration]--------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7975581A-433D-4605-A0B8-261BC0D8073D','Default','{"$type":"Vanrise.Runtime.Entities.RuntimeNodeConfigurationSettings, Vanrise.Runtime.Entities","Processes":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities]], mscorlib","895cf5db-c01b-4a5c-bd6c-545baa7a807e":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Regulator Service Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":1,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","d1a060c1-f746-48eb-8793-b6ee8ae37902":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Regulator Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.BusinessProcess.BPRegulatorRuntimeService, Vanrise.BusinessProcess","ServiceTypeUniqueName":"Vanrise.BusinessProcess.BPRegulatorRuntimeService, Vanrise.BusinessProcess, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null","Interval":"00:00:01"}}}}}},"b3ad53ed-420f-48dd-b2c9-231561b8b438":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Services Process","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeProcessConfigurationSettings, Vanrise.Runtime.Entities","IsEnabled":true,"NbOfInstances":3,"Services":{"$type":"System.Collections.Generic.Dictionary`2[[System.Guid, mscorlib],[Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities]], mscorlib","d3a4ab4f-f49a-4f5f-9210-d5645f362651":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfiguration, Vanrise.Runtime.Entities","Name":"Business Process Service","Settings":{"$type":"Vanrise.Runtime.Entities.RuntimeServiceConfigurationSettings, Vanrise.Runtime.Entities","RuntimeService":{"$type":"Vanrise.BusinessProcess.BusinessProcessService, Vanrise.BusinessProcess","ServiceTypeUniqueName":"VR_BusinessProcess_BusinessProcessService","Interval":"00:00:01"}}}}}}}}',null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy]))
merge	[runtime].[RuntimeNodeConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[Settings] = s.[Settings],[CreatedBy] = s.[CreatedBy],[LastModifiedBy] = s.[LastModifiedBy]
when not matched by target then
	insert([ID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
	values(s.[ID],s.[Name],s.[Settings],s.[CreatedBy],s.[LastModifiedBy]);



--[runtime].[RuntimeNode]---------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('BEE2D7C8-7217-4CC7-9AA8-A1FBB103F3C8','7975581A-433D-4605-A0B8-261BC0D8073D','Node 1',null,null,null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy]))
merge	[runtime].[RuntimeNode] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[RuntimeNodeConfigurationID] = s.[RuntimeNodeConfigurationID],[Name] = s.[Name],[Settings] = s.[Settings],[CreatedBy] = s.[CreatedBy],[LastModifiedBy] = s.[LastModifiedBy]
when not matched by target then
	insert([ID],[RuntimeNodeConfigurationID],[Name],[Settings],[CreatedBy],[LastModifiedBy])
	values(s.[ID],s.[RuntimeNodeConfigurationID],s.[Name],s.[Settings],s.[CreatedBy],s.[LastModifiedBy]);


--[bp].[BPDefinition]-------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('92082EE6-B150-46DE-937B-1CF56B5BA453','Vanrise.GenericData.BP.Arguments.CDRCorrelationProcessInput','CDR Correlation','Vanrise.GenericData.BP.CDRCorrelationProcess,Vanrise.GenericData.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"ManualExecEditor":"vr-genericdata-cdrcorrelation-process","ScheduledExecEditor":"vr-genericdata-cdrcorrelation-task","IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":false,"NotVisibleInManagementScreen":false,"ExtendedSettings":{"$type":"Vanrise.GenericData.MainExtensions.CDRCorrelationBPDefinitionSettings, Vanrise.GenericData.MainExtensions"},"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Manage"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"a99a836d-0c03-4946-a0e2-5a758354807b","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Manage"]}}]}}},"BusinessRuleSetSupported":false}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then Delete;
----------------------------------------------------------------------------------------------------
END