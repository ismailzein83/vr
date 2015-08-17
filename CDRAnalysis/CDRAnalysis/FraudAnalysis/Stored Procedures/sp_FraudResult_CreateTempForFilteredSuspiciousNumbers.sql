





CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_CreateTempForFilteredSuspiciousNumbers]
(
	@TempTableName varchar(200),	
	@FromDate datetime,
	@ToDate datetime,
	@StrategiesList varchar(100) = '',
	@SuspiciousLevelsList varchar(100) = '',
	@CaseStatusesList varchar(100) = ''
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
		
		CREATE TABLE #SuspectionLevel(Id int, Name varchar(20))
		CREATE TABLE #Strategy(Id int, Name varchar(20))
		CREATE TABLE #CaseStatus(Id int, Name varchar(20))
		
		
		IF(@SuspiciousLevelsList = '')
			begin
				EXEC('INSERT INTO #SuspectionLevel SELECT Id,Name FROM [FraudAnalysis].SuspicionLevel')
			end
		else
			begin
				EXEC('INSERT INTO #SuspectionLevel SELECT Id,Name FROM [FraudAnalysis].Suspicion_Level s WHERE s.Id IN ('+@SuspiciousLevelsList+')')
			end
			
			
			
		IF(@StrategiesList = '')
			begin
				EXEC('INSERT INTO #Strategy SELECT Id,Name FROM [FraudAnalysis].Strategy ')
			end
		else
			begin
				EXEC('INSERT INTO #Strategy SELECT Id,Name FROM [FraudAnalysis].Strategy s WHERE s.Id IN ('+@StrategiesList+')')
			end
			
			
		IF(@CaseStatusesList = '')
			begin
				EXEC('INSERT INTO #CaseStatus SELECT Id,Name FROM [FraudAnalysis].CaseStatus ')
			end
		else
			begin
				EXEC('INSERT INTO #CaseStatus SELECT Id,Name FROM [FraudAnalysis].CaseStatus cs WHERE cs.Id IN ('+@CaseStatusesList+')')
			end
			
		Select  temp.CaseStatus CaseStatus , temp.StatusId StatusId, temp.ValidTill ValidTill, temp.NumberofOccurances NumberofOccurances, temp.LastOccurance LastOccurance, temp.StrategyName StrategyName, temp.AccountNumber AccountNumber, temp.SuspicionLevelId SuspicionLevelId INTO #Result from
		
		
		(SELECT ISNULL(cs.Name, 'Open')  CaseStatus,  ISNULL(sc.StatusId, 1) StatusId, sc.ValidTill ValidTill  , COUNT(st.Id) as NumberofOccurances,  MAX(st.DateDay) as LastOccurance,   #Strategy.Name as StrategyName, st.AccountNumber as AccountNumber , max(#SuspectionLevel.Id) as SuspicionLevelId 
				from [FraudAnalysis].[AccountThreshold] st
				inner join #SuspectionLevel ON st.SuspicionLevelId=#SuspectionLevel.Id 
				inner join #Strategy ON #Strategy.Id=st.StrategyId 
				inner join [FraudAnalysis].[AccountCase] sc  ON sc.AccountNumber=st.AccountNumber 
				inner join #CaseStatus cs ON cs.Id=sc.StatusId
				WHERE  SuspicionLevelId <> 0 and dateday between @fromDate and @ToDate 
				group by st.AccountNumber, #Strategy.Name, cs.Name , sc.StatusId, sc.ValidTill
				
		union
			
		SELECT 'Open'  CaseStatus,   1 as StatusId, null ValidTill  , COUNT(st.Id) as NumberofOccurances,  MAX(st.DateDay) as LastOccurance,   #Strategy.Name as StrategyName, st.AccountNumber as AccountNumber , max(#SuspectionLevel.Id) as SuspicionLevelId 
				from [FraudAnalysis].[AccountThreshold] st
				inner join #SuspectionLevel ON st.SuspicionLevelId=#SuspectionLevel.Id 
				inner join #Strategy ON #Strategy.Id=st.StrategyId 
				WHERE  SuspicionLevelId <> 0 and dateday between @fromDate and @ToDate  and  AccountNumber not in (select AccountNumber from AccountCase) and  (@CaseStatusesList = '' or @CaseStatusesList LIKE '%1%'  )
				group by st.AccountNumber, #Strategy.Name
				
				
				
				) as temp
		
				
				declare @sql varchar(1000)
				set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
				exec(@sql)
		END
		
		SET NOCOUNT OFF
	END