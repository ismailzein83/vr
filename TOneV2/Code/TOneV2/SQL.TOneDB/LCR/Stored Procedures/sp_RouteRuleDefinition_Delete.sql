Create PROCEDURE [LCR].[sp_RouteRuleDefinition_Delete] 
	@ID int
AS
BEGIN
	UPDATE [LCR].[RouteRuleDefinition]
	SET IsDeleted = 1
	WHERE RouteRuleId = @ID
END
