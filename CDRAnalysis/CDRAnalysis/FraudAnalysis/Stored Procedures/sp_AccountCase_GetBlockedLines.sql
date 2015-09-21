-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_GetBlockedLines]
	@TempTableName VARCHAR(200),
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		SELECT ac.AccountNumber,
			SUM(cdr.DurationInSeconds) / 60 AS Volume,
			COUNT(DISTINCT ac.ID) AS GeneratedCases,
			ac.Reason AS ReasonofBlocking,
			COUNT(DISTINCT CAST(cdr.connectdatetime AS DATE)) AS ActiveDays,
			COUNT(DISTINCT ac.AccountNumber) AS BlockedLinesCount
		
		INTO #RESULT
		
		FROM FraudAnalysis.AccountCase AS ac WITH(NOLOCK)
		INNER JOIN FraudAnalysis.NormalCDR AS cdr WITH(NOLOCK, INDEX = IX_NormalCDR_MSISDN) ON ac.AccountNumber = cdr.MSISDN
		
		WHERE ac.[Status] = 3
		AND cdr.Call_Type = 1
		AND ac.CreatedTime BETWEEN @FromDate
		AND @ToDate

		GROUP BY ac.AccountNumber,ac.Reason
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
	
	SET NOCOUNT OFF;
	
END