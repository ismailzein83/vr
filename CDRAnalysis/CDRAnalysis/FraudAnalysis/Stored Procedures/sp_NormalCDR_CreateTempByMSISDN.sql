

CREATE PROCEDURE [FraudAnalysis].[sp_NormalCDR_CreateTempByMSISDN]
(
	@TempTableName VARCHAR(200),
	@MSISDN VARCHAR(30),
	@FromDate DATETIME,
	@ToDate DATETIME
)
AS
BEGIN
	SET NOCOUNT ON
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
    BEGIN
		SELECT  cdrs.[MSISDN] ,cdrs.[IMSI] ,cdrs.[ConnectDateTime] ,cdrs.[Destination] ,
		cdrs.[DurationInSeconds] ,cdrs.[DisconnectDateTime] ,cdrs.[CallClassID]  ,cdrs.[IsOnNet] ,
		cdrs.[CallTypeID] ,cdrs.[SubscriberTypeID] ,cdrs.[IMEI]
		,cdrs.[BTS]  ,cdrs.[Cell]  ,cdrs.[SwitchId]  ,cdrs.[UpVolume]  ,cdrs.[DownVolume] ,
		cdrs.[CellLatitude]  ,cdrs.[CellLongitude]  ,cdrs.[InTrunkID]  ,cdrs.[OutTrunkID]  ,cdrs.[ServiceTypeID]  ,cdrs.[ServiceVASName] 
		, cdrs.[ReleaseCode], cdrs.MSISDNAreaCode, cdrs.DestinationAreaCode
                                                

			
		INTO #RESULT
		
		FROM	FraudAnalysis.NormalCDR cdrs with(nolock,index=IX_NormalCDR_MSISDN)
		
		WHERE cdrs.MSISDN = @MSISDN
		AND cdrs.ConnectDateTime >= @FromDate
		AND cdrs.ConnectDateTime <= @ToDate
		
		ORDER BY ConnectDateTime DESC
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
	
	SET NOCOUNT OFF
END