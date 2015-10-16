CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_CreateTempByFilters]
	@TempTableName VARCHAR(200),
	@AccountNumber VARCHAR(50),
	@FromDate DATETIME,
	@ToDate DATETIME,
	@StrategyIDs varchar(1000)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN

	 IF (@StrategyIDs IS NOT NULL)
			BEGIN
				DECLARE @StrategyIDsTable TABLE (StrategyID INT);
				INSERT INTO @StrategyIDsTable (StrategyID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@StrategyIDs);
			END


		SELECT distinct ac.ID AS CaseID,
			ac.AccountNumber,
			ac.UserID,
			ac.[Status] AS StatusID,
			ac.StatusUpdatedTime,
			ac.ValidTill,
			ac.CreatedTime
			
		INTO #RESULT
			
		FROM FraudAnalysis.AccountCase ac

		inner join FraudAnalysis.StrategyExecutionDetails details on ac.ID=details.CaseID
		inner join FraudAnalysis.StrategyExecution exe on details.StrategyExecutionID=exe.ID

		WHERE (exe.StrategyID in (SELECT StrategyID FROM @StrategyIDsTable)   or @StrategyIDs is null )  and  
		(@FromDate is null or exe.FromDate >= @FromDate)
		and (@ToDate is null or exe.ToDate <= @ToDate)
		and (@AccountNumber is null or details.AccountNumber = @AccountNumber)


		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END