

CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_CreateTempForDailyVolumeLooses]
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
		
			select sum(cdr.DurationInSeconds) as Volume, CONVERT(date, cdr.ConnectDateTime) as DateDay
			into #Result
			from FraudAnalysis.NormalCDR cdr 
			inner join FraudAnalysis.AccountCase ac on cdr.MSISDN=ac.AccountNumber
			
			where ac.CreatedTime between @FromDate and @ToDate and ac.Status = 3 and cdr.Call_Type=1
			group by CONVERT(date, cdr.ConnectDateTime) 
			
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END