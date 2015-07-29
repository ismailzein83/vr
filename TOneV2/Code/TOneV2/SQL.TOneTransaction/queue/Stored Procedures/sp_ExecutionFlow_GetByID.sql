-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE queue.sp_ExecutionFlow_GetByID 
	@ExecutionFlowID int
AS
BEGIN
	
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ef.ID, ef.ExecutionFlowDefinitionID, ef.Name, efd.ExecutionTree
	FROM queue.ExecutionFlow ef
	JOIN queue.ExecutionFlowDefinition efd ON ef.ExecutionFlowDefinitionID = efd.ID
	WHERE ef.ID = @ExecutionFlowID
	
END