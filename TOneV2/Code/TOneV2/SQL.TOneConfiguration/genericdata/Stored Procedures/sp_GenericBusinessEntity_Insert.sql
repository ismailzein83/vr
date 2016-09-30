-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_GenericBusinessEntity_Insert] 
	@BusinessEntityDefinitionID uniqueidentifier,
	@Details VARCHAR(MAX),
	@ID BigInt out
	
AS
BEGIN
	Insert into genericdata.GenericBusinessEntity (BusinessEntityDefinitionID, Details)
	values(@BusinessEntityDefinitionID, @Details)
	SET @ID = SCOPE_IDENTITY()

END