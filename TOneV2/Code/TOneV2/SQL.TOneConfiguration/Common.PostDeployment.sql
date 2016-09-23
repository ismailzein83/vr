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
--[sec].[SystemAction]------------------------------------------------------------------------------
begin
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VRCommon/Country/GetFilteredCountries','VRCommon_Country: View'),
('VRCommon/Country/GetCountriesInfo',null),
('VRCommon/Country/GetCountry',null),
('VRCommon/Country/AddCountry','VRCommon_Country: Add'),
('VRCommon/Country/UpdateCountry','VRCommon_Country: Edit'),
('VRCommon/Country/GetCountrySourceTemplates',null),
('VRCommon/Country/DownloadCountriesTemplate','VRCommon_Country: Download Template'),
('VRCommon/Country/UploadCountries','VRCommon_Country: Upload'),

('VRCommon/City/AddCity','VRCommon_City: Add'),
('VRCommon/City/GetCitiesInfo',null),
('VRCommon/City/GetCity',null),
('VRCommon/City/GetFilteredCities','VRCommon_City: View'),
('VRCommon/City/UpdateCity','VRCommon_City: Edit'),

('VRCommon/LogAttribute/GetFilteredLoggers','VRCommon_System_Log: View'),

('VRCommon/EmailTemplate/GetFilteredEmailTemplates','VRCommon_EmailTemplates:View'),
('VRCommon/EmailTemplate/UpdateEmailTemplate','VRCommon_EmailTemplates:Edit'),
('VRCommon/EmailTemplate/GetEmailTemplate',null),

('VRCommon/Settings/GetFilteredSettings','VRCommon_Settings:View'),
('VRCommon/Settings/UpdateSetting','VRCommon_Settings:Edit'),
('VRCommon/Settings/GetSetting',null),
('VRCommon/Settings/GetDistinctSettingCategories',null),

('VRCommon/RateType/GetFilteredRateTypes','VRCommon_RateType: View'),

('VRCommon/Currency/GetFilteredCurrencies','VRCommon_Currency: View'),
('VRCommon/CurrencyExchangeRate/GetFilteredExchangeRateCurrencies','VRCommon_CurrencyExchangeRate: View'),

('VRCommon/VRObjectTypeDefinition/GetFilteredVRObjectTypeDefinitions','VRCommon_VRObjectTypeDefinition: View'),
('VRCommon/VRObjectTypeDefinition/GetVRObjectTypeDefinition',null),
('VRCommon/VRObjectTypeDefinition/AddVRObjectTypeDefinition','VRCommon_VRObjectTypeDefinition: Add'),
('VRCommon/VRObjectTypeDefinition/UpdateVRObjectTypeDefinition','VRCommon_VRObjectTypeDefinition: Edit'),
('VRCommon/VRObjectTypeDefinition/GetVRObjectTypeDefinitionsInfo',null),
('VRCommon/VRMailMessageType/GetFilteredMailMessageTypes','VRCommon_VRMailMessageType: View'),
('VRCommon/VRMailMessageType/GetMailMessageType',null),
('VRCommon/VRMailMessageType/AddMailMessageType','VRCommon_VRMailMessageType: Add'),
('VRCommon/VRMailMessageType/UpdateMailMessageType','VRCommon_VRMailMessageType: Edit'),
('VRCommon/VRMailMessageType/GetMailMessageTypesInfo',null),
('VRCommon/VRMailMessageTemplate/GetFilteredMailMessageTemplates','VRCommon_VRMailMessageTemplate: View'),
('VRCommon/VRMailMessageTemplate/GetMailMessageTemplate',null),
('VRCommon/VRMailMessageTemplate/AddMailMessageTemplate','VRCommon_VRMailMessageTemplate: Add'),
('VRCommon/VRMailMessageTemplate/UpdateMailMessageTemplate','VRCommon_VRMailMessageTemplate: Edit'),

('VRCommon/TimeZone/GetFilteredVRTimeZones','VRCommon_TimeZone: View'),('VRCommon/TimeZone/GetVRTimeZonesInfo',null),('VRCommon/TimeZone/GetVRTimeZone',null),('VRCommon/TimeZone/AddVRTimeZone','VRCommon_TimeZone: Add'),('VRCommon/TimeZone/UpdateVRTimeZone','VRCommon_TimeZone: Edit')
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
----------------------------------------------------------------------------------------------------
end


--[sec].[BusinessEntityModule]------------------------201 to 300----------------------------------------------
begin

set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(201,'Business Entities',1,0),
(202,'Lookups',201,0),
(203,'System',2,0)
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
--------------------------------------------------------------------------------------------------------------
end


--[sec].[BusinessEntity]------------------301 to 600----------------------------------------------------------
begin
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(301,'VRCommon_Country','Country',202,0,'["View", "Add", "Edit", "Download Template", "Upload", "Add City"]'),
(305,'VRCommon_City','City',202,0,'["View","Add","Edit"]'),
(306,'VRCommon_Currency','Currency',202,0,'["View"]'),
(307,'VRCommon_CurrencyExchangeRate','Currency Exchange Rate',202,0,'["View"]'),
(308,'VRCommon_RateType','Rate Type',202,0,'["View"]'),

(302,'VRCommon_System_Log','Logs',203,0,'["View"]'),
(303,'VRCommon_EmailTemplates','Email Templates',203,0,'["View", "Edit"]'),
(304,'VRCommon_Settings','Settings',203,0,'["View", "Edit"]'),
(330,'VRCommon_VRObjectTypeDefinition','Object Type Definition',203,0,'["View","Add","Edit"]'),
(331,'VRCommon_VRMailMessageType','Mail Message Type',203,0,'["View","Add","Edit"]'),
(332,'VRCommon_VRMailMessageTemplate','Mail Message Template',203,0,'["View","Add","Edit"]'),
(350,'VRCommon_TimeZone','Time Zone',202,0,'["View","Add","Edit"]')
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

--------------------------------------------------------------------------------------------------------------
end

--[sec].[Module]------------------------------101 to 200------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('E73C4ABA-FD03-4137-B047-F3FB4F7EED03','Business Entities','Business Entities',null,'/Client/images/menu-icons/Business Entities.png',10,0),
('89254E36-5D91-4DB1-970F-9BFEF404679A','Lookups','Lookups','E73C4ABA-FD03-4137-B047-F3FB4F7EED03',null,1,1),
('A459D3D0-35AE-4B0E-B267-54436FDA729A','Entities Definition',null,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,4,0)
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
--------------------------------------------------------------------------------------------------------------
end


--[sec].[View]-----------------------------1001 to 2000--------------------------------------------------------
begin

set nocount on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('66DE2441-8A96-41E7-94EA-9F8AF38A3515','Style Definitions','Style Definitions','#/view/Common/Views/StyleDefinition/StyleDefinitionManagement','A459D3D0-35AE-4B0E-B267-54436FDA729A','VRCommon/StyleDefinition/GetFilteredStyleDefinitions',null,null,null,0,7),
('7079BD63-BFE2-4519-9B1B-8158A2F3A12A','Event Logs','Event Logs','#/view/Common/Views/MasterLog/MasterLogManagement','BAAF681E-AB1C-4A64-9A35-3F3951398881','VRCommon/LogAttribute/GetFilteredLoggers',null,null,null,0,15),
('604B2CB5-B839-4E51-8D13-3C1C84D05DEE','Countries','Countries','#/view/Common/Views/Country/CountryManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/Country/GetFilteredCountries',null,null,null,0,5),
('25994374-CB99-475B-8047-3CDB7474A083','Cities','Cities','#/view/Common/Views/City/CityManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/City/GetFilteredCities',null,null,null,0,10),
('9F691B87-4936-4C4C-A757-4B3E12F7E1D9','Currencies','Currencies','#/view/Common/Views/Currency/CurrencyManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/Currency/GetFilteredCurrencies',null,null,null,0,15),
('E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9','Currency Exchange Rates','Currency Exchange Rates','#/view/Common/Views/CurrencyExchangeRate/CurrencyExchangeRateManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/CurrencyExchangeRate/GetFilteredExchangeRateCurrencies',null,null,null,0,20),
('4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712','Rate Types','Rate Types','#/view/Common/Views/RateType/RateTypeManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/RateType/GetFilteredRateTypes',null,null,null,0,25),
--(1008,'Email Templates','Email Templates','#/view/Common/Views/EmailTemplate/EmailTemplateManagement',3,'VRCommon/EmailTemplate/GetFilteredEmailTemplates',null,null,null,0,9),
('C8272FEA-32E8-4C3B-949A-50090DB82981','Settings','Settings','#/view/Common/Views/Settings/SettingsManagement','BAAF681E-AB1C-4A64-9A35-3F3951398881','VRCommon/Settings/GetFilteredSettings',null,null,null,0,10),
('3FDB5A9D-F366-4C9E-9316-D2CB90F51C0B','Email Templates','Email Templates','#/view/Common/Views/EmailTemplate/EmailTemplateManagement','BAAF681E-AB1C-4A64-9A35-3F3951398881','VRCommon/EmailTemplate/GetFilteredEmailTemplates',null,null,null,0,5),
('CFCF02C6-0C70-443D-A91E-B8D291F5263E','Object Type Definitions','Object Type Definitions','#/view/Common/Views/VRObjectTypeDefinition/VRObjectTypeDefinitionManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VRCommon/VRObjectTypeDefinition/GetFilteredVRObjectTypeDefinitions',null,null,null,0,55),
('33AC6A20-F8BE-4D6F-A659-F643DADE1065','Mail Message Types','Mail Message Types','#/view/Common/Views/VRMail/VRMailMessageTypeManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VRCommon/VRMailMessageType/GetFilteredMailMessageTypes',null,null,null,0,60),

('40A3247C-375A-4FE3-8E5E-8370D086F8FA','Mail Templates','Mail Templates','#/view/Common/Views/VRMail/VRMailMessageTemplateManagement','BAAF681E-AB1C-4A64-9A35-3F3951398881','VRCommon/VRMailMessageTemplate/GetFilteredMailMessageTemplates',null,null,null,0,9),
('0F111ADC-B7F6-46A4-81BC-72FFDEB305EB','Time Zone','Time Zone Management','#/view/Common/Views/VRTimeZone/VRTimeZoneManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/TimeZone/GetFilteredVRTimeZones',null,null,null,0,9)
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
---------------------------------------------------------------------------------------------------------------
end

--[common].[Setting]--------------------1 to 100----------------------------------------------------
begin
set nocount on;
set identity_insert [common].[Setting] on;
;with cte_data([Id],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'System Mail','VR_Common_Email','General','{"Editor":"vr-common-emailtemplate-settings-editor"}',null,0),
(2,'System Currency','VR_Common_BaseCurrency','General','{"Editor":"vr-common-currency-settings-editor"}',null,0),
(3,'Security','VR_Sec_Settings','General','{"Editor" : "vr-sec-settings-editor"}','{"$type":"Vanrise.Security.Entities.SecuritySettings, Vanrise.Security.Entities","MailMessageTemplateSettings":{"$type":"Vanrise.Security.Entities.MailMessageTemplateSettings, Vanrise.Security.Entities","NewUserId":"d9b56fc2-eb3e-4340-8918-159a281b95bc","ResetPasswordId":"10264fe7-99d5-4f6a-8e8c-44a0702f392e","ForgotPasswordId":"e21cd125-61f0-4091-a03e-200cfe33f6e3"}}',1)
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

----------------------------------------------------------------------------------------------------
end

--[common].[RateType]--------------------------- -1 to -100 ----------------------------------------
begin
set nocount on;
set identity_insert [common].[RateType] on;
;with cte_data([ID],[Name])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(-3,'Weekend'),
(-2,'Off Peak'),
(-1,'Holiday')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name]))
merge	[common].[RateType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name]
when not matched by target then
	insert([ID],[Name])
	values(s.[ID],s.[Name]);
set identity_insert [common].[RateType] off

----------------------------------------------------------------------------------------------------
end

--common.[extensionconfiguration]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('385C968A-415A-49E2-B7EF-189C2A6DD484','VR_Common_VRObjectTypes_ProductInfoPropertyEvaluator','ProductInfo Property','VR_Common_ProductInfo_PropertyEvaluator','{"Editor":"vr-common-productinfopropertyevaluator"}'),('E7BA05B0-4982-4D1D-9CAB-43EF692F4F17','Substring','Substring','VRCommon_TextManipulationActionSettings','{"Editor":"vr-common-textmanipulationsettings-substring"}'),('55CC79FE-4569-4D34-A1D2-49FAA6445979','VR_Common_VRObjectTypes_Text','Text','VR_Common_ObjectType','{"Editor":"vr-common-textobjecttype", "PropertyEvaluatorExtensionType": "VR_Common_TextValue_PropertyEvaluator"}'),('1789A664-94FC-4702-8625-80B28A3E0E54','AddPrefix','Add Prefix','VRCommon_TextManipulationActionSettings','{"Editor":"vr-common-textmanipulationsettings-addprefix"}'),('6E193BD6-4B98-4EA6-AA9B-934C65B59810','ReplaceString','Replace String','VRCommon_TextManipulationActionSettings','{"Editor":"vr-common-textmanipulationsettings-replacestring"}'),('4CD9F093-41D3-42AE-B878-9AFBDB656262','VR_Common_VRObjectTypes_ProductInfoObjectType','Product Information','VR_Common_ObjectType','{"Editor":"vr-common-productinfoobjecttype", "PropertyEvaluatorExtensionType": "VR_Common_ProductInfo_PropertyEvaluator"}'),('BD6BFC0B-92FF-4ECE-8A18-C3AD4B108FA0','VR_Common_VRObjectTypes_TextValuePropertyEvaluator','Text Value','VR_Common_TextValue_PropertyEvaluator','{"Editor":"vr-common-textvaluepropertyevaluator"}'),('C1C2F3B8-9707-4FD1-B871-C4018FD77B04','CSSClass','CSS Class','VRCommon_StyleFormating','{"Editor":"vr-common-styleformating-cssclass"}'),('5B63FFFD-8DD6-4365-9933-D62D1979E16E','TOne V2 Countries','TOne V2 Countries','VRCommon_SourceCountryReader','{"Editor":"vr-common-sourcecountryreader-tonev2"}'),('E1CA5EBB-B41E-46BE-BD3B-DE684EC75885','TOne V1 Countries','TOne V1 Countries','VRCommon_SourceCountryReader','{"Editor":"vr-common-sourcecountryreader-tonev1"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[extensionconfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end--common.[VRObjectTypeDefinition]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('1C93042E-939B-4022-9F13-43C3718EF644','Text','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.TextObjectType, Vanrise.Common.MainExtensions","ConfigId":"55cc79fe-4569-4d34-a1d2-49faa6445979"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Value":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Value","Description":"Value","PropertyEvaluator":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.TextValuePropertyEvaluator, Vanrise.Common.MainExtensions","ConfigId":"bd6bfc0b-92ff-4ece-8a18-c3ad4b108fa0"}}}}'),('62B9A4DA-0018-4514-BCFD-8268A58F53A2','Product','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.ProductInfoObjectType, Vanrise.Common.MainExtensions","ConfigId":"4cd9f093-41d3-42ae-b878-9afbdb656262"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Product Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Product Name","Description":"Name of the product","PropertyEvaluator":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.ProductInfoPropertyEvaluator, Vanrise.Common.MainExtensions","ProductField":0,"ConfigId":"385c968a-415a-49e2-b7ef-189c2a6dd484"}},"Version Number":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Version Number","Description":"Version number","PropertyEvaluator":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.ProductInfoPropertyEvaluator, Vanrise.Common.MainExtensions","ProductField":1,"ConfigId":"385c968a-415a-49e2-b7ef-189c2a6dd484"}}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[VRObjectTypeDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);----------------------------------------------------------------------------------------------------end