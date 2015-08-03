CREATE PROCEDURE [dbo].[SaveMenuItemPermission]
(
	@ItemId int,
	@PermissionId varchar(255)
)
AS

Update WebsiteMenuItemPermission
SET PermissionId = @PermissionId
Where ItemId = @ItemId

if(@@RowCount = 0)
	INSERT INTO WebsiteMenuItemPermission (ItemId,PermissionId)
	VALUES (@ItemId, @PermissionId)