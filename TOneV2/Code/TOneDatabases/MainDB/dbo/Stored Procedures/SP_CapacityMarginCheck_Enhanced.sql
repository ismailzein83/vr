CREATE PROCEDURE [dbo].[SP_CapacityMarginCheck_Enhanced] 
    @FromDateTime	datetime,
	@ToDateTime		DATETIME
WITH RECOMPILE
AS
BEGIN	
	SET NOCOUNT ON;
	
	WITH 
		CarrierTraffic AS (
			SELECT
				CustomerID AS customerid,
				SupplierID AS supplierid,
				SwitchId AS switchid,
				Port_IN AS Port_In,
				Port_OUT AS Port_Out,
				SuccessfulAttempts AS successfulattempts,
				DurationsInSeconds AS durationinseconds,
				UtilizationInSeconds AS utilizationinseconds,
				NumberOfCalls AS numberofcalls
			FROM TrafficStats WITH (NOLOCK)
			WHERE
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		),
		Result AS (
			SELECT
				switchid,
				customerid,
				supplierid,
				Port_In,
				Port_Out,
				SUM(numberofcalls) as Attempts,
				SUM(successfulattempts) AS SuccessfulAttempts,
				SUM(successfulattempts) * 100.0 /
				Cast(NULLIF(SUM(numberofcalls) + 0.0, 0) AS NUMERIC) AS ASR,
				SUM(durationinseconds) / 60 AS DuraionInMinutes,
				SUM(utilizationinseconds) / 60 AS UtilizationInMinutes
			FROM CarrierTraffic WITH (NOLOCK)
			GROUP BY
				switchid,
				customerid,
				supplierid,
				Port_In,
				Port_Out
		)
		
		SELECT * FROM Result
							    
END