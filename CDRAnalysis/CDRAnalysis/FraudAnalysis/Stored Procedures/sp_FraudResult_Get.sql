
CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_Get]
(
	@FromDate datetime,
	@ToDate datetime,
	@StrategiesList varchar(10) = '',
	@SuspiciousLevelsList varchar(10) = '',
	@AccountNumber varchar(50)
)
	AS
	BEGIN
		
		CREATE TABLE #SuspectionLevel(Id int, Name varchar(20))
		CREATE TABLE #Strategy(Id int, Name varchar(20))
		
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
				EXEC('INSERT INTO #Strategy SELECT Id,Name FROM [FraudAnalysis].Strategy ')
			end
		else
			begin
				EXEC('INSERT INTO #Strategy SELECT Id,Name FROM [FraudAnalysis].Strategy s WHERE s.Id IN ('+@StrategiesList+')')
			end
			
		
		SELECT cs.Name  CaseStatus,  ac.StatusId, ac.ValidTill   , COUNT(st.Id) as NumberofOccurances,  MAX(st.DateDay) as LastOccurance,   #Strategy.Name as StrategyName, st.AccountNumber as AccountNumber , max(#SuspectionLevel.Id) as SuspicionLevelId 
				from [FraudAnalysis].[AccountThreshold] st
				inner join #SuspectionLevel ON st.SuspicionLevelId=#SuspectionLevel.Id 
				inner join #Strategy ON #Strategy.Id=st.StrategyId 
				inner join [FraudAnalysis].[AccountCase] ac  ON ac.AccountNumber=st.AccountNumber 
				inner join [FraudAnalysis].[CaseStatus] cs ON cs.Id=ac.StatusId
				WHERE   dateday between @fromDate and @ToDate and st.AccountNumber=@AccountNumber and ac.Id =(select MAX(Id) from accountcase ac2 where ac2.accountnumber= st.AccountNumber )
				group by st.AccountNumber, #Strategy.Name, cs.Name , ac.StatusId, ac.ValidTill
				
		
	END