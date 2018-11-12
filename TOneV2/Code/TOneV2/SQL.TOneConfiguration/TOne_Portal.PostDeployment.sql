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


--[sec].[User]--------------------------------------------------------------------------------
Begin
set nocount on;
;with cte_data([Name],[Password],[Email],[TenantId],[Description],[EnabledTill],[ExtendedSettings],[SecurityProviderId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('Portal Account','rURnXpnTPlyQ/3u4fZuiStxsBcOS','portalsystemaccount@nodomain.com',1,'System Account used to connect from the Portal Application',null,null,'9554069B-795E-4BB1-BFF3-9AF0F47FC0FF')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[Password],[Email],[TenantId],[Description],[EnabledTill],[ExtendedSettings],[SecurityProviderId]))
merge	[sec].[User] as t
using	cte_data as s
on		1=1 and t.[Email] = s.[Email]
--when matched then
--	update set
--	[Name] = s.[Name],[Password] = s.[Password],[TenantId] = s.[TenantId],[Description] = s.[Description],[EnabledTill] = s.[EnabledTill],[ExtendedSettings] = s.[ExtendedSettings]
when not matched by target then
	insert([Name],[Password],[Email],[TenantId],[Description],[EnabledTill],[ExtendedSettings],[SecurityProviderId])
	values(s.[Name],s.[Password],s.[Email],s.[TenantId],s.[Description],s.[EnabledTill],s.[ExtendedSettings],s.[SecurityProviderId]);
----------------------------------------------------------------------------------------------------
END


--[common].[Connection]-----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('B50314A1-7B2B-4465-917D-5A8E60BFA09E','Carrier Portal','{"$type":"Vanrise.Common.Business.VRInterAppRestConnection, Vanrise.Common.Business","ConfigId":"5cd2aac3-1c74-401f-8010-8b9b5fd9c011","BaseURL":"http://localhost:7755","Username":"tonesystemaccount@nodomain.com","Password":"toneaccount@nodomain.com"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[Connection] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);