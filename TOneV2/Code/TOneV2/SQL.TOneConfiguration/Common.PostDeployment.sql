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

('VRCommon/LogEntry/GetFilteredLogs','VRCommon_System_Log: View'),

('VRCommon/Settings/GetFilteredSettings','VRCommon_Settings:View'),
('VRCommon/Settings/UpdateSetting','VRCommon_Settings:Edit'),
('VRCommon/Settings/GetSetting',null),
('VRCommon/Settings/GetDistinctSettingCategories',null),
('VRCommon/Settings/GetFilteredTechnicalSettings','VRCommon_Settings_Technical:  View'),('VRCommon/Settings/UpdateTechnicalSetting','VRCommon_Settings_Technical:  Edit'),('VRCommon/Settings/GetTechnicalSetting', null),

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

('VRCommon/TimeZone/GetFilteredVRTimeZones','VRCommon_TimeZone: View'),('VRCommon/TimeZone/GetVRTimeZonesInfo',null),('VRCommon/TimeZone/GetVRTimeZone',null),('VRCommon/TimeZone/AddVRTimeZone','VRCommon_TimeZone: Add'),('VRCommon/TimeZone/UpdateVRTimeZone','VRCommon_TimeZone: Edit'),

('VRCommon/StyleDefinition/GetFilteredStyleDefinitions','VRCommon_StyleDefinition: View'),('VRCommon/StyleDefinition/GetStyleDefinition',null),('VRCommon/StyleDefinition/AddStyleDefinition','VRCommon_StyleDefinition: Add'),('VRCommon/StyleDefinition/UpdateStyleDefinition','VRCommon_StyleDefinition: Edit'),('VRCommon/StyleDefinition/GetStyleFormatingExtensionConfigs',null),('VRCommon/StyleDefinition/GetStyleDefinitionsInfo',null),

('VRCommon/UserActionAudit/GetFilteredUserActionAudits','VRCommon_UserActionAudit: View'),

('VRCommon/VRConnection/GetFilteredVRConnections','VRCommon_VRConnection: View'),('VRCommon/VRConnection/GetVRConnection',null),('VRCommon/VRConnection/AddVRConnection','VRCommon_VRConnection: Add'),('VRCommon/VRConnection/UpdateVRConnection','VRCommon_VRConnection: Edit'),('VRCommon/VRConnection/GetVRConnectionInfos',null),('VRCommon/VRConnection/GetVRConnectionConfigTypes',null)
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
set nocount on;;with cte_data([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('B6B8F582-4759-43FB-9220-AA7662C366EA',null,'System Processes','5A9E78AE-229E-41B9-9DBF-492997B42B61',2,0),--root('954705D8-ABC5-41AB-BDB2-FEC686C7BE09',null,'System Processes','7913ACD9-38C5-43B3-9612-BEFF66606F22',-1,0),--system configuration('9BBD7C00-011D-4AC9-8B25-36D3E2A8F7CF',null,'Rules','5A9E78AE-229E-41B9-9DBF-492997B42B61',2,0),--root('D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',201,'Business Entities','5A9E78AE-229E-41B9-9DBF-492997B42B61',1,0),('8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',202,'Lookups','D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',201,0),('0BA03544-A3D8-4570-8855-5162B42B50AB',203,'System','61451603-E7B9-40C6-AE27-6CBA974E1B3B',2,0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance]))merge	[sec].[BusinessEntityModule] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[ParentId] = s.[ParentId],[OldParentId] = s.[OldParentId],[BreakInheritance] = s.[BreakInheritance]when not matched by target then	insert([ID],[OldId],[Name],[ParentId],[OldParentId],[BreakInheritance])	values(s.[ID],s.[OldId],s.[Name],s.[ParentId],s.[OldParentId],s.[BreakInheritance]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[BusinessEntity]------------------301 to 600----------------------------------------------------------
begin
set nocount on;;with cte_data([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('A91EFECE-00E1-4900-982F-68F01A7185D0',301,'VRCommon_Country','Country'					,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',202,0,'["View", "Add", "Edit", "Download Template", "Upload", "Add City"]'),('9A285D4E-D4A6-4ABA-A5DA-22E7E237E808',305,'VRCommon_City','City'							,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',202,0,'["View","Add","Edit"]'),('92EA996E-C5E9-4937-9157-7CD36EF0DA37',306,'VRCommon_Currency','Currency'					,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',202,0,'["View"]'),('99E37CF7-98B2-48F0-9050-28D473C23D43',307,'VRCommon_CurrencyExchangeRate'					,'Currency Exchange Rate','8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',202,0,'["View"]'),('8BE95D10-688E-40F3-99C1-86397A51AE9B',308,'VRCommon_RateType','Rate Type'					,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',202,0,'["View"]'),('A1DBA375-456A-4769-AD55-CC12C61C721F',350,'VRCommon_TimeZone','Time Zone'					,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',202,0,'["View","Add","Edit"]'),('70B312DC-AA2D-4565-A70C-407FBA3A7D78',302,'VRCommon_System_Log','Logs'								,'0BA03544-A3D8-4570-8855-5162B42B50AB',203,0,'["View"]'),('A4FCC225-8614-43D7-8147-B474502C5D78',304,'VRCommon_Settings','Settings'								,'0BA03544-A3D8-4570-8855-5162B42B50AB',203,0,'["View", "Edit"]'),('363CE71B-A320-47C3-8738-24A6423FC182',null,'VRCommon_Settings_Technical','Settings Technical'			,'0BA03544-A3D8-4570-8855-5162B42B50AB',null,1,'["View","Edit"]'),('62DBAA63-9427-42CD-81DA-BD5E422B0083',330,'VRCommon_VRObjectTypeDefinition','Object Type Definition'	,'4C9719E3-F818-454D-9977-01A9668E7ABA',203,0,'["View","Add","Edit"]'),('8DA97941-F843-479E-A941-83647F57A0A0',331,'VRCommon_VRMailMessageType','Mail Message Type'			,'4C9719E3-F818-454D-9977-01A9668E7ABA',203,0,'["View","Add","Edit"]'),('D884B97B-E179-4ED5-B09B-5EA69F274DA8',332,'VRCommon_VRMailMessageTemplate','Mail Message Template'	,'0BA03544-A3D8-4570-8855-5162B42B50AB',203,0,'["View","Add","Edit"]'),('2E0AFB92-BC85-4159-82B3-663765B161B9',5412,'VRCommon_StyleDefinition','Style Definition'				,'7913ACD9-38C5-43B3-9612-BEFF66606F22',1701,0,'["View","Add","Edit"]'),('9994CE7C-9960-4394-8DC2-072A757B2713',null,'VRCommon_UserActionAudit','User Action Audit'				,'0BA03544-A3D8-4570-8855-5162B42B50AB',null,0,'["View"]'),('BE46E2DC-CE86-46F5-A071-65656E8CCB25',null,'VRCommon_VRConnection','VR Connection','4C9719E3-F818-454D-9977-01A9668E7ABA',null,0,'["View","Add","Edit"]')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions]))merge	[sec].[BusinessEntity] as tusing	cte_data as son		1=1 and t.[Id] = s.[Id]when matched then	update set	[OldId] = s.[OldId],[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[OleModuleId] = s.[OleModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]when not matched by target then	insert([Id],[OldId],[Name],[Title],[ModuleId],[OleModuleId],[BreakInheritance],[PermissionOptions])	values(s.[Id],s.[OldId],s.[Name],s.[Title],s.[ModuleId],s.[OleModuleId],s.[BreakInheritance],s.[PermissionOptions]);
--------------------------------------------------------------------------------------------------------------
end

--[sec].[Permission]--------------------------------------------------------------------------------
begin
set nocount on;;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////(0,'1',1,'363ce71b-a320-47c3-8738-24a6423fc182','[{"Name":"View","Value":1}]')-- give view permission on VRCommon_Settings_Technical to admisitrator--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags]))merge	[sec].[Permission] as tusing	cte_data as son		1=1 and t.[HolderType] = s.[HolderType] and t.[HolderId] = s.[HolderId] and t.[EntityType] = s.[EntityType] and t.[EntityId] = s.[EntityId]--when matched then--	update set--	[PermissionFlags] = s.[PermissionFlags]when not matched by target then	insert([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])	values(s.[HolderType],s.[HolderId],s.[EntityType],s.[EntityId],s.[PermissionFlags]);
----------------------------------------------------------------------------------------------------
end

--[sec].[Module]------------------------------101 to 200------------------------------------------------------
begin
set nocount on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('FC9D12D3-9CBF-4D99-8748-5C2BDD6C5ED9','System Processes'		,null,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,20,0),--'System Configuration'
('1037157D-BBC9-4B28-B53F-908936CEC137','System Processes'		,null,null,'/Client/Images/menu-icons/SystemProcesses.png',7,null),
('E73C4ABA-FD03-4137-B047-F3FB4F7EED03','Business Entities'		,null,null,'/Client/Images/menu-icons/Business Entities.png',10,0),
('1C7569FA-43C9-4853-AE4C-1152746A34FD','Rules'					,null,null,'/Client/Images/menu-icons/other.png',11,0),
('89254E36-5D91-4DB1-970F-9BFEF404679A','Lookups'				,null,'E73C4ABA-FD03-4137-B047-F3FB4F7EED03',null,1,1),
('A459D3D0-35AE-4B0E-B267-54436FDA729A','Entities Definition'	,null,'D018C0CD-F15F-486D-80C3-F9B87C3F47B8',null,65,0)
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
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('604B2CB5-B839-4E51-8D13-3C1C84D05DEE','Countries','Countries','#/view/Common/Views/Country/CountryManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/Country/GetFilteredCountries',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,5),
('25994374-CB99-475B-8047-3CDB7474A083','Cities','Cities','#/view/Common/Views/City/CityManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/City/GetFilteredCities',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,10),
('9F691B87-4936-4C4C-A757-4B3E12F7E1D9','Currencies','Currencies','#/view/Common/Views/Currency/CurrencyManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/Currency/GetFilteredCurrencies',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,15),
('E5CA33D9-18AC-4BA1-8E8E-FB476ECAA9A9','Exchange Rates','Currency Exchange Rates','#/view/Common/Views/CurrencyExchangeRate/CurrencyExchangeRateManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/CurrencyExchangeRate/GetFilteredExchangeRateCurrencies',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,20),
('0F111ADC-B7F6-46A4-81BC-72FFDEB305EB','Time Zone','Time Zone Management','#/view/Common/Views/VRTimeZone/VRTimeZoneManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/TimeZone/GetFilteredVRTimeZones',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,25),
('4D7BF410-E4C6-4D6F-B519-D6B5C2C2F712','Rate Types','Rate Types','#/view/Common/Views/RateType/RateTypeManagement','89254E36-5D91-4DB1-970F-9BFEF404679A','VRCommon/RateType/GetFilteredRateTypes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,30),

('C8272FEA-32E8-4C3B-949A-50090DB82981','Settings','Settings','#/view/Common/Views/Settings/SettingsManagement','BAAF681E-AB1C-4A64-9A35-3F3951398881','VRCommon/Settings/GetFilteredSettings',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,10),

('40A3247C-375A-4FE3-8E5E-8370D086F8FA','Mail Templates','Mail Templates','#/view/Common/Views/VRMail/VRMailMessageTemplateManagement','BAAF681E-AB1C-4A64-9A35-3F3951398881','VRCommon/VRMailMessageTemplate/GetFilteredMailMessageTemplates',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,20),

('66DE2441-8A96-41E7-94EA-9F8AF38A3515','Style','Style Definitions','#/view/Common/Views/StyleDefinition/StyleDefinitionManagement','A459D3D0-35AE-4B0E-B267-54436FDA729A','VRCommon/StyleDefinition/GetFilteredStyleDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,7),

('CFCF02C6-0C70-443D-A91E-B8D291F5263E','Object Type Definitions','Object Type Definitions','#/view/Common/Views/VRObjectTypeDefinition/VRObjectTypeDefinitionManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VRCommon/VRObjectTypeDefinition/GetFilteredVRObjectTypeDefinitions',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,30),
('33AC6A20-F8BE-4D6F-A659-F643DADE1065','Mail Message Types','Mail Message Types','#/view/Common/Views/VRMail/VRMailMessageTypeManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VRCommon/VRMailMessageType/GetFilteredMailMessageTypes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,35),

('A20E826F-FB18-4A40-ADAC-7D257399A1CA','Connections','Connections Management','#/view/Common/Views/VRConnection/VRConnectionManagement','D018C0CD-F15F-486D-80C3-F9B87C3F47B8','VRCommon/VRConnection/GetFilteredVRConnections',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',0,40)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank]))merge	[sec].[View] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[OldType] = s.[OldType],[Rank] = s.[Rank]when not matched by target then	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[OldType],[Rank])	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[OldType],s.[Rank]);
---------------------------------------------------------------------------------------------------------------
end

--[common].[Setting]--------------------1 to 100----------------------------------------------------
begin
set nocount on;;with cte_data([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('81F62AC3-BAE4-4A2F-A60D-A655494625EA',null,'Company Setting','VR_Common_CompanySettings','General','{"Editor":"vr-common-companysettings-editor"}',null,0),('1CB20F2C-A835-4320-AEC7-E034C5A756E9',null,'Bank Details','VR_Common_BankDetailsSettings','General','{"Editor" : "vr-common-bankdetailssettings-editor"}',null,0),('5174E166-3AE1-4ED5-9874-07008504C737',1,'System Mail','VR_Common_Email','General','{"Editor":"vr-common-emailtemplate-settings-editor"}','{"$type":"Vanrise.Entities.EmailSettingData, Vanrise.Entities","SenderEmail":null,"SenderPassword":null,"Host":null,"Port":null,"Timeout":6000,"EnabelSsl":null}',0),('1C833B2D-8C97-4CDD-A1C1-C1B4D9D299DE',2,'System Currency','VR_Common_BaseCurrency','General','{"Editor":"vr-common-currency-settings-editor"}',null,0),('D8EC8190-A3AC-4384-A945-9AF18B933889',null,'General Settings','VR_Common_GeneralSetting','General','{"Editor":"vr-common-general-settings-editor"}','{"$type":"Vanrise.Entities.GeneralSettingData, Vanrise.Entities","UIData":{"$type":"Vanrise.Entities.UISettingData, Vanrise.Entities","NormalPrecision":2,"LongPrecision":4,"GridPageSize":10}}',0)--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))merge	[common].[Setting] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]--when matched then--	update set--	[OldId] = s.[OldId],[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]when not matched by target then	insert([ID],[OldId],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])	values(s.[ID],s.[OldId],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);
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

--common.[extensionconfiguration]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Title],[ConfigType],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('385C968A-415A-49E2-B7EF-189C2A6DD484','VR_Common_VRObjectTypes_ProductInfoPropertyEvaluator','ProductInfo Property','VR_Common_ProductInfo_PropertyEvaluator','{"Editor":"vr-common-productinfopropertyevaluator"}'),('E7BA05B0-4982-4D1D-9CAB-43EF692F4F17','Substring','Substring','VRCommon_TextManipulationActionSettings','{"Editor":"vr-common-textmanipulationsettings-substring"}'),('55CC79FE-4569-4D34-A1D2-49FAA6445979','VR_Common_VRObjectTypes_Text','Text','VR_Common_ObjectType','{"Editor":"vr-common-textobjecttype", "PropertyEvaluatorExtensionType": "VR_Common_TextValue_PropertyEvaluator"}'),('1789A664-94FC-4702-8625-80B28A3E0E54','AddPrefix','Add Prefix','VRCommon_TextManipulationActionSettings','{"Editor":"vr-common-textmanipulationsettings-addprefix"}'),('6E193BD6-4B98-4EA6-AA9B-934C65B59810','ReplaceString','Replace String','VRCommon_TextManipulationActionSettings','{"Editor":"vr-common-textmanipulationsettings-replacestring"}'),('4CD9F093-41D3-42AE-B878-9AFBDB656262','VR_Common_VRObjectTypes_ProductInfoObjectType','Product Information','VR_Common_ObjectType','{"Editor":"vr-common-productinfoobjecttype", "PropertyEvaluatorExtensionType": "VR_Common_ProductInfo_PropertyEvaluator"}'),('BD6BFC0B-92FF-4ECE-8A18-C3AD4B108FA0','VR_Common_VRObjectTypes_TextValuePropertyEvaluator','Text Value','VR_Common_TextValue_PropertyEvaluator','{"Editor":"vr-common-textvaluepropertyevaluator"}'),('C1C2F3B8-9707-4FD1-B871-C4018FD77B04','CSSClass','CSS Class','VRCommon_StyleFormating','{"Editor":"vr-common-styleformating-cssclass"}'),('5B63FFFD-8DD6-4365-9933-D62D1979E16E','TOne V2 Countries','TOne V2 Countries','VRCommon_SourceCountryReader','{"Editor":"vr-common-sourcecountryreader-tonev2"}'),('E1CA5EBB-B41E-46BE-BD3B-DE684EC75885','TOne V1 Countries','TOne V1 Countries','VRCommon_SourceCountryReader','{"Editor":"vr-common-sourcecountryreader-tonev1"}'),('3F26B7E1-33D8-4428-9A3C-986805289C91','Status Definition','Status Definition','VR_GenericData_BusinessEntityDefinitionSettingsConfig','{"Editor":"vr-common-statusdefinitionbe-editor"}'),('6F3FBD7B-275A-4D92-8E06-AD7F7B04C7D6','Generic Business Entity','Generic BE','VR_GenericData_BusinessEntityDefinitionSettingsConfig','{"Editor":"vr-genericdata-genericbusinessentity-editor"}'),('8224B27C-C128-4150-A4E4-5E2034BB3A36','SQL Connection','SQL Connection','VRCommon_ConnectionConfig','{"Editor":"vr-common-sqlconnection-editor"}'),('5CD2AAC3-1C74-401F-8010-8B9B5FD9C011','Inter App','Inter App','VRCommon_ConnectionConfig','{"Editor":"vr-common-interapprestconnection-editor"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Title],[ConfigType],[Settings]))merge	[common].[extensionconfiguration] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Title] = s.[Title],[ConfigType] = s.[ConfigType],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Title],[ConfigType],[Settings])	values(s.[ID],s.[Name],s.[Title],s.[ConfigType],s.[Settings]);----------------------------------------------------------------------------------------------------end--common.[VRObjectTypeDefinition]-------------------------------------------------------------------beginset nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('1C93042E-939B-4022-9F13-43C3718EF644','Text','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.TextObjectType, Vanrise.Common.MainExtensions","ConfigId":"55cc79fe-4569-4d34-a1d2-49faa6445979"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Value":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Value","Description":"Value","PropertyEvaluator":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.TextValuePropertyEvaluator, Vanrise.Common.MainExtensions","ConfigId":"bd6bfc0b-92ff-4ece-8a18-c3ad4b108fa0"}}}}'),('62B9A4DA-0018-4514-BCFD-8268A58F53A2','Product','{"$type":"Vanrise.Entities.VRObjectTypeDefinitionSettings, Vanrise.Entities","ObjectType":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.ProductInfoObjectType, Vanrise.Common.MainExtensions","ConfigId":"4cd9f093-41d3-42ae-b878-9afbdb656262"},"Properties":{"$type":"System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities]], mscorlib","Product Name":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Product Name","Description":"Name of the product","PropertyEvaluator":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.ProductInfoPropertyEvaluator, Vanrise.Common.MainExtensions","ProductField":0,"ConfigId":"385c968a-415a-49e2-b7ef-189c2a6dd484"}},"Version Number":{"$type":"Vanrise.Entities.VRObjectTypePropertyDefinition, Vanrise.Entities","Name":"Version Number","Description":"Version number","PropertyEvaluator":{"$type":"Vanrise.Common.MainExtensions.VRObjectTypes.ProductInfoPropertyEvaluator, Vanrise.Common.MainExtensions","ProductField":1,"ConfigId":"385c968a-415a-49e2-b7ef-189c2a6dd484"}}}}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[VRObjectTypeDefinition] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);----------------------------------------------------------------------------------------------------end--[runtime].[SchedulerTaskActionType]-------------------------------------------------------------------------------------------------------------------------------------------------------------------set nocount on;;with cte_data([ID],[OldID],[Name],[ActionTypeInfo])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////('0A15BC35-A3A7-4ED3-B09B-1B41A7A9DDC9',401,'Exchange Rate','{"URL":"","SystemType":false,"Editor":"vr-common-exchangerate-fxsauder","FQTN":"Vanrise.Common.MainExtensions.ExchangeRateTaskAction, Vanrise.Common.MainExtensions"}')--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[OldID],[Name],[ActionTypeInfo]))merge	[runtime].[SchedulerTaskActionType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[OldID] = s.[OldID],[Name] = s.[Name],[ActionTypeInfo] = s.[ActionTypeInfo]when not matched by target then	insert([ID],[OldID],[Name],[ActionTypeInfo])	values(s.[ID],s.[OldID],s.[Name],s.[ActionTypeInfo]);