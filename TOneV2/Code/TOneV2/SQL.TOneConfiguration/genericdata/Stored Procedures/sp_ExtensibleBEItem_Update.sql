-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_ExtensibleBEItem_Update]
	@ID uniqueidentifier,
	@Details VARCHAR(MAX)
	
AS
BEGIN
IF EXISTS(SELECT 1 FROM genericdata.ExtensibleBEItem WHERE ID = @ID)
	BEGIN
		Update genericdata.ExtensibleBEItem
		Set  Details = @Details
		Where ID = @ID
	END
END