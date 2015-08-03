-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CReate PROCEDURE [queue].sp_ExecutionFlow_GetAll 
AS
BEGIN
	
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ef.ID, ef.ExecutionFlowDefinitionID, ef.Name, efd.ExecutionTree
	FROM queue.ExecutionFlow ef
	JOIN queue.ExecutionFlowDefinition efd ON ef.ExecutionFlowDefinitionID = efd.ID
	
END