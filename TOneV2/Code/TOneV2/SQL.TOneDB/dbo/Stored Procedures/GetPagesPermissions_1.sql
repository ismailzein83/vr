CREATE PROCEDURE [dbo].[GetPagesPermissions]
AS
select * 
FROM Permission
Where PermissionLevel = 1 -- Page