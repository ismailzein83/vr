




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
      ,cdrs.[InTrunkID]
      ,cdrs.[OutTrunkID]
      ,cdrs.[ConnectDateTime]
      ,cdrs.[DurationInSeconds]
      ,cdrs.[DisconnectDateTime]
	  ,cdrs.[CGPNAreaCode]
	  ,cdrs.[CDPNAreaCode]
FROM	[FraudAnalysis].StagingCDR cdrs with(nolock,index=IX_StagingCDRS_ConnectDateTime)
WHERE	cdrs.connectDateTime between @From and @To 
ORDER BY cdrs.connectDateTime
END