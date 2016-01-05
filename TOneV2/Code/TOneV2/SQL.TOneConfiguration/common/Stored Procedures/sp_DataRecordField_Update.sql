-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_DataRecordField_Update]
	@ID int,
	@Details varchar(max)
AS
BEGIN
IF EXISTS(SELECT 1 FROM common.DataRecordField WHERE ID = @ID )
	BEGIN
		Update common.DataRecordField
	Set [Details] = @Details
	Where ID = @ID
	END
END