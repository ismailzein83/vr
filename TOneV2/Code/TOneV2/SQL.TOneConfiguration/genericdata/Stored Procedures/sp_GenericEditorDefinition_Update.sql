-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [genericdata].[sp_GenericEditorDefinition_Update]
	@ID INT,
	@BusinessEntityID nvarchar(255),
	@Details VARCHAR(MAX)
	
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.GenericEditorDefinition WHERE ID != @ID)
	BEGIN
		Update genericdata.GenericEditorDefinition
		Set BusinessEntityID = @BusinessEntityID, Details = @Details
		Where ID = @ID
	END
END