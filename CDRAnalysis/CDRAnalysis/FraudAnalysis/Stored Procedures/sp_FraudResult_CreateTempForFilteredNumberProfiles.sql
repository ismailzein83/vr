

CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_CreateTempForFilteredNumberProfiles]
(
	@TempTableName varchar(200),	
	@FromDate DATETIME,
	@ToDate DATETIME,
	@SubscriberNumber varCHAR(100)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT     FromDate, ToDate,StrategyId, SubscriberNumber, AggregateValues
            into #Result
			FROM         FraudAnalysis.NumberProfile
			where SubscriberNumber=@SubscriberNumber and  FromDate >=   @FromDate and ToDate<=@ToDate
			
			
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END