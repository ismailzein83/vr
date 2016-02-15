
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecution_Close]
	@ID INT,
	@NumberofSubscribers BIGINT,
	@NumberofCDRs BIGINT,
	@NumberofSuspicions	BIGINT,
	@ExecutionDuration float
AS
BEGIN

	UPDATE [FraudAnalysis].[StrategyExecution]
	set 
       [NumberofSubscribers] = @NumberofSubscribers
      ,[NumberofCDRs] = @NumberofCDRs
	  ,[NumberofSuspicions] = @NumberofSuspicions
      ,[ExecutionDuration] = @ExecutionDuration
      ,[Status] = 10


	WHERE ID = @ID
END