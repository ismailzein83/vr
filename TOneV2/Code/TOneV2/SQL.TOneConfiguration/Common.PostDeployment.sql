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
--[sec].[SystemAction]------------------------------------------------------------------------------
begin
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VRCommon/Country/GetFilteredCountries','VRCommon_Country: View'),
('VRCommon/Country/GetCountriesInfo',null),
('VRCommon/Country/GetCountry','VRCommon_Country: View'),
('VRCommon/Country/AddCountry','VRCommon_Country: Add'),
('VRCommon/Country/UpdateCountry','VRCommon_Country: Edit'),
('VRCommon/Country/GetCountrySourceTemplates',null),
('VRCommon/Country/DownloadCountriesTemplate','VRCommon_Country: Add'),
('VRCommon/Country/UploadCountries','VRCommon_Country: Add'),

('VRCommon/City/AddCity','VRCommon_City: Add'),
('VRCommon/City/GetCitiesInfo',null),
('VRCommon/City/GetCity','VRCommon_City: View'),
('VRCommon/City/GetFilteredCities','VRCommon_City: View'),
('VRCommon/City/UpdateCity','VRCommon_City: Edit'),

('VRCommon/LogEntry/GetFilteredLogs','VRCommon_System_Log: View General Logs'),

('VRCommon/Settings/GetFilteredSettings','VRCommon_Settings:View'),
('VRCommon/Settings/UpdateSetting','VRCommon_Settings:Edit'),
('VRCommon/Settings/GetSetting','VRCommon_Settings:View'),
('VRCommon/Settings/GetDistinctSettingCategories','VRCommon_Settings:View'),
('VRCommon/Settings/GetFilteredTechnicalSettings','VR_SystemConfiguration:  View'),('VRCommon/Settings/UpdateTechnicalSetting','VR_SystemConfiguration:  Edit'),('VRCommon/Settings/GetTechnicalSetting', 'VR_SystemConfiguration:  View'),

('VRCommon/RateType/GetFilteredRateTypes','VRCommon_RateType: View'),
('VRCommon/RateType/GetRateType','VRCommon_RateType: View'),
('VRCommon/RateType/AddRateType','VRCommon_RateType: Add'),
('VRCommon/RateType/UpdateRateType','VRCommon_RateType: Edit'),

('VRCommon/Currency/GetFilteredCurrencies','VRCommon_Currency: View'),
('VRCommon/Currency/GetCurrency','VRCommon_Currency: View'),
('VRCommon/Currency/AddCurrency','VRCommon_Currency: Add'),
('VRCommon/Currency/UpdateCurrency','VRCommon_Currency: Edit'),
('VRCommon/CurrencyExchangeRate/GetFilteredExchangeRateCurrencies','VRCommon_Currency: View Exchange Rates'),
('VRCommon/CurrencyExchangeRate/AddCurrencyExchangeRate','VRCommon_Currency: Edit Exchange Rates'),
('VRCommon/CurrencyExchangeRate/UpdateCurrencyExchangeRate','VRCommon_Currency: Edit Exchange Rates'),

('VRCommon/VRObjectTypeDefinition/GetFilteredVRObjectTypeDefinitions','VR_SystemConfiguration: View'),
('VRCommon/VRObjectTypeDefinition/GetVRObjectTypeDefinition',null),
('VRCommon/VRObjectTypeDefinition/AddVRObjectTypeDefinition','VR_SystemConfiguration: Add'),
('VRCommon/VRObjectTypeDefinition/UpdateVRObjectTypeDefinition','VR_SystemConfiguration: Edit'),
('VRCommon/VRObjectTypeDefinition/GetVRObjectTypeDefinitionsInfo',null),
('VRCommon/VRMailMessageType/GetFilteredMailMessageTypes','VR_SystemConfiguration: View'),
('VRCommon/VRMailMessageType/GetMailMessageType',null),
('VRCommon/VRMailMessageType/AddMailMessageType','VR_SystemConfiguration: Add'),
('VRCommon/VRMailMessageType/UpdateMailMessageType','VR_SystemConfiguration: Edit'),
('VRCommon/VRMailMessageType/GetMailMessageTypesInfo',null),
('VRCommon/VRMailMessageTemplate/GetFilteredMailMessageTemplates','VRCommon_VRMailMessageTemplate: View'),
('VRCommon/VRMailMessageTemplate/GetMailMessageTemplate','VRCommon_VRMailMessageTemplate: View'),
('VRCommon/VRMailMessageTemplate/AddMailMessageTemplate','VRCommon_VRMailMessageTemplate: Add'),
('VRCommon/VRMailMessageTemplate/UpdateMailMessageTemplate','VRCommon_VRMailMessageTemplate: Edit'),

('VRCommon/TimeZone/GetFilteredVRTimeZones','VRCommon_TimeZone: View'),('VRCommon/TimeZone/GetVRTimeZonesInfo',null),('VRCommon/TimeZone/GetVRTimeZone','VRCommon_TimeZone: View'),('VRCommon/TimeZone/AddVRTimeZone','VRCommon_TimeZone: Add'),('VRCommon/TimeZone/UpdateVRTimeZone','VRCommon_TimeZone: Edit'),

('VRCommon/StyleDefinition/GetFilteredStyleDefinitions','VR_SystemConfiguration: View'),('VRCommon/StyleDefinition/GetStyleDefinition','VR_SystemConfiguration: View'),('VRCommon/StyleDefinition/AddStyleDefinition','VR_SystemConfiguration: Add'),('VRCommon/StyleDefinition/UpdateStyleDefinition','VR_SystemConfiguration: Edit'),('VRCommon/StyleDefinition/GetStyleFormatingExtensionConfigs',null),('VRCommon/StyleDefinition/GetStyleDefinitionsInfo',null),

('VRCommon/UserActionAudit/GetFilteredUserActionAudits','VRCommon_System_Log: View Action Audit'),

('VRCommon/VRConnection/GetFilteredVRConnections','VRCommon_VRConnection: View'),('VRCommon/VRConnection/GetVRConnection','VRCommon_VRConnection: View'),('VRCommon/VRConnection/AddVRConnection','VRCommon_VRConnection: Add'),('VRCommon/VRConnection/UpdateVRConnection','VRCommon_VRConnection: Edit'),('VRCommon/VRConnection/GetVRConnectionInfos',null),('VRCommon/VRConnection/GetVRConnectionConfigTypes',null),

('VRCommon/StatusDefinition/GetFilteredStatusDefinitions','VR_SystemConfiguration: View'),('VRCommon/StatusDefinition/AddStatusDefinition','VR_SystemConfiguration: Add'),('VRCommon/StatusDefinition/UpdateStatusDefinition','VR_SystemConfiguration: Edit'),('VRCommon/StatusDefinition/GetStatusDefinition','VR_SystemConfiguration: View'),('VRCommon/StatusDefinition/GetStatusDefinitionsInfo',null),

('VRCommon/VRComponentType/GetFilteredVRComponentTypes','VR_SystemConfiguration: View'),('VRCommon/VRComponentType/GetVRComponentType','VR_SystemConfiguration: View'),('VRCommon/VRComponentType/AddVRComponentType','VR_SystemConfiguration: Add'),('VRCommon/VRComponentType/UpdateVRComponentType','VR_SystemConfiguration: Edit'),('VRCommon/VRComponentType/GetVRComponentTypeExtensionConfigs',null),('VRCommon/VRComponentType/GetVRComponentTypeExtensionConfigById',null),

('VRCommon/VRApplicationVisibility/GetFilteredVRApplicationVisibilities','VR_SystemConfiguration: View'),('VRCommon/VRApplicationVisibility/GetVRApplicationVisibilityEditorRuntime',null),('VRCommon/VRApplicationVisibility/AddVRApplicationVisibility','VR_SystemConfiguration: Add'),('VRCommon/VRApplicationVisibility/UpdateVRApplicationVisibility','VR_SystemConfiguration: Edit'),('VRCommon/VRApplicationVisibility/GetVRApplicationVisibiltiesInfo',null),('VRCommon/VRApplicationVisibility/GetVRModuleVisibilityExtensionConfigs',null),('VRCommon/VRApplicationVisibility/GetVRApplicationVisibilitiesInfo',null)

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
set nocount on;;with cte_data([ID],[Name],[ParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('5A9E78AE-229E-41B9-9DBF-492997B42B61','Root',null,0),('7913ACD9-38C5-43B3-9612-BEFF66606F22','Configuration'			,'5A9E78AE-229E-41B9-9DBF-492997B42B61',1),('61451603-E7B9-40C6-AE27-6CBA974E1B3B','Administration'		,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0),('B6B8F582-4759-43FB-9220-AA7662C366EA','System Processes'		,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0),('8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493','Lookups'				,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0),('D9666AEA-9517-4DC5-A3D2-D074B2B99A1C','Business Entities'		,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0),
('520558FA-CF2F-440B-9B58-09C23B6A2E9B','Billing'				,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0),
('16419FE1-ED56-49BA-B609-284A5E21FC07','Traffic'				,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0),('5B13C15E-C118-41DC-A2D4-437C9E93F13B','Security'				,'61451603-E7B9-40C6-AE27-6CBA974E1B3B',0),('0BA03544-A3D8-4570-8855-5162B42B50AB','System'				,'61451603-E7B9-40C6-AE27-6CBA974E1B3B',0),('9BBD7C00-011D-4AC9-8B25-36D3E2A8F7CF','Rules'					,'5A9E78AE-229E-41B9-9DBF-492997B42B61',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[ParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[Name],[ParentId],[BreakInheritance])	values(s.[ID],s.[Name],s.[ParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]------------------301 to 600----------------------------------------------------------
begin
set nocount on;;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('106BCFC5-238D-4EFA-B15F-CB91C2704F7E','VR_SystemConfiguration','System Configuration'		,'7913ACD9-38C5-43B3-9612-BEFF66606F22',0,'["View", "Add", "Edit"]'),('A91EFECE-00E1-4900-982F-68F01A7185D0','VRCommon_Country','Country'						,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',0,'["View", "Add", "Edit"]'),('9A285D4E-D4A6-4ABA-A5DA-22E7E237E808','VRCommon_City','City'								,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',0,'["View","Add","Edit"]'),('92EA996E-C5E9-4937-9157-7CD36EF0DA37','VRCommon_Currency','Currency'						,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',0,'["View","Add","Edit","View Exchange Rates","Edit Exchange Rates"]'),('8BE95D10-688E-40F3-99C1-86397A51AE9B','VRCommon_RateType','Rate Type'						,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',0,'["View","Add","Edit"]'),('A1DBA375-456A-4769-AD55-CC12C61C721F','VRCommon_TimeZone','Time Zone'						,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',0,'["View","Add","Edit"]'),('70B312DC-AA2D-4565-A70C-407FBA3A7D78','VRCommon_System_Log','Logs'						,'0BA03544-A3D8-4570-8855-5162B42B50AB',0,'["View General Logs","View Action Audit","View Technical Logs"]'),('A4FCC225-8614-43D7-8147-B474502C5D78','VRCommon_Settings','Settings'						,'0BA03544-A3D8-4570-8855-5162B42B50AB',0,'["View", "Edit"]'),('D884B97B-E179-4ED5-B09B-5EA69F274DA8','VRCommon_VRMailMessageTemplate','Email Templates'	,'0BA03544-A3D8-4570-8855-5162B42B50AB',0,'["View","Add","Edit"]'),('BE46E2DC-CE86-46F5-A071-65656E8CCB25','VRCommon_VRConnection','Connections'				,'0BA03544-A3D8-4570-8855-5162B42B50AB',0,'["View","Add","Edit"]'),('A611A651-B60B-483D-BC83-1C2B667A120A','TrafficData','Traffic Data'						,'16419FE1-ED56-49BA-B609-284A5E21FC07',0,'["View"]'),('AB794846-853C-4402-A8E4-6F5C3A75F5F2','BillingData','Billing Data'						,'520558FA-CF2F-440B-9B58-09C23B6A2E9B',0,'["View"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[Module]------------------------------101 to 200------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('D018C0CD-F15F-486D-80C3-F9B87C3F47B8','Configuration'			,null,null,'/Client/Images/menu-icons/Administration.png',1,0),

('A28351BA-A5D7-4651-913C-6C9E09B92AC1','System',null				,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,1,0),
('FC9D12D3-9CBF-4D99-8748-5C2BDD6C5ED9','System Processes',null		,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,20,0),
('A459D3D0-35AE-4B0E-B267-54436FDA729A','Entities Definition',null	,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,65,0),

('50624672-CD25-44FD-8580-0E3AC8E34C71','Administration',null,null,'/Client/Images/menu-icons/admin.png',5,0),
('BAAF681E-AB1C-4A64-9A35-3F3951398881','System',null				,'50624672-CD25-44FD-8580-0E3AC8E34C71',null,1,0),
('9B73765C-BDD7-487B-8D32-E386288DB79B','Security',null				,'50624672-CD25-44FD-8580-0E3AC8E34C71',null,5,0),

('1037157D-BBC9-4B28-B53F-908936CEC137','System Processes'		,null,null,'/Client/Images/menu-icons/SystemProcesses.png',10,null),

('E73C4ABA-FD03-4137-B047-F3FB4F7EED03','Business Entities'		,null,null,'/Client/Images/menu-icons/Business Entities.png',15,0),
('89254E36-5D91-4DB1-970F-9BFEF404679A','Lookups'				,null,'E73C4ABA-FD03-4137-B047-F3FB4F7EED03',null,1,1),

('1C7569FA-43C9-4853-AE4C-1152746A34FD','Rules'					,null,null,'/Client/Images/menu-icons/other.png',25,0),
('6471DA6F-E4DD-4B2A-BFB6-F8EA498CD37C','Billing'				,null,null,'/images/menu-icons/billing.png',50,0),

('EB303A61-929A-4D33-BF50-18F40308BC86','Reports & Dashboards'	,null,null,'/images/menu-icons/busines intel.png',75,1)
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
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('604B2CB5-B839-4E51-8D13-3C1C84D05DEE','Countries','Countries','#/view/Common/Views/Country/CountryManagement'																,'89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/Country/GetFilteredCountries',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5),
('25994374-CB99-475B-8047-3CDB7474A083','Cities','Cities','#/view/Common/Views/City/CityManagement'																			,'89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/City/GetFilteredCities',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',10),
('9F691B87-4936-4C4C-A757-4B3E12F7E1D9','Currencies','Currencies','#/view/Common/Views/Currency/CurrencyManagement'															,'89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/Currency/GetFilteredCurrencies',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',15),
('E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9','Exchange Rates','Currency Exchange Rates','#/view/Common/Views/CurrencyExchangeRate/CurrencyExchangeRateManagement'				,'89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/CurrencyExchangeRate/GetFilteredExchangeRateCurrencies',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',20),
('0F111ADC-B7F6-46A4-81BC-72FFDEB305EB','Time Zone','Time Zone Management','#/view/Common/Views/VRTimeZone/VRTimeZoneManagement'											,'89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/TimeZone/GetFilteredVRTimeZones',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',25),
('4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712','Rate Types','Rate Types','#/view/Common/Views/RateType/RateTypeManagement'															,'89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/RateType/GetFilteredRateTypes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',30),
('66DE2441-8A96-41E7-94EA-9F8AF38A3515','Style','Style Definitions','#/view/Common/Views/StyleDefinition/StyleDefinitionManagement'											,'A459D3D0-35AE-4B0E-B267-54436FDA729A','VRCommon/StyleDefinition/GetFilteredStyleDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',7),

('C8272FEA-32E8-4C3B-949A-50090DB82981','Settings','Settings','#/view/Common/Views/Settings/SettingsManagement'																,'BAAF681E-AB1C-4A64-9A35-3F3951398881','VRCommon/Settings/GetFilteredSettings',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',10),
('A20E826F-FB18-4A40-ADAC-7D257399A1CA','Connections','Connections Management','#/view/Common/Views/VRConnection/VRConnectionManagement'									,'baaf681e-ab1c-4a64-9a35-3f3951398881','VRCommon/VRConnection/GetFilteredVRConnections',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',40),
('40A3247C-375A-4FE3-8E5E-8370D086F8FA','Mail Templates','Mail Templates','#/view/Common/Views/VRMail/VRMailMessageTemplateManagement'										,'BAAF681E-AB1C-4A64-9A35-3F3951398881','VRCommon/VRMailMessageTemplate/GetFilteredMailMessageTemplates',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',20),

('CFCF02C6-0C70-443D-A91E-B8D291F5263E','Object Type Definitions','Object Type Definitions','#/view/Common/Views/VRObjectTypeDefinition/VRObjectTypeDefinitionManagement'	,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VRCommon/VRObjectTypeDefinition/GetFilteredVRObjectTypeDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',30),
('33AC6A20-F8BE-4D6F-A659-F643DADE1065','Mail Message Types','Mail Message Types','#/view/Common/Views/VRMail/VRMailMessageTypeManagement'									,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VRCommon/VRMailMessageType/GetFilteredMailMessageTypes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',35),
('8AC4B99E-01A0-41D1-AE54-09E679309086','Status Definitions','Status Definitions','#/view/Common/Views/StatusDefinition/StatusDefinitionManagement'							,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VRCommon/StatusDefinition/GetFilteredStatusDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',14),
('52C580DE-C91F-45E2-8E3A-46E0BA9E7EFD','Component Types','Component Types','#/view/Common/Views/VRComponentType/VRComponentTypeManagement'									,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VRCommon/VRComponentType/GetFilteredVRComponentTypes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',4),
('2CF7E0BE-1396-4305-AA27-11070ACFC18F','Application Visibilities','Application Visibilities','#/view/Common/Views/VRApplicationVisibility/VRApplicationVisibilityManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VRCommon/VRApplicationVisibility/GetFilteredVRApplicationVisibilities',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',26)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);
---------------------------------------------------------------------------------------------------------------
end

--[common].[Setting]--------------------1 to 100----------------------------------------------------
begin
set nocount on;;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('81F62AC3-BAE4-4A2F-A60D-A655494625EA','Company','VR_Common_CompanySettings','General','{"Editor":"vr-common-companysettings-editor"}',null,0),('1CB20F2C-A835-4320-AEC7-E034C5A756E9','Bank Details','VR_Common_BankDetailsSettings','General','{"Editor" : "vr-common-bankdetailssettings-editor"}',null,0),('5174E166-3AE1-4ED5-9874-07008504C737','System Mail','VR_Common_Email','General','{"Editor":"vr-common-emailtemplate-settings-editor"}','{"$type":"Vanrise.Entities.EmailSettingData, Vanrise.Entities","SenderEmail":null,"SenderPassword":null,"Host":null,"Port":null,"Timeout":6000,"EnabelSsl":null}',0),('1C833B2D-8C97-4CDD-A1C1-C1B4D9D299DE','System Currency','VR_Common_BaseCurrency','General','{"Editor":"vr-common-currency-settings-editor"}',null,0),('D8EC8190-A3AC-4384-A945-9AF18B933889','General','VR_Common_GeneralSetting','General','{"Editor":"vr-common-general-settings-editor"}','{"$type":"Vanrise.Entities.GeneralSettingData, Vanrise.Entities","UIData":{"$type":"Vanrise.Entities.UISettingData, Vanrise.Entities","NormalPrecision":2,"LongPrecision":4,"GridPageSize":10},"CacheData":{"$type":"Vanrise.Entities.CacheSettingData, Vanrise.Entities","ClientCacheNumber":1500}}',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))merge	[common].[Setting] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]when not matched by target then	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
----------------------------------------------------------------------------------------------------
end
--Update ClientCacheNumber with each version
UPDATE  [common].[Setting] 
SET		[Data]=Replace([Data], SUBSTRING ( [Data] ,CHARINDEX('ClientCacheNumber',[Data]) , len([Data]) -  CHARINDEX('ClientCacheNumber',[Data])-1), 'ClientCacheNumber":'+CAST(cast(SUBSTRING ( [Data] ,CHARINDEX('ClientCacheNumber',[Data])+19 , len([Data])-CHARINDEX('ClientCacheNumber',[Data])-20) as int)+1 AS varchar(100)))
WHERE	[ID]='D8EC8190-A3AC-4384-A945-9AF18B933889' and CHARINDEX('ClientCacheNumber', [Data]) > 0

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

--common.[extensionconfiguration]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('385C968A-415A-49E2-B7EF-189C2A6DD484','VR_Common_VRObjectTypes_ProductInfoPropertyEvaluator','ProductInfo Property','VR_Common_ProductInfo_PropertyEvaluator'		,'{"Editor":"vr-common-productinfopropertyevaluator"}'),('E7BA05B0-4982-4D1D-9CAB-43EF692F4F17','Substring','Substring','VRCommon_TextManipulationActionSettings'															,'{"Editor":"vr-common-textmanipulationsettings-substring"}'),('55CC79FE-4569-4D34-A1D2-49FAA6445979','VR_Common_VRObjectTypes_Text','Text','VR_Common_ObjectType'																,'{"Editor":"vr-common-textobjecttype", "PropertyEvaluatorExtensionType": "VR_Common_TextValue_PropertyEvaluator"}'),('1789A664-94FC-4702-8625-80B28A3E0E54','AddPrefix','Add Prefix','VRCommon_TextManipulationActionSettings'															,'{"Editor":"vr-common-textmanipulationsettings-addprefix"}'),('6E193BD6-4B98-4EA6-AA9B-934C65B59810','ReplaceString','Replace String','VRCommon_TextManipulationActionSettings'													,'{"Editor":"vr-common-textmanipulationsettings-replacestring"}'),('4CD9F093-41D3-42AE-B878-9AFBDB656262','VR_Common_VRObjectTypes_ProductInfoObjectType','Product Information','VR_Common_ObjectType'								,'{"Editor":"vr-common-productinfoobjecttype", "PropertyEvaluatorExtensionType": "VR_Common_ProductInfo_PropertyEvaluator"}'),('BD6BFC0B-92FF-4ECE-8A18-C3AD4B108FA0','VR_Common_VRObjectTypes_TextValuePropertyEvaluator','Text Value','VR_Common_TextValue_PropertyEvaluator'					,'{"Editor":"vr-common-textvaluepropertyevaluator"}'),('C1C2F3B8-9707-4FD1-B871-C4018FD77B04','CSSClass','CSS Class','VRCommon_StyleFormating'																			,'{"Editor":"vr-common-styleformating-cssclass"}'),('5B63FFFD-8DD6-4365-9933-D62D1979E16E','TOne V2 Countries','TOne V2 Countries','VRCommon_SourceCountryReader'														,'{"Editor":"vr-common-sourcecountryreader-tonev2"}'),('E1CA5EBB-B41E-46BE-BD3B-DE684EC75885','TOne V1 Countries','TOne V1 Countries','VRCommon_SourceCountryReader'														,'{"Editor":"vr-common-sourcecountryreader-tonev1"}'),('3F26B7E1-33D8-4428-9A3C-986805289C91','Status Definition','Status Definition','VR_GenericData_BusinessEntityDefinitionSettingsConfig'								,'{"Editor":"vr-common-statusdefinitionbe-editor"}'),('8224B27C-C128-4150-A4E4-5E2034BB3A36','SQL Connection','SQL Connection','VRCommon_ConnectionConfig'																,'{"Editor":"vr-common-sqlconnection-editor"}'),('5CD2AAC3-1C74-401F-8010-8B9B5FD9C011','Inter App','Inter App','VRCommon_ConnectionConfig'																			,'{"Editor":"vr-common-interapprestconnection-editor"}'),('4829119D-F86F-4A6C-A6C0-CDB3FC8274C1','VR Rest API','VR Rest API','VR_GenericData_DataStoreConfig'																,'{"Editor":"vr-genericdata-datastoresetting-restapi", "DataRecordSettingsEditor": "vr-genericdata-datarecordstoragesettings-restapi"}'),
('B4F22FFB-B663-4F5F-AF53-ACBEF7224DFB','VR Rest API Business Entity','VR Rest API Business Entity','VR_GenericData_BusinessEntityDefinitionSettingsConfig'			,'{"Editor":"vr-genericdata-restapibedefinitions-editor"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[extensionconfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end--common.[VRObjectTypeDefinition]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('1C93042E-939B-4022-9F13-43C3718EF644','Text','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.TextObjectType, Vanrise.Common.MainExtensions","ConfigId":"55cc79fe-4569-4d34-a1d2-49faa6445979"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Value":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Value","Description":"Value","PropertyEvaluator":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.TextValuePropertyEvaluator, Vanrise.Common.MainExtensions","ConfigId":"bd6bfc0b-92ff-4ece-8a18-c3ad4b108fa0"}}}}'),('62B9A4DA-0018-4514-BCFD-8268A58F53A2','Product','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.ProductInfoObjectType, Vanrise.Common.MainExtensions","ConfigId":"4cd9f093-41d3-42ae-b878-9afbdb656262"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Product Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Product Name","Description":"Name of the product","PropertyEvaluator":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.ProductInfoPropertyEvaluator, Vanrise.Common.MainExtensions","ProductField":0,"ConfigId":"385c968a-415a-49e2-b7ef-189c2a6dd484"}},"Version Number":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Version Number","Description":"Version number","PropertyEvaluator":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.ProductInfoPropertyEvaluator, Vanrise.Common.MainExtensions","ProductField":1,"ConfigId":"385c968a-415a-49e2-b7ef-189c2a6dd484"}}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[VRObjectTypeDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);----------------------------------------------------------------------------------------------------end--[runtime].[SchedulerTaskActionType]---------------------------------------------------------------BEGINset nocount on;;with cte_data([ID],[Name],[ActionTypeInfo])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('0A15BC35-A3A7-4ED3-B09B-1B41A7A9DDC9','Exchange Rate','{"URL":"","SystemType":false,"Editor":"vr-common-exchangerate-fxsauder","FQTN":"Vanrise.Common.MainExtensions.ExchangeRateTaskAction, Vanrise.Common.MainExtensions","Security":{"$type":"Vanrise.Runtime.Entities.ActionTypeInfoSecurity, Vanrise.Runtime.Entities","ViewPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["View"]}]},"ConfigurePermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Configure"]}]},"RunPermission":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"361B017B-9344-4292-9E48-D9AD056D5528","PermissionOptions":["Run"]}]}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[ActionTypeInfo]))merge	[runtime].[SchedulerTaskActionType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[ActionTypeInfo] = s.[ActionTypeInfo]when not matched by target then	insert([ID],[Name],[ActionTypeInfo])	values(s.[ID],s.[Name],s.[ActionTypeInfo]);----------------------------------------------------------------------------------------------------END--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
begin
set nocount on;;with cte_data([ID],[Name],[Title],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('DF5CDC08-DDF1-4D4E-B1F6-D17B3833452F','VR_Common_Country','Country'			,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-common-country-selector","ManagerFQTN":"Vanrise.Common.Business.CountryManager,Vanrise.Common.Business", "IdType": "System.Int32"}'),('D41EA344-C3C0-4203-8583-019B6B3EDB76','VR_Common_Currency','Currency'			,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-common-currency-selector","GroupSelectorUIControl":"","ManagerFQTN":"Vanrise.Common.Business.CurrencyManager,Vanrise.Common.Business","IdType":"System.Int32"}'),('C4140358-D1C4-4630-B688-25FE027E448C','VR_Common_RateType','Rate Type'			,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-common-ratetype-selector","GroupSelectorUIControl":"","ManagerFQTN":"Vanrise.Common.Business.RateTypeManager,Vanrise.Common.Business","IdType":"System.Int32","NullDisplayText":"Normal"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[Settings]))merge	[genericdata].[BusinessEntityDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[Settings]);
----------------------------------------------------------------------------------------------------
end--[logging].[LoggableEntity]------------------------------------------------------------------------BEGINset nocount on;;with cte_data([ID],[UniqueName],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('99E91333-693F-462B-A061-B64D2FF46A9A','VR_Common_Setting','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_Setting_ViewHistoryItem"}'),('701D12CF-C808-4565-B8BE-EE3D3BC4D952','VR_Common_RateType','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_RateType_ViewHistoryItem"}'),('2A74AFB2-D011-41A6-91D6-3BEF91B11ED4','VR_Common_Country','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_Country_ViewHistoryItem"}'),('E2073DEE-FCAD-4928-9564-424C60E4DC59','VR_Common_Currency','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_Currency_ViewHistoryItem"}'),('0F00B91E-AB1C-47F6-B3A9-B1EB8A358184','VR_Common_City','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_City_ViewHistoryItem"}'),('F8F31C52-523A-4348-924A-1BFB5ACD1D8C','VR_Common_VRMailMessageTemplate','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_VRMailMessageTemplate_ViewHistoryItem"}'),('355900AF-C4F0-4D52-AE4C-D04EC1190D7E','VR_Common_MailMessageType','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_MailMessageType_ViewHistoryItem"}'),('8D06C0F1-8706-4F78-A0A1-E0C38E73B891','VR_Common_ObjectTypeDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_ObjectTypeDefinition_ViewHistoryItem"}'),('15A6CABA-4E0F-40E1-94BC-66B0E175611C','VR_Common_StatusDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_StatusDefinition_ViewHistoryItem"}'),('32AF3865-AC51-48A0-A94A-89D62D66F062','VR_Common_StyleDefinition','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_StyleDefinition_ViewHistoryItem"}'),('02628AEA-F6D9-4F64-8714-81ED3C3DC1E5','VR_Common_TimeZone','{"$type":"Vanrise.Entities.VRLoggableEntitySettings, Vanrise.Entities","ViewHistoryItemClientActionName":"VR_Common_TimeZone_ViewHistoryItem"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[UniqueName],[Settings]))merge	[logging].[LoggableEntity] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]when not matched by target then	insert([ID],[UniqueName],[Settings])	values(s.[ID],s.[UniqueName],s.[Settings]);----------------------------------------------------------------------------------------------------END