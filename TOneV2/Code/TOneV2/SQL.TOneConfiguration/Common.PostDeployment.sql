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
----------------------------------------------------------------------------------------------------
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

('VRCommon/LogAttribute/GetFilteredLoggers','VRCommon_LogAttribute: View'),

('VRCommon/EmailTemplate/GetFilteredEmailTemplates','VRCommon_EmailTemplates:View'),
('VRCommon/EmailTemplate/UpdateEmailTemplate','VRCommon_EmailTemplates:Edit'),
('VRCommon/EmailTemplate/GetEmailTemplate',null),

('VRCommon/Settings/GetFilteredSettings','VRCommon_Settings:View'),
('VRCommon/Settings/UpdateSetting','VRCommon_Settings:Edit'),
('VRCommon/Settings/GetSetting',null),
('VRCommon/Settings/GetDistinctSettingCategories',null),

('VRCommon/RateType/GetFilteredRateTypes','VRCommon_RateType: View'),

('VRCommon/Currency/GetFilteredCurrencies','VRCommon_Currency: View'),
('VRCommon/CurrencyExchangeRate/GetFilteredExchangeRateCurrencies','VRCommon_CurrencyExchangeRate: View')

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

--[sec].[BusinessEntityModule]------------------------201 to 300----------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(201,'Business Entities',1,0),
(202,'Lookups',201,0)
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

--[sec].[BusinessEntity]------------------301 to 600----------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(301,'VRCommon_Country','Country',202,0,'["View", "Add", "Edit", "Download Template", "Upload", "Add City"]'),
(302,'VRCommon_LogAttribute','Logs',2,0,'["View"]'),
(303,'VRCommon_EmailTemplates','Email Templates',2,0,'["View", "Edit"]'),
(304,'VRCommon_Settings','Settings',2,0,'["View", "Edit"]'),
(305,'VRCommon_City','City',202,0,'["View","Add","Edit"]'),
(306,'VRCommon_Currency','Currency',202,0,'["View"]'),
(307,'VRCommon_CurrencyExchangeRate','Currency Exchange Rate',202,0,'["View"]'),
(308,'VRCommon_RateType','Rate Type',202,0,'["View"]')

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

--[sec].[Module]------------------------------101 to 200------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[Module] on;
;with cte_data([Id],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(101,'Business Entities','Business Entities',null,'/images/menu-icons/Business Entities.png',2,0),
(102,'Lookups','Lookups',101,null,1,0)
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

--[sec].[View]-----------------------------1001 to 2000--------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[View] on;
;with cte_data([Id],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1001,'Countries','Countries','#/view/Common/Views/Country/CountryManagement',102,'VRCommon/Country/GetFilteredCountries',null,null,null,0,5),
(1002,'Cities','Cities','#/view/Common/Views/City/CityManagement',102,'VRCommon/City/GetFilteredCities',null,null,null,0,10),
(1003,'Currencies','Currencies','#/view/Common/Views/Currency/CurrencyManagement',102,'VRCommon/Currency/GetFilteredCurrencies',null,null,null,0,15),
(1004,'Currency Exchange Rates','Currency Exchange Rates','#/view/Common/Views/CurrencyExchangeRate/CurrencyExchangeRateManagement',102,'VRCommon/CurrencyExchangeRate/GetFilteredExchangeRateCurrencies',null,null,null,0,20),
(1005,'Rate Types','Rate Types','#/view/Common/Views/RateType/RateTypeManagement',102,'VRCommon/RateType/GetFilteredRateTypes',null,null,null,0,25),
(1007,'Event Logs','Event Logs','#/view/Common/Views/MasterLog/MasterLogManagement',3,'VRCommon/LogAttribute/GetFilteredLoggers',null,null,null,0,15),
(1008,'Email Templates','Email Templates','#/view/Common/Views/EmailTemplate/EmailTemplateManagement',3,'VRCommon/EmailTemplate/GetFilteredEmailTemplates',null,null,null,0,9),
(1009,'Settings','Settings','#/view/Common/Views/Settings/SettingsManagement',3,'VRCommon/Settings/GetFilteredSettings',null,null,null,0,10)

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

--[common].[TemplateConfig]----------50001 to 60000---------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[TemplateConfig] on;
;with cte_data([ID],[Name],[ConfigType],[Editor],[BehaviorFQTN],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(50001,'Static Group','VR_Sec_GroupSettings','vr-sec-group-static',null,null)
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

--[common].[Setting]--------------------1 to 100----------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [common].[Setting] on;
;with cte_data([Id],[Name],[Type],[Category],[Settings],[Data])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'System Mail','VR_Common_Email','General','{"Editor":"vr-common-emailtemplate-settings-editor"}','{"$type":"Vanrise.Entities.EmailSettingData, Vanrise.Entities","SenderEmail":"testemail.vr@gmail.com","SenderPassword":"gmer2juIda","Host":"smtp.gmail.com","Port":587,"Timeout":10000}'),
(2,'System Currency','VR_Common_BaseCurrency','General','{"Editor":"vr-common-currency-settings-editor"}','{"$type":"Vanrise.Entities.CurrencySettingData, Vanrise.Entities","CurrencyId":1}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Type],[Category],[Settings],[Data]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data]
when not matched by target then
	insert([Id],[Name],[Type],[Category],[Settings],[Data])
	values(s.[Id],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data])
when not matched by source then
	delete;
set identity_insert [common].[Setting] off;