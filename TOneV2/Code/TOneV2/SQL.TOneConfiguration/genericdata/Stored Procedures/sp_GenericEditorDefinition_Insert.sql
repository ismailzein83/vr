-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [genericdata].[sp_GenericEditorDefinition_Insert]
	@BusinessEntityID int,
	@Details VARCHAR(MAX),
	@ID INT OUT
AS
BEGIN
	INSERT INTO genericdata.GenericEditorDefinition(BusinessEntityID,Details)
	VALUES (@BusinessEntityID,@Details)
	SET @ID = @@IDENTITY 
END