-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Account_Insert]
	@Name NVARCHAR(255),
	@TypeID uniqueidentifier,
	@Settings NVARCHAR(MAX),
	@ExtendedSettings nvarchar(MAX),
	@ParentID BIGINT,
	@StatusID uniqueidentifier,
	@SourceID nvarchar(255),
	@CreatedTime datetime = null,
	@CreatedBy int,
	@LastModifiedBy int,
	@ID INT OUT
AS
BEGIN
		IF @CreatedTime IS NULL
			SET @CreatedTime = GETDATE()
		INSERT INTO Retail.Account (Name, [TypeID], Settings, ExtendedSettings, ParentID,StatusID, SourceID,CreatedTime, CreatedBy, LastModifiedBy, LastModifiedTime)
		VALUES (@Name, @TypeID, @Settings, @ExtendedSettings, @ParentID,@StatusID,@SourceID, @CreatedTime, @CreatedBy, @LastModifiedBy, GETDATE())
		SET @ID = SCOPE_IDENTITY()

END