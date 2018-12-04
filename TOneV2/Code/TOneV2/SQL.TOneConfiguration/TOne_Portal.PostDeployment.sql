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
--when matched then
--	update set
--	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);


	
DECLARE @PortalAccountID int = (SELECT ID from [sec].[User] WHERE Email = 'portalsystemaccount@nodomain.com')

--[sec].[Permission]--------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(0, CONVERT(varchar, @PortalAccountID),1,'a611a651-b60b-483d-bc83-1c2b667a120a','[{"Name":"View","Value":1}]'),
(0, CONVERT(varchar, @PortalAccountID),1,'ab794846-853c-4402-a8e4-6f5c3a75f5f2','[{"Name":"View","Value":1}]'),
(0, CONVERT(varchar, @PortalAccountID),1,'69A42A6E-DDA8-4396-A5A2-1F540D961739','[{"Name":"View","Value":1}]'),
(0, CONVERT(varchar, @PortalAccountID),1,'DF1DBAF9-3236-4A66-9568-A5EA429976F6','[{"Name":"View","Value":1}]'),
(0, CONVERT(varchar, @PortalAccountID),1,'09C5BF94-BDD0-4A8A-A4CD-3E524B1A705B','[{"Name":"View","Value":1}]'),
(0, CONVERT(varchar, @PortalAccountID),1,'FB962789-B602-46DE-8B1B-A55F64D5AAF3','[{"Name":"View","Value":1}]')
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