

CREATE PROCEDURE [FraudAnalysis].[sp_Dashboard_CreateTempForTopTenHighValueBTS]
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
		
			select top 10 sum(cdr.DurationInSeconds) as Volume, cdr.BTS_id  
			into #Result
			from FraudAnalysis.NormalCDR cdr 
			with (nolock, index=IX_NormalCDR_MSISDN)
			where cdr.ConnectDateTime between @FromDate and @ToDate 
			group by BTS_id
			order by sum(cdr.DurationInSeconds) desc
		
			
			
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END