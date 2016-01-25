

CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecution_CreateTempByFilters]
	@TempTableName VARCHAR(200),
	@FromCDRDate DATETIME,
	@ToCDRDate DATETIME, 
	@FromExecutionDate DATETIME,
	@ToExecutionDate DATETIME, 
	@StrategyIDs varchar(1000),
	@PeriodIDs varchar(100) = NULL

AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN

	DECLARE @StrategyIDsTable TABLE (StrategyID INT);

        IF (@StrategyIDs IS NOT NULL)
			BEGIN
				INSERT INTO @StrategyIDsTable (StrategyID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@StrategyIDs);
			END

    DECLARE @PeriodIDsTable TABLE (PeriodID INT);
			IF (@PeriodIDs IS NOT NULL)
			BEGIN
				INSERT INTO @PeriodIDsTable (PeriodID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@PeriodIDs);
			END



		SELECT exe.[ID] ,exe.[ProcessID] ,exe.[StrategyID] ,exe.[FromDate]	,exe.[ToDate]	,exe.[PeriodID]	,exe.[ExecutionDate]	
		INTO #RESULT
		FROM [FraudAnalysis].[StrategyExecution] exe WITH(NOLOCK)
		WHERE (@FromCDRDate is null or exe.FromDate>=@FromCDRDate) and (@ToCDRDate is null or exe.ToDate<@ToCDRDate)
		and   (@FromExecutionDate is null or exe.ExecutionDate>=@FromExecutionDate) and (@ToExecutionDate is null or exe.ExecutionDate<@ToExecutionDate)
		and   (@StrategyIDs is null or exe.StrategyID in (SELECT StrategyID FROM @StrategyIDsTable))
		and (@PeriodIDs IS NULL OR exe.PeriodId IN (SELECT PeriodID FROM @PeriodIDsTable))
			
		ORDER BY exe.[ExecutionDate] DESC
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END