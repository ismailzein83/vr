

CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_CreateTempForFilteredSuspiciousNumbers]
(
	@TempTableName varchar(200),
	@FromDate datetime,
	@ToDate datetime,
	@StrategiesList varchar(100) = '',
	@SuspiciousLevelsList varchar(100) = '',
	@CaseStatusesList varchar(100) = '',
	@AccountNumber 	varchar(50)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
		
		CREATE TABLE #SuspectionLevel(Id int, Name varchar(20))
		CREATE TABLE #Strategy(Id int, Name varchar(20), PeriodId int)
		CREATE TABLE #CaseStatus(Id int, Name varchar(20))
		
		
		IF(@SuspiciousLevelsList = '')
			begin
				EXEC('INSERT INTO #SuspectionLevel SELECT Id,Name FROM [FraudAnalysis].SuspicionLevel')
			end
		else
			begin
				EXEC('INSERT INTO #SuspectionLevel SELECT Id,Name FROM [FraudAnalysis].SuspicionLevel s WHERE s.Id IN ('+@SuspiciousLevelsList+')')
			end
			
			
			
		IF(@StrategiesList = '')
			begin
				EXEC('INSERT INTO #Strategy SELECT Id,Name,PeriodId FROM [FraudAnalysis].Strategy ')
			end
		else
			begin
				EXEC('INSERT INTO #Strategy SELECT Id,Name,PeriodId FROM [FraudAnalysis].Strategy s WHERE s.Id IN ('+@StrategiesList+')')
			end
			
			
		IF(@CaseStatusesList = '')
			begin
				EXEC('INSERT INTO #CaseStatus SELECT Id,Name FROM [FraudAnalysis].CaseStatus ')
			end
		else
			begin
				EXEC('INSERT INTO #CaseStatus SELECT Id,Name FROM [FraudAnalysis].CaseStatus cs WHERE cs.Id IN ('+@CaseStatusesList+')')
			end
			
		SELECT  cs.Name  CaseStatus,  ac.StatusId, ac.ValidTill ValidTill  , cast(count(st.Id) as varchar(10))+ ( case when  s.PeriodId = 2 then  ' Day(s)' else ' Hour(s)' end )as NumberofOccurances,  MAX(st.DateDay) as LastOccurance,   s.Name as StrategyName, st.AccountNumber as AccountNumber , max(#SuspectionLevel.Id) as SuspicionLevelId 
				
				INTO #Result
				
				from [FraudAnalysis].[AccountThreshold] st
				inner join #SuspectionLevel ON st.SuspicionLevelId=#SuspectionLevel.Id 
				inner join #Strategy s ON s.Id=st.StrategyId 
				inner join [FraudAnalysis].[AccountCase] ac  ON ac.AccountNumber=st.AccountNumber 
				inner join #CaseStatus cs ON cs.Id=ac.StatusId 
				WHERE (@AccountNumber is null or st.AccountNumber=@AccountNumber) and dateday between @fromDate and @ToDate and ac.Id =(select MAX(Id) from accountcase ac2 where ac2.accountnumber= st.AccountNumber )
				group by st.AccountNumber, s.Name, cs.Name , ac.StatusId, ac.ValidTill, s.PeriodId
				
						
				
				declare @sql varchar(1000)
				set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
				exec(@sql)
		END
		
		SET NOCOUNT OFF
	END