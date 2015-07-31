




CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_GetNumberProfile]
(
	@FromRow int ,
	@ToRow int,
	@FromDate DATETIME,
	@ToDate DATETIME,
	@SubscriberNumber varCHAR(100)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		;WITH NumberProfile_CTE (FromDate, ToDate,StrategyId, PeriodId,  SubscriberNumber, AggregateValues,RowNumber) AS 
			(
				
				SELECT     FromDate, ToDate,StrategyId, PeriodId, SubscriberNumber, AggregateValues
                      , ROW_NUMBER() OVER ( ORDER BY  FromDate ASC) AS RowNumber 
				FROM         FraudAnalysis.NumberProfile
				where SubscriberNumber=@SubscriberNumber and  FromDate >=   @FromDate and ToDate<=@ToDate
			)
			
		SELECT FromDate, ToDate,StrategyId, PeriodId,SubscriberNumber, AggregateValues,RowNumber
		FROM NumberProfile_CTE WHERE RowNumber between @FromRow AND @ToRow  

		SET NOCOUNT OFF
		/*

		exec [FraudAnalysis].[sp_FraudResult_GetNumberProfile]
			@FromRow =1 ,
			@ToRow =1000,
			@FromDate ='2015-03-14 01:59:59.000',
			@ToDate ='2015-03-14 05:59:59.000',
			@SubscriberNumber='202010904977227'
		*/
	END