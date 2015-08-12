-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [queue].[sp_ExecutionFlowDefinition_GetAll] 
	
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [ID]
      ,[Name]
      ,[Title]
       from [queue].ExecutionFlowDefinition
       
     SET NOCOUNT OFF;
END