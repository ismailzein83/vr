CREATE PROCEDURE [dbo].[Sp_SwapCarrierReports]
    @FromDate     datetime,
    @ToDate       datetime,
    @CarrierAccountID    varchar(10) = NULL,
    @OurZoneID    int = NULL 
WITH RECOMPILE
AS
BEGIN   
    SET NOCOUNT ON;
 

    WITH TrafficTable AS
    (
    SELECT
        TS.OurZoneID AS OurZoneID,
    	SUM(CASE WHEN ts.CustomerID = @CarrierAccountID  THEN ts.Attempts ELSE 0 END)   AS InAttempts,
	    SUM(CASE WHEN ts.SupplierID = @CarrierAccountID THEN ts.Attempts ELSE 0 END)   AS OutAttempts,
	        
		SUM(CASE WHEN ts.CustomerID = @CarrierAccountID THEN ts.SuccessfulAttempts ELSE 0 END) AS  InSuccessfulAttempts,
		SUM(CASE WHEN ts.SupplierID = @CarrierAccountID THEN ts.SuccessfulAttempts ELSE 0 END) AS  OutSuccessfulAttempts,
	        
		SUM(CASE WHEN ts.CustomerID = @CarrierAccountID THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
		NULLIF(SUM(CASE WHEN ts.CustomerID = @CarrierAccountID THEN ts.Attempts ELSE 0 END), 0) AS InASR,
	        
		SUM(CASE WHEN ts.SupplierID = @CarrierAccountID THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
		NULLIF(SUM(CASE WHEN ts.SupplierID = @CarrierAccountID THEN ts.Attempts ELSE 0 END),0) AS OutASR,
	       
		SUM(CASE WHEN ts.CustomerID = @CarrierAccountID THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS InDurationsInMinutes,
		SUM(CASE WHEN ts.SupplierID = @CarrierAccountID THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS OutDurationsInMinutes 
        
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))  
   WHERE  
        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
        AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
    GROUP BY TS.OurZoneID
    )

SELECT * FROM trafficTable T

END