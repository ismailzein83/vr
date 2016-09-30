-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_GenericBusinessEntity_Update]
	@ID BigInt,
	@BusinessEntityDefinitionID uniqueidentifier,
	@Details VARCHAR(MAX)
AS
BEGIN
	Update genericdata.GenericBusinessEntity
	Set BusinessEntityDefinitionID = @BusinessEntityDefinitionID,
		Details=@Details
	WHERE ID=@ID

END