-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Account_Insert]
	@Name NVARCHAR(255),
	@TypeID uniqueidentifier,
	@Settings NVARCHAR(MAX),
	@ParentID BIGINT,
	@StatusID uniqueidentifier,
	@SourceID nvarchar(255),
	@CreatedTime datetime = null,
	@ID INT OUT
AS
BEGIN
		IF @CreatedTime IS NULL
			SET @CreatedTime = GETDATE()
		INSERT INTO Retail.Account (Name, [TypeID], Settings, ParentID,StatusID, SourceID,CreatedTime)
		VALUES (@Name, @TypeID, @Settings, @ParentID,@StatusID,@SourceID, @CreatedTime)
		SET @ID = SCOPE_IDENTITY()

END