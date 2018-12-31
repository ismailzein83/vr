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
	@LastModifiedBy int,
	@ParentID BIGINT
AS
BEGIN
If NOT EXISTS(SELECT TOP 1 NULL FROM Retail.Account WHERE Name = @Name AND TypeID =@TypeID AND (@ParentID is null or ParentID = @ParentID) AND ID != @ID)
BEGIN
	UPDATE Retail.Account
	SET Name = @Name, [TypeID] = @TypeID, Settings = @Settings, SourceID = @SourceID, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	WHERE ID = @ID
END

END