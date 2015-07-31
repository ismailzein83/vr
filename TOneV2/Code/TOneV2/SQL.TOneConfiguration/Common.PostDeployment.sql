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

--[sec].[BusinessEntityModule]----
set nocount on;
set identity_insert [sec].[BusinessEntityModule] on;
;with cte_data([Id],[Name],[ParentId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'TOne',null,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View","Add","Edit", "Delete", "Full Control"]}'),
(2,'Routing Module',1,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View","Add","Edit", "Delete", "Full Control"]}'),
(3,'Business Intelligence Module',1,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View","Add","Edit", "Delete", "Full Control"]}'),
(4,'Sales Module',1,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View","Add","Edit", "Delete", "Full Control"]}'),
(5,'Business Entity Module',1,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View","Add","Edit", "Delete", "Full Control"]}'),
(6,'Administration Module',1,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View","Add","Edit", "Delete", "Full Control"]}'),
(7,'Billing Module',3,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View","Add","Edit", "Delete", "Full Control"]}'),
(8,'Trafic Module',3,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View","Add","Edit", "Delete", "Full Control"]}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ParentId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntityModule] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ParentId] = s.[ParentId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[ParentId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[ParentId],s.[BreakInheritance],s.[PermissionOptions])
when not matched by source then
	delete;
set identity_insert [sec].[BusinessEntityModule] off;


--[sec].[BusinessEntity]-----
set nocount on;
set identity_insert [sec].[BusinessEntity] on;
;with cte_data([Id],[Name],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Routes',2,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}'),
(2,'Route Rules',2,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View","Add Block Route","Edit Block Route", "Add Override Route", "Edit Override Route"]}'),
(3,'Billing Statistics',7,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}'),
(4,'CDRs',7,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}'),
(6,'Traffic Statistics',8,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}'),
(7,'CDRs',8,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}'),
(8,'Rates',4,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View", "Edit"]}'),
(9,'LCR',4,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}'),
(10,'Carrier',5,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View", "Edit", "Delete"]}'),
(11,'Code',5,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View", "Edit"]}'),
(12,'Zone',5,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View", "Edit"]}'),
(13,'Users',6,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View", "Add", "Edit", "Reset Password"]}'),
(14,'Groups',6,0,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View", "Add", "Edit", "Delete"]}'),
(15,'System Entities',6,1,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View", "Assign Permissions"]}'),
(17,'Dynamic Pages',6,1,'{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View", "Add", "Edit", "Delete","Validate"]}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions])
when not matched by source then
	delete;
set identity_insert [sec].[BusinessEntity] off;


--[sec].[Permission]---
set nocount on;
;with cte_data([HolderType],[HolderId],[EntityType],[EntityId],[PermissionFlags])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(0,'1',0,'1','{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.PermissionFlag, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.PermissionFlag, Vanrise.Security.Entities","Name":"Full Control","Value":1}]}'),
(0,'1',1,'15','{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.PermissionFlag, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.PermissionFlag, Vanrise.Security.Entities","Name":"View","Value":1},{"$type":"Vanrise.Security.Entities.PermissionFlag, Vanrise.Security.Entities","Name":"Assign Permissions","Value":1}]}'),
(0,'1',1,'17','{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.PermissionFlag, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.PermissionFlag, Vanrise.Security.Entities","Name":"View","Value":1},{"$type":"Vanrise.Security.Entities.PermissionFlag, Vanrise.Security.Entities","Name":"Edit","Value":1},{"$type":"Vanrise.Security.Entities.PermissionFlag, Vanrise.Security.Entities","Name":"Delete","Value":1},{"$type":"Vanrise.Security.Entities.PermissionFlag, Vanrise.Security.Entities","Name":"Add","Value":1},{"$type":"Vanrise.Security.Entities.PermissionFlag, Vanrise.Security.Entities","Name":"Validate","Value":1}]}')
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

