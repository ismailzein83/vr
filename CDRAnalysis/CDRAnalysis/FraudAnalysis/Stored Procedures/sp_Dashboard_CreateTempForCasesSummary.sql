

CREATE PROCEDURE [FraudAnalysis].[sp_Dashboard_CreateTempForCasesSummary]
(
	@TempTableName varchar(200),	
	@FromDate datetime,
	@ToDate datetime
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
		
			select IsNuLL(cs.Name,'Opened') as StatusName, count(st.SubscriberNumber)as CountCases 
			into #Result
			from FraudAnalysis.SubscriberThreshold st 
			left join FraudAnalysis.SubscriberCase sc on st.SubscriberNumber = sc.SubscriberNumber 
			left join FraudAnalysis.CaseStatus cs on cs.Id=sc.StatusId
			
			
			where st.DateDay between @FromDate and @ToDate
			group by cs.Name,cs.Id
		
			
			
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END