-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE genericdata.sp_GenericBusinessEntity_Update
	@ID BigInt,
	@BusinessEntityDefinitionID INT,
	@Details VARCHAR(MAX)
AS
BEGIN
	Update genericdata.GenericBusinessEntity
	Set BusinessEntityDefinitionID = @BusinessEntityDefinitionID,
		Details=@Details
	WHERE ID=@ID

END