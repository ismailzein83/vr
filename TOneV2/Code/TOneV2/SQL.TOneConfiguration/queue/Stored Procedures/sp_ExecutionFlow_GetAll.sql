-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_ExecutionFlow_GetAll] 
AS
BEGIN
	
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ef.ID, ef.ExecutionFlowDefinitionID, ef.Name, efd.ExecutionTree
	FROM queue.ExecutionFlow ef WITH(NOLOCK) 
	INNER JOIN queue.ExecutionFlowDefinition efd  WITH(NOLOCK) ON ef.ExecutionFlowDefinitionID = efd.ID
	ORDER BY ef.[Name]
END