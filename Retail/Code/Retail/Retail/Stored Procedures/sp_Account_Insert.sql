-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Account_Insert]
	@Name NVARCHAR(255),
	@TypeID uniqueidentifier,
	@Settings NVARCHAR(MAX),
	@ParentID INT,
	@StatusID uniqueidentifier,
	@SourceID nvarchar(255),
	@ID INT OUT
AS
BEGIN

		INSERT INTO Retail.Account (Name, [TypeID], Settings, ParentID,StatusID, SourceID)
		VALUES (@Name, @TypeID, @Settings, @ParentID,@StatusID,@SourceID)
		SET @ID = SCOPE_IDENTITY()

END