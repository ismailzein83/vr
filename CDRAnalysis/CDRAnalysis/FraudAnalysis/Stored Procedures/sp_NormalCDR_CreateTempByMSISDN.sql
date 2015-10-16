

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
		SELECT IMSI,
			ConnectDateTime,
			Destination,
			DurationInSeconds,
			Call_Class,
			Call_Type,
			Sub_Type,
			IMEI,
			Cell_Id,
			Up_Volume,
			Down_Volume,
			Service_Type,
			Service_VAS_Name,
			ReleaseCode,
			MSISDNAreaCode,
			DestinationAreaCode
			
		INTO #RESULT
		
		FROM FraudAnalysis.NormalCDR AS cdr with(nolock,index=IX_NormalCDR_MSISDN)
		
		WHERE MSISDN = @MSISDN
		AND ConnectDateTime >= @FromDate
		AND ConnectDateTime <= @ToDate
		
		ORDER BY ConnectDateTime DESC
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
	
	SET NOCOUNT OFF
END