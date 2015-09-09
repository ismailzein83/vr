

CREATE PROCEDURE [FraudAnalysis].[sp_NormalCDR_CreateTempForFilteredNormalCDRs]
(
	@TempTableName varchar(200),	
	@FromDate DATETIME,
	@ToDate DATETIME,
	@MSISDN varCHAR(100)
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
			Service_VAS_Name
			
		into #Result
		
		FROM FraudAnalysis.NormalCDR as cdr
		
		where MSISDN=@MSISDN and  ConnectDateTime between   @FromDate and @ToDate
		
		declare @sql varchar(1000)
		set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
		exec(@sql)
	END
	
	SET NOCOUNT OFF
END