

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_RelatedNumber_GetByAccountNumber]
	@AccountNumber VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT AccountNumber, RelatedAccountNumber
		
	FROM FraudAnalysis.RelatedNumber WITH (NOLOCK)
	
	WHERE AccountNumber = @AccountNumber
END