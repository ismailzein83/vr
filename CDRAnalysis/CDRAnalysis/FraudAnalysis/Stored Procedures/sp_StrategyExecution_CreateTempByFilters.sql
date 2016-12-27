

CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecution_CreateTempByFilters]
	@TempTableName VARCHAR(200),
	@FromCDRDate DATETIME,
	@ToCDRDate DATETIME, 
	@FromExecutionDate DATETIME,
	@ToExecutionDate DATETIME,
	@FromCancellationDate DATETIME,
	@ToCancellationDate DATETIME, 
	@StrategyIDs varchar(1000),
	@PeriodID int,
	@UserIDs varchar(1000),
	@StatusIDs varchar(1000)
	
	

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

    DECLARE @UserIDsTable TABLE (UserID INT);
			IF (@UserIDs IS NOT NULL)
			BEGIN
				INSERT INTO @UserIDsTable (UserID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@UserIDs);
			END

	DECLARE @StatusIDsTable TABLE (StatusID INT);
			IF (@StatusIDs IS NOT NULL)
			BEGIN
				INSERT INTO @StatusIDsTable (StatusID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@StatusIDs);
			END

	
		SELECT exe.[ID] ,exe.[ProcessID] ,exe.[StrategyID] ,exe.[FromDate]	,exe.[ToDate]	,exe.[PeriodID]	,exe.[ExecutionDate], exe.[CancellationDate]	
		     , exe.[ExecutedBy], exe.[CancelledBy], exe.[NumberofSubscribers], exe.[NumberofCDRs], exe.[ExecutionDuration],exe.[NumberofSuspicions], exe.[Status]
		INTO #RESULT
		FROM [FraudAnalysis].[StrategyExecution] exe WITH(NOLOCK)
		WHERE (@FromCDRDate is null or exe.FromDate>=@FromCDRDate) and (@ToCDRDate is null or exe.ToDate<@ToCDRDate)
		and   (@FromExecutionDate is null or exe.ExecutionDate>=@FromExecutionDate) and (@ToExecutionDate is null or exe.ExecutionDate<@ToExecutionDate)
		and   (@FromCancellationDate is null or exe.CancellationDate>=@FromCancellationDate) and (@ToCancellationDate is null or exe.CancellationDate<=@ToCancellationDate)
		and   (@StrategyIDs is null or exe.StrategyID in (SELECT StrategyID FROM @StrategyIDsTable))
		and   (@PeriodID is null or exe.PeriodID = @PeriodID)
		and   (@UserIDs IS NULL OR exe.ExecutedBy IN (SELECT UserID FROM @UserIDsTable)OR exe.CancelledBy IN (SELECT UserID FROM @UserIDsTable))
		and   (@StatusIDs IS NULL OR exe.[Status] IN (SELECT StatusID FROM @StatusIDsTable))
			
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END