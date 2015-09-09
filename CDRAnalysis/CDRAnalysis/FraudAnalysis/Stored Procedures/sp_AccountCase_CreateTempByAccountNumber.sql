-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_CreateTempByAccountNumber]
	@TempTableName VARCHAR(200),
	@AccountNumber VARCHAR(50),
	@From DATETIME,
	@To DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		SELECT ac.ID AS CaseID,
			ac.AccountNumber,
			ac.UserID,
			u.Name AS UserName,
			ac.[Status] AS StatusID,
			ac.StatusUpdatedTime,
			ac.ValidTill,
			ac.CreatedTime
			
		INTO #RESULT
			
		FROM FraudAnalysis.AccountCase ac
		INNER JOIN CDRAnalysisConfiguration.sec.[User] u ON ac.UserID = u.ID
		
		WHERE ac.AccountNumber = @AccountNumber
			AND ac.CreatedTime >= @From
			AND ac.CreatedTime <= @To
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END