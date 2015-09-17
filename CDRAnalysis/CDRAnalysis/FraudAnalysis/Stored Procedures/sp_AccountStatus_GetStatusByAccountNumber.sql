-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_GetStatusByAccountNumber]
	@AccountNumber VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT [Status] FROM FraudAnalysis.AccountStatus WHERE AccountNumber = @AccountNumber
END