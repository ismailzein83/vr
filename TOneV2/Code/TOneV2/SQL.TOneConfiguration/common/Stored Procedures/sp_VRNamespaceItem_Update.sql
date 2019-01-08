
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRNamespaceItem_Update]
	@ID uniqueidentifier,
	@vrNamespaceId uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX),
	@LastModifiedBy int
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.VRNamespaceItem WHERE ID != @ID and Name = @Name)
	BEGIN
		update common.VRNamespaceItem
		set  VRNamespaceId = @vrNamespaceId, Name = @Name, Settings = @Settings, LastModifiedBy = @LastModifiedBy, LastModifiedTime = getdate()
		where  ID = @ID
	END
END