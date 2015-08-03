CREATE PROCEDURE [dbo].[SavePluginPages]
(
	@pagesTable _PluginPage READONLY,
	@permissionsTable _PluginPermission READONLY
)
AS
INSERT INTO PluginPermissions(Id, Name, PermissionLevel)
SELECT Distinct Id, Name, PermissionLevel
FROM @permissionsTable
Where Id NOT in (SELECT Id FROM PluginPermissions)

INSERT INTO PluginPages(PagePath, PluginName, PageName, PermissionRoot, PermissionId)
SELECT Distinct PagePath, PluginName, PageName, PermissionRoot, PermissionId
FROM @pagesTable
Where PagePath NOT in (SELECT PagePath FROM PluginPages)