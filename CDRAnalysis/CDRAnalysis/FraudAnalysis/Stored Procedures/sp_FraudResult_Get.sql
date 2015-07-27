
CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_Get]
(
	@FromDate datetime,
	@ToDate datetime,
	@StrategiesList varchar(10) = '',
	@SuspiciousLevelsList varchar(10) = '',
	@SubscriberNumber varchar(50)
)
	AS
	BEGIN
		
		CREATE TABLE #SuspectionLevel(Id int, Name varchar(20))
		CREATE TABLE #Strategy(Id int, Name varchar(20))
		
		IF(@SuspiciousLevelsList = '')
			begin
				EXEC('INSERT INTO #SuspectionLevel SELECT Id,Name FROM [FraudAnalysis].Suspicion_Level')
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
			
		
		SELECT ISNULL(cs.Name, 'Pending')  CaseStatus,  ISNULL(sc.StatusId, 1) StatusId, sc.ValidTill ValidTill  , COUNT(st.Id) as NumberofOccurances,  MAX(st.DateDay) as LastOccurance,   #Strategy.Name as StrategyName, st.SubscriberNumber as SubscriberNumber , max(#SuspectionLevel.Id) as SuspicionLevelId 
				from [FraudAnalysis].[SubscriberThreshold] st
				inner join #SuspectionLevel ON st.SuspicionLevelId=#SuspectionLevel.Id 
				inner join #Strategy ON #Strategy.Id=st.StrategyId 
				left join [FraudAnalysis].[SubscriberCase] sc  ON sc.SubscriberNumber=st.SubscriberNumber 
				left join [FraudAnalysis].[CaseStatus] cs ON cs.Id=sc.StatusId
				WHERE  SuspicionLevelId <> 0 and dateday between @fromDate and @ToDate and st.SubscriberNumber=@SubscriberNumber
				group by st.SubscriberNumber, #Strategy.Name, cs.Name , sc.StatusId, sc.ValidTill
				
		
	END