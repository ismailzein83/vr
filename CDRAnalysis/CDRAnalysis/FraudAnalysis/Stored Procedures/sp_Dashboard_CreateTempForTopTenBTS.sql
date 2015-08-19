

CREATE PROCEDURE [FraudAnalysis].[sp_Dashboard_CreateTempForTopTenBTS]
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
		
			select top 10 count(distinct ac.AccountNumber) as CountCases, cdr.BTS_id BTS_Id 
			into #Result
			from FraudAnalysis.NormalCDR cdr 
			inner join FraudAnalysis.AccountCase ac on cdr.MSISDN=ac.AccountNumber
			
			
			where ac.LogDate between @FromDate and @ToDate and ac.StatusID = 3
			group by BTS_id
			order by count(distinct ac.AccountNumber) desc
		
			
			
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END