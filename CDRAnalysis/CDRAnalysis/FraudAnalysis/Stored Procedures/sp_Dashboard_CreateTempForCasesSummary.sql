

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
		
			select IsNuLL(cs.Name,'Open') as StatusName, count(st.AccountNumber)as CountCases 
			into #Result
			from FraudAnalysis.AccountThreshold st 
			left join FraudAnalysis.AccountCase sc on st.AccountNumber = sc.AccountNumber 
			left join FraudAnalysis.CaseStatus cs on cs.Id=sc.StatusId
			
			
			where st.DateDay between @FromDate and @ToDate
			group by cs.Name,cs.Id
		
			
			
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END