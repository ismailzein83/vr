-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Account_Insert]
	@Name NVARCHAR(255),
	@TypeID INT,
	@Settings NVARCHAR(MAX),
	@ParentID INT,
	@StatusID uniqueidentifier,
	@SourceID nvarchar(255),
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1 FROM Retail.Account
		WHERE Name = @Name AND ((@ParentID IS NULL AND ParentID IS NULL) OR (@ParentID IS NOT NULL AND ParentID = @ParentID))
	)
	BEGIN
		INSERT INTO Retail.Account (Name, [TypeID], Settings, ParentID,StatusID, SourceID)
		VALUES (@Name, @TypeID, @Settings, @ParentID,@StatusID,@SourceID)
		SET @ID = @@IDENTITY
	END
END