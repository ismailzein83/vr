




CREATE PROCEDURE [FraudAnalysis].[sp_StagingCDR_GetByConnectDateTime] 
(
	@From datetime ,
	@To datetime
)
AS
BEGIN

SELECT cdrs.[CGPN]
      ,cdrs.[CDPN]
      ,cdrs.[SwitchID]
      ,cdrs.[InTrunkSymbol]
      ,cdrs.[OutTrunkSymbol]
      ,cdrs.[ConnectDateTime]
      ,cdrs.[DurationInSeconds]
      ,cdrs.[DisconnectDateTime]
FROM	StagingCDR cdrs with(nolock,index=IX_StagingCDRS_ConnectDateTime)
WHERE	cdrs.connectDateTime between @From and @To 
ORDER BY cdrs.connectDateTime
END