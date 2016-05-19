-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE genericdata.sp_BELookupRuleDefinition_Delete
	@ID INT
AS
BEGIN
	DELETE FROM genericdata.BELookupRuleDefinition WHERE ID = @ID
END