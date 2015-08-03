--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE  PROCEDURE [dbo].[SP_TrafficStats_ConnectivityReport] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CarrierAccountID varchar(10),
	@SupplierID		varchar(10) = NULL,
    @SwitchID		tinyInt = NULL,
    @OurZoneID 		int = NULL,
    @CodeGroup		varchar(10) = NULL,
    @ConnectionType VARCHAR(20),
    @PeriodType		VARCHAR(10),
    @PortInOut		varchar(50) = NULL,
	@SelectPort		int = NULL 
WITH RECOMPILE
AS
BEGIN	
	SET NOCOUNT ON
	
	
	SET @FromDateTime = DATEADD(day,0,datediff(day,0, @FromDateTime))
	SET @ToDateTime = DATEADD(s,-1,DATEADD(day,1,datediff(day,0, @ToDateTime)))


	DECLARE @ConnectivityValue TINYINT
	SELECT  @ConnectivityValue = Value FROM Enumerations e WHERE e.Enumeration = 'Tone.ConnectionType' AND e.[Name] = @ConnectionType; 

DECLARE @IPpattern VARCHAR(10)
SET @IPpattern = '%.%.%.%'

IF @ConnectionType = 'VoIP'
BEGIN 	
	WITH trafficTable AS 
	(
		SELECT
			Period = CASE @PeriodType
			WHEN 'Hourly'   THEN datepart(hour,FirstCDRAttempt)
			WHEN 'Daily'  THEN datepart(dd,FirstCDRAttempt)
			WHEN 'Weekly' THEN datepart(ww,FirstCDRAttempt)
			WHEN 'Monthly' THEN datepart(month,FirstCDRAttempt)
			ELSE NULL END,
			DATEADD(day,0,datediff(day,0, FirstCDRAttempt)) AS [Date],
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern  THEN ts.NumberOfCalls ELSE 0 END)   AS InAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)   AS OutAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern  THEN ts.SuccessfulAttempts ELSE 0 END) AS  InSuccesfulAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) AS  OutSuccesfulAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			Cast(NULLIF(SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END) + 0.0, 0) AS NUMERIC) AS InASR,
	        
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			cast(NULLIF(SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)+ 0.0, 0) AS NUMERIC) AS OutASR,
	       
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS InDurationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS OutDurationsInMinutes, 
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS InUtilizationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS OutUtilizationsInMinutes
	   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))--,IX_TrafficStats_Customer,IX_TrafficStats_Supplier))
			LEFT JOIN Zone AS Z ON TS.OurZoneID = Z.ZoneID                                      
	   WHERE   
			FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			--AND (Ts.CustomerID = @CarrierAccountID OR Ts.SupplierID = @CarrierAccountID)
			AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
			AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)      
			AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
			AND (@SupplierID is null or TS.SupplierID = @SupplierID)	    
			AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
			AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
		GROUP BY CASE @PeriodType
			WHEN 'Hourly'   THEN datepart(hour,FirstCDRAttempt)
			WHEN 'Daily'  THEN datepart(dd,FirstCDRAttempt)
			WHEN 'Weekly' THEN datepart(ww,FirstCDRAttempt)
			WHEN 'Monthly' THEN datepart(month,FirstCDRAttempt)
			ELSE NULL END,
			DATEADD(day,0,datediff(day,0, FirstCDRAttempt))
	),
		ConnectivityTable AS 
		(
			SELECT *
			  FROM dbo.GetDailyConnectivity(@ConnectivityValue,@CarrierAccountID,@SwitchID,'Y', @FromDateTime,@ToDateTime,'N') C
		)

SELECT 
		  T.[Period],
		  T.[Date],
		  isnull(T.InAttempts,0) AS InAttempts,
		  isnull(T.OutAttempts,0) AS OutAttempts,
		  isnull(T.InSuccesfulAttempts,0) AS InSuccesfulAttempts ,
		  isnull(T.OutSuccesfulAttempts,0) AS OutSuccesfulAttempts,
		  isnull(T.InASR,0) AS InASR,
		  isnull(T.OutASR,0) AS OutASR,
		  isnull(T.InDurationsInMinutes,0) AS InDurationsInMinutes,
		  isnull(T.OutDurationsInMinutes,0) AS OutDurationsInMinutes,
		  isnull(T.InUtilizationsInMinutes,0) AS InUtilizationsInMinutes,
		  isnull(T.OutUtilizationsInMinutes,0) AS OutUtilizationsInMinutes,
		  isnull(C.NumberOfChannels_In,0)  AS NumberOfChannels_In,  -- nb of E1s
		  isnull(C.NumberOfChannels_Out,0)  AS NumberOfChannels_Out,  -- nb of E1s
		  isnull(C.NumberOfChannels_Total,0) AS NumberOfChannels_Total -- nb of E1s
		FROM TrafficTable T
		  LEFT JOIN ConnectivityTable C ON T.Date = C.Date
		ORDER BY [Period],[Date]
END 

IF @ConnectionType = 'TDM'
BEGIN 
		WITH trafficTable AS (
	SELECT
        Period = CASE @PeriodType
        WHEN 'Hourly'   THEN datepart(hour,FirstCDRAttempt)
        WHEN 'Daily'  THEN datepart(dd,FirstCDRAttempt)
        WHEN 'Weekly' THEN datepart(ww,FirstCDRAttempt)
        WHEN 'Monthly' THEN datepart(month,FirstCDRAttempt)
        ELSE NULL END,
        DATEADD(day,0,datediff(day,0, FirstCDRAttempt)) AS [Date],
        SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN NOT LIKE @IPpattern THEN  ts.NumberOfCalls ELSE NULL END)   AS InAttempts,
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out NOT LIKE @IPpattern THEN  ts.NumberOfCalls ELSE NULL END)   AS OutAttempts,
        
        SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN NOT LIKE @IPpattern THEN  ts.SuccessfulAttempts ELSE NULL END) AS  InSuccesfulAttempts,
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out NOT LIKE @IPpattern THEN  ts.SuccessfulAttempts ELSE NULL END) AS  OutSuccesfulAttempts,
        
        SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN NOT LIKE @IPpattern THEN  ts.SuccessfulAttempts ELSE NULL END) * 100 /
        Cast(NULLIF(SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN NOT LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END) + 0.0, 0) AS NUMERIC) AS InASR,
        
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out NOT LIKE @IPpattern THEN  ts.SuccessfulAttempts ELSE NULL END) * 100 /
        cast(NULLIF(SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out NOT LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)+ 0.0, 0) AS NUMERIC) AS OutASR,
        
       
        SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN NOT LIKE @IPpattern THEN  ts.DurationsInSeconds ELSE NULL END)/60.0   AS InDurationsInMinutes,
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out NOT LIKE @IPpattern THEN  ts.DurationsInSeconds ELSE NULL END)/60.0   AS OutDurationsInMinutes, 
        SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN NOT LIKE @IPpattern THEN  ts.UtilizationInSeconds ELSE NULL END)/60.0   AS InUtilizationsInMinutes,
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out NOT LIKE @IPpattern THEN  ts.UtilizationInSeconds ELSE NULL END)/60.0   AS OutUtilizationsInMinutes
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))--,IX_TrafficStats_Customer,IX_TrafficStats_Supplier))
        LEFT JOIN Zone AS Z ON TS.OurZoneID = Z.ZoneID                                      
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
	    --AND (Ts.CustomerID = @CarrierAccountID OR Ts.SupplierID = @CarrierAccountID)
	    AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)      
	    AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)	    
	    AND (@SupplierID is null or TS.SupplierID = @SupplierID)
	    AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
	GROUP BY CASE @PeriodType
        WHEN 'Hourly'   THEN datepart(hour,FirstCDRAttempt)
        WHEN 'Daily'  THEN datepart(dd,FirstCDRAttempt)
        WHEN 'Weekly' THEN datepart(ww,FirstCDRAttempt)
        WHEN 'Monthly' THEN datepart(month,FirstCDRAttempt)
        ELSE NULL END,
        DATEADD(day,0,datediff(day,0, FirstCDRAttempt))
		),
		ConnectivityTable AS 
		(
			SELECT *
			  FROM dbo.GetDailyConnectivity(@ConnectivityValue,@CarrierAccountID,@SwitchID,'Y', @FromDateTime,@ToDateTime,'N') C
		)

		SELECT 
		  T.[Period],
		  T.[Date],
		  isnull(T.InAttempts,0) AS InAttempts,
		  isnull(T.OutAttempts,0) AS OutAttempts,
		  isnull(T.InSuccesfulAttempts,0) AS InSuccesfulAttempts ,
		  isnull(T.OutSuccesfulAttempts,0) AS OutSuccesfulAttempts,
		  isnull(T.InASR,0) AS InASR,
		  isnull(T.OutASR,0) AS OutASR,
		  isnull(T.InDurationsInMinutes,0) AS InDurationsInMinutes,
		  isnull(T.OutDurationsInMinutes,0) AS OutDurationsInMinutes,
		  isnull(T.InUtilizationsInMinutes,0) AS InUtilizationsInMinutes,
		  isnull(T.OutUtilizationsInMinutes,0) AS OutUtilizationsInMinutes,
		  isnull(C.NumberOfChannels_In,0)  AS NumberOfChannels_In,  -- nb of E1s
		  isnull(C.NumberOfChannels_Out,0)  AS NumberOfChannels_Out,  -- nb of E1s
		  isnull(C.NumberOfChannels_Total,0) AS NumberOfChannels_Total -- nb of E1s
		FROM TrafficTable T
		  LEFT JOIN ConnectivityTable C ON T.Date = C.Date
		ORDER BY [Period],[Date]
	
END
END