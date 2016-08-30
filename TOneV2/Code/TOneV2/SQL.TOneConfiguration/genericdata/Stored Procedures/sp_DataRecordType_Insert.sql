-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataRecordType_Insert]
	@Name nvarchar(255),
	@ParentId INT,
	@Fields VARCHAR(MAX),
	@ID INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.[DataRecordType] WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.[DataRecordType](Name,ParentID,Fields)
		VALUES (@Name,@ParentId,@Fields)
		SET @ID = SCOPE_IDENTITY() 
	END
END