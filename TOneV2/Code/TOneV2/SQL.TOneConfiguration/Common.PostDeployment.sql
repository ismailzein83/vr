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
('VRCommon/City/GetFilteredCities',null),
('VRCommon/City/GetCitiesInfo',null),
('VRCommon/City/GetCity',null),
('VRCommon/City/AddCity','VRCommon_Country: Add City'),
('VRCommon/City/UpdateCity',null)
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

--[sec].[BusinessEntity]------------------301 to 600----------------------------------------------------------
--------------------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(301,'VRCommon_Country','Country',2,0,'["View", "Add", "Edit", "Download Template", "Upload", "Add City"]')
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

--[sec].[View]-----------------------------1001 to 2000--------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
