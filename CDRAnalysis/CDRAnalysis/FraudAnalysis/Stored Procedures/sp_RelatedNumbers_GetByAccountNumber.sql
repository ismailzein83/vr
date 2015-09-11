-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_RelatedNumbers_GetByAccountNumber]
	@AccountNumber VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT RelatedNumbers
		
	FROM FraudAnalysis.RelatedNumbers
	
	WHERE AccountNumber = @AccountNumber
END