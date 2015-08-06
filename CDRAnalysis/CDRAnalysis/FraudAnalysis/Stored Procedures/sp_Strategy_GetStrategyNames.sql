




CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetStrategyNames] 
@StrategyIds varchar(200)
AS
BEGIN
 exec ('SELECT [Name]  FROM FraudAnalysis.Strategy  Where Id in ('+@StrategyIds+')')
END