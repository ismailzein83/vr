

CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_DeleteStrategyResults] 
    @StrategiesList varchar(200), 
	@FromDate DateTime,
	@ToDate DateTime
AS
BEGIN

	EXEC('Delete FraudAnalysis.[NumberProfile] where StrategyId in ('+@StrategiesList+') and FromDate>='''+@FromDate+''' and ToDate <='''+@ToDate+'''')
    EXEC('Delete FraudAnalysis.[SubscriberThreshold] where StrategyId in ('+@StrategiesList+') and DateDay>='''+@FromDate+''' and DateDay <='''+@ToDate+'''')
    
END