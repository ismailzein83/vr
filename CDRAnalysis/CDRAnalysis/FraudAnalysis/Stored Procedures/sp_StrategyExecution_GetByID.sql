


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecution_GetByID]
	@ID int
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT [ID]
      ,[ProcessID]
      ,[StrategyID]
      ,[FromDate]
      ,[ToDate]
      ,[PeriodID]
      ,[ExecutionDate]
      ,[CancellationDate]
      ,[ExecutedBy]
      ,[CancelledBy]
      ,[NumberofSubscribers]
      ,[NumberofCDRs]
	  ,[NumberofSuspicions]
      ,[ExecutionDuration]
      ,[Status]
  FROM [FraudAnalysis].[StrategyExecution]  WITH (NOLOCK)
	
	WHERE ID = @ID
	
END