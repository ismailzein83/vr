CREATE PROCEDURE [dbo].[SP_CapacityPeakMarginCheck_Enhanced] 
    @TheDate DATETIME
WITH RECOMPILE
AS
BEGIN	
	SET NOCOUNT ON
DECLARE @FromDateTime DATETIME
DECLARE @ToDateTime DATETIME

   SET @FromDateTime = dbo.DateOf(@TheDate)
	
   SET @ToDateTime= CAST(
     (
		STR( YEAR(@TheDate ) ) + '-' +
		STR( MONTH(@TheDate ) ) + '-' +
		STR( DAY(@TheDate ) ) + ' 23:59:59.99'
      )
		AS DATETIME
	  )	  

	;WITH
	CarrierTraffic AS (
		SELECT
			CustomerID AS CustomerID,
			SupplierID AS SupplierID,
			SwitchId AS SwitchID,
			Port_IN AS Port_IN,
			Port_Out AS Port_Out,
			FirstCDRAttempt AS FirstCDRAttempt,
			Attempts AS Attempts,
			SuccessfulAttempts AS SuccessfulAttempts,
			DurationsInSeconds AS DurationInSeconds,
			UtilizationInSeconds AS UtilizationInSeconds,
			NumberOfCalls AS NumberOfCalls
		FROM TrafficStats WITH (NOLOCK)
		WHERE FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
	),
	Result AS (
		SELECT
			DATEPART(hour, FirstCDRAttempt) AS [Hour],
			SwitchID,
			CustomerID,
			SupplierID,
			Port_In,
			Port_Out,
			SUM(NumberOfCalls) AS Attempts,
			SUM(SuccessfulAttempts) AS SuccessfulAttempts,
			SUM(SuccessfulAttempts) * 100.0 /
			Cast(NULLIF(SUM(NumberOfCalls) + 0.0, 0) AS NUMERIC) AS ASR,
			SUM(DurationInSeconds)/60.0   AS DurationsInMinutes,
			SUM(UtilizationInSeconds)/60.0   AS UtilizationsInMinutes
		FROM CarrierTraffic WITH (NOLOCK)
		GROUP BY
			DATEPART(hour, FirstCDRAttempt),
			SwitchID,
			CustomerID,
			SupplierID,
			Port_IN,
			Port_Out
	)
	
	SELECT * FROM Result
END