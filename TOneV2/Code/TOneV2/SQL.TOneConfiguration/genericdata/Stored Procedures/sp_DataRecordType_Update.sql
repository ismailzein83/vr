-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataRecordType_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@ParentId uniqueidentifier,
	@Fields VARCHAR(MAX),
	@ExtraFieldsEvaluator nvarchar(max)
	
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.[DataRecordType] WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update genericdata.[DataRecordType]
		Set Name = @Name, ParentID = @ParentId , Fields = @Fields, ExtraFieldsEvaluator = @ExtraFieldsEvaluator
		Where ID = @ID
	END
END