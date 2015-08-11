--[sec].[User]------
set nocount on;
set identity_insert [sec].[User] on;
;with cte_data([ID],[Name],[Password],[Email],[Status])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Administrator','1','admin@vanrise.com',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Password],[Email],[Status]))
merge	[sec].[User] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Password] = s.[Password],[Email] = s.[Email],[Status] = s.[Status]
when not matched by target then
	insert([ID],[Name],[Password],[Email],[Status])
	values(s.[ID],s.[Name],s.[Password],s.[Email],s.[Status])
when not matched by source then
	delete;
set identity_insert [sec].[User] off;




--[sec].[Permission]---
set nocount on;
;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(0,'1',0,'1','[{"Name":"Full Control","Value":1}]'),
(0,'1',1,'15','[{"Name":"View","Value":1}, {"Name":"Assign Permissions","Value":1}]'),
(0,'1',1,'17','[{"Name":"View","Value":1}, {"Name":"Edit","Value":1}, {"Name":"Delete","Value":1}, {"Name":"Add","Value":1}, {"Name":"Validate","Value":1}]')
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
	values(s.[HolderType],s.[HolderId],s.[EntityType],s.[EntityId],s.[PermissionFlags])
when not matched by source then
	delete;

