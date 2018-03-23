-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Account_Update]
	@ID INT,
	@Name NVARCHAR(255),
	@TypeID uniqueidentifier,
	@Settings NVARCHAR(MAX),
	@SourceID nvarchar(255),
	@LastModifiedBy int
AS
BEGIN
	UPDATE Retail.Account
	SET Name = @Name, [TypeID] = @TypeID, Settings = @Settings, SourceID = @SourceID, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	WHERE ID = @ID
END