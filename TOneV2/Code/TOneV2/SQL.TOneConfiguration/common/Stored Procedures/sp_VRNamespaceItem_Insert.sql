
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRNamespaceItem_Insert]
	@ID uniqueidentifier,
	@vrNamespaceId uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(Max),
	@CreatedBy int,
	@LastModifiedBy int 
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.VRNamespaceItem WHERE Name = @Name)
	BEGIN
		INSERT INTO common.VRNamespaceItem (ID, VRNamespaceId, Name, Settings, CreatedBy, LastModifiedBy)
		VALUES (@ID, @vrNamespaceId, @Name,@Settings, @CreatedBy , @LastModifiedBy)
	END
END