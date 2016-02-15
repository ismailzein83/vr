
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecution_Cancel]
	@ID INT,
	@UserID INT
AS
BEGIN
	UPDATE FraudAnalysis.StrategyExecution
	SET CancelledBy = @UserID,
		CancellationDate = GETDATE(),
		[Status]= 20
	WHERE ID = @ID
END