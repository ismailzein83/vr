-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Account_Update]
	@ID INT,
	@Name NVARCHAR(255),
	@TypeID INT,
	@Settings NVARCHAR(MAX),
	@ParentID INT,
	@SourceID nvarchar(255)
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1 FROM Retail.Account
		WHERE ID != @ID
			AND Name = @Name
			AND ((@ParentID IS NULL AND ParentID IS NULL) OR (@ParentID IS NOT NULL AND ParentID = @ParentID))
	)
	BEGIN
		UPDATE Retail.Account
		SET Name = @Name, [TypeID] = @TypeID, Settings = @Settings, SourceID = @SourceID
		WHERE ID = @ID
	END
END