-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_ExecutionFlowDefinition_GetAll] 
	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [ID]
      ,[Name]
      ,[Title]
      ,[Stages]
       from [queue].ExecutionFlowDefinition WITH(NOLOCK) 
       
     SET NOCOUNT OFF;
END