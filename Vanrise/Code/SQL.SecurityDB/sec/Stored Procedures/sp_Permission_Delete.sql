-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [sec].[sp_Permission_Delete] 
	@HolderType int,
	@HolderId varchar(50),
	@EntityType int,
	@EntityId varchar(50)
AS
BEGIN
    Delete from sec.Permission 
    where sec.Permission.HolderType = @HolderType
    and	sec.Permission.HolderId = @HolderId
    and sec.Permission.EntityType = @EntityType
    and sec.Permission.EntityId = @EntityId
END