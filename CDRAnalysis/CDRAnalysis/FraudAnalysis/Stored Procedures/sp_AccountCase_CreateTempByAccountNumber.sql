-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_CreateTempByAccountNumber]
	@TempTableName VARCHAR(200),
	@AccountNumber VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		SELECT ac.ID AS CaseID,
			ac.AccountNumber,
			ac.UserID,
			ac.[Status] AS StatusID,
			ac.StatusUpdatedTime,
			ac.ValidTill,
			ac.CreatedTime
			
		INTO #RESULT
			
		FROM FraudAnalysis.AccountCase ac
		
		WHERE ac.AccountNumber = @AccountNumber
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END