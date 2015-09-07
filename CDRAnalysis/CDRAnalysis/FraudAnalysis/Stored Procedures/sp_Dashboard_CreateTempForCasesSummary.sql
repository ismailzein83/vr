

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
		
		select Temp.StatusName, sum(Temp.CountCases) as CountCases 
		into #Result
		from
		    (select cs.Name StatusName, 0 as CountCases 
			from  FraudAnalysis.CaseStatus cs 
			
			union       	
		
		
			select cs.Name StatusName, count(ac.AccountNumber)as CountCases 
			from  FraudAnalysis.AccountCase ac right join FraudAnalysis.CaseStatus cs on cs.Id=ac.Status
			where ac.CreatedTime between @FromDate and @ToDate
			group by cs.Name,cs.Id) as Temp
			
			group by Temp.StatusName
			
		
			
			
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END