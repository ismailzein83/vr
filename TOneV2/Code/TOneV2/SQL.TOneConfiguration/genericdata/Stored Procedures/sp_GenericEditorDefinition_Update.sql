-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_GenericEditorDefinition_Update]
	@ID INT,
	@BusinessEntityID int,
	@Details VARCHAR(MAX)
	
AS
BEGIN
IF EXISTS(SELECT 1 FROM genericdata.GenericEditorDefinition WHERE ID = @ID)
	BEGIN
		Update genericdata.GenericEditorDefinition
		Set BusinessEntityID = @BusinessEntityID, Details = @Details
		Where ID = @ID
	END
END