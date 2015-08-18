
CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_DeleteStrategyResults] 
    @StrategiesList varchar(200), 
	@FromDate DateTime,
	@ToDate DateTime
AS
BEGIN

    
		DECLARE @Deleted_Rows_NP INT;
		SET @Deleted_Rows_NP = 1;
		WHILE (@Deleted_Rows_NP > 0)
		BEGIN
			  BEGIN TRANSACTION
			  exec('Delete TOP (1000) FraudAnalysis.[NumberProfile] Where Id in ( SELECT Id FROM FraudAnalysis.[NumberProfile] WITH (NOLOCK,index=IX_NumberProfile_StrategyId)  where StrategyId in ( '''+@StrategiesList+''') and FromDate>='''+@FromDate+''' and ToDate <='''+@ToDate+'''     )')
			  COMMIT TRANSACTION
			  CHECKPOINT -- for simple recovery model
			  SET @Deleted_Rows_NP = @@ROWCOUNT;
	    END
		--------------------------------------------------------------------------
		--------------------------------------------------------------------------
		DECLARE @Deleted_Rows_ST INT;
		SET @Deleted_Rows_ST = 1;
		WHILE (@Deleted_Rows_ST > 0)
		  BEGIN
			  BEGIN TRANSACTION
			  exec('Delete TOP (1000) FraudAnalysis.[AccountThreshold] Where Id in ( SELECT Id FROM FraudAnalysis.[AccountThreshold] WITH (NOLOCK,index=IX_AccountThreshold_StrategyId)  where StrategyId in ('''+@StrategiesList+''') and DateDay>='''+@FromDate+''' and DateDay <='''+@ToDate+'''     )')
			  COMMIT TRANSACTION
			  CHECKPOINT -- for simple recovery model
			  SET @Deleted_Rows_ST = @@ROWCOUNT;
		END







END

SET NOCOUNT OFF