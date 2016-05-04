
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_Delete]
	@AccountNumber varchar(50)
AS
BEGIN
	DELETE FROM FraudAnalysis.AccountStatus WHERE AccountNumber = @AccountNumber
END