-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataRecordType_Insert]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@ParentId uniqueidentifier,
	@Fields VARCHAR(MAX)
	
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.[DataRecordType] WHERE Name = @Name)
	BEGIN
		INSERT INTO genericdata.[DataRecordType](ID,Name,ParentID,Fields)
		VALUES (@ID,@Name,@ParentId,@Fields)

	END
END