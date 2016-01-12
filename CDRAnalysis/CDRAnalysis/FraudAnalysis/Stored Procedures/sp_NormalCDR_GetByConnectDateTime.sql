
CREATE PROCEDURE [FraudAnalysis].[sp_NormalCDR_GetByConnectDateTime] 
(
	@From datetime ,
	@To datetime
)
AS
BEGIN

SELECT  cdrs.[MSISDN] ,cdrs.[IMSI] ,cdrs.[ConnectDateTime] ,cdrs.[Destination] ,
		cdrs.[DurationInSeconds] ,cdrs.[DisconnectDateTime] ,cdrs.[CallClassID]  ,cdrs.[IsOnNet] ,
		cdrs.[CallTypeID] ,cdrs.[SubscriberTypeID] ,cdrs.[IMEI]
		,cdrs.[BTS]  ,cdrs.[Cell]  ,cdrs.[SwitchId]  ,cdrs.[UpVolume]  ,cdrs.[DownVolume] ,
		cdrs.[CellLatitude]  ,cdrs.[CellLongitude]  ,cdrs.[InTrunkID]  ,cdrs.[OutTrunkID]  ,cdrs.[ServiceTypeID]  ,cdrs.[ServiceVASName] 
		, cdrs.[ReleaseCode], cdrs.MSISDNAreaCode, cdrs.DestinationAreaCode
                                                
FROM	FraudAnalysis.NormalCDR cdrs

 with(nolock)
WHERE	cdrs.connectDateTime between @From and @To 
ORDER BY cdrs.MSISDN, cdrs.connectdatetime
END