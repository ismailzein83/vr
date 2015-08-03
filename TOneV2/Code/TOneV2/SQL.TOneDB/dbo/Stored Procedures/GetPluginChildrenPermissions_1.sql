CREATE PROCEDURE [dbo].[GetPluginChildrenPermissions]
(	@PermissionRoot varchar(500))
AS
SELECT ID, Name, Description, Permissionlevel
FROM PluginPermissions
Where ID Like @PermissionRoot+'/%'