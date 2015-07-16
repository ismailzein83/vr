-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sec.sp_Permission_Update 
	@HolderType int,
	@HolderId varchar(50),
	@EntityType int,
	@EntityId varchar(50),
	@PermissionFlags varchar(1000)
AS
BEGIN
    IF EXISTS(
    SELECT * FROM sec.Permission WHERE sec.Permission.HolderType = @HolderType
    and sec.Permission.HolderId = @HolderId and sec.Permission.EntityType = @EntityType
    and sec.Permission.EntityId = @EntityId
    )
		UPDATE sec.Permission SET sec.Permission.PermissionFlags = @PermissionFlags
		WHERE sec.Permission.HolderType = @HolderType
				and sec.Permission.HolderId = @HolderId 
				and sec.Permission.EntityType = @EntityType
				and sec.Permission.EntityId = @EntityId
	ELSE
		INSERT INTO sec.Permission (HolderType, HolderId, EntityType, EntityId, PermissionFlags)
		VALUES (@HolderType, @HolderId, @EntityType, @EntityId, @PermissionFlags)
END